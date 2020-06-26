using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.AnalysisService;
using Decuplr.Serialization.Binary.LayoutService.Internal;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService.Internal {

    internal class LayoutMemberCollection : ILayoutMemberCollection {


        public void ValidateLayout(IEnumerable<MemberMetaInfo> members, IDiagnosticReporter reporter) {

        }
    }

}

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

        public void ElectMember(ILayoutMemberCollection filter) {
            filter.WhereAttribute<IndexAttribute>()
                  .InvalidOn(member => member.ContainsAttribute<IgnoreAttribute>())
                  .ReportDiagnostic((member, location) => DiagnosticHelper.ConflictingAttributes(member, member.GetAttribute<IgnoreAttribute>()!, member.GetAttribute<IndexAttribute>()!))

                  .InvalidOn(member => member.IsStatic)
                  .ReportDiagnostic((member, location) => DiagnosticHelper.InvalidKeyword("static", member))

                  .InvalidOn(member => member.IsConst)
                  .ReportDiagnostic((member, location) => DiagnosticHelper.InvalidKeyword("const", member))

                  .InvalidOn(member => HasUnsupportedReturnType(member.ReturnType))
                  .ReportDiagnostic((member, location) => DiagnosticHelper.UnsupportedType(member));
        }

        private IEnumerable<MemberMetaInfo> GetSequentialOrder(IEnumerable<MemberMetaInfo> memberInfo, IDiagnosticReporter diagnostic, bool isExplicit) {
            var indexes = memberInfo.Where(x => x.ContainsAttribute<IndexAttribute>());
            if (indexes.Any()) {
                Debug.Assert(isExplicit);
                foreach (var indexmember in indexes)
                    diagnostic.ReportDiagnostic(DiagnosticHelper.ExplicitSequentialShouldNotIndex(indexmember, indexmember.GetLocation<IndexAttribute>()!));
                return Enumerable.Empty<MemberMetaInfo>();
            }
            if (memberInfo.Any(x => x.ContainingFullType.IsPartial)) {
                if (isExplicit)
                    diagnostic.ReportDiagnostic();
                else
                    diagnostic.ReportDiagnostic();
                return Enumerable.Empty<MemberMetaInfo>();
            }

            // Get all the possible members that will be serialized
            // We will try to filter out all static and const members first
            var electedMember = memberInfo.Where(x => !x.ContainsAttribute<IgnoreAttribute>())
                                          .Where(x => !x.IsConst)
                                          .Where(x => !x.IsStatic)
                                          .Where(x => x.ReturnType != null);

            var invalidMembers = electedMember.Where(x => HasUnsupportedReturnType(x.ReturnType!));
            if (invalidMembers.Any()) {
                foreach (var member in invalidMembers)
                    diagnostic.ReportDiagnostic(DiagnosticHelper.UnsupportedType(member));
                return Enumerable.Empty<MemberMetaInfo>();
            }

            return electedMember;
        }

        private IEnumerable<MemberMetaInfo> GetExplicitOrder(IEnumerable<MemberMetaInfo> memberInfo, IDiagnosticReporter diagnostic, bool isExplicit) {
            if (memberInfo.Any(x => x.ContainsAttribute<IgnoreAttribute>())) {
                diagnostic.ReportDiagnostic();
                return Enumerable.Empty<MemberMetaInfo>();
            }

            var indexLookup = new Dictionary<int, MemberMetaInfo>();
            foreach(var member in memberInfo) {
                if (!member.ContainsAttribute<IndexAttribute>())
                    continue;
                var index = (int)member.GetAttribute<IndexAttribute>()!.ConstructorArguments[0].Value!;
                if (indexLookup.ContainsKey(index)) {
                    diagnostic.ReportDiagnostic();
                    return Enumerable.Empty<MemberMetaInfo>();
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

    internal class TypeValidator {

        private readonly TypeMetaInfo _type;
        private readonly SchemaPrecusor _precusor;

        private readonly DiagnosticReporter _reporter = new DiagnosticReporter();

        public TypeValidator(TypeMetaInfo type, SchemaPrecusor precusor) {
            _type = type;
            _precusor = precusor;
        }

        /// <summary>
        /// Validate if every attribute is correct and can generate correct layout
        /// </summary>
        public bool ValidateLayout<TSelector>(IMemberFilter filter, out TypeLayout? layout, out IEnumerable<Diagnostic> diagnostics) where TSelector : IOrderSelector, new() {
            var selector = new TSelector();

            var layoutFilter = new LayoutMemberCollection();
            selector.ElectMember(layoutFilter);
            layoutFilter.ValidateLayout(_type.Members, _reporter);

            var layoutMembers = filter.SelectMembers(_type.Members);
            layoutMembers = selector.GetOrder(layoutMembers, _precusor.RequestLayout, _reporter);

            diagnostics = _reporter.ExportDiagnostics();
            if (_reporter.IsUnrecoverable) {
                layout = null;
                return false;
            }
            layout = new TypeLayout(_type, layoutMembers.ToList());
            return true;
        }
    }
}
