using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary.Utility {
    internal readonly struct NoopWriteState : IBinaryWriteState<NoopWriteState> {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetNextState<T>(in T item, out NoopWriteState state) {
            state = this;
            return true;
        }
    }

}
