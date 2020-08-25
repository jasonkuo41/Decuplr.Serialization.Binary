using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    internal static class SyntaxFactsExtensions {
        public static void EnsureValidIdentifiers(this IEnumerable<string> names) {
            var invalidNames = names.Where(x => !SyntaxFacts.IsValidIdentifier(x));
            if (invalidNames.Any())
                throw new ArgumentException($"'{string.Join(", ", invalidNames)}' contains invalid identifiers");
        }
        public static void EnsureValidIdentifier(this string name) {
            if (SyntaxFacts.IsValidIdentifier(name))
                throw new ArgumentException($"'{name}' is a invalid identifier");
        }

    }
}
