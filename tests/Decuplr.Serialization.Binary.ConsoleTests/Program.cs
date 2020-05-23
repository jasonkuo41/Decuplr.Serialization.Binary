using System;

namespace Decuplr.Serialization.Binary.ConsoleTests {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!dd");
            TestGenerated.Analyzed.PrintResult();
        }
    }

    [BinaryFormat]
    public class TestClass {

    }

    [Obsolete]
    public class TestClass2 {

    }

    [BinaryFormat]
    public class TestClass3 {

        [Index(1)]
        public byte FormatData { get; }

        [Index(2)]
        [FormatCondition(nameof(Condition))]
        public BaseA NextData { get; }

        private BaseA Condition() {
            if ((FormatData >> 3 & 1) == 0)
                return new BaseB();
            return new BaseA();
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

    [BinaryFormat]
    public struct TestStruct {

    }
}
