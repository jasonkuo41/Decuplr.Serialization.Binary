using System;
using System.Buffers;
using System.IO;
using Apex.Serialization;
using AutoFixture;
using AutoFixture.Kernel;
using Benchmark.Models;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Decuplr.Serialization.Binary;
using MessagePack;

namespace Benchmark {
    public class Program {



        public static void Main() {
            var test = new SerializeTest();
            BenchmarkRunner.Run<SerializeTest>();
        }
    }

    [MemoryDiagnoser]
    public class SerializeTest {

        public ArrayBufferWriter<byte> MsgPackWriter { get; } = new ArrayBufferWriter<byte>();
        private SimplePoco poco;
        private TypeParser<SimplePoco> PocoParser;
        private readonly MemoryStream _memoryStream = new MemoryStream(new byte[100]);
        private IBinary _binaryTree = Binary.Create(new Settings { SerializationMode = Mode.Tree, UseSerializedVersionId = false }.MarkSerializable(x => true));

        private readonly byte[] DecuplrTarget = new byte[100];

        [GlobalSetup]
        public void Setup() {
            var fixture = new Fixture();
            poco = fixture.Build<SimplePoco>().Create();
            PocoParser = BinaryPacker.Shared.GetParser<SimplePoco>();
        }

        [Benchmark]
        public void MessagePackSerialize() {
            MessagePackSerializer.Serialize(poco);
        }

        [Benchmark]
        public void DecuplrBinarySerialize() {
            PocoParser.GetBinaryLength(poco);
            PocoParser.Serialize(poco, DecuplrTarget);
        }

        [Benchmark]
        public void ApexSerialize() {
            _memoryStream.Seek(0, SeekOrigin.Begin);
            _binaryTree.Write(poco, _memoryStream);
        }
    }
}
