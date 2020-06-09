using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;

[assembly: BinaryPackerAssemblyEntryPointAttribute(typeof(AssemblyFormatProvider_Decuplr_Serialization_Binary_ConsoleTests_41QRTFMAAACQ2))]

namespace Decuplr.Serialization.Binary.Internal {
[GeneratedCode ("Decuplr.Serialization.Binary.SourceGenerator", "1.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
internal partial class AssemblyFormatProvider_Decuplr_Serialization_Binary_ConsoleTests_41QRTFMAAACQ2 : AssemblyPackerEntryPoint {

#region FoamStruct
internal struct FoamStruct_8V1SAAV4UQJV3_TypeParserArgs{
public TypeParser<int> Parser_0_0;
public TypeParser<long> Parser_1_0;
}


private sealed class FoamStruct_8V1SAAV4UQJV3_TypeParser : TypeParser <Decuplr.Serialization.Binary.ConsoleTests.FoamStruct>{
private readonly FoamStruct_8V1SAAV4UQJV3_TypeParserArgs ParserCollections;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
// For plain default namespace, useful if type is sealed
public FoamStruct_8V1SAAV4UQJV3_TypeParser (INamespaceRoot root) : this(root.CreateDiscovery()){
}
// This is used for TryProvideParser pattern if not sealed
public FoamStruct_8V1SAAV4UQJV3_TypeParser (IParserDiscovery parser, out bool isSuccess){
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
isSuccess = true;
}
// This is used for ProvideParser pattern if not sealed
public FoamStruct_8V1SAAV4UQJV3_TypeParser (IParserDiscovery parser){
fixedSize = 0;
var parserSpace_0 = parser;
ParserCollections.Parser_0_0 = parserSpace_0.GetParser<int>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
var parserSpace_1 = parser;
ParserCollections.Parser_1_0 = parserSpace_1.GetParser<long>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_1_0.FixedSize;
}
}

// Deserialization Function
private static DeserializeResult TryCreateType(in FoamStruct_8V1SAAV4UQJV3_TypeParserArgs parsers, ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.FoamStruct result){
result = new Decuplr.Serialization.Binary.ConsoleTests.FoamStruct (parsers, span, out readBytes, out var deserializeResult);
return deserializeResult;
}


// TrySerialization Function
private static bool TrySerializeType(in FoamStruct_8V1SAAV4UQJV3_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.FoamStruct value, Span<byte> destination, out int writtenBytes){
return Decuplr.Serialization.Binary.ConsoleTests.FoamStruct.___generated__no_invoke_FoamStruct_TrySerializer (in parsers, value, destination, out writtenBytes);
}


// Serialization Function
private static int SerializeType(in FoamStruct_8V1SAAV4UQJV3_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.FoamStruct value, Span<byte> destination){
return Decuplr.Serialization.Binary.ConsoleTests.FoamStruct.___generated__no_invoke_FoamStruct_Serializer (in parsers, value, destination);
}


// GetLength Function
private static int GetSerializedBinaryLength(in FoamStruct_8V1SAAV4UQJV3_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.FoamStruct value){
return Decuplr.Serialization.Binary.ConsoleTests.FoamStruct.___generated__no_inoke_FoamStruct_GetLength (in parsers, value);
}


// TryDeserialize Function
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.FoamStruct result){
return TryCreateType(in ParserCollections, span, out readBytes, out result);
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.FoamStruct value){
return GetSerializedBinaryLength(in ParserCollections, value);
}

// TrySerialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.FoamStruct value, Span<byte> destination, out int writtenBytes){
return TrySerializeType(in ParserCollections, value, destination, out writtenBytes);
}

// Serialize Function
public override int Serialize(Decuplr.Serialization.Binary.ConsoleTests.FoamStruct value, Span<byte> destination){
return SerializeType(in ParserCollections, value, destination);
}
}

private class FoamStruct_8V1SAAV4UQJV3_TypeParser_Provider : IParserProvider<Decuplr.Serialization.Binary.ConsoleTests.FoamStruct>{
public TypeParser<Decuplr.Serialization.Binary.ConsoleTests.FoamStruct> ProvideParser(IParserDiscovery discovery){
return new FoamStruct_8V1SAAV4UQJV3_TypeParser(discovery);
}
public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<Decuplr.Serialization.Binary.ConsoleTests.FoamStruct> parser){
parser = new FoamStruct_8V1SAAV4UQJV3_TypeParser(discovery, out var isSuccess);
if(!isSuccess){
parser = null;
}
return isSuccess;
}
}


#endregion

#region TestClass3
internal struct TestClass3_V3UCWTP448Y71_TypeParserArgs{
public TypeParser<Decuplr.Serialization.Binary.ConsoleTests.FoamStruct> Parser_0_0;
public TypeParser<System.Collections.Generic.List<byte>> Parser_1_0;
public TypeParser<(bool IsValid, bool IsCurrent, bool IsCompressed)> Parser_2_0;
}


private sealed class TestClass3_V3UCWTP448Y71_TypeParser : TypeParser <Decuplr.Serialization.Binary.ConsoleTests.TestClass3>{
private readonly TestClass3_V3UCWTP448Y71_TypeParserArgs ParserCollections;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
// For plain default namespace, useful if type is sealed
public TestClass3_V3UCWTP448Y71_TypeParser (INamespaceRoot root) : this(root.CreateDiscovery()){
}
// This is used for TryProvideParser pattern if not sealed
public TestClass3_V3UCWTP448Y71_TypeParser (IParserDiscovery parser, out bool isSuccess){
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
isSuccess = true;
}
// This is used for ProvideParser pattern if not sealed
public TestClass3_V3UCWTP448Y71_TypeParser (IParserDiscovery parser){
fixedSize = 0;
var parserSpace_0 = parser;
ParserCollections.Parser_0_0 = parserSpace_0.GetParser<Decuplr.Serialization.Binary.ConsoleTests.FoamStruct>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
var parserSpace_1 = parser;
ParserCollections.Parser_1_0 = parserSpace_1.GetParser<System.Collections.Generic.List<byte>>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_1_0.FixedSize;
}
var parserSpace_2 = parser;
ParserCollections.Parser_2_0 = parserSpace_2.GetParser<(bool IsValid, bool IsCurrent, bool IsCompressed)>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_2_0.FixedSize;
}
}

// Deserialization Function
private static DeserializeResult TryCreateType(in TestClass3_V3UCWTP448Y71_TypeParserArgs parsers, ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestClass3 result){
result = new Decuplr.Serialization.Binary.ConsoleTests.TestClass3 (parsers, span, out readBytes, out var deserializeResult);
return deserializeResult;
}


// TrySerialization Function
private static bool TrySerializeType(in TestClass3_V3UCWTP448Y71_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestClass3 value, Span<byte> destination, out int writtenBytes){
return Decuplr.Serialization.Binary.ConsoleTests.TestClass3.___generated__no_invoke_TestClass3_TrySerializer (in parsers, value, destination, out writtenBytes);
}


// Serialization Function
private static int SerializeType(in TestClass3_V3UCWTP448Y71_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestClass3 value, Span<byte> destination){
return Decuplr.Serialization.Binary.ConsoleTests.TestClass3.___generated__no_invoke_TestClass3_Serializer (in parsers, value, destination);
}


// GetLength Function
private static int GetSerializedBinaryLength(in TestClass3_V3UCWTP448Y71_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestClass3 value){
return Decuplr.Serialization.Binary.ConsoleTests.TestClass3.___generated__no_inoke_TestClass3_GetLength (in parsers, value);
}


// TryDeserialize Function
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestClass3 result){
return TryCreateType(in ParserCollections, span, out readBytes, out result);
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.TestClass3 value){
return GetSerializedBinaryLength(in ParserCollections, value);
}

// TrySerialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.TestClass3 value, Span<byte> destination, out int writtenBytes){
return TrySerializeType(in ParserCollections, value, destination, out writtenBytes);
}

// Serialize Function
public override int Serialize(Decuplr.Serialization.Binary.ConsoleTests.TestClass3 value, Span<byte> destination){
return SerializeType(in ParserCollections, value, destination);
}
}

private class TestClass3_V3UCWTP448Y71_TypeParser_Provider : IParserProvider<Decuplr.Serialization.Binary.ConsoleTests.TestClass3>{
public TypeParser<Decuplr.Serialization.Binary.ConsoleTests.TestClass3> ProvideParser(IParserDiscovery discovery){
return new TestClass3_V3UCWTP448Y71_TypeParser(discovery);
}
public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<Decuplr.Serialization.Binary.ConsoleTests.TestClass3> parser){
parser = new TestClass3_V3UCWTP448Y71_TypeParser(discovery, out var isSuccess);
if(!isSuccess){
parser = null;
}
return isSuccess;
}
}


#endregion

#region NestedClassTarget
internal struct NestedClassTarget_7VS9HJUOT01A2_TypeParserArgs{
public TypeParser<int> Parser_0_0;
public TypeParser<int> Parser_1_0;
}


private sealed class NestedClassTarget_7VS9HJUOT01A2_TypeParser : TypeParser <Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget>{
private readonly NestedClassTarget_7VS9HJUOT01A2_TypeParserArgs ParserCollections;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
// For plain default namespace, useful if type is sealed
public NestedClassTarget_7VS9HJUOT01A2_TypeParser (INamespaceRoot root) : this(root.CreateDiscovery()){
}
// This is used for TryProvideParser pattern if not sealed
public NestedClassTarget_7VS9HJUOT01A2_TypeParser (IParserDiscovery parser, out bool isSuccess){
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
isSuccess = true;
}
// This is used for ProvideParser pattern if not sealed
public NestedClassTarget_7VS9HJUOT01A2_TypeParser (IParserDiscovery parser){
fixedSize = 0;
var parserSpace_0 = parser;
ParserCollections.Parser_0_0 = parserSpace_0.GetParser<int>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
var parserSpace_1 = parser;
ParserCollections.Parser_1_0 = parserSpace_1.GetParser<int>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_1_0.FixedSize;
}
}

// Deserialization Function
private static DeserializeResult TryCreateType(in NestedClassTarget_7VS9HJUOT01A2_TypeParserArgs parsers, ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget result){
result = new Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget (parsers, span, out readBytes, out var deserializeResult);
return deserializeResult;
}


// TrySerialization Function
private static bool TrySerializeType(in NestedClassTarget_7VS9HJUOT01A2_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget value, Span<byte> destination, out int writtenBytes){
return Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.___generated__no_invoke_NestedClassTarget_TrySerializer (in parsers, value, destination, out writtenBytes);
}


// Serialization Function
private static int SerializeType(in NestedClassTarget_7VS9HJUOT01A2_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget value, Span<byte> destination){
return Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.___generated__no_invoke_NestedClassTarget_Serializer (in parsers, value, destination);
}


// GetLength Function
private static int GetSerializedBinaryLength(in NestedClassTarget_7VS9HJUOT01A2_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget value){
return Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.___generated__no_inoke_NestedClassTarget_GetLength (in parsers, value);
}


// TryDeserialize Function
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget result){
return TryCreateType(in ParserCollections, span, out readBytes, out result);
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget value){
return GetSerializedBinaryLength(in ParserCollections, value);
}

// TrySerialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget value, Span<byte> destination, out int writtenBytes){
return TrySerializeType(in ParserCollections, value, destination, out writtenBytes);
}

// Serialize Function
public override int Serialize(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget value, Span<byte> destination){
return SerializeType(in ParserCollections, value, destination);
}
}

private class NestedClassTarget_7VS9HJUOT01A2_TypeParser_Provider : IParserProvider<Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget>{
public TypeParser<Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget> ProvideParser(IParserDiscovery discovery){
return new NestedClassTarget_7VS9HJUOT01A2_TypeParser(discovery);
}
public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget> parser){
parser = new NestedClassTarget_7VS9HJUOT01A2_TypeParser(discovery, out var isSuccess);
if(!isSuccess){
parser = null;
}
return isSuccess;
}
}


#endregion

#region NestedClassTarget3
internal struct NestedClassTarget3_08XCBS51PJUZ_TypeParserArgs{
public TypeParser<int> Parser_0_0;
}


private sealed class NestedClassTarget3_08XCBS51PJUZ_TypeParser : TypeParser <Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3>{
private readonly NestedClassTarget3_08XCBS51PJUZ_TypeParserArgs ParserCollections;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
// For plain default namespace, useful if type is sealed
public NestedClassTarget3_08XCBS51PJUZ_TypeParser (INamespaceRoot root) : this(root.CreateDiscovery()){
}
// This is used for TryProvideParser pattern if not sealed
public NestedClassTarget3_08XCBS51PJUZ_TypeParser (IParserDiscovery parser, out bool isSuccess){
isSuccess = false;
fixedSize = 0;
var parserSpace_0 = parser;
if (!parserSpace_0.TryGetParser(out ParserCollections.Parser_0_0)){
return;
}
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
isSuccess = true;
}
// This is used for ProvideParser pattern if not sealed
public NestedClassTarget3_08XCBS51PJUZ_TypeParser (IParserDiscovery parser){
fixedSize = 0;
var parserSpace_0 = parser;
ParserCollections.Parser_0_0 = parserSpace_0.GetParser<int>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
}

// Deserialization Function
private static DeserializeResult TryCreateType(in NestedClassTarget3_08XCBS51PJUZ_TypeParserArgs parsers, ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 result){
result = new Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 (parsers, span, out readBytes, out var deserializeResult);
return deserializeResult;
}


// TrySerialization Function
private static bool TrySerializeType(in NestedClassTarget3_08XCBS51PJUZ_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 value, Span<byte> destination, out int writtenBytes){
return Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3.___generated__no_invoke_NestedClassTarget3_TrySerializer (in parsers, value, destination, out writtenBytes);
}


// Serialization Function
private static int SerializeType(in NestedClassTarget3_08XCBS51PJUZ_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 value, Span<byte> destination){
return Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3.___generated__no_invoke_NestedClassTarget3_Serializer (in parsers, value, destination);
}


// GetLength Function
private static int GetSerializedBinaryLength(in NestedClassTarget3_08XCBS51PJUZ_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 value){
return Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3.___generated__no_inoke_NestedClassTarget3_GetLength (in parsers, value);
}


// TryDeserialize Function
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 result){
return TryCreateType(in ParserCollections, span, out readBytes, out result);
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 value){
return GetSerializedBinaryLength(in ParserCollections, value);
}

// TrySerialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 value, Span<byte> destination, out int writtenBytes){
return TrySerializeType(in ParserCollections, value, destination, out writtenBytes);
}

// Serialize Function
public override int Serialize(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 value, Span<byte> destination){
return SerializeType(in ParserCollections, value, destination);
}
}

private class NestedClassTarget3_08XCBS51PJUZ_TypeParser_Provider : IParserProvider<Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3>{
public TypeParser<Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3> ProvideParser(IParserDiscovery discovery){
return new NestedClassTarget3_08XCBS51PJUZ_TypeParser(discovery);
}
public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3> parser){
parser = new NestedClassTarget3_08XCBS51PJUZ_TypeParser(discovery, out var isSuccess);
if(!isSuccess){
parser = null;
}
return isSuccess;
}
}


#endregion

#region TestStruct
internal struct TestStruct_TNWLILY1TQ2B3_TypeParserArgs{
public TypeParser<int> Parser_0_0;
public TypeParser<Decuplr.Serialization.Binary.ConsoleTests.TestClass3> Parser_1_0;
}


private sealed class TestStruct_TNWLILY1TQ2B3_TypeParser : TypeParser <Decuplr.Serialization.Binary.ConsoleTests.TestStruct>{
private readonly TestStruct_TNWLILY1TQ2B3_TypeParserArgs ParserCollections;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
// For plain default namespace, useful if type is sealed
public TestStruct_TNWLILY1TQ2B3_TypeParser (INamespaceRoot root) : this(root.CreateDiscovery()){
}
// This is used for TryProvideParser pattern if not sealed
public TestStruct_TNWLILY1TQ2B3_TypeParser (IParserDiscovery parser, out bool isSuccess){
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
isSuccess = true;
}
// This is used for ProvideParser pattern if not sealed
public TestStruct_TNWLILY1TQ2B3_TypeParser (IParserDiscovery parser){
fixedSize = 0;
var parserSpace_0 = parser;
ParserCollections.Parser_0_0 = parserSpace_0.GetParser<int>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
var parserSpace_1 = parser;
ParserCollections.Parser_1_0 = parserSpace_1.GetParser<Decuplr.Serialization.Binary.ConsoleTests.TestClass3>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_1_0.FixedSize;
}
}

// Deserialization Function
private static DeserializeResult TryCreateType(in TestStruct_TNWLILY1TQ2B3_TypeParserArgs parsers, ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestStruct result){
result = new Decuplr.Serialization.Binary.ConsoleTests.TestStruct (parsers, span, out readBytes, out var deserializeResult);
return deserializeResult;
}


// TrySerialization Function
private static bool TrySerializeType(in TestStruct_TNWLILY1TQ2B3_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestStruct value, Span<byte> destination, out int writtenBytes){
return Decuplr.Serialization.Binary.ConsoleTests.TestStruct.___generated__no_invoke_TestStruct_TrySerializer (in parsers, value, destination, out writtenBytes);
}


// Serialization Function
private static int SerializeType(in TestStruct_TNWLILY1TQ2B3_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestStruct value, Span<byte> destination){
return Decuplr.Serialization.Binary.ConsoleTests.TestStruct.___generated__no_invoke_TestStruct_Serializer (in parsers, value, destination);
}


// GetLength Function
private static int GetSerializedBinaryLength(in TestStruct_TNWLILY1TQ2B3_TypeParserArgs parsers, Decuplr.Serialization.Binary.ConsoleTests.TestStruct value){
return Decuplr.Serialization.Binary.ConsoleTests.TestStruct.___generated__no_inoke_TestStruct_GetLength (in parsers, value);
}


// TryDeserialize Function
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestStruct result){
return TryCreateType(in ParserCollections, span, out readBytes, out result);
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.TestStruct value){
return GetSerializedBinaryLength(in ParserCollections, value);
}

// TrySerialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.TestStruct value, Span<byte> destination, out int writtenBytes){
return TrySerializeType(in ParserCollections, value, destination, out writtenBytes);
}

// Serialize Function
public override int Serialize(Decuplr.Serialization.Binary.ConsoleTests.TestStruct value, Span<byte> destination){
return SerializeType(in ParserCollections, value, destination);
}
}

private class TestStruct_TNWLILY1TQ2B3_TypeParser_Provider : IParserProvider<Decuplr.Serialization.Binary.ConsoleTests.TestStruct>{
public TypeParser<Decuplr.Serialization.Binary.ConsoleTests.TestStruct> ProvideParser(IParserDiscovery discovery){
return new TestStruct_TNWLILY1TQ2B3_TypeParser(discovery);
}
public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<Decuplr.Serialization.Binary.ConsoleTests.TestStruct> parser){
parser = new TestStruct_TNWLILY1TQ2B3_TypeParser(discovery, out var isSuccess);
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
// Generated Count : 5
namespaces["Default"].AddParserProvider<FoamStruct_8V1SAAV4UQJV3_TypeParser_Provider, Decuplr.Serialization.Binary.ConsoleTests.FoamStruct>(new FoamStruct_8V1SAAV4UQJV3_TypeParser_Provider());
namespaces["Default"].AddParserProvider<TestClass3_V3UCWTP448Y71_TypeParser_Provider, Decuplr.Serialization.Binary.ConsoleTests.TestClass3>(new TestClass3_V3UCWTP448Y71_TypeParser_Provider());
namespaces["Default"].AddParserProvider<NestedClassTarget_7VS9HJUOT01A2_TypeParser_Provider, Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget>(new NestedClassTarget_7VS9HJUOT01A2_TypeParser_Provider());
namespaces["Default"].AddParserProvider<NestedClassTarget3_08XCBS51PJUZ_TypeParser_Provider, Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3>(new NestedClassTarget3_08XCBS51PJUZ_TypeParser_Provider());
namespaces["Default"].AddParserProvider<TestStruct_TNWLILY1TQ2B3_TypeParser_Provider, Decuplr.Serialization.Binary.ConsoleTests.TestStruct>(new TestStruct_TNWLILY1TQ2B3_TypeParser_Provider());
}
}
}
