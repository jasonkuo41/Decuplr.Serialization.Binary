using System;
using System.Linq;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite {
    internal interface IBinarySerializationProvider {
        void SerializeWriter(CodeNodeBuilder builder, MethodSignature signature);
        void SerializeSpan(CodeNodeBuilder builder, MethodSignature signature);
        void DeserializeCursor(CodeNodeBuilder builder, MethodSignature signature);
        void DeserializeSpan(CodeNodeBuilder builder, MethodSignature signature);
    }
}

namespace Decuplr.Serialization.Binary.TypeComposite.Internal {

    internal struct TypeComposer {

    }


    internal class BinaryTypeCompositeBuilder {
        private static class Property {
            public static string MemberName(int index) => $"Member_{index}";
        }

        private readonly NamedTypeMetaInfo _type;
        private readonly BinaryMemberCompositeBuilderProvider _memberFactory;
        private readonly BinaryConverterMethods _methods;

        public BinaryTypeCompositeBuilder(NamedTypeMetaInfo type, ISourceAddition sourceAddition, ITypeSymbolProvider symbolProvider) {

        }

        public TypeComposer AddStructToSource(IBinarySchema schema, IComponentResolver resolver, GeneratingTypeName typeName, Func<MemberMetaInfo, GeneratingTypeName> memberCompositeNameProvider) {
            const string discovery = nameof(discovery);

            var mCompositName = schema.SerializeOrder.Select(x => memberCompositeNameProvider(x)).ToList();
            var composites = schema.SerializeOrder.Select((x, i) => _memberFactory.AddMemberCompositeStruct(x, mCompositName[i], resolver)).ToList();

            var builder = new CodeSourceFileBuilder(typeName.Namespace);
            builder.Using("System");
            builder.Using("System.Collections.Generic");

            builder.AttributeHideEditor();
            builder.AttributeGenerated(typeof(BinaryTypeCompositeBuilder).Assembly);

            builder.NestType(typeName, $"internal readonly struct {typeName.TypeName}", node => {
                for (var i = 0; i < mCompositName.Count; ++i) {
                    node.State($"public {mCompositName[i]} {Property.MemberName(i)} {{ get; }}");
                }

                node.State($"public static IReadOnlyList<int?> BinaryLength {{ get; }} = new int?[] {{ {string.Join(",", composites.Select(x => x.ConstantLength))} }}");

                node.NewLine();
                node.AddNode($"public {typeName.TypeName} ({typeof(IBinaryNamespaceDiscovery).FullName} {discovery})", node => {
                    for (var i = 0; i < mCompositName.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {mCompositName[i]} ({discovery})");
                    }
                }).NewLine();

                // Serialize Span
                node.AddMethod(_methods.SerializeSpan, node => {

                });

                node.AddMethod(_methods.SerializeWriter, node => {

                });

                node.AddMethod(_methods.DeserializeSpan, node => {

                });

                node.AddMethod(_methods.DeserializeCursor, node => {

                });

                node.AddMethod(_methods.GetSpanLength, node => {
                    node.Return(string.Join("+ ", composites.Select(stringCompose)));

                    string stringCompose(BinaryMemberCompositeStruct composite) {
                        if (composite.ConstantLength.HasValue)
                            return composite.ConstantLength.ToString();
                        var typeParams = _methods.GetSpanLength.TypeParameters.Select(x => x.GenericName);
                        var args = _methods.GetSpanLength.Arguments.Select(x => x.ArgName);
                        return new BinaryConverterMethods(composite.Name, schema.SourceType.Symbol).GetSpanLength.GetInvocationString(typeParams, args);
                    }
                });

                node.AddMethod(_methods.GetBlockLengthSpan, node => {
                    const string length = nameof(length);
                    var currentIteration = 0;

                    node.State($"var {length} = 0");
                    for(var i = 0; i < composites.Count; ++i) {

                    }
                });
            });
        }
    }
}
