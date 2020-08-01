using System;

namespace Decuplr.CodeAnalysis.Serialization.StartupServices {
    // Make this public?
    internal interface IGenerationStartupServices {
        IServiceProvider GetStartupScopeService(IGenerationStartup startup);
    }
}
