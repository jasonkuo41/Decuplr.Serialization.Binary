using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

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

    internal class BinaryConverterMethods {

        public const string T_WRITER = "TWriter";
        public const string T_STATE = "TState";

        private readonly TypeName _sourceName;
        private readonly ITypeSymbol _convertSymbol;

        public BinaryConverterMethods(TypeName sourceName, ITypeSymbol convertSymbol) {
            _sourceName = sourceName;
            _convertSymbol = convertSymbol;
        }

        public static TypeName IBinaryWriteStateName(string tStateName) => new TypeName("Decuplr.Serialization.Binary", $"IBinaryWriteState<{tStateName}>");

        public MethodSignature SerializeWriter
            => MethodSignatureBuilder.CreateMethod(_sourceName, "Serialize")
                                     .AddGenerics(T_STATE, GenericConstrainKind.Struct, IBinaryWriteStateName(T_STATE))
                                     .AddGenerics(T_WRITER, GenericConstrainKind.Struct, TypeName.FromType<IBufferWriter<byte>>())
                                     .AddArgument((RefKind.In, _convertSymbol, "source"))
                                     .AddArgument((TypeName.FromGenericArgument(T_STATE), "state"))
                                     .AddArgument((RefKind.Ref, TypeName.FromGenericArgument(T_WRITER), "writer"))
                                     .WithReturn(TypeName.Void);

        public MethodSignature SerializeSpan
            => MethodSignatureBuilder.CreateMethod(_sourceName, "Serialize")
                                     .AddGenerics(T_STATE, GenericConstrainKind.Struct, IBinaryWriteStateName(T_STATE))
                                     .AddArgument((RefKind.In, _convertSymbol, "source"))
                                     .AddArgument((TypeName.FromGenericArgument(T_STATE), "state"))
                                     .AddArgument((TypeName.FromType(typeof(Span<byte>)), "data"))
                                     .WithReturn(TypeName.FromType<int>());

        public MethodSignature DeserializeCursor
            => MethodSignatureBuilder.CreateMethod(_sourceName, "Deserialize")
                                     .AddArgument((RefKind.Ref, TypeName.FromType(typeof(SequenceCursor<byte>)), "cursor"))
                                     .AddArgument((RefKind.Out, _convertSymbol, "result"))
                                     .WithReturn(TypeName.FromType<bool>());

        public MethodSignature DeserializeSpan
            => MethodSignatureBuilder.CreateMethod(_sourceName, "Deserialize")
                                     .AddArgument((RefKind.Ref, TypeName.FromType(typeof(ReadOnlySpan<byte>)), "data"))
                                     .AddArgument((RefKind.Out, _convertSymbol, "result"))
                                     .WithReturn(TypeName.FromType<int>());

        public MethodSignature GetSpanLength
            => MethodSignatureBuilder.CreateMethod(_sourceName, "GetSpanLength")
                                     .AddGenerics(T_STATE, GenericConstrainKind.Struct, IBinaryWriteStateName(T_STATE))
                                     .AddArgument((RefKind.In, _convertSymbol, "source"))
                                     .AddArgument((TypeName.FromGenericArgument(T_STATE), "state"))
                                     .WithReturn(TypeName.FromType<int>());
        public MethodSignature GetBlockLengthCursor
            => MethodSignatureBuilder.CreateMethod(_sourceName, "GetBlockLength")
                                     .AddArgument((RefKind.Ref, TypeName.FromType(typeof(SequenceCursor<byte>)), "cursor"))
                                     .WithReturn(TypeName.FromType<int>());

        public MethodSignature GetBlockLengthSpan
            => MethodSignatureBuilder.CreateMethod(_sourceName, "GetBlockLength")
                                     .AddArgument((TypeName.FromType(typeof(ReadOnlySpan<byte>)), "data"))
                                     .WithReturn(TypeName.FromType<int>());

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

        public TypeComposer AddStructToSource(SchemaLayout layout, IComponentResolver resolver, GeneratingTypeName typeName, Func<MemberMetaInfo, GeneratingTypeName> memberCompositeNameProvider) {
            const string discovery = nameof(discovery);

            var mCompositName = layout.Members.Select(x => memberCompositeNameProvider(x)).ToList();
            var composites = layout.Members.Select((x, i) => _memberFactory.AddMemberCompositeStruct(x, mCompositName[i], resolver)).ToList();

            var builder = new CodeSourceFileBuilder(typeName.Namespace);
            builder.Using("System");
            builder.Using("System.Collections.Generic");

            builder.AttributeHideEditor();
            builder.AttributeGenerated(typeof(BinaryTypeCompositeBuilder).Assembly);

            builder.NestType(typeName, $"internal readonly struct {typeName.TypeName}", node => {
                for(var i = 0; i < mCompositName.Count; ++i) {
                    node.State($"public {mCompositName[i]} {Property.MemberName(i)} {{ get; }}");
                }

                node.State($"public static IReadOnlyList<int?> BinaryLength {{ get; }} = new int?[] {{ {string.Join(",", composites.Select(x => x.ConstantLength))} }}");

                node.NewLine();
                node.AddNode($"public {typeName.TypeName} ({typeof(IBinaryNamespaceDiscovery).FullName} {discovery})", node => {
                    for(var i = 0; i < mCompositName.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {mCompositName[i]} ({discovery})");
                    }
                }).NewLine();

                // Serialize Span
                node.AddMethod(_methods.SerializeSpan, node => {

                });
            });
        }
    }
}
