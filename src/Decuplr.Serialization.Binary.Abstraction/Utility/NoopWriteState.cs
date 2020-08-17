using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary.Utility {
    internal struct NoopWriteState<T> : IBinaryWriteState<NoopWriteState<T>, T> {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Write(in T item, out NoopWriteState<T> state) {
            state = this;
            return true;
        }
    }

}
