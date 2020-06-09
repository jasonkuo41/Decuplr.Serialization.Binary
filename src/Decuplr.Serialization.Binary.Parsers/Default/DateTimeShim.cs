using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Decuplr.Serialization.Binary.Annotations;
using Decuplr.Serialization.Binary.Annotations.Namespaces;

namespace Decuplr.Serialization.Binary.Internal.DefaultParsers {
    // Uses Microsoft ToBinary and FromBinary
    [BinaryParser(typeof(DateTime))]
    [BinaryParserNamespace("Default")]
    internal partial struct DateTimeShim : ITypeConvertible<DateTime> {
        public DateTimeShim (DateTime time) {
            ActualTime = time.ToBinary();
        }

        [Index(0)]
        public long ActualTime { get; }

        public DateTime ConvertTo() => DateTime.FromBinary(ActualTime);
    }
}
