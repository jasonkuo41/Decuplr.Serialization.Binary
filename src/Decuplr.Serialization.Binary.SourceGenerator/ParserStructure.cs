using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    // Here we define the parser structure
    // The parser comes in with three different places
    // 1. Additional partial type to the og type
    // 2. 
    // 3. 

    // This file can then be finalized into actual code
    class ParserMainStructure {

    }

    class ParserArgsStructure {
        public string? StructName { get; set; }

        public List<EnlistedParsers> Parsers { get; } = new List<EnlistedParsers>();
    }

    abstract class EnlistedParsers {
        public abstract string ParserArgName { get; }

        public abstract string ParserTypeName { get; }

        public abstract string GetTrySerializeFunction(string parserArgsName, string argName, string destName);
        public abstract string GetSerializeFunction(string parserArgsName, string argName, string destName);

        public abstract string GetTryDeserializeFunction(string parserArgsName, string argName, string destName);
    }

    internal class TypeParserArgument : EnlistedParsers {

    }
}
