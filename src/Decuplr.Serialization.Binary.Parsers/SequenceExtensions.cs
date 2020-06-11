using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Decuplr.Serialization.Binary.Parsers {
    static class SequenceExtensions {
        /// <summary>
        /// Copies with loop, really slow, you should read it once before invoking this
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TrySlowCopyTo(this in ReadOnlySequence<byte> sequence, Span<byte> data) {
            var pos = sequence.Start;
            var left = sizeof(int);
            while (sequence.TryGet(ref pos, out var memory, true)) {
                if (memory.Length <= 0)
                    continue;
                var nextSpan = memory.Span;
                var readLength = Math.Min(left, nextSpan.Length);
                memory.Slice(0, readLength).Span.CopyTo(data.Slice(data.Length - left));
                if (readLength == left)
                    return true;
                left -= readLength;
            }
            return false;
        }

    }
}
