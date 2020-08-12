using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    public class MethodArg {

        private string? _paramString;

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

        public MethodArg(RefKind refKind, TypeQualifyName typeName, string name) {
            if (!SyntaxFacts.IsValidIdentifier(name))
                throw new ArgumentException("Invalid method name", nameof(name));
            Name = name;
            RefKind = refKind;
            TypeName = typeName.ToString();
            BackingQualifyName = typeName;
        }

        public MethodArg Rename(string name) {
            if (BackingSymbol is null)
                return new MethodArg(RefKind, BackingQualifyName, name);
            return new MethodArg(RefKind, BackingSymbol, name);
        }

        public static implicit operator MethodArg((ITypeSymbol, string) tuple) => new MethodArg(tuple.Item1, tuple.Item2);
        public static implicit operator MethodArg((RefKind, ITypeSymbol, string) tuple) => new MethodArg(tuple.Item1, tuple.Item2, tuple.Item3);
        public static implicit operator MethodArg((ITypeSymbol, string, RefKind) tuple) => new MethodArg(tuple.Item3, tuple.Item1, tuple.Item2);

        public override string ToString() => Name;
        public string ToParamString() {
            return _paramString ??= CreateParamString();

            string CreateParamString() {
                var str = new StringBuilder();
                str.Append(RefKind switch
                {
                    RefKind.Ref => "ref ",
                    RefKind.Out => "out ",
                    RefKind.In => "in ",
                    _ => ""
                });
                str.Append(TypeName);
                str.Append(' ');
                str.Append(Name);
                return str.ToString();
            }
        }
    }

}
