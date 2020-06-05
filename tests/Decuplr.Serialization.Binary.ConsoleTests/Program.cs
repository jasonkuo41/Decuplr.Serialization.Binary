using System;
using System.Collections.Generic;
using Decuplr.Serialization.Binary.Annotations.Namespaces;

namespace Decuplr.Serialization.Binary.ConsoleTests {
    internal class Program {
        private static void Main(string[] args) {
            byte[] target = new byte[12];
            BinaryPacker.Shared.GetParser<int>().Serialize(123, target);
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

