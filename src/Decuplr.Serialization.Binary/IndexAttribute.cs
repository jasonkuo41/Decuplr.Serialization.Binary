using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class IndexAttribute : Attribute {
        public IndexAttribute(int index) {
            Index = index;
            FixedSize = -1;
        }

        public int Index { get; }
        public int FixedSize { get; set; } = -1;
        public int MaxSize { get; set; } = -1;
        public int MinSize { get; set; } = -1;
    }
}
