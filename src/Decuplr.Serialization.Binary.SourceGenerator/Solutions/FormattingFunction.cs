namespace Decuplr.Serialization.Binary.SourceGenerator {
    struct FormattingFunction {
        public FormattingFunction(string name, string functionSource) {
            FunctionName = name;
            FunctionSourceText = functionSource;
        }

        public string FunctionSourceText { get; set; }
        public string FunctionName { get; set; }
    }

}
