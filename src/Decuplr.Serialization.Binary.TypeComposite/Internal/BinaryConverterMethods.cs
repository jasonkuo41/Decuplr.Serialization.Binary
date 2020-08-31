using System;
using System.Buffers;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal {
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
}
