using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Serialization.Binary.AnalysisService;
using Decuplr.Serialization.Binary.Annotations;
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
        public static Diagnostic SequentialCannotBePartial(NamedTypeMetaInfo typeInfo, bool isExplicit)
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
        public static Diagnostic NoMember(NamedTypeMetaInfo typeInfo) => CreateDiagnostic(typeInfo.FirstLocation, new object[] { typeInfo.Symbol });

        [Diagnostic(9, DiagnosticSeverity.Warning,
            "Applied attribute is meaningless on target member",
            "'{0}' will not apply any effects on '{1}' since it will be never serialized. You should remove the attribute."
        )]
        public static Diagnostic MeaninglessAttribute(Type attribute, MemberMetaInfo warningTarget)
            => CreateDiagnostic(
                warningTarget.GetLocation(attribute) ?? throw new ArgumentException("attribute cannot be located"),
                new object[] { attribute.Name, warningTarget.Symbol.Name }
            );

        [Diagnostic(10, DiagnosticSeverity.Error,
            "Compared value is not a valid type",
            "'{0}' is not a valid type for operand '{1}'{2}."
        )]
        public static Diagnostic CompareValueInvalid(Condition condition, Location attributeLocation, string? additionExplanation = null) 
            => CreateDiagnostic(attributeLocation, new object?[] { condition.ComparedValue, condition.Operator, additionExplanation });

        [Diagnostic(11, DiagnosticSeverity.Error,
            "Unable to locate the member for comparsion",
            "Unable to locate member '{0}' for comparsion. The source of the compared target must locate within the same type of '{1}'."
        )]
        public static Diagnostic CompareSourceNotFound(string memberName, NamedTypeMetaInfo sourceType, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new object[] { memberName, sourceType.Symbol });

        [Diagnostic(12, DiagnosticSeverity.Error,
            "Invalid operator for comparsion",
            "Operand '{0}' is not valid operator for comparsion"
        )]
        public static Diagnostic InvalidOperator(Operator operand, Location attributeLocation) => CreateDiagnostic(attributeLocation, new object[] { operand });

        [Diagnostic(13, DiagnosticSeverity.Error,
            "Target member has invalid return type for comparsion",
            "Target member '{0}' contains a invalid return type '{1}' which is not allowed."
        )]
        public static Diagnostic CompareSourceReturnInvalidType(MemberMetaInfo targetMember, string typeName, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new Location[] { targetMember.Location }, new object[] { targetMember.Symbol, typeName });

        [Diagnostic(14, DiagnosticSeverity.Error,
            "Target member's return type doesn't implement IComparable for comparation",
            "Target member '{0}' has a return type '{1}' that doesn't implement IComparable which is required for 'Operator.{2}'."
        )]
        public static Diagnostic ReturnTypeNotComparable(MemberMetaInfo targetMember, Operator @operator, Location attributeLocation) 
            => CreateDiagnostic(attributeLocation, new Location[] { targetMember.Location }, new object?[] { targetMember.Symbol, targetMember.ReturnType, @operator });

        [Diagnostic(15, DiagnosticSeverity.Error,
            "Target member's return type doesn't implement an acceptable IComparable<T>",
            "Target member '{0}' has a return type '{1}' that doesn't implement a IComparable<T> where T is '{2}' or is built-in numeric / char convertible from '{2}'."
        )]
        public static Diagnostic ReturnTypeInvalidComparable(MemberMetaInfo targetMember, Type type, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new Location[] { targetMember.Location }, new object?[] { targetMember.Symbol, targetMember.ReturnType, type.Name });

        [Diagnostic(16, DiagnosticSeverity.Error,
            "Target member is not a valid kind for comparison",
            "Target member '{0}' is a '{1}' which is not a valid kind for comparsion."
        )]
        public static Diagnostic CompareSourceInvalidKind(MemberMetaInfo targetMember, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new Location[] { targetMember.Location }, new object[] { targetMember.Symbol, targetMember.Symbol.Kind });
    }
}
