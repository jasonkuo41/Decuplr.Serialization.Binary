using System.Collections.Generic;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IGenerationFinalization {
        /// <summary>
        /// A callback that occurs when all the generation is completed and needed to clean up or finailize
        /// </summary>
        void OnGenerationFinalized(IEnumerable<TypeParserInfo> parsedTypes);
    }
}
