using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Decuplr.Sourceberg {
    /// <summary>
    /// Describes the lifetime of the compilation
    /// </summary>
    public interface ICompilationLifetime {
        /// <summary>
        /// Notifies when compilation is canceled
        /// </summary>
        CancellationToken OnCompilationCanceled { get; }
    }
}
