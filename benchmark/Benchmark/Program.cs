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
    public class Program {



        public static void Main() {
            //BenchmarkRunner.Run<SerializeTest>();
            BenchmarkRunner.Run<PrimitiveTest>();
        }
    }


    [MemoryDiagnoser]
    //[HardwareCounters(HardwareCounter.BranchMispredictions , HardwareCounter.CacheMisses , HardwareCounter.InstructionRetired)]
    public class PrimitiveTest {

        private long Value;
        private long Value2;
        private long Value3;
        private long Value4;
        private long Value5;
        private DateTime Time;
        private TypeParser<long> MyLong;
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
            //DateTimeParser = BinaryPacker.Shared.GetParser<DateTime>();
            Values = new long[] { Value, Value2, Value3 };
        }


        [Benchmark]
        public int SerializeLong2() {
            PrimitiveParsers.WriteInt64Unsafe(Value, Space, true);
            return sizeof(long);
        }

        [Benchmark]
        public int SerializeLongManual() {
            MemoryMarshal.Write(Space, ref Value);
            //MemoryMarshal.Write(Space.AsSpan(8), ref Value2);
            //MemoryMarshal.Write(Space.AsSpan(16), ref Value3);
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
            PocoParser.GetLength(poco);
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
