using Decuplr.CodeAnalysis.Serialization;
using Decuplr.Serialization.Binary.FormatSource;

namespace Decuplr.Serialization.Binary {
    internal class CommonGeneratorSetup {
        
        public static ICodeGeneratorFactory CreateCommonFactory() {

            var builder = new CodeGeneratorBuilder();

            builder.AddStartup<BinaryFormatProvider>();
            builder.AddStartup<BinaryParserProvider>();

            return builder.BuildGenerator();
        }
    }
}
