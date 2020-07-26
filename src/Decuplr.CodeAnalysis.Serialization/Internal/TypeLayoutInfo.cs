
namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal readonly struct TypeLayoutInfo {
        public SchemaLayout Layout { get; }

        public IGenerationStartup Startup { get; }

        public SchemaInfo SchemaInfo { get; }

        public TypeLayoutInfo(SchemaLayout layout, IGenerationStartup startup, SchemaInfo schemaInfo) {
            Layout = layout;
            Startup = startup;
            SchemaInfo = schemaInfo;
        }

        public void Deconstruct(out SchemaLayout layout, out IGenerationStartup startup, out SchemaInfo config) {
            layout = Layout;
            startup = Startup;
            config = SchemaInfo;
        }

    }

}
