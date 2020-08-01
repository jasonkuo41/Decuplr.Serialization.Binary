using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.CodeAnalysis.Diagnostics.Internal;
using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.CodeAnalysis.Diagnostics.Internal {

    internal class SourceValidation : ISourceValidation {

        private readonly IDiagnosticReporter _diagnostics;
        private readonly IEnumerable<IGroupValidationProvider> _groupProviders;
        private readonly IEnumerable<ITypeValidationProvider> _typeProviders;

        public SourceValidation(IEnumerable<IGroupValidationProvider> groupValidationProviders, IEnumerable<ITypeValidationProvider> typeValidationProviders, IDiagnosticReporter diagnostics) {
            _groupProviders = groupValidationProviders;
            _typeProviders = typeValidationProviders;
            _diagnostics = diagnostics;
        }

        public void Validate(NamedTypeMetaInfo type, Func<MemberMetaInfo, bool> memberSelector) {
            foreach (var typeValidation in _typeProviders) {
                var typeValidator = new FluentMemberValidator(type, type.Members);
                typeValidation.ConfigureValidation(typeValidator);
                typeValidator.Validate(_diagnostics);
            }
            foreach (var groupValidation in _groupProviders) {
                var groupValidator = new FluentTypeValidator(type, type.Members.Where(memberSelector), type.Members.Where(x => !memberSelector(x)));
                groupValidation.ConfigureValidation(groupValidator);
                groupValidator.Validate(_diagnostics);
            }
        }

    }
}
