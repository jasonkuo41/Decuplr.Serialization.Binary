using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.Binary.Analyzers;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {

    public struct FormatInfo {
        public bool CanDeserialize { get; }

        public bool IsSealed { get; }
    }

    public class TypeFormatLayout {

        public AnalyzedType Type { get; }

        public IReadOnlyList<MemberFormatInfo> Member { get; }

        public INamedTypeSymbol TypeSymbol => Type.TypeSymbol;

        public Location FirstLocation => Type.Declarations[0].DeclaredLocation;

        public FormatInfo? FormatInfo { get; }

        private TypeFormatLayout(AnalyzedType type, IReadOnlyList<MemberFormatInfo> member) {
            Type = type;
            Member = member;
        }

        public static bool TryGetLayout(AnalyzedType type, out IList<Diagnostic> diagnostics, out TypeFormatLayout? layout) {
            diagnostics = new List<Diagnostic>();
            layout = default;

            // Check if type contains what layout
            // TODO : hard code [BinaryFormat] [BinaryParser] and [InlineData] since there are the only three; also make them return format info
            var formatAttribute = type.GetAttributes<BinaryFormatAttribute>().FirstOrDefault();
            if (formatAttribute.IsEmpty)
                return false;
            // We get the user's claimed layout type
            var statedLayout = formatAttribute.Data.GetNamedArgumentValue<BinaryLayout>(nameof(BinaryFormatAttribute.Layout)) ?? BinaryLayout.Auto;
            // We also get if we have index attributes
            var indexAttributes = type.Declarations.SelectMany(x => x.Members)
                .Where(member => member.ContainsAttribute<IndexAttribute>())
                .Select(member => (Member: member, Attribute: member.GetAttributes<IndexAttribute>().First()))
                .ToList();

            if (!TryEnsureLayout(type, ref statedLayout, formatAttribute, indexAttributes.Select(x => x.Attribute).ToList(), diagnostics))
                return false;

            IReadOnlyList<AnalyzedMember> orderedMembers;
            if (statedLayout == BinaryLayout.Explicit) {
                // with explicit we use index attribute as our order guidance
                var indexs = indexAttributes.Select(x => (Index: (int)x.Attribute.Data.ConstructorArguments[0].Value!, x.Member)).ToList();
                {
                    // locate duplicate indexs
                    var foundDuplicate = false;
                    foreach (var gmember in indexs.GroupBy(x => x.Index).Where(x => x.Count() > 1)) {
                        diagnostics.Add(Diagnostic.Create(DiagnosticHelper.DuplicateIndexs, gmember.First().Member.Declarations[0].DeclaredLocation, gmember.Key));
                        foundDuplicate = true;
                    }
                    if (foundDuplicate)
                        return false;
                }
                orderedMembers = indexs.OrderBy(x => x.Index).Select(x => x.Member).ToList();
            }
            else {
                // with sequential we use the declared order and use ignore attribute to skip member
                Debug.Assert(type.Declarations.Count == 1);
                orderedMembers = type.Declarations[0].Members.Where(type => !type.ContainsAttribute<IgnoreAttribute>()).ToList();
            }
            layout = new TypeFormatLayout(type, MemberFormatInfo.CreateFormatInfo(orderedMembers, statedLayout, diagnostics).ToList());
            return true;
        }

        private static bool TryEnsureLayout(AnalyzedType type, ref BinaryLayout statedLayout, AnalyzedAttribute formatAttribute, IReadOnlyList<AnalyzedAttribute> indexAttributes, IList<Diagnostic> diagnostics) {

            // If it's auto or sequential, we will now try to locate if there's Index set of our member.
            // 
            var originalLayout = statedLayout;
            statedLayout = (statedLayout == BinaryLayout.Auto && indexAttributes.Count == 0) ? BinaryLayout.Sequential : BinaryLayout.Explicit;

            if (statedLayout == BinaryLayout.Sequential) {
                // If we detect more then one "index", since they ask for sequential, dump error
                if (indexAttributes.Count != 0) {
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.SequentialShouldNotIndex, indexAttributes[0].Location, indexAttributes.Select(x => x.Location)));
                    return false;
                }
                // If there's mutliple declaration, dump error
                if (type.Declarations.Count > 1) {
                    if (originalLayout == BinaryLayout.Auto)
                        diagnostics.Add(Diagnostic.Create(DiagnosticHelper.AutoAsSequentialTooMuchDeclare, type.Declarations[0].DeclaredLocation, type.Declarations.Select(x => x.DeclaredLocation), type.TypeSymbol));
                    else
                        diagnostics.Add(Diagnostic.Create(DiagnosticHelper.SequentialTooMuchDeclare, type.Declarations[0].DeclaredLocation, type.Declarations.Select(x => x.DeclaredLocation), type.TypeSymbol));
                    return false;
                }
            }

            // If it's explicit and we see NeverFormat, we hint the user that we don't really need that
            else {
                if (indexAttributes.Count == 0) {
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.ExplicitNoIndex, formatAttribute.Location));
                    return false;
                }
                // If we found NeverFormat, just softly hint 
                var neverFormat = type.Declarations.SelectMany(x => x.Members).SelectMany(partial => partial.GetAttributes<IgnoreAttribute>()).ToList();
                if (neverFormat.Count != 0)
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.ExplicitDontNeedNeverFormat, neverFormat[0].Location, neverFormat.Select(x => x.Location)));
            }

            // We are done,
            return true;
        }

    }

}
