using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary {
    internal static class SourceExtensions {

        [Conditional("DEBUG")]
        private static void WriteFiles(GeneratedSourceText sourceCode, string path) {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), path);
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, sourceCode.HintName), sourceCode.Text.ToString());
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

        public static void AddSourceWithDebug(this SourceGeneratorContext generatorContext, GeneratedSourceText sourceCode, string? debugOutputPath = null) {
            debugOutputPath ??= ".generated";
            WriteFiles(sourceCode, debugOutputPath);
            generatorContext.AddSource(sourceCode.HintName, sourceCode.Text);
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

    }
}
