using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Decuplr.CodeAnalysis {
    /// <summary>
    /// Describes the lifetime of the compilation
    /// </summary>
    public interface ICompilationLifetime {
        /// <summary>
        /// Notifys when compilation is cancelled
        /// </summary>
        CancellationToken OnCompilationCancelled { get; }
    }
}
