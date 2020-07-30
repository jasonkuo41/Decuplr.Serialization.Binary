using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Serialization.SourceBuilder {
    public struct GeneratingTypeName {

        private readonly string _namespace;
        private readonly string _typeName;
        private readonly ParentTypeNameInfo[]? _parents;

        public bool Empty => _typeName is null;
        public string Namespace => _namespace ?? string.Empty;

        public IReadOnlyList<ParentTypeNameInfo> Parents => _parents ?? Array.Empty<ParentTypeNameInfo>();

        public string TypeName => _typeName ?? string.Empty;

        public GeneratingTypeName(string namespaceName, string typeName) {
            if (!SyntaxFacts.IsValidIdentifier(typeName))
                throw new ArgumentException($"Invalid type name '{typeName}'");
            TypeQualifyName.VerifyChainedIndentifier(namespaceName, nameof(namespaceName));
            _namespace = namespaceName;
            _typeName = typeName;
            _parents = null;
        }

        public GeneratingTypeName(string namespaceName, string typeName, params ParentTypeNameInfo[] parentName)
            : this (namespaceName, typeName) {
            if (parentName.Any(x => x.Empty))
                throw new ArgumentException("Cannot have a empty parent name");
            _parents = parentName;
        }

        public TypeQualifyName GetQualifyName() => new TypeQualifyName(this);

        public static implicit operator TypeQualifyName(GeneratingTypeName typeName) => typeName.GetQualifyName();

        public override string ToString() {
            if (Empty)
                return "";
            if (Parents.Count == 0)
                return $"{Namespace}.{TypeName}";
            return $"{Namespace}.{string.Join(".", Parents)}.{TypeName}";
        }
    }
}
