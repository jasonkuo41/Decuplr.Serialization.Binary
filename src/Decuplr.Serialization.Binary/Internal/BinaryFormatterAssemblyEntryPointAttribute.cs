using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Decuplr.Serialization.Binary.Internal {
    /// <summary>
    /// <b>This attribute should not be used directly.</b>
    /// Used to identitfy the entry point for the assembly of a generated serializer to the Shared instance.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class BinaryFormatterAssemblyEntryPointAttribute : Attribute {
        public const string EntryFunctionName = "AppendFormatters";
    }
}
