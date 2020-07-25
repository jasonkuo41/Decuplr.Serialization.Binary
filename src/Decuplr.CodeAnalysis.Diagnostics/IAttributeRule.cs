using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface IAttributeRule<TMeta> : ISymbolCondition<TMeta, IAttributeRule<TMeta>> {
        /// <summary>
        /// Verify if the conditions that a attribute contains is correctly contained
        /// </summary>
        IAttributeRule<TMeta> VerifyCondition(Func<AttributeData, Condition> conditionProvider);
    }
}
