using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    // ref     System.IO.Stream stream
    // RefKind TypeName         Name
    public class MethodArg {

        private string? _paramString;

        public TypeName TypeName { get; }

        public string Name { get; }

        public RefKind RefKind { get; }

        public MethodArg(ITypeSymbol type, string argName) 
            : this(RefKind.None, type, argName) {
        }

        public MethodArg(RefKind refKind, ITypeSymbol type, string name)
            : this(refKind, new TypeName(type), name) {
        }

        public MethodArg(RefKind refKind, TypeName typeName, string name) {
            if (!SyntaxFacts.IsValidIdentifier(name))
                throw new ArgumentException("Invalid method name", nameof(name));
            Name = name;
            RefKind = refKind;
            TypeName = typeName;
        }

        /// <summary>
        /// Creates a clone of this instance and rename it to <paramref name="name"/>
        /// </summary>
        /// <param name="name">The new renaming name</param>
        /// <returns>A new clone for the instance</returns>
        public MethodArg Rename(string name) => new MethodArg(RefKind, TypeName, name);

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
