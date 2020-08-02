using System;
using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface ISourceValidation {
        void Validate(TypeMetaSelection selection);
        void ValidateExternal(TypeMetaSelection selection, IGroupValidationProvider groupValidation, IDiagnosticReporter reporter);
        void ValidateExternal(NamedTypeMetaInfo type, ITypeValidationProvider typeValidation, IDiagnosticReporter reporter);
    }
}