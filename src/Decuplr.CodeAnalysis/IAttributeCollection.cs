using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {
    public interface IAttributeCollection {

        /// <summary>
        /// The attributes that this collection contains, may not be in order.
        /// </summary>
        IReadOnlyList<AttributeSourceInfo> Attributes { get; }

        /// <summary>
        /// Contains the location and the order of the attributes, maybe empty if it's not applicable for such feature.
        /// </summary>
        /// <remarks>Only symbols that are located in source code have this property.</remarks>
        IReadOnlyDictionary<Location, IReadOnlyList<AttributeSourceInfo>> AttributeLocations { get; }

        bool ContainsAttribute<TAttribute>() where TAttribute : Attribute;
        bool ContainsAttribute(Type attributeType);
        AttributeSourceInfo<TAttribute>? GetAttribute<TAttribute>() where TAttribute : Attribute;
        AttributeSourceInfo? GetAttribute(Type attributeType);
        IEnumerable<AttributeSourceInfo<TAttribute>> GetAttributes<TAttribute>() where TAttribute : Attribute;
        IEnumerable<AttributeSourceInfo> GetAttributes(Type attributeType);
    }

}
