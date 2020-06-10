using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AutoFixture;
using Benchmark.Models;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Decuplr.Serialization;
using Decuplr.Serialization.Binary;
using MessagePack;

namespace Benchmark {
    public class Program {



        public static void Main() {
            //BenchmarkRunner.Run<SerializeTest>();
            BenchmarkRunner.Run<PrimitiveTest>();
        }
    }

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
        private DateTime Time;
        private TypeParser<long> LongParser;
        private TypeParser<long> MyLong;
        private MyLongParser MyLong2;
        private TypeParser<DateTime> DateTimeParser;
        private byte[] Space;

        [GlobalSetup]
        public void Setup() {
            Space = new byte[8];
            var random = new Random();
            Value = (random.Next() << 4) + random.Next();
            Time = DateTime.FromBinary(Value);
            LongParser = BinaryPacker.Shared.GetParser<long>();
            DateTimeParser = BinaryPacker.Shared.GetParser<DateTime>();
            MyLong = new MyLongParser();
            MyLong2 = new MyLongParser();
        }

        [Benchmark]
        public int SerializeLong() {
            return LongParser.Serialize(Value, Space);
        }

        [Benchmark]
        public int SerializeLong1() {
            return MyLong.Serialize(Value, Space);
        }

        [Benchmark]
        public int SerializeLong2() {
            return MyLong2.Serialize(Value, Space);
        }

        //[Benchmark]
        public int SerializeLongManual() {
            MemoryMarshal.Write(Space, ref Value);
            return sizeof(long);
        }

        [Benchmark]
        public int SerializeLongManual2() {
            BinaryPrimitives.WriteInt64LittleEndian(Space, Value);
            return sizeof(int);
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
    //[HardwareCounters(HardwareCounter.CacheMisses | HardwareCounter.BranchMispredictions)]
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

        [Benchmark]
        public void MessagePackSerialize() {
            MessagePackSerializer.Serialize(poco);
        }

        [Benchmark]
        public void DecuplrBinarySerialize() {
            // This suppose to be a bug :(
            PocoParser.GetBinaryLength(poco);
            PocoParser.Serialize(poco, BinaryTarget);
        }

        [Benchmark]
        public void DecuplrBinarySerialize2() {
            // This suppose to be a bug :(
            //PocoParser.GetBinaryLength(poco);
            PocoParser.Serialize(poco, BinaryTarget);
        }

        [Benchmark]
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
