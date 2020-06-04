using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.Binary.Analyzers;
using Decuplr.Serialization.Binary.Annotations.Namespaces;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {

    public struct ConstantInfo {
        public bool NeverVerify { get; set; }
    }

    public class MemberFormatInfo {

        public int Index { get; }

        public ConstantInfo? ConstantInfo { get; }

        /// <summary>
        /// The type that the member format wants to be formatted into
        /// </summary>
        public TypeDecisionAnnotation DecisionAnnotation { get; }

        // IgnoreIf
        public IReadOnlyList<Condition> FormatConditions { get; }

        public IReadOnlyList<string> UsedNamespaces { get; }

        public AnalyzedMember Analyzed { get; }

        public int? ConstantLength { get; }

        public ISymbol Symbol => Analyzed.MemberSymbol;

        public ITypeSymbol TypeSymbol { get; } // Cast to IFieldSymbol or IProperty symbol to capture this

        public MemberFormatInfo(int index, AnalyzedMember member, ITypeSymbol typeSymbol, TypeDecisionAnnotation annotation, ConstantInfo? constantInfo, IReadOnlyList<Condition> formatConditions, IReadOnlyList<string> namespaces) {
            Index = index;
            TypeSymbol = typeSymbol;
            Analyzed = member;
            DecisionAnnotation = annotation;
            ConstantInfo = constantInfo;
            FormatConditions = formatConditions;
            UsedNamespaces = namespaces;
        }

        public static IEnumerable<MemberFormatInfo> CreateFormatInfo(IReadOnlyList<AnalyzedMember> members, BinaryLayout layout, IList<Diagnostic> diagnostics) {
            // We elect members we actually want to serialize
            var targetMembers = new List<AnalyzedMember>();
            foreach (var member in members) {
                if (ShouldCreateFormatInfo(member, layout, diagnostics))
                    targetMembers.Add(member);
            }

            // we capture all associated propertysymbol so we can make sure the "constant"cy of a member is
            var asProp = targetMembers.Select(member => member.MemberSymbol is IFieldSymbol symbol ? symbol.AssociatedSymbol as IPropertySymbol : null)
                                      .Where(x => x != null)
                                      .Distinct();

            for (int i = 0; i < targetMembers.Count; i++) {
                yield return CreateFormatInfo(i, targetMembers[i], members, new HashSet<IPropertySymbol>(asProp!), diagnostics);
            }
        }

        // This functions decide what members we want to capture, and also tell user their stupid ideas to make some unsupported member serializable
        private static bool ShouldCreateFormatInfo(AnalyzedMember member, BinaryLayout layout, IList<Diagnostic> diagnostics) {
            var symbol = member.MemberSymbol;
            if (symbol.IsImplicitlyDeclared)
                return false;
            if (!(symbol is IPropertySymbol || symbol is IFieldSymbol)) {
                if (layout == BinaryLayout.Explicit)
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.NotPropertyOrFieldNeverFormats, member.Declarations[0].DeclaredLocation, symbol.Locations, symbol.Name));
                return false;
            }
            if (symbol.IsStatic) {
                // Note we only mark the first appear location of declared type, we should mark the location where the [Index] is, maybe later
                if (layout == BinaryLayout.Explicit)
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.StaticNeverFormats, member.Declarations[0].DeclaredLocation, symbol.Locations, symbol.Name));
                return false;
            }
            if (symbol is IFieldSymbol fieldSymbol && fieldSymbol.IsConst) {
                if (layout == BinaryLayout.Explicit)
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.ConstNeverFormats, member.FirstLocation, symbol.Locations, symbol.Name));
                return false;
            }
            if (symbol.ContainingType.TypeKind == TypeKind.Delegate) {
                if (layout == BinaryLayout.Explicit) {
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.DelegatesNeverFormats, member.Declarations[0].DeclaredLocation, symbol.Locations, symbol.Name));
                    return false;
                }
                diagnostics.Add(Diagnostic.Create(DiagnosticHelper.DelegatesNeverFormatsHint, member.Declarations[0].DeclaredLocation, symbol.Locations, symbol.Name));
                return false;
            }
            return true;
        }

        // Then we try to create it
        private static MemberFormatInfo CreateFormatInfo(int index, AnalyzedMember member, IReadOnlyList<AnalyzedMember> referencedMember, ISet<IPropertySymbol> associated, IList<Diagnostic> diagnostics) {
            ITypeSymbol typeSymbol;
            ConstantInfo? isConstant = GetConstantInfo(member, associated, diagnostics);
        }

        private static ConstantInfo? GetConstantInfo(AnalyzedMember member, ISet<IPropertySymbol> associated, IList<Diagnostic> diagnostics) {
            // (note : we don't allow const fields or static fields for verification or functions)
            if (!(member.MemberSymbol is IPropertySymbol propertySymbol))
                return null;
            // Checks if this property is readonly and is not backed by any property symbol 
            if (!propertySymbol.IsReadOnly || associated.Contains(propertySymbol))
                return null;
            if (!member.ContainsAttribute<ConstantAttribute>()) {
                diagnostics.Add(Diagnostic.Create(DiagnosticHelper.ShouldApplyConstant, member.FirstLocation, member.MemberSymbol.Name));
                return new ConstantInfo {
                    NeverVerify = ((bool?)member.GetAttributes<ConstantAttribute>().First().Data.NamedArguments.First(x => x.Key == nameof(ConstantAttribute.NeverVerify)).Value.Value) ?? false
                };
            }
            if (propertySymbol.IsWriteOnly) {
                diagnostics.Add(Diagnostic.Create(DiagnosticHelper.PropertyCannotBeWriteOnly, member.FirstLocation, propertySymbol.Name));
                return null;
            }
            return null;
        }
    }

}
