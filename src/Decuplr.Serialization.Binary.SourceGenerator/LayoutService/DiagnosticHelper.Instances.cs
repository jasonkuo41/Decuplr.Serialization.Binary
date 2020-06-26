using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Serialization.Binary.AnalysisService;
using Decuplr.Serialization.Binary.LayoutService.Internal;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {

    internal partial class DiagnosticHelper {

        [Diagnostic(0, DiagnosticSeverity.Error, "Invalid serialization target", "Keyword {0} of '{1}' is not a valid target for serialization")]
        public static Diagnostic InvalidKeyword(string invalidKeyword, MemberMetaInfo member) => CreateDiagnostic(member.Location, new object?[] { invalidKeyword, member.MemberSymbol });

        [Diagnostic(1, DiagnosticSeverity.Error, "Invalid serialization type", "Return type '{0}' of '{1}' is a {2} type, thus is not a valid type for serialization.")]
        public static Diagnostic UnsupportedType(MemberMetaInfo member) => CreateDiagnostic(member.Location, new object?[] { member.ReturnType?.Name, member.MemberSymbol, member.ReturnType?.TypeKind });

        [Diagnostic(2, DiagnosticSeverity.Error, "Conflicting attributes", "{0} cannot be applied to the same target '{1}' as they conflict their intensions with each other.")]
        public static Diagnostic ConflictingAttributes(MemberMetaInfo member, params AttributeData[] conflictingAttributes) {
            var conflicts = conflictingAttributes.ToList();
            if (conflicts.Count < 2)
                throw new ArgumentOutOfRangeException(nameof(conflictingAttributes));

            var primeLocation = member.GetLocation(conflicts[0]);
            var secondary = conflicts.Select(conflict => member.GetLocation(conflict));
            var conflictNames = string.Join(",", conflictingAttributes.Select(x => x.AttributeClass?.Name));

            return CreateDiagnostic(primeLocation, secondary, new object[] { conflictNames, member });
        }

        [Diagnostic(3, DiagnosticSeverity.Error, "Explicit sequential layout should not contain IndexAttribute", "'{0}' contains IndexAttribute which is not allowed with explicitly assigned sequential layout.")]
        public static Diagnostic ExplicitSequentialShouldNotIndex(MemberMetaInfo member, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new object[] { member.MemberSymbol.ToString() });
    }
}
