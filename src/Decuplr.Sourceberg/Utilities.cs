using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Sourceberg {
    internal static class Utilities {
        public static void ThrowIfNull<T>(this T item, string name) where T : class {
            if (item is null)
                throw new NullReferenceException(name);
        }
    }
}
