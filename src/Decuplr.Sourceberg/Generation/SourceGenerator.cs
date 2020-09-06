namespace Decuplr.Sourceberg.Generation {
    public abstract class SourceGenerator<TStartup> where TStartup : GeneratorStartup {
        public abstract void Execute(GeneratingContext context);
    }
}
