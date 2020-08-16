using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Decuplr.Serialization.Namespaces {

    /// <summary>
    /// Provides convenience methods for creating instances of <see cref="INamespaceTree"/> and it's extension methods.
    /// </summary>
    public static class NamespaceTree {
        /// <summary>
        /// Clones the read only namespace tree to a new created tree for modification.
        /// </summary>
        /// <param name="tree">The tree to be copied</param>
        /// <returns>A new copied tree</returns>
        public static INamespaceTree Clone(this IReadOnlyNamespaceTree tree) => new NamespaceTreeSource(tree);

        /// <summary>
        /// Creates a new instance of <see cref="INamespaceTree"/>.
        /// </summary>
        /// <returns>A new namespace tree instance</returns>
        public static INamespaceTree Create() => new NamespaceTreeSource();
    }

}
