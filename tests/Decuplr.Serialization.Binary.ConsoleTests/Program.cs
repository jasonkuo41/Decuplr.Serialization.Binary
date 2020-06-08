using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Decuplr.Serialization.Binary.Annotations.Namespaces;

namespace Decuplr.Serialization.Binary.ConsoleTests {
    internal class Program {
        private static void Main(string[] args) {
            /*
            var foamStruct = new FoamStruct {
                Data = 123,
                Implement = 12345678765334
            };
            */
            var foamStruct = new FoamStruct(123, 12345678765334);
            var foamParser = BinaryPacker.Shared.GetParser<FoamStruct>();
            Span<byte> stack = stackalloc byte[foamParser.GetBinaryLength(foamStruct)];
            foamParser.Serialize(foamStruct, stack);

            var xresult = foamParser.TryDeserialize(stack, out _, out var result);
            Console.Write(result);
            //DebugContent.PrintDebugInfo();
        }
    }

    [BinaryFormat]
    public partial struct FoamStruct {
        public FoamStruct(int value, long impl) {
            Data = value;
            Implement = impl;
        }
        //[Index(0)]
        public int Data { get; }
        //[Index(1)]
        public long Implement { get; }

        public override string ToString() => $"{Data} {Implement}";
    }

    [BinaryFormat]
    public partial class TestClass3 {

        [Index(0)]
        [Endianess(ByteOrder.BigEndian)]
        public FoamStruct InfoData { get; }

        [Index(1)]
        public List<byte> FormatData { get; }

        [Index(2)]
        public (bool IsValid, bool IsCurrent, bool IsCompressed) ConditionGroup { get; }

        public int Test => 3;

        [BinaryFormat]
        public partial class NestedClassTarget {
            [Index(0)]
            public int Target { get; }

            [Index(1)]
            public int Result { get; }

            [BinaryFormat]
            internal partial class NestedClassTarget3 {
                [Index(0)]
                public int Safety { get; set; }
            }
        }
    }


    [BinaryFormat]
    public partial struct TestStruct {

        [Index(0)]
        public int Target { get; }

        [Index(1)]
        public TestClass3 Result { get; }

    }
}

