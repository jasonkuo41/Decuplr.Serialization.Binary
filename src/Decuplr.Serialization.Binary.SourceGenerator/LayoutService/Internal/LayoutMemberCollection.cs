using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.AnalysisService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService.Internal {

    internal class LayoutMemberCollection : ILayoutMemberValidation {

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

            private readonly LayoutMemberCollection _parent;
            private readonly Type _attributeType;
            private Func<MemberMetaInfo, bool>? _invalidOn;

            public AttributeRules(LayoutMemberCollection parent, Type attribute) {
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

            private readonly LayoutMemberCollection _parent;
            private readonly Func<MemberMetaInfo, bool> _predicate;
            private Func<MemberMetaInfo, bool>? _invalidOn;

            public MemberRules(LayoutMemberCollection parent, Func<MemberMetaInfo, bool> predicate) {
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

        public void ValidateLayout(TypeMetaInfo full, IEnumerable<MemberMetaInfo> electedMembers, IDiagnosticReporter reporter) {
            var electedSet = new HashSet<MemberMetaInfo>(electedMembers);
            var unelectedMembers = full.Members.Where(x => !electedSet.Contains(x));

            foreach(var attributePredicate in _attributePredicates) {
                var (attribute, conditions) = (attributePredicate.Key, attributePredicate.Value);
                foreach(var condition in conditions) {
                    switch (condition) {
                        case IInvalidDiagnosticConditions invalidConditions: ReportCondition(invalidConditions); break;
                        case IConditionRules conditionRules: ValidateConditionRules(conditionRules);  break;
                        default: throw new ArgumentException($"Invalid Condition Type : {condition.GetType()}");
                    }
                }
                ReportUnselectMemberNotUsed(attribute);
            }

            foreach(var member in _memberPredicates) {
                reporter.ReportDiagnostic(member.Conditions.GetDiagnsotics(electedMembers.Where(member.Predicate)));
            }

            // local functions

            void ReportCondition(IInvalidDiagnosticConditions invalidConditions) 
                => reporter.ReportDiagnostic(invalidConditions.GetDiagnsotics(electedMembers));

            // generate warnings if unselect members contain attribute
            void ReportUnselectMemberNotUsed(Type attribute) 
                => reporter.ReportDiagnostic(unelectedMembers.Where(x => x.ContainsAttribute(attribute))
                                                             .Select(warningTarget => DiagnosticHelper.MeaninglessAttribute(attribute, warningTarget)));

            void ValidateConditionRules(IConditionRules conditionRules) {
                " This is much more complex i feel";
            }

        }

    }

}
