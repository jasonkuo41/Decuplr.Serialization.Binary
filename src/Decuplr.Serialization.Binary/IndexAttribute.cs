using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class IndexAttribute : Attribute {
        public IndexAttribute(int index) {
            Index = index;
        }

        public int Index { get; }
    }
}
