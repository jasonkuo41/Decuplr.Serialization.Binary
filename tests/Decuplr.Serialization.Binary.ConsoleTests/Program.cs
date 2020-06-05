using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary.ConsoleTests {
    class Program {
        static void Main(string[] args) {
            BinaryPacker.Shared.GetParser<int>().Serialize(123, new byte[32]);
            Console.WriteLine("Hello World!dd");
            //DebugContent.PrintDebugInfo();
        }
    }

    [BinaryFormat(Sealed = true)]
    public partial struct FoamStruct {
        [Index(0)]
        public int Data { get; set; }
        [Index(1)]
        public int Implement { get; set; }
    }

    [BinaryFormat]
    public partial class TestClass3 {

        [Index(0)]
        [Endianess(ByteOrder.BigEndian)]
        public FoamStruct InfoData { get; }

        [Index(1, FixedSize = 3)]
        public List<byte> FormatData { get; }

        [Index(2)]
        public (bool IsValid, bool IsCurrent, bool IsCompressed) ConditionGroup { get; }

        public int Test => 3;

        /* Reserved concept
        [Index(3)]
        [TypeProvider("Index[2].IsCompressed ? 0 : 1", typeof(ClassB), typeof(ClassC))]
        public BaseA NextData { get; }

        internal static Type FormatCondition([IndexSelect(2, Union = 3)] bool isCompressed) {
            if (isCompressed)
                return typeof(ClassB);
            return typeof(ClassC);
        }
        */

        /*
        [Index(4)]
        [IgnoreIf(3, Union = 3)]
        public int CompressionSize { get; }

        [Index(5)]
        [LengthProvider(Index = 3)]
        public ReadOnlySpan<byte> RemainContent { get; }
        */

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

    public interface IConditionAttribute {

    }

    public class FormatConditionAttribute : Attribute {
        public FormatConditionAttribute(string condition) {

        }

    }

    public class BaseA { }

    public class ClassB : BaseA { }

    public class ClassC : BaseA { }

    [BinaryFormat]
    public partial struct TestStruct {

        [Index(0)]
        public int Target { get; }

        [Index(1)]
        public TestClass3 Result { get; }

    }
}

