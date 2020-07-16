namespace Decuplr.Serialization.CodeGeneration.ParserGroup {
    public struct ParserMethodNames {
        public string TryDeserializeSpan { get; set; }
        public string TryDeserializeSequence { get; set; }
        public string DeserializeSpan { get; set; }
        public string DeserializeSequence { get; set; }
        public string TrySerialize { get; set; }
        public string Serialize { get; set; }
        public string GetLength { get; set; }

        public static ParserMethodNames DefaultNames =>
            new ParserMethodNames {
                DeserializeSequence = "Deserialize",
                DeserializeSpan = "Deserialize",
                TryDeserializeSequence = "TryDeserialize",
                TryDeserializeSpan = "TryDeserialize",
                Serialize = "Serialize",
                TrySerialize = "TrySerialize",
                GetLength = "GetLength"
            };
    }
}
