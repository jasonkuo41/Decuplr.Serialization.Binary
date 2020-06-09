using System;
using System.ComponentModel;
using System.CodeDom.Compiler;
using Decuplr.Serialization;
using static Decuplr.Serialization.Binary.Internal.AssemblyFormatProvider_Benchmark_SV5T16NWE1PR2;


namespace Benchmark.Models {
// This file is automatically generated by Decuplr.Serilization.Binary library
// For more information, see https://decuplr.dev/serialization/binary

// Debug Info : Built by PartialTypeSerialize
public  partial class SimplePoco{
[GeneratedCode ("Decuplr.Serialization.Binary.SourceGenerator", "1.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
internal static bool ___generated__no_invoke_SimplePoco_TrySerializer (in SimplePoco_DXLT9ZBUDDGJ3_TypeParserArgs parsers, Benchmark.Models.SimplePoco value, Span<byte> destination, out int writtenBytes){
writtenBytes = -1;
var oglength = destination.Length;
int currentWrittenBytes;
if (!parsers.Parser_0_0.TrySerialize(value.OldAccountId, destination, out currentWrittenBytes)) {
return false;
}
destination = destination.Slice(currentWrittenBytes);
if (!parsers.Parser_1_0.TrySerialize(value.NewAccountId, destination, out currentWrittenBytes)) {
return false;
}
destination = destination.Slice(currentWrittenBytes);
if (!parsers.Parser_2_0.TrySerialize(value.InfoId, destination, out currentWrittenBytes)) {
return false;
}
destination = destination.Slice(currentWrittenBytes);
if (!parsers.Parser_3_0.TrySerialize(value.LastChangeTime, destination, out currentWrittenBytes)) {
return false;
}
destination = destination.Slice(currentWrittenBytes);
if (!parsers.Parser_4_0.TrySerialize(value.FinalChangeTime, destination, out currentWrittenBytes)) {
return false;
}
destination = destination.Slice(currentWrittenBytes);
if (!parsers.Parser_5_0.TrySerialize(value.FastChangeTime, destination, out currentWrittenBytes)) {
return false;
}
destination = destination.Slice(currentWrittenBytes);
if (!parsers.Parser_6_0.TrySerialize(value.FinalCommentId, destination, out currentWrittenBytes)) {
return false;
}
destination = destination.Slice(currentWrittenBytes);
writtenBytes = oglength - destination.Length;
return true;
}
[GeneratedCode ("Decuplr.Serialization.Binary.SourceGenerator", "1.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
internal static int ___generated__no_invoke_SimplePoco_Serializer (in SimplePoco_DXLT9ZBUDDGJ3_TypeParserArgs parsers, Benchmark.Models.SimplePoco value, Span<byte> destination){
var oglength = destination.Length;
int currentWrittenBytes;
currentWrittenBytes = parsers.Parser_0_0.Serialize(value.OldAccountId, destination);
destination = destination.Slice(currentWrittenBytes);
currentWrittenBytes = parsers.Parser_1_0.Serialize(value.NewAccountId, destination);
destination = destination.Slice(currentWrittenBytes);
currentWrittenBytes = parsers.Parser_2_0.Serialize(value.InfoId, destination);
destination = destination.Slice(currentWrittenBytes);
currentWrittenBytes = parsers.Parser_3_0.Serialize(value.LastChangeTime, destination);
destination = destination.Slice(currentWrittenBytes);
currentWrittenBytes = parsers.Parser_4_0.Serialize(value.FinalChangeTime, destination);
destination = destination.Slice(currentWrittenBytes);
currentWrittenBytes = parsers.Parser_5_0.Serialize(value.FastChangeTime, destination);
destination = destination.Slice(currentWrittenBytes);
currentWrittenBytes = parsers.Parser_6_0.Serialize(value.FinalCommentId, destination);
destination = destination.Slice(currentWrittenBytes);
return oglength - destination.Length;
}
[GeneratedCode ("Decuplr.Serialization.Binary.SourceGenerator", "1.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
internal static int ___generated__no_inoke_SimplePoco_GetLength(in SimplePoco_DXLT9ZBUDDGJ3_TypeParserArgs parsers, Benchmark.Models.SimplePoco value){
var result = 0;
result += parsers.Parser_0_0.GetBinaryLength(value.OldAccountId);
result += parsers.Parser_1_0.GetBinaryLength(value.NewAccountId);
result += parsers.Parser_2_0.GetBinaryLength(value.InfoId);
result += parsers.Parser_3_0.GetBinaryLength(value.LastChangeTime);
result += parsers.Parser_4_0.GetBinaryLength(value.FinalChangeTime);
result += parsers.Parser_5_0.GetBinaryLength(value.FastChangeTime);
result += parsers.Parser_6_0.GetBinaryLength(value.FinalCommentId);
return result;
}
}
}
