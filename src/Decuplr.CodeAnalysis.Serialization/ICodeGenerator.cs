using System;
using System.Collections.Generic;
using System.Threading;
using Decuplr.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface ICodeGenerator {
        void VerifySyntax();
        void GenerateFiles();
    }
}