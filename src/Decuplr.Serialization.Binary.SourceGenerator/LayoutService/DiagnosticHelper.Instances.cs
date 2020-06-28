using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Serialization.Binary.AnalysisService;
using Decuplr.Serialization.Binary.LayoutService.Internal;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {

    internal partial class DiagnosticHelper {

        [Diagnostic(0, DiagnosticSeverity.Error, 
            "Invalid serialization target", 
            "Keyword {0} of '{1}' is not a valid target for serialization."
        )]
        public static Diagnostic InvalidKeyword(string invalidKeyword, MemberMetaInfo member) => CreateDiagnostic(member.Location, new object?[] { invalidKeyword, member.Symbol });


        [Diagnostic(1, DiagnosticSeverity.Error, 
            "Invalid serialization type", 
            "Return type '{0}' of '{1}' is a {2} type, thus is not a valid target for serialization."
        )]
        public static Diagnostic UnsupportedType(MemberMetaInfo member) => CreateDiagnostic(member.Location, new object?[] { member.ReturnType?.Name, member.Symbol, member.ReturnType?.TypeKind });

        [Diagnostic(2, DiagnosticSeverity.Warning,
            "Invalid serialization type should be marked as ignored",
            "Return type '{0}' of '{1}' is a {2} type, thus will not be serialized, IgnoreAttribute should be applied to explicitly."
        )]
        public static Diagnostic UnsupportedTypeHint(MemberMetaInfo member) => CreateDiagnostic(member.Location, new object?[] { member.ReturnType?.Name, member.Symbol, member.ReturnType?.TypeKind });

        [Diagnostic(3, DiagnosticSeverity.Error, 
            "Conflicting attributes", 
            "{0} cannot be applied to the same target '{1}' as they conflict their intensions with each other."
        )]
        public static Diagnostic ConflictingAttributes(MemberMetaInfo member, params AttributeData[] conflictingAttributes) {
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
        public static Diagnostic ExplicitSequentialShouldNotIndex(MemberMetaInfo member, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new object[] { member.Symbol.ToString() });


        [Diagnostic(5, DiagnosticSeverity.Error,
            "Sequential layout doesn't support multiple declaration", 
            "Sequenctial layout ({0}) doesn't support multiple declartion (partial) type '{1}', because the order cannot be determianted.")]
        public static Diagnostic SequentialCannotBePartial(TypeMetaInfo typeInfo, bool isExplicit)
            => CreateDiagnostic(typeInfo.FirstLocation, typeInfo.Declarations.Select(x => x.Location), new object[] { 
                isExplicit ? "explicitly defined" : "automatic - as no IndexAttribute is present", 
                typeInfo.Symbol 
            });

        [Diagnostic(6, DiagnosticSeverity.Warning,
            "IgnoreAttribute is meaningless with explicit layout",
            "Explicit layout doesn't require the usage of IgnoreAttribute on '{0}' as non IndexAttributes annotated member are ignored as default."
        )]
        public static Diagnostic ExplicitDontNeedIgnore(MemberMetaInfo member)
            => CreateDiagnostic(member.Location, new object[] { member.Symbol });

        [Diagnostic(7, DiagnosticSeverity.Error,
            "Duplicate index is not allowed",
            "Duplicate index '{0}' is found at '{1}' and '{2}', which is not allowed."
        )]
        public static Diagnostic DuplicateIndexs(int index, MemberMetaInfo firstMember, MemberMetaInfo secondMember)
            => CreateDiagnostic(secondMember.Location, new object[] { index, firstMember.Symbol, secondMember.Symbol });

        [Diagnostic(8, DiagnosticSeverity.Warning,
            "No suitable member is found for serialization",
            "Type '{0}' will not be a resolved for serialization, because it doesn't contain any serializable member."
        )]
        public static Diagnostic NoMember(TypeMetaInfo typeInfo) => CreateDiagnostic(typeInfo.FirstLocation, new object[] { typeInfo.Symbol });

        [Diagnostic(9, DiagnosticSeverity.Warning,
            "Applied attribute is meaningless on target member",
            "'{0}' will not apply any effects on '{1}' since it will be never serialized. You should remove the attribute."
        )]
        public static Diagnostic MeaninglessAttribute(Type attribute, MemberMetaInfo warningTarget)
            => CreateDiagnostic(
                warningTarget.GetLocation(attribute) ?? throw new ArgumentException("attribute cannot be located"),
                new object[] { attribute.Name, warningTarget.Symbol.Name }
            );
    }
}
