using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Includes a set of extensions that help with accessing the functionality of the packer
    /// </summary>
    public static class BinaryPackerExtensions {

        public static IParserNamespace GetNamespace(this INamespaceRoot packer, params string[] parserNamespace) => packer.GetNamespace(parserNamespace);

        public static TypeParser<T> GetParser<T>(this BinaryPacker packer) => packer.DefaultNamespace.GetParser<T>();

        public static bool TryGetParser<T>(this BinaryPacker packer, out TypeParser<T> parser) => packer.DefaultNamespace.TryGetParser<T>(out parser);

        //public static T Serialize<T>(this BinaryPacker packer, T value) => packer.GetParser<T>().TrySerialize();
    }
}

