using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.AnalysisService;
using Decuplr.Serialization.Binary.LayoutService.Internal;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {

    internal class TypeValidator {

        private readonly TypeMetaInfo Type;
        private readonly SchemaPrecusor Precusor;
        private readonly DiagnosticReporter Reporter;
        private readonly IMemberFilter Filter;

        public ILayoutMemberCollection LayoutMembers { get; }

        public ILayoutMemberCollection AllMembers { get; }


        public TypeValidator(TypeMetaInfo type, SchemaPrecusor precusor, IMemberFilter filter) {
            Type = type;
            Precusor = precusor;
        }

        /// <summary>
        /// Validate if every attribute is correct and can generate correct layout
        /// </summary>
        public bool ValidateLayout<TSelector>(IMemberFilter filter, out TypeFormatLayout layout, out IEnumerable<Diagnostic> diagnostics) where TSelector : IOrderSelector, new() {
            BinaryLayout memberLayout = Precusor.RequestLayout;
            if (Precusor.RequestLayout == BinaryLayout.Auto)
                memberLayout = Filter.Selector.GetAutoLayoutImplication(Type);

            var layoutMembers = Filter.SelectMembers(Type.Members);
            if (Reporter.IsFaulted)
                return false;

        }
    }
}
