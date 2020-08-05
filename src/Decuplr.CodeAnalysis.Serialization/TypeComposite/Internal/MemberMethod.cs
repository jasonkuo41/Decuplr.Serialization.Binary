namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal static class MemberMethod {
        public static string InitializeComponent(int count) => $"{nameof(InitializeComponent)}_{count}";
        public static string TryInitializeComponent(int count) => $"{nameof(TryInitializeComponent)}_{count}";
        public static string TryDeserializeState(int index) => $"TryDeserializeState_{index}";
        public static string DeserializeState(int index) => $"DeserializeState_{index}";
        public static string TrySerializeState(int index) => $"TrySerializeState_{index}";
        public static string SerializeState(int index) => $"SerializeState_{index}";
        public static string GetLengthState(int index) => $"GetLengthState_{index}";
    }
}
