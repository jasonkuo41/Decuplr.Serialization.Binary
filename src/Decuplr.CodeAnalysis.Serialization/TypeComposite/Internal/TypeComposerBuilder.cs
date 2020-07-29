using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

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

        /// The member composers of this type composer
        /// </summary>
        IReadOnlyList<IMemberComposer> MemberComposers { get; }

        /// <summary>
        /// Methods that this signature presents
        /// </summary>
        IReadOnlyList<MethodSignature> Methods { get; }
    }

    public interface IMemberComposer {

    }

}

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {

    internal class TypeComposer : ITypeComposer {
        public SchemaLayout SourceSchema { get; }

        public string FullName { get; }
    }

    internal class TypeComposerBuilder {

        private static class Property {
            public static string MemberName(int index) => $"Member_{index}";
        }

        public const string DefaultNamespace = "Decuplr.Serialization.Internal.Parsers";

        private const string discovery = "discovery";
        private const string isSuccess = "isSuccess";

        private readonly SchemaLayout _type;
        private readonly ISourceAddition _sourceAddition;
        private readonly ITypeSymbolProvider _symbolSource;
        private readonly IEnumerable<IConditionResolverProvider> _resolvers;
        private readonly IEnumerable<IMemberDataFormatterProvider> _formatter;

        public TypeComposerBuilder(SchemaLayout layout, ISourceAddition sourceAddition, ITypeSymbolProvider symbolProvider, IEnumerable<IConditionResolverProvider> resolvers, IEnumerable<IMemberDataFormatterProvider> memberFormatter) {
            _type = layout;
            _resolvers = resolvers;
            _formatter = memberFormatter;
            _sourceAddition = sourceAddition;
            _symbolSource = symbolProvider;
        }

        private MethodSignature NonTryPattern(string fullName, IComponentProvider provider)
            => MethodSignature.CreateConstructor(fullName, (provider.DiscoveryType, discovery));

        private MethodSignature TryPattern(string fullName, IComponentProvider provider)
            => MethodSignature.CreateConstructor(fullName, (provider.DiscoveryType, discovery), (MethodArgModifier.Out, _symbolSource.GetSymbol<bool>(), isSuccess));

        public ITypeComposer Build(string typeComposer, IComponentProvider provider) => Build("Decuplr.Serialization.Internal.Parsers", typeComposer, provider);
        public ITypeComposer Build(string typeComposerNamespace, string typeComposerName, IComponentProvider provider) {

            var composers = _type.Members.Select(member => new MemberComposerBuilder(member, _resolvers, _formatter).CreateStruct(provider)).ToList();

            var builder = new CodeSourceFileBuilder(typeComposerNamespace);
            builder.Using("System");

            builder.DenoteHideEditor();
            builder.DenoteGenerated(typeof(TypeComposerBuilder).Assembly);
            builder.AddNode($"internal readonly struct {typeComposerName}", node => {
                for (var i = 0; i < composers.Count; ++i) {
                    node.State($"public {composers[i].Name} {Property.MemberName(i)} {{ get; }}");
                }

                node.NewLine();
                node.Comment("Non-try Pattern");
                node.AddNode($"public {typeComposerName} ({provider.DiscoveryType} {discovery})", node => {
                    for (var i = 0; i < composers.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {composers[i].Name} ({discovery})");
                    }
                }).NewLine();

                node.Comment("Try Pattern");
                node.AddNode($"public {typeComposerName} ({provider.DiscoveryType} {discovery}, out bool {isSuccess})", node => {
                    for (var i = 0; i < composers.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {composers[i].Name} ({discovery}, out {isSuccess})");
                        node.If($"!{isSuccess}", node => {
                            node.Return();
                        }).NewLine();
                    }
                });
            });

            _sourceAddition.AddSource($"{typeComposerName.Replace('.', '_')}_{typeComposerName}.cs", builder.ToString());

            return new TypeComposer(typeComposerNamespace, typeComposerName);
        }
    }
}