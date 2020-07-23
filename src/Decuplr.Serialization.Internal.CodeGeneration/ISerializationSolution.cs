using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ISerializationSolution {
        IEnumerable<GeneratedParserInfo> Generate(IComponentProvider provider, SchemaLayout layout, CancellationToken ct);
    }
}
