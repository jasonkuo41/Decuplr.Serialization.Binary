namespace Decuplr.Serialization.CodeGeneration {
    public struct FunctionSource<TArgs> {

        private readonly string _functionName;

        public FunctionSource(string functionName, TArgs args) {
            _functionName = functionName;
        }

        public string GetNextFunction(TArgs args) {

        }
    }
}
