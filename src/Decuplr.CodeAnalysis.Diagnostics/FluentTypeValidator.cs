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

        public FluentTypeValidator(NamedTypeMetaInfo type, IEnumerable<MemberMetaInfo> includedMember, IEnumerable<MemberMetaInfo> excludedMember) {
            _layoutMembers = new FluentMemberValidator(type, type.Members);
            _anyMembers = new FluentMemberValidator(type, includedMember);
            _excludedMembers = new FluentMemberValidator(type, excludedMember);
        }

        public void Validate(IDiagnosticReporter reporter) {
            _anyMembers.Validate(reporter);
            _layoutMembers.Validate(reporter);
            _excludedMembers.Validate(reporter);
        }
    }
}
