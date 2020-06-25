using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.AnalysisService {
    internal struct AttributeListing {
        public IReadOnlyList<IReadOnlyList<AttributeData>> Lists { get; set; }
        public IReadOnlyDictionary<AttributeData, Location> Locations { get; set; }
    }
}
