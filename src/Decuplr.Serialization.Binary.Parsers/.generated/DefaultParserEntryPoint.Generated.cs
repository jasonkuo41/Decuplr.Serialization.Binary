using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;


namespace Decuplr.Serialization.Binary.Internal {
[GeneratedCode ("Decuplr.Serialization.Binary.SourceGenerator", "1.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
internal partial class DefaultParserEntryPoint : AssemblyPackerEntryPoint {

#region DateTimeShim
internal struct DateTimeShim_SJ32A9XBUFRV_TypeParserArgs{
public TypeParser<long> Parser_0_0;
}


private sealed class DateTimeShim_SJ32A9XBUFRV_TypeParser : TypeParser <Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim>{
private readonly DateTimeShim_SJ32A9XBUFRV_TypeParserArgs ParserCollections;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
// For plain default namespace, useful if type is sealed
public DateTimeShim_SJ32A9XBUFRV_TypeParser (INamespaceRoot root) : this(root.CreateDiscovery()){
}
// This is used for TryProvideParser pattern if not sealed
public DateTimeShim_SJ32A9XBUFRV_TypeParser (IParserDiscovery parser, out bool isSuccess){
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
public DateTimeShim_SJ32A9XBUFRV_TypeParser (IParserDiscovery parser){
fixedSize = 0;
var parserSpace_0 = parser;
ParserCollections.Parser_0_0 = parserSpace_0.GetParser<long>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
}

// Deserialization Function
private static DeserializeResult TryCreateType(in DateTimeShim_SJ32A9XBUFRV_TypeParserArgs parsers, ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim result){
result = new Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim (parsers, span, out readBytes, out var deserializeResult);
return deserializeResult;
}


// TrySerialization Function
private static bool TrySerializeType(in DateTimeShim_SJ32A9XBUFRV_TypeParserArgs parsers, Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim value, Span<byte> destination, out int writtenBytes){
return Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim.___generated__no_invoke_DateTimeShim_TrySerializer (in parsers, value, destination, out writtenBytes);
}


// Serialization Function
private static int SerializeType(in DateTimeShim_SJ32A9XBUFRV_TypeParserArgs parsers, Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim value, Span<byte> destination){
return Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim.___generated__no_invoke_DateTimeShim_Serializer (in parsers, value, destination);
}


// GetLength Function
private static int GetSerializedBinaryLength(in DateTimeShim_SJ32A9XBUFRV_TypeParserArgs parsers, Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim value){
return Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim.___generated__no_inoke_DateTimeShim_GetLength (in parsers, value);
}


// TryDeserialize Function
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim result){
return TryCreateType(in ParserCollections, span, out readBytes, out result);
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim value){
return GetSerializedBinaryLength(in ParserCollections, value);
}

// TrySerialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim value, Span<byte> destination, out int writtenBytes){
return TrySerializeType(in ParserCollections, value, destination, out writtenBytes);
}

// Serialize Function
public override int Serialize(Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim value, Span<byte> destination){
return SerializeType(in ParserCollections, value, destination);
}
}

private class DateTimeShim_SJ32A9XBUFRV_TypeParser_Provider : IParserProvider<Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim>{
public TypeParser<Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim> ProvideParser(IParserDiscovery discovery){
return new DateTimeShim_SJ32A9XBUFRV_TypeParser(discovery);
}
public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim> parser){
parser = new DateTimeShim_SJ32A9XBUFRV_TypeParser(discovery, out var isSuccess);
if(!isSuccess){
parser = null;
}
return isSuccess;
}
}


#endregion

#region System.DateTime
private class TypeParserWrapper_DateTime_KB93HSXXRNTV2_As_DateTimeShim_SJ32A9XBUFRV : TypeParser<System.DateTime>{
private readonly TypeParser<Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim> Parser;
public TypeParserWrapper_DateTime_KB93HSXXRNTV2_As_DateTimeShim_SJ32A9XBUFRV (IParserDiscovery discovery){
Parser = discovery.GetParser<Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim>();
}
public TypeParserWrapper_DateTime_KB93HSXXRNTV2_As_DateTimeShim_SJ32A9XBUFRV (IParserDiscovery discovery, out bool isSuccess){
isSuccess = discovery.TryGetParser<Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim>(out Parser);
}
public override int? FixedSize => Parser.FixedSize;
public override bool TrySerialize(System.DateTime value, Span<byte> destination, out int writtenBytes) => Parser.TrySerialize(new Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim(value), destination, out writtenBytes);
public override int Serialize(System.DateTime value, Span<byte> destination) => Parser.Serialize(new Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim(value), destination);
public override int GetBinaryLength(System.DateTime value) => Parser.GetBinaryLength(new Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim(value));
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out System.DateTime result){
var deserializeResult = Parser.TryDeserialize(span, out readBytes, out var ogResult);
result = ogResult.ConvertTo();
return deserializeResult;
}
}

private class TypeParserWrapper_DateTime_KB93HSXXRNTV2_As_DateTimeShim_SJ32A9XBUFRV_Provider : IParserProvider<System.DateTime>{
public TypeParser<System.DateTime> ProvideParser(IParserDiscovery discovery){
return new TypeParserWrapper_DateTime_KB93HSXXRNTV2_As_DateTimeShim_SJ32A9XBUFRV(discovery);
}
public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<System.DateTime> parser){
parser = new TypeParserWrapper_DateTime_KB93HSXXRNTV2_As_DateTimeShim_SJ32A9XBUFRV(discovery, out var isSuccess);
if(!isSuccess){
parser = null;
}
return isSuccess;
}
}

#endregion

#region LazyShim
internal struct LazyShim_XAF96RTGJHE73_TypeParserArgs<T>{
public TypeParser<T> Parser_0_0;
}


private class LazyShim_XAF96RTGJHE73_TypeParser_GenericProvider<T> : GenericParserProvider{
private sealed class LazyShim_XAF96RTGJHE73_TypeParser : TypeParser <Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>>{
private readonly LazyShim_XAF96RTGJHE73_TypeParserArgs<T> ParserCollections;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
// For plain default namespace, useful if type is sealed
public LazyShim_XAF96RTGJHE73_TypeParser (INamespaceRoot root) : this(root.CreateDiscovery()){
}
// This is used for TryProvideParser pattern if not sealed
public LazyShim_XAF96RTGJHE73_TypeParser (IParserDiscovery parser, out bool isSuccess){
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
public LazyShim_XAF96RTGJHE73_TypeParser (IParserDiscovery parser){
fixedSize = 0;
var parserSpace_0 = parser;
ParserCollections.Parser_0_0 = parserSpace_0.GetParser<T>();
if (fixedSize != null){
fixedSize += ParserCollections.Parser_0_0.FixedSize;
}
}

// Deserialization Function
private static DeserializeResult TryCreateType(in LazyShim_XAF96RTGJHE73_TypeParserArgs<T> parsers, ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T> result){
result = new Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T> (parsers, span, out readBytes, out var deserializeResult);
return deserializeResult;
}


// TrySerialization Function
private static bool TrySerializeType(in LazyShim_XAF96RTGJHE73_TypeParserArgs<T> parsers, Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T> value, Span<byte> destination, out int writtenBytes){
return Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>.___generated__no_invoke_LazyShim_TrySerializer (in parsers, value, destination, out writtenBytes);
}


// Serialization Function
private static int SerializeType(in LazyShim_XAF96RTGJHE73_TypeParserArgs<T> parsers, Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T> value, Span<byte> destination){
return Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>.___generated__no_invoke_LazyShim_Serializer (in parsers, value, destination);
}


// GetLength Function
private static int GetSerializedBinaryLength(in LazyShim_XAF96RTGJHE73_TypeParserArgs<T> parsers, Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T> value){
return Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>.___generated__no_inoke_LazyShim_GetLength (in parsers, value);
}


// TryDeserialize Function
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T> result){
return TryCreateType(in ParserCollections, span, out readBytes, out result);
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T> value){
return GetSerializedBinaryLength(in ParserCollections, value);
}

// TrySerialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T> value, Span<byte> destination, out int writtenBytes){
return TrySerializeType(in ParserCollections, value, destination, out writtenBytes);
}

// Serialize Function
public override int Serialize(Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T> value, Span<byte> destination){
return SerializeType(in ParserCollections, value, destination);
}
}

public override TypeParser ProvideParser(IParserDiscovery discovery){
return new LazyShim_XAF96RTGJHE73_TypeParser(discovery);
}
public override bool TryProvideParser(IParserDiscovery discovery, out TypeParser parser){
parser = new LazyShim_XAF96RTGJHE73_TypeParser(discovery, out var isSuccess);
if(!isSuccess){
parser = null;
}
return isSuccess;
}
}


#endregion

#region System.Lazy<>
private class TypeParserWrapper_Lazy_VPZZSHF2MTVH_As_LazyShim_XAF96RTGJHE73_GenericProvider<T> : GenericParserProvider{
private class TypeParserWrapper_Lazy_VPZZSHF2MTVH_As_LazyShim_XAF96RTGJHE73 : TypeParser<System.Lazy<T>>{
private readonly TypeParser<Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>> Parser;
public TypeParserWrapper_Lazy_VPZZSHF2MTVH_As_LazyShim_XAF96RTGJHE73 (IParserDiscovery discovery){
Parser = discovery.GetParser<Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>>();
}
public TypeParserWrapper_Lazy_VPZZSHF2MTVH_As_LazyShim_XAF96RTGJHE73 (IParserDiscovery discovery, out bool isSuccess){
isSuccess = discovery.TryGetParser<Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>>(out Parser);
}
public override int? FixedSize => Parser.FixedSize;
public override bool TrySerialize(System.Lazy<T> value, Span<byte> destination, out int writtenBytes) => Parser.TrySerialize(new Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>(value), destination, out writtenBytes);
public override int Serialize(System.Lazy<T> value, Span<byte> destination) => Parser.Serialize(new Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>(value), destination);
public override int GetBinaryLength(System.Lazy<T> value) => Parser.GetBinaryLength(new Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<T>(value));
public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out System.Lazy<T> result){
var deserializeResult = Parser.TryDeserialize(span, out readBytes, out var ogResult);
result = ogResult.ConvertTo();
return deserializeResult;
}
}

public override TypeParser ProvideParser(IParserDiscovery discovery){
return new TypeParserWrapper_Lazy_VPZZSHF2MTVH_As_LazyShim_XAF96RTGJHE73(discovery);
}
public override bool TryProvideParser(IParserDiscovery discovery, out TypeParser parser){
parser = new TypeParserWrapper_Lazy_VPZZSHF2MTVH_As_LazyShim_XAF96RTGJHE73(discovery, out var isSuccess);
if(!isSuccess){
parser = null;
}
return isSuccess;
}
}

#endregion

#region BooleanParser
#endregion

#region ByteParser
#endregion

#region SByteParser
#endregion

#region UInt16Parser
#endregion

#region UInt32Parser
#endregion

#region UInt64Parser
#endregion

#region Int16Parser
#endregion

#region Int32Parser
#endregion

#region Int64ParserImproved
#endregion

#region SingleParser
#endregion

#region DoubleParser
#endregion

#region CharParser
#endregion
public override void LoadContext (INamespaceRoot root){
var namespaces = new Dictionary<string, IMutableNamespace>();
namespaces[""] = root.DefaultNamespace;
namespaces["default"] = root.DefaultNamespace;
namespaces["Default"] = root.DefaultNamespace;
namespaces["DEFAULT"] = root.DefaultNamespace;
// Generated Count : 16
namespaces["Default"].AddParserProvider<DateTimeShim_SJ32A9XBUFRV_TypeParser_Provider, Decuplr.Serialization.Binary.Internal.DefaultParsers.DateTimeShim>(new DateTimeShim_SJ32A9XBUFRV_TypeParser_Provider());
namespaces["Default"].AddParserProvider<TypeParserWrapper_DateTime_KB93HSXXRNTV2_As_DateTimeShim_SJ32A9XBUFRV_Provider, System.DateTime>(new TypeParserWrapper_DateTime_KB93HSXXRNTV2_As_DateTimeShim_SJ32A9XBUFRV_Provider());
namespaces["Default"].AddGenericParserProvider(typeof(LazyShim_XAF96RTGJHE73_TypeParser_GenericProvider<>), typeof(Decuplr.Serialization.Binary.Internal.DefaultParsers.LazyShim<>).GetGenericTypeDefinition());
namespaces["Default"].AddGenericParserProvider(typeof(TypeParserWrapper_Lazy_VPZZSHF2MTVH_As_LazyShim_XAF96RTGJHE73_GenericProvider<>), typeof(System.Lazy<>).GetGenericTypeDefinition());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.BooleanParser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.ByteParser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.SByteParser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.UInt16Parser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.UInt32Parser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.UInt64Parser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.Int16Parser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.Int32Parser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.Int64ParserImproved());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.SingleParser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.DoubleParser());
namespaces["Default"].AddSealedParser(new Decuplr.Serialization.Binary.Internal.DefaultParsers.CharParser());
}
}
}
