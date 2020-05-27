using System.ComponentModel;

namespace Decuplr.Serialization.Binary.Generic {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class NestedGenericOneArgsParserProvider : GenericParserProvider {

        public TypeParser<TGeneric> ProvideNestedGenericFormatter<TGeneric, T1>(TypeParser<T1> parser1)
            => (TypeParser<TGeneric>)ProvideFormatter(parser1);

        protected abstract object ProvideFormatter<T1>(TypeParser<T1> parser1);
    }
}
