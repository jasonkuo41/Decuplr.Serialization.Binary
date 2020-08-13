using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Decuplr.Serialization.Binary.Annotations;
using Decuplr.Serialization.Binary.Annotations.Namespaces;

namespace Decuplr.Serialization.Binary.Internal.DefaultParsers {
    // Uses Microsoft ToBinary and FromBinary
    [BinaryParser(typeof(DateTime))]
    [BinaryParserNamespace("Default")]
    internal partial struct DateTimeShim : ITypeConvertible<DateTime> {
        public DateTimeShim(in DateTime time) {
            ActualTime = time.ToBinary();
        }

        [Index(0)]
        public long ActualTime { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime ConvertTo() => DateTime.FromBinary(ActualTime);
    }
}
