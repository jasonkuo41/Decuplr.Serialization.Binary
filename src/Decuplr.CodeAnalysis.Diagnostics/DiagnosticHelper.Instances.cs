using System;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.Serialization.Annotations;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics {

    public partial class DiagnosticHelper {

        [Diagnostic(0, DiagnosticSeverity.Warning,
            "No suitable member is found for serialization",
            "Type '{0}' will not be a resolved for serialization, because it doesn't contain any serializable member."
        )]
        internal static Diagnostic NoMember(NamedTypeMetaInfo typeInfo) => CreateDiagnostic(typeInfo.FirstLocation, new object[] { typeInfo.Symbol });

        [Diagnostic(1, DiagnosticSeverity.Warning,
            "Applied attribute is meaningless on target member",
            "'{0}' will not apply any effects on '{1}' since it will be never serialized. You should remove the attribute."
        )]
        internal static Diagnostic MeaninglessAttribute(Type attribute, MemberMetaInfo warningTarget)
            => CreateDiagnostic(
                warningTarget.GetLocation(attribute) ?? throw new ArgumentException("attribute cannot be located"),
                new object[] { attribute.Name, warningTarget.Symbol.Name }
            );

        [Diagnostic(2, DiagnosticSeverity.Error,
            "Compared value is not a valid type",
            "'{0}' is not a valid type for operand '{1}'{2}."
        )]
        internal static Diagnostic CompareValueInvalid(Condition condition, Location attributeLocation, string? additionExplanation = null)
            => CreateDiagnostic(attributeLocation, new object?[] { condition.ComparedValue, condition.Operator, additionExplanation });

        [Diagnostic(3, DiagnosticSeverity.Error,
            "Unable to locate the member for comparsion",
            "Unable to locate member '{0}' for comparsion. The source of the compared target must locate within the same type of '{1}'."
        )]
        internal static Diagnostic CompareSourceNotFound(string memberName, NamedTypeMetaInfo sourceType, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new object[] { memberName, sourceType.Symbol });

        [Diagnostic(4, DiagnosticSeverity.Error,
            "Invalid operator for comparsion",
            "Operand '{0}' is not valid operator for comparsion"
        )]
        internal static Diagnostic InvalidOperator(Operator operand, Location attributeLocation) => CreateDiagnostic(attributeLocation, new object[] { operand });

        [Diagnostic(5, DiagnosticSeverity.Error,
            "Target member has invalid return type for comparsion",
            "Target member '{0}' contains a invalid return type '{1}' which is not allowed."
        )]
        internal static Diagnostic CompareSourceReturnInvalidType(MemberMetaInfo targetMember, string typeName, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new Location[] { targetMember.Location }, new object[] { targetMember.Symbol, typeName });

        [Diagnostic(6, DiagnosticSeverity.Error,
            "Target member's return type doesn't implement IComparable for comparation",
            "Target member '{0}' has a return type '{1}' that doesn't implement IComparable which is required for 'Operator.{2}'."
        )]
        internal static Diagnostic ReturnTypeNotComparable(MemberMetaInfo targetMember, Operator @operator, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new Location[] { targetMember.Location }, new object?[] { targetMember.Symbol, targetMember.ReturnType, @operator });

        [Diagnostic(7, DiagnosticSeverity.Error,
            "Target member's return type doesn't implement an acceptable IComparable<T>",
            "Target member '{0}' has a return type '{1}' that doesn't implement a IComparable<T> where T is '{2}' or is built-in numeric / char convertible from '{2}'."
        )]
        internal static Diagnostic ReturnTypeInvalidComparable(MemberMetaInfo targetMember, Type type, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new Location[] { targetMember.Location }, new object?[] { targetMember.Symbol, targetMember.ReturnType, type.Name });

        [Diagnostic(8, DiagnosticSeverity.Error,
            "Target member is not a valid kind for comparison",
            "Target member '{0}' is a '{1}' which is not a valid kind for comparsion."
        )]
        internal static Diagnostic CompareSourceInvalidKind(MemberMetaInfo targetMember, Location attributeLocation)
            => CreateDiagnostic(attributeLocation, new Location[] { targetMember.Location }, new object[] { targetMember.Symbol, targetMember.Symbol.Kind });
    }
}
