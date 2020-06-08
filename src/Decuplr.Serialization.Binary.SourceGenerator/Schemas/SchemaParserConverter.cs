using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator.Schemas {
    /// <summary>
    /// Magically turns schema into parsers at no time
    /// </summary>
    static class SchemaParserConverter {
        public static bool TryConvert(AnalyzedType type, Compilation compilation, SchemaPrecusor precusor, out IList<Diagnostic> diagnosticReports, out GeneratedParser parser) {
            parser = default;

            // We dump the type and get the useful layout for this type
            if (!TypeFormatLayout.TryGetLayout(type, ref precusor, out diagnosticReports, out var typeLayout))
                return false;

            // Check if the target parser would need to use `partial` keyword to help up invoke into the code
            // I mean if it's already partial, we might as well as embed into it

            /* Currently, there are since it's kind of complicated to decide whether we can not use partial, so we now force users to partial everything  */
            // If it's not partial, we check if we can access every member with 
            //if (CanAccessAllGetValue(typeLayout) && (typeLayout.CanDeserialize || CanAccessAllSetValue(typeLayout))) {
            // If we can then we use observer class to invoke the class

            //}
            // Otherwise the type must be partial, or else we don't like it
            if (!typeLayout!.Type.IsPartial) {
                diagnosticReports = new List<Diagnostic> { Diagnostic.Create(DiagnosticHelper.ShouldDeclarePartial, typeLayout.Type.Declarations[0].DeclaredLocation, typeLayout.Type.TypeSymbol.Name) };
                return false;
            }
            // We pump a constructor for our sweet partial class here
            parser = new TypeParserGenerator(typeLayout, new PartialTypeDeserialize(compilation, typeLayout), new PartialTypeSerialize(compilation, typeLayout)).GetFormatterCode();
            return true;
        }
    }
}
