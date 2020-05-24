using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Decuplr.Serialization.Binary.ConsoleTests {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!dd");
            DebugContent.PrintDebugInfo();
        }
    }

    [BinaryFormat]
    public partial class TestClass {

    }

    [Obsolete]
    public class TestClass2 {

    }

    [BinaryFormat]
    public partial class TestClass3 {

        [Index(0)]
        [NetworkByteOrder]
        public int InfoData { get; }

        [Index(1, FixedSize = 3)]
        public byte FormatData { get; }

        [Index(2)]
        [FormatCondition(nameof(Condition))]
        public BaseA NextData { get; }

        private BaseA Condition() {
            if ((FormatData >> 3 & 1) == 0)
                return new BaseB();
            return new BaseC();
        }

        [BinaryFormat]
        public partial class NestedClassTarget {
            [Index(0)]
            public NestedClassTarget Target { get; }


            [BinaryFormat]
            private partial class NestedClassTarget3 {
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

    public class BaseB : BaseA { }
    public class BaseC : BaseA { }

    [BinaryFormat]
    public partial struct TestStruct {

    }
}

