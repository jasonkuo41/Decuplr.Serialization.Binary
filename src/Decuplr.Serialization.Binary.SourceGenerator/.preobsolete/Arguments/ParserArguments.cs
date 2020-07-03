﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Arguments {

    // Provides easy to read arguments that actually just takes the string and output the string

    internal struct ParsingTypeArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator ParsingTypeArgs(string str) => new ParsingTypeArgs { Name = str };
    }

    internal struct TargetFieldArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator TargetFieldArgs(string str) => new TargetFieldArgs { Name = str };
    }

    internal struct BufferArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator BufferArgs(string str) => new BufferArgs { Name = str };
    }

    internal struct ConstructStructArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator ConstructStructArgs(string str) => new ConstructStructArgs { Name = str };
    }

    internal struct TypeSourceArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator TypeSourceArgs(string str) => new TypeSourceArgs { Name = str };
    }

    internal struct InArgs<T> {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator InArgs<T>(string str) => new InArgs<T> { Name = str };
    }

    internal struct OutArgs<T> {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator OutArgs<T>(string str) => new OutArgs<T> { Name = str };
    }

    // IParserDiscovery
    internal struct ParserDiscoveryArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator ParserDiscoveryArgs(string str) => new ParserDiscoveryArgs { Name = str };
    }

    // i.e. Type_0X5JHH14_Constructor ctor; in TypeParser
    internal struct ParserConstructArgs {
        public string Name { get; set; }
        public override string ToString() => Name;
        public static implicit operator ParserConstructArgs(string str) => new ParserConstructArgs { Name = str };
    }

}
