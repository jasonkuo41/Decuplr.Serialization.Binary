using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface ISymbolCondition<TMeta, TSource> {
        /// <summary>
        /// Invalid on targets that matches the conditions
        /// </summary>
        ISymbolCondition<TMeta, TSource> InvalidOn(Func<TMeta, bool> predicate);
        /// <summary>
        /// Report Diagnostic 
        /// </summary>
        TSource ReportDiagnostic(Func<TMeta, Diagnostic> diagnostic);
    }
}
