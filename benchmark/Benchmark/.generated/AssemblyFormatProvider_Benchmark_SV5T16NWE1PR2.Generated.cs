using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;

[assembly: BinaryPackerAssemblyEntryPointAttribute(typeof(AssemblyFormatProvider_Benchmark_SV5T16NWE1PR2))]

namespace Decuplr.Serialization.Binary.Internal {
[GeneratedCode ("Decuplr.Serialization.Binary.SourceGenerator", "1.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
internal partial class AssemblyFormatProvider_Benchmark_SV5T16NWE1PR2 : AssemblyPackerEntryPoint {

#region SimplePoco
internal struct SimplePoco_DXLT9ZBUDDGJ3_TypeParserArgs{
public TypeParser<long> Parser_0_0;
public TypeParser<long> Parser_1_0;
public TypeParser<long> Parser_2_0;
public TypeParser<System.DateTime> Parser_3_0;
public TypeParser<System.DateTime> Parser_4_0;
public TypeParser<System.DateTime> Parser_5_0;
public TypeParser<long> Parser_6_0;
}


private sealed class SimplePoco_DXLT9ZBUDDGJ3_TypeParser : TypeParser <Benchmark.Models.SimplePoco>{
private readonly SimplePoco_DXLT9ZBUDDGJ3_TypeParserArgs ParserCollections;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
// For plain default namespace, useful if type is sealed
public SimplePoco_DXLT9ZBUDDGJ3_TypeParser (INamespaceRoot root) : this(root.CreateDiscovery()){
}
// This is used for TryProvideParser pattern if not sealed
public SimplePoco_DXLT9ZBUDDGJ3_TypeParser (IParserDiscovery parser, out bool isSuccess){
isSuccess = false;
fixedSize = 0;
var parserSpace_0 = parser;
if (!parserSpace_0.TryGetParser(out ParserCollections.Parser_0_0)){
return;
}
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
var parserSpace_1 = parser;
if (!parserSpace_1.TryGetParser(out ParserCollections.Parser_1_0)){
return;
}
if (fixedSize != null){
fixedSize += ParserCollections.Parser_1_0.FixedSize;
}
var parserSpace_2 = parser;
if (!parserSpace_2.TryGetParser(out ParserCollections.Parser_2_0)){
return;
}
if (fixedSize != null){
fixedSize += ParserCollections.Parser_2_0.FixedSize;
}
var parserSpace_3 = parser;
if (!parserSpace_3.TryGetParser(out ParserCollections.Parser_3_0)){
return;
}
if (fixedSize != null){
fixedSize += ParserCollections.Parser_3_0.FixedSize;
}
var parserSpace_4 = parser;
if (!parserSpace_4.TryGetParser(out ParserCollections.Parser_4_0)){
return;
}
if (fixedSize != null){
fixedSize += ParserCollections.Parser_4_0.FixedSize;
}
var parserSpace_5 = parser;
if (!parserSpace_5.TryGetParser(out ParserCollections.Parser_5_0)){
return;
}
if (fixedSize != null){
fixedSize += ParserCollections.Parser_5_0.FixedSize;
}
var parserSpace_6 = parser;
if (!parserSpace_6.TryGetParser(out ParserCollections.Parser_6_0)){
return;
}
if (fixedSize != null){
fixedSize += ParserCollections.Parser_6_0.FixedSize;
}
isSuccess = true;
}
// This is used for ProvideParser pattern if not sealed
public SimplePoco_DXLT9ZBUDDGJ3_TypeParser (IParserDiscovery parser){
fixedSize = 0;
var parserSpace_0 = parser;
ParserCollections.Parser_0_0 = parserSpace_0.GetParser<long>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
var parserSpace_1 = parser;
ParserCollections.Parser_1_0 = parserSpace_1.GetParser<long>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_1_0.FixedSize;
}
var parserSpace_2 = parser;
ParserCollections.Parser_2_0 = parserSpace_2.GetParser<long>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_2_0.FixedSize;
}
var parserSpace_3 = parser;
ParserCollections.Parser_3_0 = parserSpace_3.GetParser<System.DateTime>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_3_0.FixedSize;
}
var parserSpace_4 = parser;
ParserCollections.Parser_4_0 = parserSpace_4.GetParser<System.DateTime>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_4_0.FixedSize;
}
var parserSpace_5 = parser;
ParserCollections.Parser_5_0 = parserSpace_5.GetParser<System.DateTime>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_5_0.FixedSize;
}
var parserSpace_6 = parser;
ParserCollections.Parser_6_0 = parserSpace_6.GetParser<long>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_6_0.FixedSize;
}
}

// Deserialization Function
private static DeserializeResult TryCreateType(in SimplePoco_DXLT9ZBUDDGJ3_TypeParserArgs parsers, ReadOnlySpan<byte> span, out int readBytes, out Benchmark.Models.SimplePoco result){
result = new Benchmark.Models.SimplePoco (parsers, span, out readBytes, out var deserializeResult);
return deserializeResult;
}


// TrySerialization Function
private static bool TrySerializeType(in SimplePoco_DXLT9ZBUDDGJ3_TypeParserArgs parsers, Benchmark.Models.SimplePoco value, Span<byte> destination, out int writtenBytes){
return Benchmark.Models.SimplePoco.___generated__no_invoke_SimplePoco_TrySerializer (in parsers, value, destination, out writtenBytes);
}


// Serialization Function
private static int SerializeType(in SimplePoco_DXLT9ZBUDDGJ3_TypeParserArgs parsers, Benchmark.Models.SimplePoco value, Span<byte> destination){
return Benchmark.Models.SimplePoco.___generated__no_invoke_SimplePoco_Serializer (in parsers, value, destination);
}


// GetLength Function
private static int GetSerializedBinaryLength(in SimplePoco_DXLT9ZBUDDGJ3_TypeParserArgs parsers, Benchmark.Models.SimplePoco value){
return Benchmark.Models.SimplePoco.___generated__no_inoke_SimplePoco_GetLength (in parsers, value);
}


// TryDeserialize Function
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Benchmark.Models.SimplePoco result){
return TryCreateType(in ParserCollections, span, out readBytes, out result);
}

// GetBinaryLength Function
public override int GetBinaryLength(Benchmark.Models.SimplePoco value){
return GetSerializedBinaryLength(in ParserCollections, value);
}

// TrySerialize Function
public override bool TrySerialize(Benchmark.Models.SimplePoco value, Span<byte> destination, out int writtenBytes){
return TrySerializeType(in ParserCollections, value, destination, out writtenBytes);
}

// Serialize Function
public override int Serialize(Benchmark.Models.SimplePoco value, Span<byte> destination){
return SerializeType(in ParserCollections, value, destination);
}
}

private class SimplePoco_DXLT9ZBUDDGJ3_TypeParser_Provider : IParserProvider<Benchmark.Models.SimplePoco>{
public TypeParser<Benchmark.Models.SimplePoco> ProvideParser(IParserDiscovery discovery){
return new SimplePoco_DXLT9ZBUDDGJ3_TypeParser(discovery);
}
public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<Benchmark.Models.SimplePoco> parser){
parser = new SimplePoco_DXLT9ZBUDDGJ3_TypeParser(discovery, out var isSuccess);
if(!isSuccess){
parser = null;
}
return isSuccess;
}
}


#endregion
public override void LoadContext (INamespaceRoot root){
var namespaces = new Dictionary<string, IMutableNamespace>();
namespaces[""] = root.DefaultNamespace;
namespaces["default"] = root.DefaultNamespace;
namespaces["Default"] = root.DefaultNamespace;
namespaces["DEFAULT"] = root.DefaultNamespace;
// Generated Count : 1
namespaces["Default"].AddParserProvider<SimplePoco_DXLT9ZBUDDGJ3_TypeParser_Provider, Benchmark.Models.SimplePoco>(new SimplePoco_DXLT9ZBUDDGJ3_TypeParser_Provider());
}
}
}
