﻿using System;

namespace Decuplr.Serialization.CodeGeneration {

    public struct GeneratedSourceCode {

        public GeneratedSourceCode(string desiredFileName, string sourceText) {
            DesiredFileName = desiredFileName;
            SourceText = sourceText;
        }

        public string DesiredFileName { get; set; }
        public string SourceText { get; set; }

        public static implicit operator GeneratedSourceCode((string, string) tuple) => new GeneratedSourceCode(tuple.Item1, tuple.Item2);
    }
}