using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal class ParsingContext : CompilationContext {

        public SchemaLayout? CurrentLayout { get; set; }

        public void SetContext(SchemaLayout layout, CompilationContext context) {
            CopyFrom(context);
            CurrentLayout = layout;
        }
    }

}
