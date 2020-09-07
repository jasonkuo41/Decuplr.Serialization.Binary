using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Sourceberg.Services.Implementation {
    internal class SourceContextAccessor : IAnalysisLifetime {
        public CancellationToken OnOperationCanceled { get; set; }

        public Compilation? SourceCompilation { get; set; }
    }

}
