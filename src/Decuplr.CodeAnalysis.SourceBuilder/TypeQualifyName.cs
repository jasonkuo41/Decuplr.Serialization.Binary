using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Serialization.SourceBuilder {

    public readonly struct TypeQualifyName {
        public bool IsEmpty => TypeName is null;
        public string Namespace { get; }
        public string[] ParentNames { get; }
        public string TypeName { get; }

        public TypeQualifyName(string qualifyName) {
            if (qualifyName.StartsWith("."))
        }

        public TypeQualifyName(string namespaceName, string typeName) {
            if (namespaceName.StartsWith(".") || namespaceName.EndsWith(".") || namespaceName.Split('.').Any(x => !SyntaxFacts.IsValidIdentifier(x)))
                throw new ArgumentException($"Invalid namespace '{namespaceName}'", nameof(namespaceName));
            Namespace = namespaceName;
            if (!SyntaxFacts.IsValidIdentifier(typeName))
                throw new ArgumentException($"Invalid type name {typeName}");
            TypeName = typeName;
            ParentNames = Array.Empty<string>();
        }

        public TypeQualifyName(string namespaceName, string parentNames, string typeName)
            : this(namespaceName, typeName) {
            ParentNames = parentNames.Split('.');
            if (parentNames.StartsWith(".") || ParentNames.Any(x => !SyntaxFacts.IsValidIdentifier(x)))
                throw new ArgumentException("Invalid parent type name", nameof(parentNames));
        }

        public TypeQualifyName(string namespaceName, string[] parentNames, string typeName)
            : this(namespaceName, typeName) {
            ParentNames = parentNames;
            if (ParentNames.Any(x => !SyntaxFacts.IsValidIdentifier(x)))
                throw new ArgumentException("Invalid parent type name", nameof(parentNames));
        }

        public override string ToString() {
            if (ParentNames.Length == 0)
                return $"{Namespace}.{TypeName}";
            return $"{Namespace}.{string.Join(".", ParentNames)}.{TypeName}";
        }
    }

}
