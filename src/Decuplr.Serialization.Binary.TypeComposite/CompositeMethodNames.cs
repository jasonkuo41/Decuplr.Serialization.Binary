
namespace Decuplr.Serialization.Binary.TypeComposite {
    public struct CompositeMethodNames {
        public string GetCursorBlockLength { get; set; }
        public string GetSpanBlockLength { get; set; }
        public string DeserializeSpan { get; set; }
        public string DeserializeCursor { get; set; }
        public string SerializeSpan { get; set; }
        public string SerializeWriter { get; set; }
        public string GetSpanLength { get; set; }

        public static CompositeMethodNames DefaultNames =>
            new CompositeMethodNames {
                DeserializeCursor    = "Deserialize",
                DeserializeSpan      = "Deserialize",
                SerializeWriter      = "Serialize",
                SerializeSpan        = "Serialize",
                GetCursorBlockLength = "GetBlockLength",
                GetSpanBlockLength   = "GetBlockLength",
                GetSpanLength        = "GetSpanLength"
            };

        internal static CompositeMethodNames AppendDefault(string appendingString)
            => new CompositeMethodNames {
                DeserializeCursor    = DefaultNames.DeserializeCursor    + appendingString,
                DeserializeSpan      = DefaultNames.DeserializeSpan      + appendingString,
                SerializeWriter      = DefaultNames.SerializeWriter      + appendingString,
                SerializeSpan        = DefaultNames.SerializeSpan        + appendingString,
                GetCursorBlockLength = DefaultNames.GetCursorBlockLength + appendingString,
                GetSpanBlockLength   = DefaultNames.GetSpanBlockLength   + appendingString,
                GetSpanLength        = DefaultNames.GetSpanLength        + appendingString
            };
    }
}
