using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Decuplr.Serialization.Binary.Parsers {
    /// <summary>
    /// Allows unmanaged data to be write into unalligned span (for example writing a four-byte int into a three byte space)
    /// </summary>
    internal static class UnallignedMemory {
        public static unsafe void WriteLittleEndian<T>(ref T value, Span<byte> dest) where T : unmanaged {
            // One liner!
            // Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref MemoryMarshal.GetReference(dest)), Unsafe.AsPointer(ref value), (uint)dest.Length);
            var typeSize = Unsafe.SizeOf<T>();
            var writeSize = Math.Min(typeSize, dest.Length);
            fixed (T* val = &value) {
                byte* bp = (byte*)val;
                if (BitConverter.IsLittleEndian) {
                    for (var i = 0; i < writeSize; ++i) {
                        dest[i] = *(bp + i);
                    }
                }
                else {
                    for (var i = typeSize - 1; i >= Math.Min(0, dest.Length - typeSize); --i) {
                        dest[typeSize -1 - i] = *(bp + i);
                    }
                }
                // Fill up zero from writeSize to the whole span
                for (var i = writeSize; i < dest.Length; ++i) {
                    dest[i] = 0;
                }
            }
        }

        public static unsafe void WriteBigEndian<T>(ref T value, Span<byte> dest) where T : unmanaged {
            throw new NotImplementedException();
        }

        public static unsafe void ReadLittleEndian<T>(in T value, ReadOnlySpan<byte> dest) where T : unmanaged {
            throw new NotImplementedException();
        }

        public static unsafe void ReadBigEndian<T>(ref T value, Span<byte> dest) where T : unmanaged {
            throw new NotImplementedException();
        }
    }
}
