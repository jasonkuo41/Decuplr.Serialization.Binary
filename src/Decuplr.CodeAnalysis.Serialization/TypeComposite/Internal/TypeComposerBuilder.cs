using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {

    public interface ITypeComposer {
        /// <summary>
        /// The layout that this composer represents
        /// </summary>
        SchemaLayout SourceSchema { get; }

        /// <summary>
        /// The symbol that the compser respresents
        /// </summary>
        ITypeSymbol ComposerSymbol { get; }

        /// <summary>
        /// The member composers of this type composer, indexed by their property name
        /// </summary>
        IReadOnlyDictionary<string, IMemberComposer> MemberComposers { get; }

        /// <summary>
        /// Methods that this signature presents
        /// </summary>
        IReadOnlyList<MethodSignature> Methods { get; }
    }

    public interface IMemberComposer {
        MemberMetaInfo TargetMember { get; }

        ITypeSymbol ComposerSymbol { get; }

        IReadOnlyList<MethodSignature> Methods { get; }
    }

}

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {

    internal class TypeComposer : ITypeComposer {

        public SchemaLayout SourceSchema { get; }

        public ITypeSymbol ComposerSymbol { get; }

        public IReadOnlyDictionary<string, IMemberComposer> MemberComposers { get; }

        public IReadOnlyList<MethodSignature> Methods { get; }

        public TypeComposer(SchemaLayout sourceSchema, ITypeSymbol composerSymbol, IReadOnlyList<IMemberComposer> memberComposers, IReadOnlyList<MethodSignature> methods) {
            SourceSchema = sourceSchema;
            ComposerSymbol = composerSymbol;
            MemberComposers = memberComposers;
            Methods = methods;
        }
    }

    internal class TypeComposerBuilder {

        private static class Property {
            public static string MemberName(int index) => $"Member_{index}";
        }

        private const string discovery = "discovery";
        private const string isSuccess = "isSuccess";

        private readonly SchemaLayout _type;
        private readonly MemberComposerFactory _memberFactory;
        private readonly ISourceAddition _sourceAddition;
        private readonly ITypeSymbolProvider _symbolSource;
        private readonly IEnumerable<IConditionResolverProvider> _resolvers;
        private readonly IEnumerable<IMemberDataFormatterProvider> _formatter;

        public TypeComposerBuilder(SchemaLayout layout, MemberComposerFactory memberFactory, ISourceAddition sourceAddition, ITypeSymbolProvider symbolProvider, IEnumerable<IConditionResolverProvider> resolvers, IEnumerable<IMemberDataFormatterProvider> memberFormatter) {
            _type = layout;
            _resolvers = resolvers;
            _formatter = memberFormatter;
            _sourceAddition = sourceAddition;
            _symbolSource = symbolProvider;
        }

        private MethodSignature NonTryPattern(GeneratingTypeName fullName, IComponentProvider provider)
            => MethodSignatureBuilder.CreateConstructor(fullName, (provider.DiscoveryType, discovery));

        private MethodSignature TryPattern(GeneratingTypeName fullName, IComponentProvider provider)
            => MethodSignatureBuilder.CreateConstructor(fullName, (provider.DiscoveryType, discovery), (RefKind.Out, _symbolSource.GetSymbol<bool>(), isSuccess));

        public ITypeComposer Build(GeneratingTypeName typeName, IComponentProvider provider, Func<MemberMetaInfo, GeneratingTypeName> memberCompositeNameFactory) {

            var composers = _type.Members.Select(member => _memberFactory.Build(member, memberCompositeNameFactory(member), provider)).ToList();

            var builder = new CodeSourceFileBuilder(typeName.Namespace);
            builder.Using("System");

            builder.DenoteHideEditor();
            builder.DenoteGenerated(typeof(TypeComposerBuilder).Assembly);

            // Possible glitches here!
            Action<CodeNodeBuilder> lastAction = builder => AddComposerStruct(builder, typeName, composers, provider);

            var buildingList = new List<Action<CodeNodeBuilder>> { lastAction };
            
            foreach (var (parentKind, parentName) in typeName.Parents.Reverse()) {
                lastAction = builder => builder.AddNode($"partial {parentKind.ToString().ToLower()} {parentName}", lastAction);
            }

            _sourceAddition.AddSource($"{typeName}_generated.cs", builder.ToString());

            return new TypeComposer(_type, _symbolSource.GetSymbol(typeName.ToString()), composers);
        }

        private void AddComposerStruct(CodeNodeBuilder builder, GeneratingTypeName typeName, List<MemberComposerPrecusor> composers, IComponentProvider provider) {

            builder.AddNode($"internal readonly struct {typeName.TypeName}", node => {
                for (var i = 0; i < composers.Count; ++i) {
                    node.State($"public {composers[i].Name} {Property.MemberName(i)} {{ get; }}");
                }

                node.NewLine();
                node.Comment("Non-try Pattern");
                node.AddNode($"public {typeName.TypeName} ({provider.DiscoveryType} {discovery})", node => {
                    for (var i = 0; i < composers.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {composers[i].Name} ({discovery})");
                    }
                }).NewLine();

                node.Comment("Try Pattern");
                node.AddNode($"public {typeName.TypeName} ({provider.DiscoveryType} {discovery}, out bool {isSuccess})", node => {
                    for (var i = 0; i < composers.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {composers[i].Name} ({discovery}, out {isSuccess})");
                        node.If($"!{isSuccess}", node => {
                            node.Return();
                        }).NewLine();
                    }
                });
            });

        }
    }
}