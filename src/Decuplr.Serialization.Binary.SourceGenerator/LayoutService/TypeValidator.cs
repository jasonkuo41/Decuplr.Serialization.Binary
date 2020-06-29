using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.AnalysisService;
using Decuplr.Serialization.Binary.LayoutService.Internal;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {

    internal class TypeValidator {

        private readonly NamedTypeMetaInfo _type;
        private readonly SchemaPrecusor _precusor;

        private readonly DiagnosticReporter _reporter = new DiagnosticReporter();
        private readonly LayoutMemberCollection _layoutMembers = new LayoutMemberCollection();
        private readonly LayoutMemberCollection _anyMembers = new LayoutMemberCollection();

        public TypeValidator(NamedTypeMetaInfo type, SchemaPrecusor precusor) {
            _type = type;
            _precusor = precusor;
        }

        public ILayoutMemberValidation WithLayoutMembers() => _layoutMembers;

        public ILayoutMemberValidation WithAnyMembers() => _anyMembers;

        /// <summary>
        /// Validate if every attribute is correct and can generate correct layout
        /// </summary>
        public bool ValidateLayout<TSelector>(Func<MemberMetaInfo, bool> memberPredicate, out TypeLayout? layout, out IEnumerable<Diagnostic> diagnostics) where TSelector : IOrderSelector, new() {
            var selector = new TSelector();

            var layoutFilter = new LayoutMemberCollection();
            selector.ValidateMembers(layoutFilter);
            layoutFilter.ValidateLayout(_type, _type.Members, _reporter);

            // If we fail the elect member should we just skip this step and return early?
            var layoutMembers = selector.GetOrder(_type.Members.Where(memberPredicate), _precusor.RequestLayout, _reporter).ToList();
            
            // If layout members is empty then we don't serialize it too
            if (layoutMembers.Count == 0)
                _reporter.ReportDiagnostic(DiagnosticHelper.NoMember(_type));

            diagnostics = _reporter.ExportDiagnostics();
            if (_reporter.IsUnrecoverable) {
                layout = null;
                return false;
            }

            layout = new TypeLayout(_type, layoutMembers);
            return true;
        }
    }
}
