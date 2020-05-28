using System;
using System.Threading;
using System.ComponentModel;
using System.CodeDom.Compiler;

namespace Decuplr.Serialization.Binary.Internal {
[BinaryPackerAssemblyEntryPointAttribute]
[GeneratedCode ("Decuplr.Serialization.Binary.SourceGenerator", "1.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class AssemblyFormatProvider_c9d6ae2b_208c_4d30_aa72_6fc9d6d9dc72{
private static int IsInit = 0;

private class Decuplr_Serialization_Binary_ConsoleTests_FoamStruct_Serializer : BinaryParser <Decuplr.Serialization.Binary.ConsoleTests.FoamStruct>{
private readonly BinaryParser<int> parser_0;
private readonly BinaryParser<int> parser_1;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
public Decuplr_Serialization_Binary_ConsoleTests_FoamStruct_Serializer (BinaryFormatter format){
fixedSize = 0;
format.TryGetFormatter(out parser_0);
if (fixedSize != null){
fixedSize += parser_0.FixedSize;
}
format.TryGetFormatter(out parser_1);
if (fixedSize != null){
fixedSize += parser_1.FixedSize;
}
}

// Deserialization Function
private static Decuplr.Serialization.Binary.ConsoleTests.FoamStruct CreateType(int s_Data,int s_Implement){
return new Decuplr.Serialization.Binary.ConsoleTests.FoamStruct (s_Data,s_Implement);
}


// Serialization Function
private static void DeconstructType(Decuplr.Serialization.Binary.ConsoleTests.FoamStruct value, out int s_0,out int s_1){
Decuplr.Serialization.Binary.ConsoleTests.FoamStruct.___generated__no_invoke_FoamStruct_Derializer (value, out s_0,out s_1);
}


// TryDeserialize Function
public override SerializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.FoamStruct result){
readBytes = 0;
result = default;
int s_0;
{
var parserResult = parser_0.TryDeserialize(span, out var scopeReadBytes, out s_0);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
int s_1;
{
var parserResult = parser_1.TryDeserialize(span, out var scopeReadBytes, out s_1);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
result = CreateType(s_0, s_1);
return SerializeResult.Success;
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.FoamStruct value){
if (fixedSize.HasValue){
return fixedSize.Value;
}
DeconstructType (value, out var s_0,out var s_1);
int length = 0;
length += parser_0.GetBinaryLength(s_0);
length += parser_1.GetBinaryLength(s_1);
return length;
}

// Serialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.FoamStruct value, Span<byte> destination, out int writtenBytes){
writtenBytes = -1;
if (fixedSize.HasValue && destination.Length < fixedSize){
return false;
}
DeconstructType (value, out var s_0,out var s_1);
int length = 0;
{
if (!parser_0.TrySerialize(s_0, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
{
if (!parser_1.TrySerialize(s_1, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
writtenBytes = length;
return true;
}
}



private class Decuplr_Serialization_Binary_ConsoleTests_TestClass3_Serializer : BinaryParser <Decuplr.Serialization.Binary.ConsoleTests.TestClass3>{
private readonly BinaryParser<Decuplr.Serialization.Binary.ConsoleTests.FoamStruct> parser_0;
private readonly BinaryParser<System.Collections.Generic.List<byte>> parser_1;
private readonly BinaryParser<(bool IsValid, bool IsCurrent, bool IsCompressed)> parser_2;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
public Decuplr_Serialization_Binary_ConsoleTests_TestClass3_Serializer (BinaryFormatter format){
fixedSize = 0;
format.TryGetFormatter(out parser_0);
if (fixedSize != null){
fixedSize += parser_0.FixedSize;
}
format.TryGetFormatter(out parser_1);
if (fixedSize != null){
fixedSize += parser_1.FixedSize;
}
format.TryGetFormatter(out parser_2);
if (fixedSize != null){
fixedSize += parser_2.FixedSize;
}
}

// Deserialization Function
private static Decuplr.Serialization.Binary.ConsoleTests.TestClass3 CreateType(Decuplr.Serialization.Binary.ConsoleTests.FoamStruct s_InfoData,System.Collections.Generic.List<byte> s_FormatData,(bool IsValid, bool IsCurrent, bool IsCompressed) s_ConditionGroup){
return new Decuplr.Serialization.Binary.ConsoleTests.TestClass3 (s_InfoData,s_FormatData,s_ConditionGroup);
}


// Serialization Function
private static void DeconstructType(Decuplr.Serialization.Binary.ConsoleTests.TestClass3 value, out Decuplr.Serialization.Binary.ConsoleTests.FoamStruct s_0,out System.Collections.Generic.List<byte> s_1,out (bool IsValid, bool IsCurrent, bool IsCompressed) s_2){
Decuplr.Serialization.Binary.ConsoleTests.TestClass3.___generated__no_invoke_TestClass3_Derializer (value, out s_0,out s_1,out s_2);
}


// TryDeserialize Function
public override SerializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestClass3 result){
readBytes = 0;
result = default;
Decuplr.Serialization.Binary.ConsoleTests.FoamStruct s_0;
{
var parserResult = parser_0.TryDeserialize(span, out var scopeReadBytes, out s_0);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
System.Collections.Generic.List<byte> s_1;
{
var parserResult = parser_1.TryDeserialize(span, out var scopeReadBytes, out s_1);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
(bool IsValid, bool IsCurrent, bool IsCompressed) s_2;
{
var parserResult = parser_2.TryDeserialize(span, out var scopeReadBytes, out s_2);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
result = CreateType(s_0, s_1, s_2);
return SerializeResult.Success;
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.TestClass3 value){
if (fixedSize.HasValue){
return fixedSize.Value;
}
DeconstructType (value, out var s_0,out var s_1,out var s_2);
int length = 0;
length += parser_0.GetBinaryLength(s_0);
length += parser_1.GetBinaryLength(s_1);
length += parser_2.GetBinaryLength(s_2);
return length;
}

// Serialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.TestClass3 value, Span<byte> destination, out int writtenBytes){
writtenBytes = -1;
if (fixedSize.HasValue && destination.Length < fixedSize){
return false;
}
DeconstructType (value, out var s_0,out var s_1,out var s_2);
int length = 0;
{
if (!parser_0.TrySerialize(s_0, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
{
if (!parser_1.TrySerialize(s_1, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
{
if (!parser_2.TrySerialize(s_2, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
writtenBytes = length;
return true;
}
}



private class Decuplr_Serialization_Binary_ConsoleTests_TestClass3_NestedClassTarget_Serializer : BinaryParser <Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget>{
private readonly BinaryParser<int> parser_0;
private readonly BinaryParser<int> parser_1;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
public Decuplr_Serialization_Binary_ConsoleTests_TestClass3_NestedClassTarget_Serializer (BinaryFormatter format){
fixedSize = 0;
format.TryGetFormatter(out parser_0);
if (fixedSize != null){
fixedSize += parser_0.FixedSize;
}
format.TryGetFormatter(out parser_1);
if (fixedSize != null){
fixedSize += parser_1.FixedSize;
}
}

// Deserialization Function
private static Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget CreateType(int s_Target,int s_Result){
return new Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget (s_Target,s_Result);
}


// Serialization Function
private static void DeconstructType(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget value, out int s_0,out int s_1){
Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.___generated__no_invoke_NestedClassTarget_Derializer (value, out s_0,out s_1);
}


// TryDeserialize Function
public override SerializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget result){
readBytes = 0;
result = default;
int s_0;
{
var parserResult = parser_0.TryDeserialize(span, out var scopeReadBytes, out s_0);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
int s_1;
{
var parserResult = parser_1.TryDeserialize(span, out var scopeReadBytes, out s_1);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
result = CreateType(s_0, s_1);
return SerializeResult.Success;
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget value){
if (fixedSize.HasValue){
return fixedSize.Value;
}
DeconstructType (value, out var s_0,out var s_1);
int length = 0;
length += parser_0.GetBinaryLength(s_0);
length += parser_1.GetBinaryLength(s_1);
return length;
}

// Serialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget value, Span<byte> destination, out int writtenBytes){
writtenBytes = -1;
if (fixedSize.HasValue && destination.Length < fixedSize){
return false;
}
DeconstructType (value, out var s_0,out var s_1);
int length = 0;
{
if (!parser_0.TrySerialize(s_0, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
{
if (!parser_1.TrySerialize(s_1, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
writtenBytes = length;
return true;
}
}



private class Decuplr_Serialization_Binary_ConsoleTests_TestClass3_NestedClassTarget_NestedClassTarget3_Serializer : BinaryParser <Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3>{
private readonly BinaryParser<int> parser_0;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
public Decuplr_Serialization_Binary_ConsoleTests_TestClass3_NestedClassTarget_NestedClassTarget3_Serializer (BinaryFormatter format){
fixedSize = 0;
format.TryGetFormatter(out parser_0);
if (fixedSize != null){
fixedSize += parser_0.FixedSize;
}
}

// Deserialization Function
private static Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 CreateType(int s_Safety){
return new Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 (s_Safety);
}


// Serialization Function
private static void DeconstructType(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 value, out int s_0){
Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3.___generated__no_invoke_NestedClassTarget3_Derializer (value, out s_0);
}


// TryDeserialize Function
public override SerializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 result){
readBytes = 0;
result = default;
int s_0;
{
var parserResult = parser_0.TryDeserialize(span, out var scopeReadBytes, out s_0);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
result = CreateType(s_0);
return SerializeResult.Success;
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 value){
if (fixedSize.HasValue){
return fixedSize.Value;
}
DeconstructType (value, out var s_0);
int length = 0;
length += parser_0.GetBinaryLength(s_0);
return length;
}

// Serialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.TestClass3.NestedClassTarget.NestedClassTarget3 value, Span<byte> destination, out int writtenBytes){
writtenBytes = -1;
if (fixedSize.HasValue && destination.Length < fixedSize){
return false;
}
DeconstructType (value, out var s_0);
int length = 0;
{
if (!parser_0.TrySerialize(s_0, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
writtenBytes = length;
return true;
}
}



private class Decuplr_Serialization_Binary_ConsoleTests_TestStruct_Serializer : BinaryParser <Decuplr.Serialization.Binary.ConsoleTests.TestStruct>{
private readonly BinaryParser<int> parser_0;
private readonly BinaryParser<Decuplr.Serialization.Binary.ConsoleTests.TestClass3> parser_1;
private readonly int? fixedSize;
public override int? FixedSize => fixedSize;
public Decuplr_Serialization_Binary_ConsoleTests_TestStruct_Serializer (BinaryFormatter format){
fixedSize = 0;
format.TryGetFormatter(out parser_0);
if (fixedSize != null){
fixedSize += parser_0.FixedSize;
}
format.TryGetFormatter(out parser_1);
if (fixedSize != null){
fixedSize += parser_1.FixedSize;
}
}

// Deserialization Function
private static Decuplr.Serialization.Binary.ConsoleTests.TestStruct CreateType(int s_Target,Decuplr.Serialization.Binary.ConsoleTests.TestClass3 s_Result){
return new Decuplr.Serialization.Binary.ConsoleTests.TestStruct (s_Target,s_Result);
}


// Serialization Function
private static void DeconstructType(Decuplr.Serialization.Binary.ConsoleTests.TestStruct value, out int s_0,out Decuplr.Serialization.Binary.ConsoleTests.TestClass3 s_1){
Decuplr.Serialization.Binary.ConsoleTests.TestStruct.___generated__no_invoke_TestStruct_Derializer (value, out s_0,out s_1);
}


// TryDeserialize Function
public override SerializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Decuplr.Serialization.Binary.ConsoleTests.TestStruct result){
readBytes = 0;
result = default;
int s_0;
{
var parserResult = parser_0.TryDeserialize(span, out var scopeReadBytes, out s_0);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
Decuplr.Serialization.Binary.ConsoleTests.TestClass3 s_1;
{
var parserResult = parser_1.TryDeserialize(span, out var scopeReadBytes, out s_1);
if (parserResult != SerializeResult.Success){
readBytes = -1;
return parserResult;
}
span = span.Slice(readBytes);
readBytes += scopeReadBytes;
}
result = CreateType(s_0, s_1);
return SerializeResult.Success;
}

// GetBinaryLength Function
public override int GetBinaryLength(Decuplr.Serialization.Binary.ConsoleTests.TestStruct value){
if (fixedSize.HasValue){
return fixedSize.Value;
}
DeconstructType (value, out var s_0,out var s_1);
int length = 0;
length += parser_0.GetBinaryLength(s_0);
length += parser_1.GetBinaryLength(s_1);
return length;
}

// Serialize Function
public override bool TrySerialize(Decuplr.Serialization.Binary.ConsoleTests.TestStruct value, Span<byte> destination, out int writtenBytes){
writtenBytes = -1;
if (fixedSize.HasValue && destination.Length < fixedSize){
return false;
}
DeconstructType (value, out var s_0,out var s_1);
int length = 0;
{
if (!parser_0.TrySerialize(s_0, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
{
if (!parser_1.TrySerialize(s_1, destination, out int bytes)) {
return false;
}
length += bytes;
destination = destination.Slice(bytes);
}
writtenBytes = length;
return true;
}
}


public static void AppendFormatters (BinaryFormatter formatter){
// This ensures that we only invoke only once (ThreadSafe)
if (Interlocked.Exchange(ref IsInit, 1) != 0){
return;
}
formatter.AddFormatter(new Decuplr_Serialization_Binary_ConsoleTests_FoamStruct_Serializer(formatter));
formatter.AddFormatter(new Decuplr_Serialization_Binary_ConsoleTests_TestClass3_Serializer(formatter));
formatter.AddFormatter(new Decuplr_Serialization_Binary_ConsoleTests_TestClass3_NestedClassTarget_Serializer(formatter));
formatter.AddFormatter(new Decuplr_Serialization_Binary_ConsoleTests_TestClass3_NestedClassTarget_NestedClassTarget3_Serializer(formatter));
formatter.AddFormatter(new Decuplr_Serialization_Binary_ConsoleTests_TestStruct_Serializer(formatter));
}
}
}
