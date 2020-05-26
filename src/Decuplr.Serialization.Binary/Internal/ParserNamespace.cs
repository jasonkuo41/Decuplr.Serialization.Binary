﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Internal {

    /// <summary>
    /// This marks our internal parsers namespace
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class ParserNamespaceAttribute : Attribute {
        public ParserNamespaceAttribute(string targetNamespace) {
            Namespace = targetNamespace;
        }

        public string Namespace { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class MutatableParserAttribute : Attribute {

    }
}
