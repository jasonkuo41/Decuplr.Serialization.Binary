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
        private readonly ConditionAnalyzer _conditionAnalyzer;

        public SourceValidation(IEnumerable<IGroupValidationProvider> groupValidationProviders, IEnumerable<ITypeValidationProvider> typeValidationProviders, ConditionAnalyzer conditions, IDiagnosticReporter diagnostics) {
            _groupProviders = groupValidationProviders;
            _typeProviders = typeValidationProviders;
            _conditionAnalyzer = conditions;
            _diagnostics = diagnostics;
        }

        public void Validate(TypeMetaSelection selection) {
            foreach (var typeValidation in _typeProviders) {
                ValidateExternal(selection.Type, typeValidation, _diagnostics);
            }
            foreach (var groupValidation in _groupProviders) {
                ValidateExternal(selection, groupValidation, _diagnostics);
            }
        }

        public void ValidateExternal(TypeMetaSelection selection, IGroupValidationProvider groupValidation, IDiagnosticReporter reporter) {
            var groupValidator = new FluentTypeValidator(selection, _conditionAnalyzer);
            groupValidation.ConfigureValidation(groupValidator);
            groupValidator.Validate(reporter);
        }

        public void ValidateExternal(NamedTypeMetaInfo type, ITypeValidationProvider typeValidation, IDiagnosticReporter reporter) {
            var typeValidator = new FluentMemberValidator(TypeMetaSelection.Any(type), _conditionAnalyzer);
            typeValidation.ConfigureValidation(typeValidator);
            typeValidator.Validate(reporter);
        }
    }
}
