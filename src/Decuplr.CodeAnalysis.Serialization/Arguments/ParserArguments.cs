using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.CodeAnalysis.Serialization.Arguments {

    // Provides easy to read arguments that actually just takes the string and output the string

    public struct ParsingTypeArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator ParsingTypeArgs(string str) => new ParsingTypeArgs { Name = str };
    }

    public struct TargetFieldArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator TargetFieldArgs(string str) => new TargetFieldArgs { Name = str };
    }

    public struct BufferArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator BufferArgs(string str) => new BufferArgs { Name = str };
    }

    public struct ConstructStructArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator ConstructStructArgs(string str) => new ConstructStructArgs { Name = str };
    }

    public struct TypeSourceArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator TypeSourceArgs(string str) => new TypeSourceArgs { Name = str };
    }

    public struct InArgs<T> {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator InArgs<T>(string str) => new InArgs<T> { Name = str };
    }

    public struct OutArgs<T> {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator OutArgs<T>(string str) => new OutArgs<T> { Name = str };
    }

    // IParserDiscovery
    public struct ParserDiscoveryArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator ParserDiscoveryArgs(string str) => new ParserDiscoveryArgs { Name = str };
    }

    // i.e. Type_0X5JHH14_Constructor ctor; in TypeParser
    public struct ParserConstructArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator ParserConstructArgs(string str) => new ParserConstructArgs { Name = str };
    }

}
