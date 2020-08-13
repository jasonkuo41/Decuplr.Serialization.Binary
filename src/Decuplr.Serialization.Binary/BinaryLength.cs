using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Defines an opaque type that holds all the relevant binary length information, which must be then provided to the BinarySerializer for consumption
    /// </summary>
    //
    //   Note we use ref struct as a guard so to hint people not to copy or attempt to cache this info.
    //
    //   Originally we want to store the reference for the item, but then for std 2.0 and below we can't do that (because MemoryMarsh.CreateSpan is not present)
    //   and using Unsafe to grab the reference might poke a hole to the GC system
    //
    //   But it's okay if the user passed two different item to GetSpanLength and Serialize, as the Serialize would likely check if the length info
    //   is correct, if not, an exception will be thrown.
    //
    public ref struct BinaryLength {

        private readonly ArrayPool<int> _pool;
        private readonly ReadOnlySpan<int> _lengthData;
        private int[]? _lengthArray;

        public int Value { get; }
        public BinarySerializerOptions Options { get; }

        internal ReadOnlySpan<int> LengthInfo {
            get {
                if (_lengthArray is null)
                    ThrowHelper.ThrowDisposed();
                return _lengthData;
            }
        }

        internal BinaryLength(int lengthValue, BinarySerializerOptions options, ArrayPool<int> pool, int[] array, int count) {
            _pool = pool;
            _lengthArray = array;
            _lengthData = array.AsSpan(0, count);
            Value = lengthValue;
            Options = options;
        }

        public void Dispose() {
            if (_lengthArray is null)
                return;
            var array = _lengthArray;
            _lengthArray = null;
            _pool.Return(array);
        }

        public static implicit operator int(BinaryLength length) => length.Value;

        private class ThrowHelper {
            public static void ThrowDisposed() => throw DisposeException();

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static Exception DisposeException() => new ObjectDisposedException(nameof(BinaryLength));
        }
    }

}
