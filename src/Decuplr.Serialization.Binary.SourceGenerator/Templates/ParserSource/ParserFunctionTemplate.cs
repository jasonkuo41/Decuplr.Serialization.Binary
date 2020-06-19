using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.Templates.ParserSource {
    internal class ParserFunctionTemplate {

        private readonly Accessibility Accessibility;
        private readonly string AccessbilityName;
        private readonly ITypeSymbol MemberType;
        private readonly INamedTypeSymbol TargetTypeSymbol;

        public ParserFunctionTemplate(ITypeSymbol memberTypeSymbol, INamedTypeSymbol targetTypeSymbol, Accessibility accessibility) {
            MemberType = memberTypeSymbol;
            TargetTypeSymbol = targetTypeSymbol;
            Accessibility = accessibility;
            AccessbilityName = Accessibility.ToString().ToLowerInvariant();
        }


        public static ParserFunctionTemplate Create(INamedTypeSymbol targetTypeSymbol, ITypeSymbol memberSymbol) => new ParserFunctionTemplate(memberSymbol, targetTypeSymbol, Accessibility.Public);

        // public {TargetFieldType} DeserializeStateless(in {TargetType} type, ref SequenceCursor<byte> cursor)
        public string DeserializeSequence(string functionName, ParsingTypeArgs targetType, BufferArgs source) 
            => $"{AccessbilityName} {MemberType} {functionName}(in {TargetTypeSymbol} {targetType}, ref SequenceCursor<byte> {source})";

        // public {TargetFieldType} DeserializeStateless(in {TargetType} type, ReadOnlySpan<byte> span, out int readBytes)
        public string DeserializeSpan(string functionName, ParsingTypeArgs targetType, BufferArgs source, OutArgs<int> readBytes) 
            => $"{AccessbilityName} {MemberType} {functionName} (in {TargetTypeSymbol} {targetType}, ReadOnlySpan<byte> {source}, out int {readBytes})";

        // public DeserializeResult TryDeserializeStateless(in {TargetType} type, ref SequenceCursor<byte> cursor, out {TargetFieldType} data)
        public string TryDeserializeSequence(string functionName, ParsingTypeArgs targetType, BufferArgs source, OutArgs<object> result)
            => $"{AccessbilityName} DeserializeResult {functionName}(in {TargetTypeSymbol} {targetType}, ref SequenceCursor<byte> {source}, out {MemberType} {result}";

        // public DeserializeResult TryDeserializeStateless(in {TargetType} type, ReadOnlySpan<byte> span, out int readBytes, out {TargetFieldType} data)
        public string TryDeserializeSpan(string functionName, ParsingTypeArgs targetType, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result)
            => $"{AccessbilityName} DeserializeResult {functionName}(in {TargetTypeSymbol} {targetType}, ReadOnlySpan<byte> {source}, out int {readBytes}, out {MemberType} {result})";

        // public int SerializeStateless(in {TargetType} type, in {TargetFieldType} data, Span<byte> dest)
        public string SerializeUnsafeSpan(string functionName, ParsingTypeArgs targetType, TargetFieldArgs fieldName, BufferArgs destination) 
            => $"{AccessbilityName} int {functionName}(in {TargetTypeSymbol} {targetType}, in {MemberType} {fieldName}, Span<byte> {destination})";

        // public bool TrySerializeUnsafeStateless(in {TargetType} type, in {TargetFieldType} data, Span<byte> dest, out int writtenBytes)
        public string TrySerializeUnsafeSpan(string functionName, ParsingTypeArgs targetType, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes) 
            => $"{AccessbilityName} bool {functionName} (in {TargetTypeSymbol} {targetType}, in {MemberType} {fieldName}, Span<byte> {destination}, out int {writtenBytes}";

        public string GetLengthFunction(string functionName, ParsingTypeArgs targetType, TargetFieldArgs fieldName) 
            => $"{AccessbilityName} int {functionName}(in {TargetTypeSymbol} {targetType}, in {MemberType} {fieldName})";

    }
}
