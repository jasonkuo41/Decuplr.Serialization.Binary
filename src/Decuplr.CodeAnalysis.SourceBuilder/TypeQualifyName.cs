﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Serialization.SourceBuilder {

    public readonly struct TypeQualifyName {

        private readonly string _typeName;
        private readonly string _namespace;
        private readonly string[] _parentNames;

        public bool IsEmpty => _typeName is null;

        public string Namespace => _namespace ?? string.Empty;

        public IReadOnlyList<string> ParentNames => _parentNames ?? Array.Empty<string>();

        public string TypeName => _typeName ?? string.Empty;

        internal static string[] VerifyChainedIndentifier(string fullTypeName, string argName) {
            var sliced = fullTypeName.Split('.');
            if (sliced.Any(x => !SyntaxFacts.IsValidIdentifier(x)))
                throw new ArgumentException($"Invalid type identification name : {fullTypeName}", argName);
            return sliced;
        }

        internal TypeQualifyName(GeneratingTypeName typeName) {
            _namespace = typeName.Namespace;
            _typeName = typeName.TypeName;
            _parentNames = typeName.Parents.Select(x => x.ParentName).ToArray();
        }

        public TypeQualifyName(string namespaceName, string typeName) {
            VerifyChainedIndentifier(namespaceName, nameof(namespaceName));
            var slicedTypeName = VerifyChainedIndentifier(typeName, nameof(typeName));
            _namespace = namespaceName;
            _typeName = slicedTypeName[slicedTypeName.Length - 1];

            Array.Resize(ref slicedTypeName, slicedTypeName.Length - 1);
            _parentNames = slicedTypeName;
        }

        public TypeQualifyName(string namespaceName, string parentNames, string typeName)
            : this(namespaceName, typeName) {
            _parentNames = VerifyChainedIndentifier(parentNames, nameof(parentNames));
        }

        public TypeQualifyName(string namespaceName, string[] parentNames, string typeName)
            : this(namespaceName, typeName) {
            _parentNames = parentNames;
            if (_parentNames.Any(x => !SyntaxFacts.IsValidIdentifier(x)))
                throw new ArgumentException("Invalid parent type name", nameof(parentNames));
        }

        public string GetFullName() {
            if (ParentNames.Count == 0)
                return $"{Namespace}.{TypeName}";
            return $"{Namespace}.{string.Join(".", ParentNames)}.{TypeName}";
        }

        public override string ToString() => GetFullName();
    }
}