using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    internal static class AccessibilityExtensions {
        public static string ToCodeString(this Accessibility accessibility) {
            switch (accessibility) {
                case Accessibility.Private:
                case Accessibility.Protected:
                case Accessibility.Internal:
                case Accessibility.Public:
                    return accessibility.ToString().ToLowerInvariant();
                case Accessibility.ProtectedOrInternal: return "protected internal";
                case Accessibility.ProtectedAndInternal: return "private protected";
                default: throw new ArgumentException($"Invalid Accessibility : {accessibility}");
            }
        }
    }

}
