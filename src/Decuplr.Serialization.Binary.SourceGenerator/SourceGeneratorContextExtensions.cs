using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Decuplr.Serialization.CodeGeneration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Serialization.Binary {
    static class SourceGeneratorContextExtensions {

        [Conditional("DEBUG")]
        private static void WriteFiles(IEnumerable<GeneratedSourceCode> sourceCodes, string path) {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), path);
            Directory.CreateDirectory(directory);
            foreach (var sourceCode in sourceCodes) {
                File.WriteAllText(Path.Combine(directory, sourceCode.DesiredFileName), sourceCode.SourceText);
            }
        }

        private static string OutputException(Exception exception) => 
@$"An Exception Has Occured : 
    {exception.GetType().Name}

Message : 
    {exception.Message}

TargetSite :
    {exception.TargetSite}

StackTrace :
    {exception.StackTrace}

{(exception.InnerException is null ? null : "===== Start of InnerException =====")}

{(exception.InnerException is null ? null : OutputException(exception.InnerException))}";

        [Conditional("DEBUG")]
        private static void WriteExceptionFiles(Exception exception, string path, string errorFilename) {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), path);
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, Path.ChangeExtension(errorFilename, ".txt")), OutputException(exception));
        }

        public static void AddSource(this SourceGeneratorContext generatorContext, IEnumerable<GeneratedSourceCode> sourceCodes, string? debugOutputPath = null) {
            debugOutputPath ??= ".generated";
            foreach(var sourceCode in sourceCodes)
                generatorContext.AddSource(sourceCode.DesiredFileName, SourceText.From(sourceCode.SourceText, Encoding.UTF8));
            WriteFiles(sourceCodes, debugOutputPath);
        }

        public static void WriteException(this SourceGeneratorContext generatorContext, Exception exception, string? debugOutputPath = null, string? errorFileName = null) {
            debugOutputPath ??= ".generated";
            errorFileName ??= "error.txt";
            WriteExceptionFiles(exception, debugOutputPath, errorFileName);

            var currentException = exception;
            while (currentException != null) {
                generatorContext.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("DSB-SG-CPE", "Exception Occured During Compilation", "Decuplr.InternalError",
                   @$"An exception has occured during compilation. {exception.GetType()} : {exception.Message} (Site : {exception.TargetSite})", DiagnosticSeverity.Warning, true), Location.None));
                currentException = currentException.InnerException;
            }
        }

        public static void ReportDiagnostic(this SourceGeneratorContext context, IEnumerable<Diagnostic> diagnostics) {
            foreach (var diagnostic in diagnostics)
                context.ReportDiagnostic(diagnostic);
        }

    }
}
