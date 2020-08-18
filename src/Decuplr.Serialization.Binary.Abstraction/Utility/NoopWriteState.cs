using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary.Utility {
    internal struct NoopWriteState : IBinaryWriteState<NoopWriteState> {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Write<T>(in T item, out NoopWriteState state) {
            state = this;
            return true;
        }
    }

}
