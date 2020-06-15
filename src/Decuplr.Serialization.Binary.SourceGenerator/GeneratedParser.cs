using System;
using System.Collections.Generic;
using Decuplr.Serialization.Analyzer.BinaryFormat;

namespace Decuplr.Serialization.Binary {
    internal class GeneratedParser {

        /// <summary> The name used to identify this parser </summary>
        public string ParserTypeName { get; set; } = string.Empty;

        /// <summary> For data that cannot be embedded in our entry point class</summary> 
        public IReadOnlyList<GeneratedSourceCode> AdditionalSourceFiles { get; set; } = Array.Empty<GeneratedSourceCode>();

        /// <summary>For external classes that we queeze into the embedded class</summary> 
        public EmbeddedCode EmbeddedCode { get; set; }

        public IReadOnlyList<IParserKindProvider> ParserKinds { get; set; } = Array.Empty<IParserKindProvider>();

        public IReadOnlyList<string> ParserNamespaces { get; set; } = Array.Empty<string>();
    }

}
