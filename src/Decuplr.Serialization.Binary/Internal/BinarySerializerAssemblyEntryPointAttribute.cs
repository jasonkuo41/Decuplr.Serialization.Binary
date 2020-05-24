using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Decuplr.Serialization.Binary.Internal {
    /// <summary>
    /// Used to identitfy the entry point of add generated serializer to the Shared instace, this class should not be used 
    /// and shall only be invoked by generated code
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class BinarySerializerAssemblyEntryPointAttribute : Attribute { }
}
