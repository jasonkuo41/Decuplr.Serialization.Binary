using System;

namespace Decuplr.Serialization.Binary.Internal {
    internal interface ICollectionLengthFormatter {
        /// <summary>
        /// Is the length appended to the front; otherwise to the end
        /// </summary>
        bool IsAppendFront { get; }

        /// <summary>
        /// Get's the collection's length
        /// </summary>
        int ReadCollectionLength(ReadOnlySpan<byte> span, out int bytesRead);

        bool TryWriteCollectionLength(int length, Span<byte> span, out int bytesWrote);

        /// <summary>
        /// Get's the binary size of a length
        /// </summary>
        int GetLengthBinarySize(int length);
    }
}
