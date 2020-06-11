using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using AutoFixture;
using Benchmark.Models;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Decuplr.Serialization;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.Binary.Parsers;
using MessagePack;

namespace Benchmark {
    public class C {

        public byte a;
        public byte b;
        public byte d;
        public byte c;

        public void Setup() {
            var rand = new Random();
            a = (byte)rand.Next(0, 255);
            b = (byte)rand.Next(0, 255);
            c = (byte)rand.Next(0, 255);
            d = (byte)rand.Next(0, 255);
        }

        [Benchmark]
        public int M() {
            Span<int> data = stackalloc int[1];
            var value = MemoryMarshal.AsBytes(data);
            value[0] = a;
            value[1] = b;
            value[2] = c;
            value[3] = d;
            return data[0];
        }

        [Benchmark]
        public int K() {
            Span<byte> data = stackalloc Byte[sizeof(int)];
            data[0] = a;
            data[1] = b;
            data[2] = c;
            data[3] = d;
            return MemoryMarshal.Read<int>(data);
        }
    }

    public class Program {



        public static void Main() {
            //BenchmarkRunner.Run<SerializeTest>();
            //BenchmarkRunner.Run<PrimitiveTest>();
            BenchmarkRunner.Run<C>();
        }
    }


    [MemoryDiagnoser]
    //[HardwareCounters(HardwareCounter.BranchMispredictions , HardwareCounter.CacheMisses , HardwareCounter.InstructionRetired)]
    public class PrimitiveTest {

        private sealed class MyLongParser : TypeParser<long> {
            public override int GetBinaryLength(long value) {
                return sizeof(long);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public override int Serialize(long value, Span<byte> destination) {
                BinaryPrimitives.WriteInt64LittleEndian(destination, value);
                return sizeof(long);
            }

            public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out long result) {
                throw new NotImplementedException();
            }

            public override bool TrySerialize(long value, Span<byte> destination, out int writtenBytes) {
                throw new NotImplementedException();
            }
        }

        private long Value;
        private long Value2;
        private long Value3;
        private long Value4;
        private long Value5;
        private DateTime Time;
        private TypeParser<long> LongParser;
        private TypeParserEx<long> MyLong;
        private MyLongParser MyLong2;
        private TypeParser<DateTime> DateTimeParser;
        private long[] Values;
        private byte[] Space;

        [GlobalSetup]
        public void Setup() {
            Space = new byte[40];
            var random = new Random();
            Value = (random.Next() << 4) + random.Next();
            Value2 = (random.Next() << 4) + random.Next();
            Value3 = (random.Next() << 4) + random.Next();
            Value4 = (random.Next() << 4) + random.Next();
            Value5 = (random.Next() << 4) + random.Next();
            Time = DateTime.FromBinary(Value);
            LongParser = BinaryPacker.Shared.GetParser<long>();
            DateTimeParser = BinaryPacker.Shared.GetParser<DateTime>();
            MyLong2 = new MyLongParser();
            Values = new long[] { Value, Value2, Value3 };
        }

        //[Benchmark]
        public int SerializeLong() {
            return MyLong2.Serialize(Value, Space);
        }

        //[Benchmark]
        public int SerializeLong1() {
            return MyLong.Serialize(Value, Space);
        }

        //[Benchmark]
        public int SerializeLong2() {
            return LongParser.Serialize(Value, Space);
        }

        [Benchmark]
        public int SerializeLongManual() {
            MemoryMarshal.Write(Space, ref Value);
            MemoryMarshal.Write(Space.AsSpan(8), ref Value2);
            MemoryMarshal.Write(Space.AsSpan(16), ref Value3);
            return sizeof(long);
        }

        //[Benchmark]
        public int SerializeLongManual2() {
            BinaryPrimitives.WriteInt64LittleEndian(Space, Value);
            return sizeof(int) * 3;
        }

        //[Benchmark]
        public int SerializeTime() {
            return DateTimeParser.Serialize(Time, Space);
        }

        //[Benchmark]
        public int SerializeTimeManual() {
            var value = Time.ToBinary();
            MemoryMarshal.Write(Space, ref value);
            return sizeof(long);
        }
    }

    [MemoryDiagnoser]
    //[HardwareCounters(HardwareCounter.CacheMisses , HardwareCounter.BranchMispredictions , HardwareCounter.InstructionRetired)]
    //[InliningDiagnoser(true, new string[] { "Decuplr.Serialization.Binary", "Decuplr.Serialization.Binary.Internal", "Decuplr.Serialization.Binary.Internal.DefaultParsers", "Decuplr.Serialization.Binary.Parsers" })]
    public class SerializeTest {

        public ArrayBufferWriter<byte> MsgPackWriter { get; } = new ArrayBufferWriter<byte>(100);
        private SimplePoco poco;
        private TypeParser<SimplePoco> PocoParser;

        private readonly byte[] BinaryTarget = new byte[100];

        private byte[] MsgPackResult;
        private byte[] BinPackResult;

        [GlobalSetup]
        public void Setup() {
            var fixture = new Fixture();
            poco = fixture.Build<SimplePoco>().Create();
            PocoParser = BinaryPacker.Shared.GetParser<SimplePoco>();
            MsgPackResult = MessagePackSerializer.Serialize(poco);
            var count = PocoParser.Serialize(poco, BinaryTarget);
            BinPackResult = new byte[count];
            BinaryTarget.AsSpan(0, count).CopyTo(BinPackResult);
        }

        //[Benchmark]
        public void MessagePackSerialize() {
            MessagePackSerializer.Serialize(poco);
        }

        //[Benchmark]
        public void DecuplrBinarySerialize() {
            // This suppose to be a bug :(
            PocoParser.GetBinaryLength(poco);
            PocoParser.Serialize(poco, BinaryTarget);
        }

        //[Benchmark]
        public void DecuplrBinarySerialize2() {
            // This suppose to be a bug :(
            //PocoParser.GetBinaryLength(poco);
            PocoParser.Serialize(poco, BinaryTarget);
        }

        //[Benchmark]
        public int ManuallySerialize() {
            var span = BinaryTarget.AsSpan();
            var id = poco.OldAccountId;
            MemoryMarshal.Write(span, ref id);

            span = span.Slice(8);
            var id2 = poco.NewAccountId;
            MemoryMarshal.Write(span, ref id2);

            span = span.Slice(8);
            var id3 = poco.InfoId;
            MemoryMarshal.Write(span, ref id3);

            span = span.Slice(8);
            var time = poco.LastChangeTime.ToBinary();
            MemoryMarshal.Write(span, ref time);

            span = span.Slice(8);
            var time2 = poco.FinalChangeTime.ToBinary();
            MemoryMarshal.Write(span, ref time2);


            span = span.Slice(8);
            var time3 = poco.FastChangeTime.ToBinary();
            MemoryMarshal.Write(span, ref time3);

            span = span.Slice(8);
            var id4 = poco.FinalCommentId;
            MemoryMarshal.Write(span, ref id4);
            span = span.Slice(8);

            return BinaryTarget.Length - span.Length;
        }

        [Benchmark]
        public SimplePoco ManuallyDeserialize() {
            var ogLength = BinPackResult.Length;
            var span = BinPackResult.AsSpan();
            var poco = new SimplePoco();
            poco.OldAccountId = MemoryMarshal.Read<long>(span);
            span = span.Slice(sizeof(long));

            poco.NewAccountId = MemoryMarshal.Read<long>(span);
            span = span.Slice(sizeof(long));

            poco.InfoId = MemoryMarshal.Read<long>(span);
            span = span.Slice(sizeof(long));

            poco.LastChangeTime = DateTime.FromBinary(MemoryMarshal.Read<long>(span));
            span = span.Slice(sizeof(long));

            poco.FinalChangeTime = DateTime.FromBinary(MemoryMarshal.Read<long>(span));
            span = span.Slice(sizeof(long));

            poco.FastChangeTime = DateTime.FromBinary(MemoryMarshal.Read<long>(span));
            span = span.Slice(sizeof(long));

            poco.FinalCommentId = MemoryMarshal.Read<long>(span);
            span = span.Slice(sizeof(long));

            var readBytes = ogLength - span.Length;
            return poco;
        }


        [Benchmark]
        public SimplePoco MsgPackDeserialize() {
            return MessagePackSerializer.Deserialize<SimplePoco>(MsgPackResult, out _);
        }

        [Benchmark]
        public SimplePoco DecuplrBinaryDeserialize() {
            PocoParser.TryDeserialize(BinPackResult, out _, out var result);
            return result;
        }
    }
}
