using System.Collections.Generic;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService.Internal;

namespace Decuplr.CodeAnalysis.Diagnostics {
    internal class FluentTypeValidator : IFluentTypeValidator {

        private readonly FluentMemberValidator _layoutMembers;
        private readonly FluentMemberValidator _anyMembers;

        public IFluentMemberValidator LayoutMembers => _layoutMembers;
        public IFluentMemberValidator AnyMembers => _anyMembers;

        public FluentTypeValidator(NamedTypeMetaInfo type, IEnumerable<MemberMetaInfo> layoutMembers) {
            _layoutMembers = new FluentMemberValidator(type, type.Members);
            _anyMembers = new FluentMemberValidator(type, layoutMembers);
        }

        public void Verify(IDiagnosticReporter reporter) {
            _anyMembers.ValidateLayout(reporter);
            _layoutMembers.ValidateLayout(reporter);
        }
    }
}
