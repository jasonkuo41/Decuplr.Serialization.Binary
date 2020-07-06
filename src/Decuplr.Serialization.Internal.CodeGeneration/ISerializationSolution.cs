using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ISerializationSolution {
        GeneratedSolution Generated();
    }

    public interface IDeserializationSolution {
        GeneratedSolution Generated();
    }

    public interface ILengthSolution {

    }

    public struct GeneratedSolution {
        public string FileName { get; set; }
        public string FullTypeName { get; set; }
        public string SourceCode { get; set; }
        public string EntryPoint { get; set; }
    }
}
