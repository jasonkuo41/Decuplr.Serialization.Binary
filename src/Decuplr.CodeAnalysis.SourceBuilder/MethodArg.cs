using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    // ref     System.IO.Stream stream
    // RefKind TypeName         Name
    public struct MethodArg : IEquatable<MethodArg> {

        // TODO : Add Empty guard!
        public bool IsEmpty => TypeName.IsEmpty && ArgName is null && RefKind == RefKind.None;

        public TypeName TypeName { get; }

        public string ArgName { get; }

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
            ArgName = name;
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
        public static implicit operator MethodArg((TypeName, string) tuple) => new MethodArg(RefKind.None, tuple.Item1, tuple.Item2);
        public static implicit operator MethodArg((RefKind, ITypeSymbol, string) tuple) => new MethodArg(tuple.Item1, tuple.Item2, tuple.Item3);
        public static implicit operator MethodArg((RefKind, TypeName, string) tuple) => new MethodArg(tuple.Item1, tuple.Item2, tuple.Item3);

        public override string ToString() => ArgName;
        public string ToParamString() {
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
            str.Append(ArgName);
            return str.ToString();
        }

        public bool Equals(MethodArg other) => TypeName.Equals(other.TypeName) && ArgName.Equals(other.ArgName) && RefKind == other.RefKind;

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }

}
