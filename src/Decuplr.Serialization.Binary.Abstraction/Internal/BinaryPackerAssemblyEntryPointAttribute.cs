using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Decuplr.Serialization.Binary.Internal {
    /// <summary>
    /// Used to identify the entry point for the assembly of a generated serializer to the Shared instance.
    /// </summary>
    /// <remarks>
    /// <b>This attribute should not be used directly.</b>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class BinaryPackerAssemblyEntryPointAttribute : Attribute {

        public BinaryPackerAssemblyEntryPointAttribute(Type entryType) {
            EntryType = entryType;
        }

        public Type EntryType { get; }

    }
}
