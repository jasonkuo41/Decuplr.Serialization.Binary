using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.Serialization;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics.Internal {
    internal class ConditionAnalyzer : IConditionAnalyzer {

        private readonly Dictionary<NamedTypeMetaInfo, Dictionary<string, MemberMetaInfo>> _typeNameCache = new Dictionary<NamedTypeMetaInfo, Dictionary<string, MemberMetaInfo>>();

        private IReadOnlyDictionary<string, MemberMetaInfo> GetMemberNameDictionary(NamedTypeMetaInfo metaInfo) {
            if (_typeNameCache.TryGetValue(metaInfo, out var value))
                return value;
            _typeNameCache.Add(metaInfo, metaInfo.Members.ToDictionary(x => x.Name));
            return value;
        }

        private static void EvaluateCondition(MemberMetaInfo targetMember, Location attributeLocation, ConditionExpression conditions, IDiagnosticReporter reporter) {
            switch (conditions.Condition) {
                // Since all objects contain Equals(object), this is the least we call back into
                // TODO : Note we might still want to warn user about that a returning type doesn't contain a correct signature
                // For example : If we try to evaluate the member equals to true,
                // even if the member returns a type that is obvious that wouldn't evaluate with true, then it would still evaluate, but just to false
                case Condition.Equal:
                case Condition.NotEqual:
                    break;

                // In these cases we need to check if either the return type implements IComparable or IComparable<T>
                // We might also want to warn about boxing issue if the given compared type is a valuetype and returning type only has IComparable though
                case Condition.GreaterThan:
                case Condition.GreaterThanOrEqual:
                case Condition.LessThan:
                case Condition.LessThanOrEqual: {
                    // Valid compare types
                    // Value Types : bool, char, byte, sbyte, ushort, short, int, uint, ulong, long, float, double 
                    // Ref Types : Types (but why), string (but why)

                    // We currently consider 'null' to be not comparable, so there's this
                    if (conditions.ComparedValue is null) {
                        reporter.ReportDiagnostic(DiagnosticHelper.CompareValueInvalid(conditions, attributeLocation));
                        break;
                    }

                    // Valid implicit conversion for IComparable is matched with C# specification :
                    // https://docs.microsoft.com/dotnet/csharp/language-reference/builtin-types/numeric-conversions
                    //
                    // [Possible Improvement]
                    // TODO : We also don't support even if IComparation<T>'s T is implicit convertible from the compared type
                    // We only support when it's primitive is implicitly convertible (supported by C# specification) to another primitive type
                    // So types that only return IComparable<long> would work well with int numbers
                    //
                    // This can be done using a compilation extensions provided out of the box: 
                    // https://docs.microsoft.com/dotnet/api/microsoft.codeanalysis.csharp.csharpextensions.classifyconversion

                    var returnType = targetMember.ReturnType!;
                    var comparedType = conditions.ComparedValue.GetType();

                    if (!returnType.Implements(typeof(IComparable)) || !returnType.Implements(typeof(IComparable<>))) {
                        reporter.ReportDiagnostic(DiagnosticHelper.ReturnTypeNotComparable(targetMember, conditions.Condition, attributeLocation));
                        break;
                    }
                    if (!returnType.Implements(typeof(IComparable<>).MakeGenericType(comparedType)) ||
                        !returnType.GetInterfaces(typeof(IComparable<>)).Any(x => x.IsImplicitConvertibleFrom(comparedType))) {
                        reporter.ReportDiagnostic(DiagnosticHelper.ReturnTypeInvalidComparable(targetMember, comparedType, attributeLocation));
                        break;
                    }
                    break;
                }

                // Since all objects can be evaluated with is operator
                case Condition.IsTypeOf:
                case Condition.IsNotTypeOf: {
                    if (!(conditions.ComparedValue is null) && conditions.ComparedValue.GetType() != typeof(Type))
                        reporter.ReportDiagnostic(DiagnosticHelper.CompareValueInvalid(conditions, attributeLocation, ", it should be only a kind of Type and none other"));
                    break;
                }

                default:
                    reporter.ReportDiagnostic(DiagnosticHelper.InvalidCondition(conditions.Condition, attributeLocation));
                    break;
            }
        }

        private string GetConditionEvalString(string targetName, Condition condition, object comparedValue) => condition switch
        {
            Condition.Equal => $"({targetName}.Equals({comparedValue}))",
            Condition.NotEqual => $"(!{targetName}.Equals({comparedValue}))",
            Condition.GreaterThan => $"({targetName}.CompareTo({comparedValue}) > 0)",
            Condition.GreaterThanOrEqual => $"({targetName}.CompareTo({comparedValue}) >= 0)",
            Condition.LessThan => $"({targetName}.CompareTo({comparedValue}) < 0)",
            Condition.LessThanOrEqual => $"({targetName}.CompareTo({comparedValue}) <= 0)",
            Condition.IsTypeOf => $"({targetName} is {comparedValue})",
            Condition.IsNotTypeOf => $"(!({targetName} is {comparedValue}))",
            _ => throw new ArgumentException($"Invalid Condition : {condition}")
        };


        public void ConditionDiagnostic(MemberMetaInfo annotatedMember, Location attributeLocation, ConditionExpression conditions, IDiagnosticReporter reporter) {
            var full = annotatedMember.ContainingFullType;
            var members = GetMemberNameDictionary(full);

            // If condition target source cannot be found, we halt
            // Currently we only support member only evaluation
            if (!members.TryGetValue(conditions.SourceName, out var targetMember)) {
                reporter.ReportDiagnostic(DiagnosticHelper.CompareSourceNotFound(conditions.SourceName, full, attributeLocation));
                return;
            }
            // If condition target is not a valid target for returning types
            if (targetMember.ReturnType is null) {
                reporter.ReportDiagnostic(DiagnosticHelper.CompareSourceInvalidKind(targetMember, attributeLocation));
                return;
            }
            // Or if the target returns void which is not comparable
            if (targetMember.ReturnType.IsVoid) {
                reporter.ReportDiagnostic(DiagnosticHelper.CompareSourceReturnInvalidType(targetMember, "void", attributeLocation));
                return;
            }
            EvaluateCondition(targetMember, attributeLocation, conditions, reporter);
        }

        public string GetEvalString(string typeArgumentName, NamedTypeMetaInfo type, ConditionExpression condition) {
            // Since we only support member only evaulation right now
            var member = GetMemberNameDictionary(type);
            if (!member.TryGetValue(condition.SourceName, out var targetMember))
                throw new ArgumentException("Condition target source cannot be found");
            return GetConditionEvalString(GetTargetName(targetMember), condition.Condition, condition.ComparedValue);

            string GetTargetName(MemberMetaInfo targetMember) {
                if (targetMember.IsStatic)
                    return $"{type.Symbol}.{targetMember.Name}";
                return $"{typeArgumentName}.{targetMember.Name} {condition}";
            }
        }

    }

}
