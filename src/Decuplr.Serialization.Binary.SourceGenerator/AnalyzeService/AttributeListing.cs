using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.AnalyzeService {
    internal struct AttributeListing {
        public IReadOnlyList<IReadOnlyList<AttributeData>> Lists { get; set; }
        public IReadOnlyDictionary<AttributeData, Location> Locations { get; set; }
    }

}
