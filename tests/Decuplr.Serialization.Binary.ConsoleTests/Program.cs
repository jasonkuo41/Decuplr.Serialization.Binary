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

    }


    [BinaryFormat]
    public struct TestStruct {

    }
}
