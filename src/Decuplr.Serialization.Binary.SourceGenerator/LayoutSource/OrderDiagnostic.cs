using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {
    
    [DiagnosticSource("DSID", "Decuplr.Serialization.IndexOrder")]
    internal class OrderDiagnostic : DiagnosticHelper {
        [Diagnostic(0, DiagnosticSeverity.Error, "Invalid serialization target", "Keyword {0} of '{1}' is not a valid target for serialization.")]
        internal static Diagnostic InvalidKeyword(string invalidKeyword, MemberMetaInfo member) => CreateDiagnostic(member.Location, new object?[] { invalidKeyword, member.Symbol });


        [Diagnostic(1, DiagnosticSeverity.Error,
            "Invalid serialization type",
            "Return type '{0}' of '{1}' is a {2} type, thus is not a valid target for serialization."
        )]
        internal static Diagnostic UnsupportedType(MemberMetaInfo member) => CreateDiagnostic(member.Location, new object?[] { member.ReturnType?.Name, member.Symbol, member.ReturnType?.Symbol.TypeKind });

        [Diagnostic(2, DiagnosticSeverity.Warning,
            "Invalid serialization type should be marked as ignored",
            "Return type '{0}' of '{1}' is a {2} type, thus will not be serialized, IgnoreAttribute should be applied to explicitly."
        )]
        internal static Diagnostic UnsupportedTypeHint(MemberMetaInfo member) => CreateDiagnostic(member.Location, new object?[] { member.ReturnType?.Name, member.Symbol, member.ReturnType?.Symbol.TypeKind });

        [Diagnostic(3, DiagnosticSeverity.Error,
            "Conflicting attributes",
            "{0} cannot be applied to the same target '{1}' as they conflict their intensions with each other."
        )]
        internal static Diagnostic ConflictingAttributes(MemberMetaInfo member, params AttributeData[] conflictingAttributes) {
            var conflicts = conflictingAttributes.ToList();
            if (conflicts.Count < 2)
                throw new ArgumentOutOfRangeException(nameof(conflictingAttributes));

            var primeLocation = member.GetLocation(conflicts[0]);
            var secondary = conflicts.Select(conflict => member.GetLocation(conflict));
            var conflictNames = string.Join(",", conflictingAttributes.Select(x => x.AttributeClass?.Name));

            return CreateDiagnostic(primeLocation, secondary, new object[] { conflictNames, member });
        }

        [Diagnostic(4, DiagnosticSeverity.Error,
            "Explicit sequential layout should not contain IndexAttribute",
            "'{0}' contains IndexAttribute which is not allowed with explicitly assigned sequential layout.")]
        internal static Diagnostic ExplicitSequentialShouldNotIndex(MemberMetaInfo member, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new object[] { member.Symbol.ToString() });

        [Diagnostic(5, DiagnosticSeverity.Error,
            "Sequential layout doesn't support multiple declaration",
            "Sequenctial layout ({0}) doesn't support multiple declartion (partial) type '{1}', because the order cannot be determianted.")]
        internal static Diagnostic SequentialCannotBePartial(NamedTypeMetaInfo typeInfo, bool isExplicit)
            => CreateDiagnostic(typeInfo.FirstLocation, typeInfo.Declarations.Select(x => x.Location), new object[] {
                isExplicit ? "explicitly defined" : "automatic - as no IndexAttribute is present",
                typeInfo.Symbol
            });

        [Diagnostic(6, DiagnosticSeverity.Warning,
            "IgnoreAttribute is meaningless with explicit layout",
            "Explicit layout doesn't require the usage of IgnoreAttribute on '{0}' as non IndexAttributes annotated member are ignored as default."
        )]
        internal static Diagnostic ExplicitDontNeedIgnore(MemberMetaInfo member)
            => CreateDiagnostic(member.Location, new object[] { member.Symbol });

        [Diagnostic(7, DiagnosticSeverity.Error,
            "Duplicate index is not allowed",
            "Duplicate index '{0}' is found at '{1}' and '{2}', which is not allowed."
        )]
        internal static Diagnostic DuplicateIndexs(int index, MemberMetaInfo firstMember, MemberMetaInfo secondMember)
            => CreateDiagnostic(secondMember.Location, new object[] { index, firstMember.Symbol, secondMember.Symbol });

    }
}
