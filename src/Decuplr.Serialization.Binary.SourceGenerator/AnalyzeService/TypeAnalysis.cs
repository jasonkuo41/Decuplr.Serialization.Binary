using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.AnalyzeService {

    interface IOrderSelector {

    }

    interface IAttributeRuleConfiguration {
        IReportDiagnostic ApplyOnly(Func<AnalyzedType, bool> selector);
        IReportDiagnostic ConflictWhen(Func<AnalyzedType, bool> selector);
    }

    interface IReportDiagnostic {
        void ReportDiagnostic(Func<AnalyzedType, Diagnostic> type);
    }

    internal class TypeAnalysis {

        private readonly SourceCodeAnalysis CodeAnalysis;
        private IOrderSelector? OrderSelector;

        public TypeAnalysis(SourceCodeAnalysis analysis) {
            CodeAnalysis = analysis;
        }

        public void UseOrderSelector<TSelector>() where TSelector : IOrderSelector, new() {
            OrderSelector = new TSelector();
        }

        public IAttributeRuleConfiguration AttributeRule<TAttribute>() where TAttribute : Attribute {

        }
    }
}
