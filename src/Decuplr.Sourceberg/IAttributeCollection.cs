using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {

    /// <summary>
    /// Represents a detailed collection of attributes
    /// </summary>
    public interface IAttributeCollection {

        /// <summary>
        /// The symbol that is holds this attribute set.
        /// </summary>
        ISymbol ContainingSymbol { get; }

        /// <summary>
        /// The attributes that this collection contains, may not be in order.
        /// </summary>
        IReadOnlyList<AttributeLayout> Attributes { get; }

        /// <summary>
        /// Contains the location and the order of the attributes, maybe empty if it's not applicable for such feature.
        /// </summary>
        /// <remarks>Only symbols that are located in source code have this property.</remarks>
        IReadOnlyDictionary<Location, IReadOnlyList<AttributeLayout>> AttributeLocations { get; }

        /// <summary>
        /// If the symbol contains <typeparamref name="TAttribute"/>
        /// </summary>
        /// <typeparam name="TAttribute">The attribute that it should contain</typeparam>
        /// <returns>If the symbol has the attribute</returns>
        bool ContainsAttribute<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        /// If the symbol contains <paramref name="attributeType"/>
        /// </summary>
        /// <param name="attributeType">The attribute that it should contain</param>
        /// <returns>If the symbol has the attribute</returns>
        bool ContainsAttribute(Type attributeType);

        AttributeLayout<TAttribute>? GetAttribute<TAttribute>() where TAttribute : Attribute;
        AttributeLayout? GetAttribute(Type attributeType);
        IEnumerable<AttributeLayout<TAttribute>> GetAttributes<TAttribute>() where TAttribute : Attribute;
        IEnumerable<AttributeLayout> GetAttributes(Type attributeType);
    }

}
