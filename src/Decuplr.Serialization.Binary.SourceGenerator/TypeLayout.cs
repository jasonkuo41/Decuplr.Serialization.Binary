using System;
using System.Text;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Decuplr.Serialization.Binary.SourceGenerator {

    internal struct FormatLengthInfo {
        public int FixedLength { get; }
        public int MinLength { get; }
        public int MaxLength { get; }
        public OverflowBehaviour OverflowBehaviour { get; }
        public UnderflowBehaviour UnderflowBehaviour { get; }
    }

    public enum OverflowBehaviour {
        /// <summary>
        /// Treat the data as faulty and should stop serializing, result in a <see cref="DeserializeResult.Faulted"/>
        /// </summary>
        FaultyData,

        /// <summary>
        /// Treat the data as normal, but trim excessive bytes
        /// </summary>
        TrimExcessive,
    }

    public enum UnderflowBehaviour {
        /// <summary>
        /// Treat the data as faulty and should stop serializing, result in a <see cref="DeserializeResult.Faulted"/>
        /// </summary>
        FaultyData,

        /// <summary>
        /// Fill the missing data with zero bytes
        /// </summary>
        FillZero,
    }
}
