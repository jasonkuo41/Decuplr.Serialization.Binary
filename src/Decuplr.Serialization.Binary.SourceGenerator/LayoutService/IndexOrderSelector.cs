using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.Serialization.Binary.AnalysisService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal class IndexOrderSelector : IOrderSelector {

        private bool HasUnsupportedReturnType(ITypeSymbol? symbol) {
            if (symbol is null)
                return true;
            switch (symbol.TypeKind) {
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
                  .ReportDiagnostic(member => DiagnosticHelper.ConflictingAttributes(member, member.GetAttribute<IgnoreAttribute>()!, member.GetAttribute<IndexAttribute>()!))

                  .InvalidOn(member => member.IsStatic)
                  .ReportDiagnostic(member => DiagnosticHelper.InvalidKeyword("static", member))

                  .InvalidOn(member => member.IsConst)
                  .ReportDiagnostic(member => DiagnosticHelper.InvalidKeyword("const", member))

                  .InvalidOn(member => HasUnsupportedReturnType(member.ReturnType))
                  .ReportDiagnostic(member => DiagnosticHelper.UnsupportedType(member));
        }

        private IEnumerable<MemberMetaInfo> GetSequentialOrder(IEnumerable<MemberMetaInfo> memberInfo, IDiagnosticReporter diagnostic, bool isExplicit) {
            var indexes = memberInfo.Where(x => x.ContainsAttribute<IndexAttribute>());
            if (indexes.Any()) {
                Debug.Assert(isExplicit);
                foreach (var indexmember in indexes)
                    diagnostic.ReportDiagnostic(DiagnosticHelper.ExplicitSequentialShouldNotIndex(indexmember, indexmember.GetLocation<IndexAttribute>()!));
                return Enumerable.Empty<MemberMetaInfo>();
            }

            var partials = memberInfo.Where(x => x.ContainingFullType.IsPartial);
            if (partials.Any()) {
                var type = partials.First().ContainingFullType;
                diagnostic.ReportDiagnostic(DiagnosticHelper.SequentialCannotBePartial(type, isExplicit));
                return Enumerable.Empty<MemberMetaInfo>();
            }

            // Get all the possible members that will be serialized
            // We will try to filter out all static and const members first
            var electedMember = memberInfo.Where(x => !x.ContainsAttribute<IgnoreAttribute>())
                                          .Where(x => !x.IsConst)
                                          .Where(x => !x.IsStatic)
                                          .Where(x => x.ReturnType != null);

            foreach (var member in electedMember.Where(x => HasUnsupportedReturnType(x.ReturnType!)))
                diagnostic.ReportDiagnostic(DiagnosticHelper.UnsupportedTypeHint(member));

            return electedMember;
        }

        private IEnumerable<MemberMetaInfo> GetExplicitOrder(IEnumerable<MemberMetaInfo> memberInfo, IDiagnosticReporter diagnostic, bool isExplicit) {

            foreach (var ignoredMember in memberInfo.Where(x => x.ContainsAttribute<IgnoreAttribute>()))
                diagnostic.ReportDiagnostic(DiagnosticHelper.ExplicitDontNeedIgnore(ignoredMember));

            var indexLookup = new Dictionary<int, MemberMetaInfo>();
            foreach(var member in memberInfo) {
                if (!member.ContainsAttribute<IndexAttribute>())
                    continue;
                var index = (int)member.GetAttribute<IndexAttribute>()!.ConstructorArguments[0].Value!;
                // Check if we have duplicate index
                if (indexLookup.TryGetValue(index, out var duplicateMember)) {
                    diagnostic.ReportDiagnostic(DiagnosticHelper.DuplicateIndexs(index, duplicateMember, member));
                    continue;
                }
                indexLookup.Add(index, member);
            }
            return indexLookup.OrderBy(x => x.Key).Select(x => x.Value);
        }

        public IEnumerable<MemberMetaInfo> GetOrder(IEnumerable<MemberMetaInfo> memberInfo, BinaryLayout layout, IDiagnosticReporter diagnostic) {
            var implicitLayout = GetImplicitLayout();
            var isExplicit = layout != BinaryLayout.Auto;
            switch (implicitLayout) {
                case BinaryLayout.Explicit: return GetExplicitOrder(memberInfo, diagnostic, isExplicit);
                case BinaryLayout.Sequential: return GetSequentialOrder(memberInfo, diagnostic, isExplicit);
                default: throw new ArgumentException("Invalid Binary Layout", nameof(layout));
            }

            BinaryLayout GetImplicitLayout() {
                if (layout != BinaryLayout.Auto)
                    return layout;
                return memberInfo.Any(x => x.ContainsAttribute<IndexAttribute>()) ? BinaryLayout.Explicit : BinaryLayout.Sequential;
            }
        }
    }
}
