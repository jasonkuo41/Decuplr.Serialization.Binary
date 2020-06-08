using System;
using Decuplr.Serialization.Binary.Annotations;
using Decuplr.Serialization.Binary.Annotations.Namespaces;

namespace Decuplr.Serialization.Binary.Internal.DefaultParsers {
    /// Note currently we parse the underlying daya asap,
    /// But what we actually can do is store a block of memory and only serialize that value when it's evaluated, basically, 
    /// We need 
    ///  (1) the class to implement IDisposable (so we can recycle the underlying memory) (or not, depends how the user implement it's class)
    ///  (2) able to determinate the layout with the nested type. For example list, then we must know how big the list is
    ///      , so we can skip that part and serialize the next part.
    /// 
    /// Thoughts:
    ///     This approach would be a bit tedious, and we need to know the underlying size of the object 
    ///     (unless we encode the object with a length info telling use how big this block is)
    ///     otherwise we still maybe need to 
    ///         (1) travel through every point to get the length info and sum together, which might be counter productive
    ///         (2) if we have a bug in our code, that would could be a disater as we misallign everything
    ///  
    [BinaryParser(typeof(Lazy<>))]
    [BinaryParserNamespace("Default")]
    internal partial class LazyShim<T> : ITypeConvertible<Lazy<T>> {

        public LazyShim(Lazy<T> lazy) {
            LazyValue = lazy.Value;
        }

        [Index(0)]
        public T LazyValue { get; }

        public Lazy<T> ConvertTo() {
#if NETSTANDARD2_0
            return new Lazy<T>(() => LazyValue, true);
#else
            return new Lazy<T>(LazyValue);
#endif
        }
    }
}
