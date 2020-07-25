using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.Serialization.LayoutService.Internal;

namespace Decuplr.CodeAnalysis.Diagnostics {

    public class TypeValidation {

        private static readonly IReadOnlyList<MemberMetaInfo> EmptyMember = Array.Empty<MemberMetaInfo>();
        private readonly NamedTypeMetaInfo _type;
        private readonly IOrderSelector _orderSelector;
        private readonly List<IValidationSource> _sources = new List<IValidationSource>();

        private IEnumerable<MemberMetaInfo> _selectedMembers;

        private TypeValidation(NamedTypeMetaInfo type, IOrderSelector orderSelector) {
            _type = type;
            _orderSelector = orderSelector;
            _selectedMembers = type.Members;
        }

        public static TypeValidation CreateFrom(NamedTypeMetaInfo type, IOrderSelector orderSelector)
            => new TypeValidation(type, orderSelector);

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
        public bool TryValidateLayout(IDiagnosticReporter reporter, out SchemaLayout? layout) {
            layout = null;
            if (!TryEvalOrder(out var layoutMembers))
                return false;

            if (!TryEvalMember(reporter))
                return false;

            Debug.Assert(!reporter.ContainsError);
            layout = new SchemaLayout(_type, layoutMembers);
            return true;

            bool TryEvalOrder(out IReadOnlyList<MemberMetaInfo> members) {
                members = EmptyMember;

                var layoutFilter = new FluentMemberValidator(_type, _type.Members);

                _orderSelector.ConfigureMemeberValidation(layoutFilter);
                layoutFilter.ValidateLayout(reporter);

                if (!_orderSelector.ContinueOnFailedValidation && reporter.ContainsError)
                    return false;

                // Generate the order from the selector
                members = _orderSelector.GetOrder(_selectedMembers, reporter).ToList();

                if (reporter.ContainsError)
                    return false;

                // If no member is present, we ignore this type and it would not be generated
                if (members.Count == 0) {
                    reporter.ReportDiagnostic(DiagnosticHelper.NoMember(_type));
                    return false;
                }

                return true;
            }

            bool TryEvalMember(IDiagnosticReporter reporter) {
                var validation = new FluentTypeValidator(_type, _selectedMembers);
                foreach (var source in _sources)
                    source.ValidConditions(validation);
                validation.Verify(reporter);
                return !reporter.ContainsError;
            }

        }

    }
}
