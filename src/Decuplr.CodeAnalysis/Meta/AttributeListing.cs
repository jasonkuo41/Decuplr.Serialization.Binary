using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Meta {
    internal struct AttributeListing {
        public IReadOnlyList<IReadOnlyList<AttributeData>> Lists { get; set; }
        public IReadOnlyDictionary<AttributeData, Location> Locations { get; set; }
        public void Deconstruct(out IReadOnlyList<IReadOnlyList<AttributeData>> lists, out IReadOnlyDictionary<AttributeData, Location> locations) {
            lists = Lists;
            locations = Locations;
        }
    }
}
