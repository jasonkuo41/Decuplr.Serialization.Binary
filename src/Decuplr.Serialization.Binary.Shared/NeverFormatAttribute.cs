using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Tell's the parser to never format this field, useful when BinaryLayout is set as <see cref="BinaryLayout.Sequential"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class NeverFormatAttribute : Attribute { }
}
