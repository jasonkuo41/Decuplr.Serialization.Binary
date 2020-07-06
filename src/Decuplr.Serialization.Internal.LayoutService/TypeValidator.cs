using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService.Internal;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.LayoutService {

    public class TypeValidator : ITypeValidator {

        private readonly NamedTypeMetaInfo _type;
        private readonly SchemaPrecusor _precusor;

        private readonly LayoutMemberCollection _layoutMembers = new LayoutMemberCollection();
        private readonly LayoutMemberCollection _anyMembers = new LayoutMemberCollection();

        public ILayoutMemberValidation LayoutMembers => _layoutMembers;
        public ILayoutMemberValidation AnyMembers => _anyMembers;

        public TypeValidator(NamedTypeMetaInfo type, SchemaPrecusor precusor) {
            _type = type;
            _precusor = precusor;
        }

        /// <summary>
        /// Validate if every attribute is correct and can generate correct layout
        /// </summary>
        public bool ValidateLayout(Func<MemberMetaInfo, bool> memberPredicate, IOrderSelector orderSelector, out TypeLayout? layout, out IEnumerable<Diagnostic> diagnostics) {
            var reporter = new DiagnosticReporter();
            var layoutMembers = ValidateLayoutInternal(reporter);

            diagnostics = reporter.ExportDiagnostics();
            if (reporter.IsUnrecoverable) {
                layout = null;
                return false;
            }
            Debug.Assert(layoutMembers != null);
            layout = new TypeLayout(_type, layoutMembers!);
            return true;

            IReadOnlyList<MemberMetaInfo>? ValidateLayoutInternal(DiagnosticReporter reporter) {
                var layoutFilter = new LayoutMemberCollection();
                orderSelector.ValidateMembers(layoutFilter);
                layoutFilter.ValidateLayout(_type, _type.Members, reporter);

                // If we fail the elect member should we just skip this step and return early?
                var layoutMembers = orderSelector.GetOrder(_type.Members.Where(memberPredicate), _precusor.RequestLayout, reporter).ToList();
                if (reporter.IsUnrecoverable)
                    return null;

                // If layout members is empty then we don't serialize it too
                if (layoutMembers.Count == 0) {
                    reporter.ReportDiagnostic(DiagnosticHelper.NoMember(_type));
                    return null;
                }

                _layoutMembers.ValidateLayout(_type, layoutMembers, reporter);
                _anyMembers.ValidateLayout(_type, _type.Members, reporter);

                if (reporter.IsUnrecoverable)
                    return null;

                return layoutMembers;
            }
        }
    }
}
