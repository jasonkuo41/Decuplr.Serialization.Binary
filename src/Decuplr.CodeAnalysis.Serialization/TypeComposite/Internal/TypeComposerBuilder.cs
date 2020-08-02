using System;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class TypeComposerBuilder : ITypeComposerBuilder {

        private static class Property {
            public static string MemberName(int index) => $"Member_{index}";
        }

        private const string discovery = "discovery";
        private const string isSuccess = "isSuccess";

        private readonly MemberComposerFactory _memberFactory;
        private readonly ISourceAddition _sourceAddition;
        private readonly ITypeSymbolProvider _symbolSource;

        public TypeComposerBuilder(MemberComposerFactory memberFactory, ISourceAddition sourceAddition, ITypeSymbolProvider symbolProvider) {
            _memberFactory = memberFactory;
            _sourceAddition = sourceAddition;
            _symbolSource = symbolProvider;
        }

        private MethodSignature NonTryPattern(GeneratingTypeName fullName, IComponentProvider provider)
            => MethodSignatureBuilder.CreateConstructor(fullName, (provider.DiscoveryType, discovery));

        private MethodSignature TryPattern(GeneratingTypeName fullName, IComponentProvider provider)
            => MethodSignatureBuilder.CreateConstructor(fullName, (provider.DiscoveryType, discovery), (RefKind.Out, _symbolSource.GetSymbol<bool>(), isSuccess));

        private MethodSignature[] GetDefaultSignature(GeneratingTypeName fullName, IComponentProvider provider) => new[] {
            NonTryPattern(fullName, provider),
            TryPattern(fullName, provider)
        };

        public ITypeComposer BuildTypeComposer(SchemaLayout layout, IComponentProvider provider, GeneratingTypeName typeName, Func<MemberMetaInfo, GeneratingTypeName> memberCompositeNameFactory) {

            var mCompName = layout.Members.Select(x => memberCompositeNameFactory(x)).ToList();

            var builder = new CodeSourceFileBuilder(typeName.Namespace);
            builder.Using("System");

            builder.DenoteHideEditor();
            builder.DenoteGenerated(typeof(TypeComposerBuilder).Assembly);

            builder.NestType(typeName, $"internal readonly struct {typeName.TypeName}", node => {
                for (var i = 0; i < mCompName.Count; ++i) {
                    node.State($"public {mCompName[i]} {Property.MemberName(i)} {{ get; }}");
                }

                node.NewLine();
                node.Comment("Non-try Pattern");
                node.AddNode($"public {typeName.TypeName} ({provider.DiscoveryType} {discovery})", node => {
                    for (var i = 0; i < mCompName.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {mCompName[i]} ({discovery})");
                    }
                }).NewLine();

                node.Comment("Try Pattern");
                node.AddNode($"public {typeName.TypeName} ({provider.DiscoveryType} {discovery}, out bool {isSuccess})", node => {
                    for (var i = 0; i < mCompName.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {mCompName[i]} ({discovery}, out {isSuccess})");
                        node.If($"!{isSuccess}", node => {
                            node.Return();
                        }).NewLine();
                    }
                });
            });

            _sourceAddition.AddSource($"{typeName}_generated.cs", builder.ToString());

            var typeComposer = new TypeComposer(layout, _symbolSource.GetSymbol(typeName.ToString()), GetDefaultSignature(typeName, provider));
            for (var i = 0; i < layout.Members.Count; ++i) {
                typeComposer.Add(Property.MemberName(i), _memberFactory.Build(typeComposer, layout.Members[i], mCompName[i], provider));
            }
            return typeComposer;
        }

    }
}