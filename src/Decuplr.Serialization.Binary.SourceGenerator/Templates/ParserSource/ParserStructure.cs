using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.Templates.ParserSource {

    internal interface IParserDependencyComponent {
        string TypeName { get; }

        string GetInitializer(ParserDiscoveryArgs args);

        string GetInitializer(ParserDiscoveryArgs args, string parserName);
    }

    internal interface IComponentName {
        string Name { get; }
        string ToString();
    }

    // Every 
    internal abstract class ParserDependencyPrecusor {

        private class ComponentName : IComponentName {

            public ComponentName(string name) {
                Name = name;
            }

            public string Name { get; }

            public override string ToString() => Name;
            public override bool Equals(object obj) => obj is ComponentName name && name.Name.Equals(Name);
            public override int GetHashCode() => Name.GetHashCode();
        }

        private readonly List<(IParserDependencyComponent Item, IComponentName Name)> Components = new List<(IParserDependencyComponent, IComponentName)>();
        private readonly List<(ParserDependencyPrecusor Dependecy, IComponentName Name)> Dependencies = new List<(ParserDependencyPrecusor Dependecy, IComponentName Name)>();

        protected IComponentName AddComponent(IParserDependencyComponent component) {
            var name = new ComponentName($"component_{Components.Count}");
            Components.Add((component, name));
            return name;
        }

        protected IComponentName AddExternalDependency(ParserDependencyPrecusor dependency) {
            var name = new ComponentName($"dependency_{Dependencies.Count}");
            Dependencies.Add((dependency, name));
            return name;
        }

        // bool TrySerializeUnsafe(in T value, Span<byte> dest, out int written);
        // Included as : bool TrySerialize{FieldName}(in {Type} type, in T {fieldName}, Span<byte> {destination}, out int {writtenBytes})
        protected abstract string TrySerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes);

        // int SerializeUnsafe(in T value, Span<byte> writer);
        protected abstract string SerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination);

        // DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int read, out T result);
        protected abstract string TryDeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result);

        // DeserializeResult TryDeserialize(ref SequenceCursor<byte> cursor);
        protected abstract string TryDeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<object> result);

        // T Deserialize(ReadOnlySpan<byte> span, out int readBytes);
        protected abstract string DeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes);

        // T Deserialize(ref SequenceCursor<byte> cursor);
        protected abstract string DeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source);

        // int GetLength(in T value);
        protected abstract string GetLengthFunction(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName);

        // Probably get functions out of it 
    }

    internal class TypeParserWrappedDependency : ParserDependencyPrecusor {

        private class WrappedTypeParser : IParserDependencyComponent {

            private readonly ITypeSymbol Symbol;

            public string TypeName { get; }

            public WrappedTypeParser(ITypeSymbol typeSymbol) {
                Symbol = typeSymbol;
                TypeName = $"TypeParser<{typeSymbol}>";
            }

            public string GetInitializer(ParserDiscoveryArgs discovery) => $"{discovery}.GetParser<{Symbol}>();";

            public string GetInitializer(ParserDiscoveryArgs args, string parserName) => $"return {args}.TryGetParser(out {parserName});";
        }

        private readonly IComponentName TypeParser;

        public TypeParserWrappedDependency(ITypeSymbol typeSymbol) {
            TypeParser = AddComponent(new WrappedTypeParser(typeSymbol));
        }

        protected override string TrySerializeUnsafeSpan(ParsingTypeArgs parsingType, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes) {
            return $"return {TypeParser}.TrySerializeUnsafe(in {fieldName}, {destination}, out {writtenBytes});";
        }

        protected override string SerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination) {
            return $"return {TypeParser}.SerializeUnsafe(in {fieldName}, {destination});";
        }

        protected override string TryDeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result) {
            return $"return {TypeParser}.TryDeserialize({source}, out {readBytes}, out {result});";
        }

        protected override string TryDeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<object> result) {
            return $"return {TypeParser}.TryDeserialize(ref {source}, out {result});";
        }

        protected override string DeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes) {
            return $"return {TypeParser}.Deserialize({source}, out {readBytes});";
        }

        protected override string DeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source) {
            return $"return {TypeParser}.Deserialize(ref {source});";
        }

        protected override string GetLengthFunction(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName) {
            return $"return {TypeParser}.GetLength(in {fieldName});";
        }
    }

    // For primitives that are only one-byte wide (bool, byte, sbyte)
    internal class PrimitiveByteDependency : ParserDependencyPrecusor {

        private class ByteOrderParserType : IParserDependencyComponent {

            public string TypeName => "ByteOrder";

            public string ParserName { get; }

            public ByteOrderParserType(string parserName) {
                ParserName = parserName;
            }

            public string GetInitializer(ParserDiscoveryArgs discovery) => $"{discovery}.{nameof(IParserDiscovery.BinaryLayout)}";

            public string GetInitializer(ParserDiscoveryArgs discovery, string parsers) {
                var node = new CodeNodeBuilder();
                node.AddStatement($"{parsers} = {discovery}.{nameof(IParserDiscovery.BinaryLayout)};");
                node.AddStatement("return true");
                return node.ToString();
            }
        }

        private readonly IParserDependencyComponent Name;
        private readonly ITypeSymbol TypeSymbol;

        public override IReadOnlyList<IParserDependencyComponent> Components { get; }

        public PrimitiveByteDependency(ITypeSymbol typeSymbol, int index, params int[] subIndexes) {
            Name = new ByteOrderParserType($"byteOrder_{MakeUniqueIndex(index, subIndexes)}");
            TypeSymbol = typeSymbol;
            Components = new IParserDependencyComponent[] { Name };
        }

        private string ThrowArgOutOfRange(string source) => $"throw new {nameof(ArgumentOutOfRangeException)}(nameof({source}))";

        public override string GetTrySerializeUnsafeSpan(TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes) {
            return $"{destination}[0] = (byte){fieldName}; return true;";
        }

        public override string SerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination) {
            return $"{destination}[0] = (byte){fieldName}; return true;";
        }

        public override string TryDeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result) {
            var node = new CodeNodeBuilder();
            node.AddStatement($"if ({source}.Length < 1) return {DeserializeResult.InsufficientSize.ToDisplayString()}");
            node.AddStatement($"{result} = ({TypeSymbol}){source}[0]");
            node.AddStatement($"{readBytes} = 1");
            node.AddStatement($"return {DeserializeResult.Success.ToDisplayString()}");
            return node.ToString();
        }

        public override string TryDeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<object> result) {
            var node = new CodeNodeBuilder();
            // Since cursor always garuantee some space left in the span, unless it's completed (point to end)
            node.AddStatement($"if ({source}.Completed) return {DeserializeResult.InsufficientSize.ToDisplayString()}");
            node.AddStatement($"{result} = ({TypeSymbol}){source}.UnreadSpan[0]");
            node.AddStatement($"{source}.Advance(1)");
            node.AddStatement($"return {DeserializeResult.Success.ToDisplayString()}");
            return node.ToString();
        }

        public override string DeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes) {
            var node = new CodeNodeBuilder();
            node.AddStatement($"if ({source}.Length < 1) {ThrowArgOutOfRange(source.ToString())}");
            node.AddStatement($"{readBytes} = 1");
            node.AddStatement($"return ({TypeSymbol}){source}[0]");
            return node.ToString();
        }

        public override string DeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source) {
            var node = new CodeNodeBuilder();
            node.AddStatement($"if ({source}.IsCompleted) {ThrowArgOutOfRange(source.ToString())}");
            node.AddStatement($"var tempResult = ({TypeSymbol}){source}.UnreadSpan[0]");
            node.AddStatement($"{source}.Advance(1)");
            node.AddStatement($"return tempResult");
            return node.ToString();
        }

        public override string GetLengthFunction(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName) => "return 1;";
    }
}
