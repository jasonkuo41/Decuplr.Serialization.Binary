using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal class IndexOrderSelector : IOrderSelector {

        private readonly static Dictionary<LayoutOrder, IndexOrderSelector> Selectors = new Dictionary<LayoutOrder, IndexOrderSelector> {
            [LayoutOrder.Auto] = new IndexOrderSelector(LayoutOrder.Auto),
            [LayoutOrder.Sequential] = new IndexOrderSelector(LayoutOrder.Sequential),
            [LayoutOrder.Explicit] = new IndexOrderSelector(LayoutOrder.Explicit),
        };

        private readonly LayoutOrder _declaredOrder;

        private IndexOrderSelector(LayoutOrder order) {
            _declaredOrder = order;
        }

        public static IOrderSelector GetSelector(LayoutOrder order) {
            if (Selectors.TryGetValue(order, out var selector))
                return selector;
            throw new ArgumentException("Invalid Order Type");
        }

        private bool HasUnsupportedReturnType(ReturnTypeMetaInfo? returnType) {
            switch (returnType?.Symbol.TypeKind ?? TypeKind.Error) {
                case TypeKind.Class:
                case TypeKind.Struct:
                case TypeKind.Array:
                case TypeKind.Enum:
                case TypeKind.Interface:
                    return false;
                default:
                    return true;
            }
        }

        public void ValidateMembers(ILayoutMemberValidation filter) {
            filter.WhereAttribute<IndexAttribute>()
                  .InvalidOn(member => member.ContainsAttribute<IgnoreAttribute>())
                  .ReportDiagnostic(member => OrderDiagnostic.ConflictingAttributes(member, member.GetAttribute<IgnoreAttribute>()!, member.GetAttribute<IndexAttribute>()!))

                  .InvalidOn(member => member.IsStatic)
                  .ReportDiagnostic(member => OrderDiagnostic.InvalidKeyword("static", member))

                  .InvalidOn(member => member.IsConst)
                  .ReportDiagnostic(member => OrderDiagnostic.InvalidKeyword("const", member))

                  .InvalidOn(member => HasUnsupportedReturnType(member.ReturnType))
                  .ReportDiagnostic(member => OrderDiagnostic.UnsupportedType(member));
        }

        private IEnumerable<MemberMetaInfo> GetSequentialOrder(IEnumerable<MemberMetaInfo> memberInfo, IDiagnosticReporter diagnostic, bool isExplicit) {
            var indexes = memberInfo.Where(x => x.ContainsAttribute<IndexAttribute>());
            if (indexes.Any()) {
                Debug.Assert(isExplicit);
                foreach (var indexmember in indexes)
                    diagnostic.ReportDiagnostic(OrderDiagnostic.ExplicitSequentialShouldNotIndex(indexmember, indexmember.GetLocation<IndexAttribute>()!));
                return Enumerable.Empty<MemberMetaInfo>();
            }

            var partials = memberInfo.Where(x => x.ContainingFullType.IsPartial);
            if (partials.Any()) {
                var type = partials.First().ContainingFullType;
                diagnostic.ReportDiagnostic(OrderDiagnostic.SequentialCannotBePartial(type, isExplicit));
                return Enumerable.Empty<MemberMetaInfo>();
            }

            // Get all the possible members that will be serialized
            // We will try to filter out all static and const members first
            var electedMember = memberInfo.Where(x => !x.ContainsAttribute<IgnoreAttribute>())
                                          .Where(x => !x.IsConst)
                                          .Where(x => !x.IsStatic)
                                          .Where(x => x.ReturnType != null);

            foreach (var member in electedMember.Where(x => HasUnsupportedReturnType(x.ReturnType!)))
                diagnostic.ReportDiagnostic(OrderDiagnostic.UnsupportedTypeHint(member));

            return electedMember;
        }

        private IEnumerable<MemberMetaInfo> GetExplicitOrder(IEnumerable<MemberMetaInfo> memberInfo, IDiagnosticReporter diagnostic) {

            foreach (var ignoredMember in memberInfo.Where(x => x.ContainsAttribute<IgnoreAttribute>()))
                diagnostic.ReportDiagnostic(OrderDiagnostic.ExplicitDontNeedIgnore(ignoredMember));

            var indexLookup = new Dictionary<int, MemberMetaInfo>();
            foreach(var member in memberInfo) {
                if (!member.ContainsAttribute<IndexAttribute>())
                    continue;
                var index = (int)member.GetAttribute<IndexAttribute>()!.ConstructorArguments[0].Value!;
                // Check if we have duplicate index
                if (indexLookup.TryGetValue(index, out var duplicateMember)) {
                    diagnostic.ReportDiagnostic(OrderDiagnostic.DuplicateIndexs(index, duplicateMember, member));
                    continue;
                }
                indexLookup.Add(index, member);
            }
            return indexLookup.OrderBy(x => x.Key).Select(x => x.Value);
        }

        public IEnumerable<MemberMetaInfo> GetOrder(IEnumerable<MemberMetaInfo> memberInfo, IDiagnosticReporter diagnostic) {
            return GetImplicitLayout(out var isExplicit) switch
            {
                LayoutOrder.Explicit => GetExplicitOrder(memberInfo, diagnostic),
                LayoutOrder.Sequential => GetSequentialOrder(memberInfo, diagnostic, isExplicit),
                _ => AssertInvalid(),
            };

            LayoutOrder GetImplicitLayout(out bool isExplicit) {
                isExplicit = _declaredOrder != LayoutOrder.Auto;
                if (isExplicit)
                    return _declaredOrder;
                return memberInfo.Any(x => x.ContainsAttribute<IndexAttribute>()) ? LayoutOrder.Explicit : LayoutOrder.Sequential;
            }

            IEnumerable<MemberMetaInfo> AssertInvalid() {
                Debug.Fail($"Invalid Layout state");
                throw new ArgumentException("Invalid Binary Layout");
            }
        }
    }
}
