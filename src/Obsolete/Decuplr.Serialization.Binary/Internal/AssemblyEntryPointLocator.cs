using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Decuplr.Serialization.Binary.Internal {
    /// <summary>
    /// Get's the entry point of an assembly
    /// </summary>
    internal static class AssemblyEntryPoint {
        private readonly static ConcurrentDictionary<Assembly, AssemblyPackerEntryPoint?> EntryPoints = new ConcurrentDictionary<Assembly, AssemblyPackerEntryPoint?>();

        public static AssemblyPackerEntryPoint? GetParserEntryPoint(this Assembly assembly) {
            return EntryPoints.GetOrAdd(assembly, asm => {
                var entryPoint = assembly.GetCustomAttribute<BinaryPackerAssemblyEntryPointAttribute>();
                if (entryPoint is null)
                    return null;
                return Activator.CreateInstance(entryPoint.EntryType) as AssemblyPackerEntryPoint;
            });
        }
    }
}
