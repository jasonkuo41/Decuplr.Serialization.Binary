using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Templates.ParserSource {

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

}
