using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService.Internal;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.LayoutService {
    public interface IValidationSource {
        void ValidConditions(ITypeValidator validator);
    }

    public class TypeValidation {

        private class DefaultValidator : ITypeValidator {

            private readonly TypeValidation _parent;
            private readonly LayoutMemberCollection _layoutMembers = new LayoutMemberCollection();
            private readonly LayoutMemberCollection _anyMembers = new LayoutMemberCollection();

            public ILayoutMemberValidation LayoutMembers => _layoutMembers;
            public ILayoutMemberValidation AnyMembers => _anyMembers;

            public DefaultValidator(TypeValidation validation) {
                _parent = validation;
            }

            public void Verify(IDiagnosticReporter reporter) {
                var type = _parent._type;
                _anyMembers.ValidateLayout(type, type.Members, reporter);
                _layoutMembers.ValidateLayout(type, _parent._selectedMembers, reporter);
            }
        }

        private readonly NamedTypeMetaInfo _type;
        private readonly SchemaPrecusor _precusor;
        private readonly IOrderSelector _orderSelector;
        private readonly List<IValidationSource> _sources = new List<IValidationSource>();
        private readonly DefaultValidator _validation;

        private IEnumerable<MemberMetaInfo> _selectedMembers;

        public ITypeValidator Validator => _validation;

        private TypeValidation(NamedTypeMetaInfo type, SchemaPrecusor precusor, IOrderSelector orderSelector) {
            _type = type;
            _precusor = precusor;
            _orderSelector = orderSelector;
            _selectedMembers = type.Members;
            _validation = new DefaultValidator(this);
        }

        public static TypeValidation CreateFrom(NamedTypeMetaInfo type, SchemaPrecusor precusor, IOrderSelector orderSelector)
            => new TypeValidation(type, precusor, orderSelector);

        public TypeValidation Where(Func<MemberMetaInfo, bool> serializeMemberSelector) {
            _selectedMembers = _selectedMembers.Where(serializeMemberSelector);
            return this;
        }

        public TypeValidation AddValidationSource(IValidationSource source) {
            _sources.Add(source);
            return this;
        }

        public TypeValidation AddValidationSource(IEnumerable<IValidationSource> source) {
            _sources.AddRange(source);
            return this;
        }

        /// <summary>
        /// Validate if every attribute is correct and can generate correct layout
        /// </summary>
        public bool ValidateLayout(out TypeLayout? layout, out IEnumerable<Diagnostic> diagnostics) {
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
                _orderSelector.ValidateMembers(layoutFilter);
                layoutFilter.ValidateLayout(_type, _type.Members, reporter);

                // If we fail the elect member should we just skip this step and return early?
                var layoutMembers = _orderSelector.GetOrder(_selectedMembers, _precusor.RequestLayout, reporter).ToList();
                if (reporter.IsUnrecoverable)
                    return null;

                // If layout members is empty then we don't serialize it too
                if (layoutMembers.Count == 0) {
                    reporter.ReportDiagnostic(DiagnosticHelper.NoMember(_type));
                    return null;
                }

                _validation.Verify(reporter);

                if (reporter.IsUnrecoverable)
                    return null;

                return layoutMembers;
            }

        }

    }
}
