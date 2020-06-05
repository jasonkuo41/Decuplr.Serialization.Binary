using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.Binary.Analyzers;
using Decuplr.Serialization.Binary.Annotations;
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

        public MemberFormatInfo(int index, AnalyzedMember member, IReadOnlyList<AnalyzedMember> members, ISet<IPropertySymbol> associated, IList<Diagnostic> diagnostics) {
            Index = index;
            TypeSymbol = member.MemberSymbol is IPropertySymbol propSymbol ? propSymbol.Type : member.MemberSymbol is IFieldSymbol field ? field.Type : throw new ArgumentException("Serializable member can only be property or field");
            Analyzed = member;
            DecisionAnnotation = GetDecisionAnnotation(member, TypeSymbol, diagnostics);
            ConstantInfo = GetConstantInfo(member, associated, diagnostics);
            FormatConditions = GetFormatConditions(member, diagnostics);
            UsedNamespaces = GetUsingNamespace(member, diagnostics);
        }

        public static IEnumerable<MemberFormatInfo> CreateFormatInfo(IReadOnlyList<AnalyzedMember> members, BinaryLayout layout, IList<Diagnostic> diagnostics) {
            // we capture all associated propertysymbol so we can make sure the "constant"cy of a member is
            var asProp = members.Select(member => member.MemberSymbol is IFieldSymbol symbol ? symbol.AssociatedSymbol as IPropertySymbol : null)
                                .Where(x => x != null)
                                .Distinct();

            // We elect members we actually want to serialize
            var targetMembers = new List<AnalyzedMember>();
            foreach (var member in members) {
                if (ShouldCreateFormatInfo(member, layout, diagnostics))
                    targetMembers.Add(member);
            }

            for (int i = 0; i < targetMembers.Count; i++) {
                yield return new MemberFormatInfo(i, targetMembers[i], members, new HashSet<IPropertySymbol>(asProp!), diagnostics);
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

        private static ConstantInfo? GetConstantInfo(AnalyzedMember member, ISet<IPropertySymbol> associated, IList<Diagnostic> diagnostics) {
            // (note : we don't allow const fields or static fields for verification or functions)
            if (!(member.MemberSymbol is IPropertySymbol propertySymbol))
                return null;
            // Checks if this property is readonly and is not backed by any property symbol 
            if (!propertySymbol.IsReadOnly || associated.Contains(propertySymbol))
                return null;
            if (!member.ContainsAttribute<ConstantAttribute>()) {
                diagnostics.Add(Diagnostic.Create(DiagnosticHelper.ShouldApplyConstant, member.FirstLocation, member.MemberSymbol.Name));
                var containsConstant = member.ContainsAttribute<ConstantAttribute>();
                return new ConstantInfo {
                    NeverVerify = containsConstant && (((bool?)member.GetAttributes<ConstantAttribute>().First().Data.NamedArguments.First(x => x.Key == nameof(ConstantAttribute.NeverVerify)).Value.Value) ?? false)
                };
            }
            if (propertySymbol.IsWriteOnly) {
                diagnostics.Add(Diagnostic.Create(DiagnosticHelper.PropertyCannotBeWriteOnly, member.FirstLocation, propertySymbol.Name));
                return null;
            }
            return null;
        }

        private static IReadOnlyList<string> GetUsingNamespace(AnalyzedMember member, IList<Diagnostic> diagnostics) {
            var analyzer = member.Analyzer;
            var useNamespaceSymbol = analyzer.GetSymbol<UseNamespaceAttribute>();
            var namespaces = new List<string>();
            // checks for mutliple attribute layout
            // TODO : Make ApplyNamespace Usable
            var foundNamespaceLocation = false;
            foreach(var partial in member.Declarations) {
                foreach (var attribute in partial.Attributes.SelectMany(x => x)) {
                    if (foundNamespaceLocation)
                        diagnostics.Add(Diagnostic.Create(DiagnosticHelper.CannotApplyNamespacePartial, attribute.Location, attribute.Data.AttributeClass?.Name));
                    if (attribute.Data.AttributeClass?.Equals(useNamespaceSymbol, SymbolEqualityComparer.Default) ?? false) {
                        namespaces.Add((string)attribute.Data.ConstructorArguments[0].Value!);
                    }
                }
                // We check how many namespaces we have added
                if (namespaces.Count > 0)
                    foundNamespaceLocation = true;
            }
            return namespaces;
        }

        private static IReadOnlyList<Condition> GetFormatConditions(AnalyzedMember member, IList<Diagnostic> diagnostics) {
            // TODO: Make this a feature
            return Array.Empty<Condition>();
        }

        private static TypeDecisionAnnotation GetDecisionAnnotation(AnalyzedMember member, ITypeSymbol returnSymbol, IList<Diagnostic> diagnostics) {
            return new DefaultTypeDecisionAnnotation(returnSymbol);
        }
    }

}
