using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Decuplr.Sourceberg.Services {
    /// <summary>
    /// Describes the lifetime of the compilation
    /// </summary>
    public interface IAnalysisLifetime {
        /// <summary>
        /// Notifies when compilation is canceled
        /// </summary>
        CancellationToken OnOperationCanceled { get; }
    }
}
