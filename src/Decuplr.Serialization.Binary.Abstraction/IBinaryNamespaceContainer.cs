using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Decuplr.Serialization.Namespaces;

namespace Decuplr.Serialization.Binary {

    /// <summary>
    /// A factory interface that is capable of creating <see cref="BinaryConverter{T}"/> instances with <see cref="IBinaryNamespaceDiscovery"/>.
    /// </summary>
    /// <typeparam name="T">The type that would be converted to binary</typeparam>
    public interface IBinaryConverterProvider<T> {
        /// <summary>
        /// Creates a converter instance with the designated discovery instance.
        /// </summary>
        /// <param name="discovery">The discovery instance to locate the converter</param>
        /// <returns>A converter instance</returns>
        BinaryConverter<T> GetConverter(IBinaryNamespaceDiscovery discovery);
        bool TryGetConverter(IBinaryNamespaceDiscovery discovery, [NotNullWhen(true)] out BinaryConverter<T>? converter);
    }

    public interface IBinaryNamespaceDiscovery : INamespaceDiscovery {
        BinaryConverter<T> GetConverter<T>();
        bool TryGetConverter<T>(out BinaryConverter<T> converter);
    }

    /* Move this to none abstraction I think
    public static class NamespaceNodeExtensions {
        public static BinaryConverter<T>? GetBinaryConverter<T>(this INamespaceNode node) {
            node.Get<BinaryConverter<T>>(Assembly.GetEntryAssembly());
        }
    }
    */
}
