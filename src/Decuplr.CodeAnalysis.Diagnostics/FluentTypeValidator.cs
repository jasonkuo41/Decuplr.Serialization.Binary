using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.CodeAnalysis.Diagnostics.Internal {
    internal class FluentTypeValidator : IFluentTypeGroupValidator {

        private readonly FluentMemberValidator _layoutMembers;
        private readonly FluentMemberValidator _anyMembers;
        private readonly FluentMemberValidator _excludedMembers;

        public IFluentMemberValidator SelectedMembers => _layoutMembers;
        public IFluentMemberValidator AnyMembers => _anyMembers;
        public IFluentMemberValidator ExcludedMembers => _excludedMembers;

        public FluentTypeValidator(TypeMetaSelection selection, ConditionAnalyzer conditionAnalyzer) {
            var type = selection.Type;
            _anyMembers = new FluentMemberValidator(TypeMetaSelection.Any(type), conditionAnalyzer);
            _layoutMembers = new FluentMemberValidator(selection, conditionAnalyzer);
            _excludedMembers = new FluentMemberValidator(selection.ReverseSelection(), conditionAnalyzer);
        }

        public void Validate(IDiagnosticReporter reporter) {
            _anyMembers.Validate(reporter);
            _layoutMembers.Validate(reporter);
            _excludedMembers.Validate(reporter);
        }
    }
}
