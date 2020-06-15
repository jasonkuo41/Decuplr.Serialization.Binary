namespace Decuplr.Serialization.Binary {
    struct GeneratedFormatFunction {
        public GeneratedFormatFunction(string name, string functionSource) {
            FunctionName = name;
            FunctionSourceText = functionSource;
        }

        public string FunctionSourceText { get; set; }
        public string FunctionName { get; set; }
    }

}
