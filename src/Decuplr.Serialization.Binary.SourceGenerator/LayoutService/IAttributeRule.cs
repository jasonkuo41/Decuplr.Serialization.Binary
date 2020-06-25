using System;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal interface IAttributeRule<TMeta> : ISymbolCondition<TMeta, IAttributeRule<TMeta>> {
        /// <summary>
        /// Verify if the conditions that a attribute contains is correctly contained
        /// </summary>
        IAttributeRule<TMeta> VerifyCondition(Func<AttributeData, Condition> conditionProvider);
    }
}
