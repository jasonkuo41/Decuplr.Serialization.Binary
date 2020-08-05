using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    public class MethodArg {

        public ITypeSymbol? BackingSymbol { get; }

        public TypeQualifyName BackingQualifyName { get; }

        public string TypeName { get; }

        public string Name { get; }

        public RefKind RefKind { get; }

        public MethodArg(ITypeSymbol type, string argName) 
            : this(RefKind.None, type, argName) {
        }

        public MethodArg(RefKind refKind, ITypeSymbol type, string name) {
            if (!SyntaxFacts.IsValidIdentifier(name))
                throw new ArgumentException("Invalid method name", nameof(name));
            Name = name;
            RefKind = refKind;
            TypeName = type.ToString();
            BackingSymbol = type;
        }

        public MethodArg(TypeQualifyName typeName, string name, RefKind refKind) {
            if (!SyntaxFacts.IsValidIdentifier(name))
                throw new ArgumentException("Invalid method name", nameof(name));
            Name = name;
            RefKind = refKind;
            TypeName = typeName.ToString();
            BackingQualifyName = typeName;
        }

        public static implicit operator MethodArg((ITypeSymbol, string) tuple) => new MethodArg(tuple.Item1, tuple.Item2);
        public static implicit operator MethodArg((RefKind, ITypeSymbol, string) tuple) => new MethodArg(tuple.Item1, tuple.Item2, tuple.Item3);
        public static implicit operator MethodArg((ITypeSymbol, string, RefKind) tuple) => new MethodArg(tuple.Item3, tuple.Item1, tuple.Item2);

        public override string ToString() => Name;
    }

}
