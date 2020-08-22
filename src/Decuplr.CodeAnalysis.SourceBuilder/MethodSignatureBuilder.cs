﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    public class MethodSignatureBuilder {

        private readonly Accessibility _accessibility;
        private readonly string _methodName;
        private readonly string _fullTypeName;
        private readonly List<MethodGenericInfo> _generics = new List<MethodGenericInfo>();
        private readonly List<MethodArg> _arguments = new List<MethodArg>();

        private MethodSignatureBuilder(string fullTypeName, string methodName) {
            if (!SyntaxFacts.IsValidIdentifier(methodName))
                throw new ArgumentException($"Invalid method name '{methodName}'.", nameof(methodName));
            _methodName = methodName;
            _fullTypeName = fullTypeName;
            _accessibility = Accessibility.Public;
        }

        public static MethodSignatureBuilder CreateMethod(INamedTypeSymbol type, string methodName)
            => new MethodSignatureBuilder(type.ToString(), methodName);

        public static MethodSignatureBuilder CreateMethod(TypeName typeName, string methodName)
            => new MethodSignatureBuilder(typeName.ToString(), methodName);

        public static MethodSignature CreateConstructor(TypeName typeName, IEnumerable<MethodArg> args)
            => CreateConstructor(Accessibility.Public, typeName, args.AsEnumerable());
        public static MethodSignature CreateConstructor(TypeName typeName, params MethodArg[] args)
            => CreateConstructor(Accessibility.Public, typeName, args.AsEnumerable());

        public static MethodSignature CreateConstructor(Accessibility accessibility, TypeName typeName, params MethodArg[] args)
            => CreateConstructor(accessibility, typeName, args.AsEnumerable());

        public static MethodSignature CreateConstructor(Accessibility accessibility, TypeName typeName, IEnumerable<MethodArg> args) {
            if (args.Any(x => x.TypeName.IsGeneric))
                throw new ArgumentException("Arguments should not contain any generic arguments for constructors", nameof(args));
            return new MethodSignature(accessibility, typeName.ToString(), null, Enumerable.Empty<MethodGenericInfo>(), args, isConstructor: true, RefKind.None);
        }

        public MethodSignatureBuilder AddGenerics(string genericName) {
            _generics.Add(new MethodGenericInfo(genericName));
            return this;
        }

        public MethodSignatureBuilder AddGenerics(string genericName, params TypeName[] constrainedType) {
            _generics.Add(new MethodGenericInfo(genericName, constrainedType));
            return this;
        }

        public MethodSignatureBuilder AddGenerics(string genericName, TypeKind constrainedKind, params TypeName[] constrainedType) {
            if (constrainedKind != TypeKind.Class && constrainedKind != TypeKind.Struct)
                throw new ArgumentException("Constrained Type can only be class or struct");
            _generics.Add(new MethodGenericInfo(genericName, constrainedKind, constrainedType));
            return this;
        }

        public MethodSignatureBuilder AddArgument(MethodArg arg) {
            _arguments.Add(arg);
            return this;
        }

        public MethodSignatureBuilder AddArgument(params MethodArg[] args) {
            _arguments.AddRange(args);
            return this;
        }

        public MethodSignature WithReturn(ITypeSymbol symbol) => WithReturn(RefKind.None, symbol);

        public MethodSignature WithReturn(RefKind refKind, ITypeSymbol symbol) {
            // Ensure arguments are correct
            var genericSet = new HashSet<string>(_generics.Select(x => x.GenericName));
            var notContainedGeneric = _arguments.Where(x => x.TypeName.IsGeneric).Where(arg => !genericSet.Contains(arg.TypeName));
            if (notContainedGeneric.Any())
                throw new ArgumentException($"Arguments contain generic that is not in the generic list : '{string.Join(",", notContainedGeneric)}'");
            return new MethodSignature(_accessibility, _methodName, symbol, _generics, _arguments, false, refKind);
        }
    }

}
