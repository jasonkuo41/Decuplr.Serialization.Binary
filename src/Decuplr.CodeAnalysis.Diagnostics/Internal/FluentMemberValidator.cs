using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.Serialization.Annotations;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics.Internal {

    internal class FluentMemberValidator : IFluentMemberValidator {

        private class ConditionProvider : IConditionProvider {

            private readonly Func<AttributeData, Condition> _attribute;
            public ConditionProvider(Func<AttributeData, Condition> attribute) => _attribute = attribute;
            public Condition ProvideCondition(AttributeData data) => _attribute.Invoke(data);
        }

        private class InvalidDiagnosticCondtions : IInvalidDiagnosticConditions {

            private readonly Func<MemberMetaInfo, bool>? _invalidConditions;
            private readonly Func<MemberMetaInfo, Diagnostic> _reportDiagnostics;

            public InvalidDiagnosticCondtions(Func<MemberMetaInfo, bool>? invalidConditions, Func<MemberMetaInfo, Diagnostic> diagnostics) {
                _invalidConditions = invalidConditions;
                _reportDiagnostics = diagnostics;
            }

            public IEnumerable<Diagnostic> GetDiagnsotics(IEnumerable<MemberMetaInfo> member) {
                var invalidMembers = Enumerable.Empty<MemberMetaInfo>();
                foreach (var condition in _invalidConditions?.GetInvocationList() as Func<MemberMetaInfo, bool>[] ?? Array.Empty<Func<MemberMetaInfo, bool>>()) {
                    invalidMembers.Concat(member.Where(condition));
                }
                return invalidMembers.Select(x => _reportDiagnostics(x));
            }

        }

        private class AttributeRules : IAttributeRule<MemberMetaInfo> {

            private readonly FluentMemberValidator _parent;
            private readonly Type _attributeType;
            private Func<MemberMetaInfo, bool>? _invalidOn;

            public AttributeRules(FluentMemberValidator parent, Type attribute) {
                _parent = parent;
                _attributeType = attribute;
            }

            public ISymbolCondition<MemberMetaInfo, IAttributeRule<MemberMetaInfo>> InvalidOn(Func<MemberMetaInfo, bool> predicate) {
                _invalidOn += predicate;
                return this;
            }

            public IAttributeRule<MemberMetaInfo> VerifyCondition(Func<AttributeData, Condition> conditionProvider) {
                _parent.Add(_attributeType, new ConditionProvider(conditionProvider));
                return new AttributeRules(_parent, _attributeType);
            }

            public IAttributeRule<MemberMetaInfo> ReportDiagnostic(Func<MemberMetaInfo, Diagnostic> diagnostic) {
                _parent.Add(_attributeType, new InvalidDiagnosticCondtions(_invalidOn, diagnostic));
                return new AttributeRules(_parent, _attributeType);
            }

        }

        private class MemberRules : ISymbolRule<MemberMetaInfo> {

            private readonly FluentMemberValidator _parent;
            private readonly Func<MemberMetaInfo, bool> _predicate;
            private Func<MemberMetaInfo, bool>? _invalidOn;

            public MemberRules(FluentMemberValidator parent, Func<MemberMetaInfo, bool> predicate) {
                _parent = parent;
                _predicate = predicate;
            }

            public ISymbolCondition<MemberMetaInfo, ISymbolRule<MemberMetaInfo>> InvalidOn(Func<MemberMetaInfo, bool> predicate) {
                _invalidOn += predicate;
                return this;
            }

            public ISymbolRule<MemberMetaInfo> ReportDiagnostic(Func<MemberMetaInfo, Diagnostic> diagnostic) {
                _parent.Add(_predicate, new InvalidDiagnosticCondtions(_invalidOn, diagnostic));
                return new MemberRules(_parent, _predicate);
            }
        }

        private struct PredicateConditionPair {
            public Func<MemberMetaInfo, bool> Predicate { get; set; }
            public IInvalidDiagnosticConditions Conditions { get; set; }
        }

        private readonly Dictionary<Type, List<IConditionRules>> _attributePredicates = new Dictionary<Type, List<IConditionRules>>();
        private readonly List<PredicateConditionPair> _memberPredicates = new List<PredicateConditionPair>();
        private readonly NamedTypeMetaInfo _type;
        private readonly IReadOnlyList<MemberMetaInfo> _selectedMembers;
        private readonly IReadOnlyList<MemberMetaInfo> _excludedMembers;

        public FluentMemberValidator(TypeMetaSelection selection) {
            _type = selection.Type;
            _selectedMembers = selection.SelectedMembers;
            _excludedMembers = selection.UnselectedMembers;
        }

        private void Add(Type type, IConditionRules rules) {
            if (_attributePredicates.TryGetValue(type, out var list))
                list.Add(rules);
            else
                _attributePredicates[type] = new List<IConditionRules> { rules };
        }

        private void Add(Func<MemberMetaInfo, bool> predicate, IInvalidDiagnosticConditions conditions)
            => _memberPredicates.Add(new PredicateConditionPair { Predicate = predicate, Conditions = conditions });

        public IAttributeRule<MemberMetaInfo> WhereAttribute<TAttribute>() where TAttribute : Attribute => new AttributeRules(this, typeof(TAttribute));

        public ISymbolRule<MemberMetaInfo> WhereMember(Func<MemberMetaInfo, bool> predicate) => new MemberRules(this, predicate);

        public void Validate(IDiagnosticReporter reporter) {

            foreach (var attributePredicate in _attributePredicates) {
                var (attribute, conditions) = (attributePredicate.Key, attributePredicate.Value);
                foreach (var condition in conditions) {
                    switch (condition) {
                        case IInvalidDiagnosticConditions invalidConditions:
                            ReportCondition(invalidConditions);
                            break;
                        case IConditionProvider conditionProvider:
                            ValidateConditionRules(attribute, conditionProvider, _type, _selectedMembers, reporter);
                            break;
                        default: throw new ArgumentException($"Invalid Condition Type : {condition.GetType()}");
                    }
                }
                ReportUnselectMemberNotUsed(attribute);
            }

            foreach (var member in _memberPredicates) {
                reporter.ReportDiagnostic(member.Conditions.GetDiagnsotics(_selectedMembers.Where(member.Predicate)));
            }

            // local functions

            void ReportCondition(IInvalidDiagnosticConditions invalidConditions)
                => reporter.ReportDiagnostic(invalidConditions.GetDiagnsotics(_selectedMembers));

            // generate warnings if unselect members contain attribute
            void ReportUnselectMemberNotUsed(Type attribute)
                => reporter.ReportDiagnostic(_excludedMembers.Where(x => x.ContainsAttribute(attribute))
                                                             .Select(warningTarget => DiagnosticHelper.MeaninglessAttribute(attribute, warningTarget)));

        }

        // TODO : maybe segregate this into a different method
        private void ValidateConditionRules(Type attributeType, IConditionProvider conditionRules, NamedTypeMetaInfo full, IEnumerable<MemberMetaInfo> electedMembers, IDiagnosticReporter reporter) {
            var members = full.Members.ToDictionary(member => member.Symbol.Name);
            foreach (var (member, attributes) in electedMembers.Select(member => (member, member.GetAttributes(attributeType)))) {
                foreach (var attribute in attributes) {
                    var conditions = conditionRules.ProvideCondition(attribute);

                    // If condition target source cannot be found, we halt
                    if (!members.TryGetValue(conditions.SourceName, out var targetMember)) {
                        reporter.ReportDiagnostic(DiagnosticHelper.CompareSourceNotFound(conditions.SourceName, full, member.GetLocation(attribute)));
                        continue;
                    }
                    // If condition target is not a valid target for returning types
                    if (targetMember.ReturnType is null) {
                        reporter.ReportDiagnostic(DiagnosticHelper.CompareSourceInvalidKind(targetMember, member.GetLocation(attribute)));
                        break;
                    }
                    // Or if the target returns void which is not comparable
                    if (targetMember.ReturnType.IsVoid) {
                        reporter.ReportDiagnostic(DiagnosticHelper.CompareSourceReturnInvalidType(targetMember, "void", member.GetLocation(attribute)));
                        continue;
                    }

                    switch (conditions.Operator) {
                        // Since all objects contain Equals(object), this is the least we call back into
                        // TODO : Note we might still want to warn user about that a returning type doesn't contain a correct signature
                        case Operator.Equal:
                        case Operator.NotEqual:
                            break;

                        // In these cases we need to check if either the return type implements IComparable or IComparable<T>
                        // We might also want to warn about boxing issue if the given compared type is a valuetype and returning type only has IComparable though
                        case Operator.GreaterThan:
                        case Operator.GreaterThanOrEqual:
                        case Operator.LessThan:
                        case Operator.LessThanOrEqual: {
                            // Valid compare types
                            // Value Types : bool, char, byte, sbyte, ushort, short, int, uint, ulong, long, float, double 
                            // Ref Types : Types (but why), string (but why)

                            // We currently consider 'null' to be not comparable, so there's this
                            if (conditions.ComparedValue is null) {
                                reporter.ReportDiagnostic(DiagnosticHelper.CompareValueInvalid(conditions, member.GetLocation(attribute)));
                                break;
                            }

                            // Valid implicit conversion for IComparable is matched with C# specification : https://docs.microsoft.com/dotnet/csharp/language-reference/builtin-types/numeric-conversions
                            //
                            // [Possible Improvement]
                            // We also don't support even if IComparation<T>'s T is implicit convertible from the compared type
                            // We only support when it's primitive is implicitly convertible (supported by C# specification) to another primitive type
                            // So types that only return IComparable<long> would work well with int numbers
                            
                            var returnType = targetMember.ReturnType;
                            var comparedType = conditions.ComparedValue.GetType();

                            if (!returnType.Implements(typeof(IComparable)) || !returnType.Implements(typeof(IComparable<>))) {
                                reporter.ReportDiagnostic(DiagnosticHelper.ReturnTypeNotComparable(targetMember, conditions.Operator, member.GetLocation(attribute)));
                                break;
                            }
                            if (!returnType.Implements(typeof(IComparable<>).MakeGenericType(comparedType)) ||
                                !returnType.GetInterfaces(typeof(IComparable<>)).Any(x => x.IsImplicitConvertibleFrom(comparedType))) {
                                reporter.ReportDiagnostic(DiagnosticHelper.ReturnTypeInvalidComparable(targetMember, comparedType, member.GetLocation(attribute)));
                                break;
                            }
                            break;
                        }

                        // Since all objects can be evaluated with is operator
                        case Operator.IsTypeOf:
                        case Operator.IsNotTypeOf: {
                            if (!(conditions.ComparedValue is null) && conditions.ComparedValue.GetType() != typeof(Type))
                                reporter.ReportDiagnostic(DiagnosticHelper.CompareValueInvalid(conditions, member.GetLocation(attribute), ", it should be only a kind of Type and none other"));
                            break;
                        }

                        default:
                            reporter.ReportDiagnostic(DiagnosticHelper.InvalidOperator(conditions.Operator, member.GetLocation(attribute)));
                            break;
                    }
                }
            }
        }

    }

}
