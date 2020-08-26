using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal class IndexOrderSelector : IOrderSelector {

        private readonly IDiagnosticReporter _diagnostic;
        private readonly LayoutOrder _declaredOrder;

        public bool ContinueDiagnosticAfterError => false;

        public IndexOrderSelector(IDiagnosticReporter diagnostic, LayoutOrder order) {
            _declaredOrder = order;
            _diagnostic = diagnostic;
        }

        private bool HasUnsupportedReturnType(ReturnTypeMetaInfo? returnType)
            => (returnType?.Symbol.TypeKind ?? TypeKind.Error) switch
            {
                TypeKind.Class or TypeKind.Struct or TypeKind.Array or TypeKind.Enum or TypeKind.Interface => false,
                _ => true,
            };

        public void ConfigureValidation(IFluentMemberValidator filter) {
            filter.WhereAttribute<IndexAttribute>()
                  .When(member => member.ContainsAttribute<IgnoreAttribute>())
                  .ReportDiagnostic(member => OrderDiagnostic.ConflictingAttributes(member, member.GetAttribute<IgnoreAttribute>()!, member.GetAttribute<IndexAttribute>()!))

                  .When(member => member.IsStatic)
                  .ReportDiagnostic(member => OrderDiagnostic.InvalidKeyword("static", member))

                  .When(member => member.IsConst)
                  .ReportDiagnostic(member => OrderDiagnostic.InvalidKeyword("const", member))

                  .When(member => HasUnsupportedReturnType(member.ReturnType))
                  .ReportDiagnostic(member => OrderDiagnostic.UnsupportedType(member));
        }

        private IReadOnlyList<MemberMetaInfo> GetSequentialOrder(IEnumerable<MemberMetaInfo> memberInfo, bool isExplicit) {
            var indexes = memberInfo.Where(x => x.ContainsAttribute<IndexAttribute>());
            if (indexes.Any()) {
                Debug.Assert(isExplicit);
                foreach (var indexmember in indexes)
                    _diagnostic.ReportDiagnostic(OrderDiagnostic.ExplicitSequentialShouldNotIndex(indexmember, indexmember.GetLocation<IndexAttribute>()!));
                return Array.Empty<MemberMetaInfo>();
            }

            var partials = memberInfo.Where(x => x.ContainingFullType.IsPartial);
            if (partials.Any()) {
                var type = partials.First().ContainingFullType;
                _diagnostic.ReportDiagnostic(OrderDiagnostic.SequentialCannotBePartial(type, isExplicit));
                return Array.Empty<MemberMetaInfo>();
            }

            // Get all the possible members that will be serialized
            // We will try to filter out all static and const members first
            var electedMember = memberInfo.Where(x => !x.ContainsAttribute<IgnoreAttribute>())
                                          .Where(x => !x.IsConst)
                                          .Where(x => !x.IsStatic)
                                          .Where(x => x.ReturnType != null);

            foreach (var member in electedMember.Where(x => HasUnsupportedReturnType(x.ReturnType!)))
                _diagnostic.ReportDiagnostic(OrderDiagnostic.UnsupportedTypeHint(member));

            return electedMember.ToList();
        }

        private IReadOnlyList<MemberMetaInfo> GetExplicitOrder(IEnumerable<MemberMetaInfo> memberInfo) {

            foreach (var ignoredMember in memberInfo.Where(x => x.ContainsAttribute<IgnoreAttribute>()))
                _diagnostic.ReportDiagnostic(OrderDiagnostic.ExplicitDontNeedIgnore(ignoredMember));

            var indexLookup = new Dictionary<int, MemberMetaInfo>();
            foreach (var member in memberInfo) {
                if (!member.ContainsAttribute<IndexAttribute>())
                    continue;
                var index = (int)member.GetAttribute<IndexAttribute>()!.ConstructorArguments[0].Value!;
                // Check if we have duplicate index
                if (indexLookup.TryGetValue(index, out var duplicateMember)) {
                    _diagnostic.ReportDiagnostic(OrderDiagnostic.DuplicateIndexs(index, duplicateMember, member));
                    continue;
                }
                indexLookup.Add(index, member);
            }
            return indexLookup.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }

        public IReadOnlyList<MemberMetaInfo> GetOrder(NamedTypeMetaInfo typeInfo) {
            var memberInfo = typeInfo.Members;
            return GetImplicitLayout(out var isExplicit) switch
            {
                LayoutOrder.Explicit => GetExplicitOrder(memberInfo),
                LayoutOrder.Sequential => GetSequentialOrder(memberInfo, isExplicit),
                _ => AssertInvalid(),
            };

            LayoutOrder GetImplicitLayout(out bool isExplicit) {
                isExplicit = _declaredOrder != LayoutOrder.Auto;
                if (isExplicit)
                    return _declaredOrder;
                return memberInfo.Any(x => x.ContainsAttribute<IndexAttribute>()) ? LayoutOrder.Explicit : LayoutOrder.Sequential;
            }

            static IReadOnlyList<MemberMetaInfo> AssertInvalid() {
                Debug.Fail($"Invalid Layout state");
                throw new ArgumentException("Invalid Binary Layout");
            }
        }

        public bool IsCandidateMember(MemberMetaInfo member) {
            throw new NotImplementedException();
        }
    }
}
