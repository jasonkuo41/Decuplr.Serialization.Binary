using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    public class MethodSignatureBuilder {

        private readonly Accessibility _accessibility;
        private readonly string _methodName;
        private readonly TypeName _containingTypeName;
        private readonly List<MethodGenericInfo> _generics = new List<MethodGenericInfo>();
        private readonly List<MethodArg> _arguments = new List<MethodArg>();

        private MethodSignatureBuilder(TypeName fullTypeName, string methodName) {
            if (!SyntaxFacts.IsValidIdentifier(methodName))
                throw new ArgumentException($"Invalid method name '{methodName}'.", nameof(methodName));
            _methodName = methodName;
            _containingTypeName = fullTypeName;
            _accessibility = Accessibility.Public;
        }

        private void EnsureNoDuplicateArgName(string argName) {
            var duplicateName = _arguments.Where(x => x.Name.Equals(argName)).FirstOrDefault();
            if (duplicateName is { })
                throw new ArgumentException($"Method has already included a argument name : '{duplicateName}'", nameof(argName));
        }

        private void EnsureNoDuplicateGenericName(string genericName, string passedName) {
            var duplicateName = _generics.Where(x => x.GenericName.Equals(genericName)).Select(x => (MethodGenericInfo?)x).FirstOrDefault();
            if (duplicateName is { })
                throw new ArgumentException($"Method has already include a generic name : '{duplicateName}'", passedName);
        }

        private void EnsureValidName(string name, string namesrc) {
            if (!SyntaxFacts.IsValidIdentifier(name))
                throw new ArgumentException($"'{name}' is not a valid identifier.", namesrc);
        }

        public static MethodSignatureBuilder CreateMethod(INamedTypeSymbol type, string methodName)
            => new MethodSignatureBuilder(new TypeName(type), methodName);

        public static MethodSignatureBuilder CreateMethod(TypeName typeName, string methodName)
            => new MethodSignatureBuilder(typeName, methodName);

        public static MethodSignature CreateConstructor(TypeName typeName, IEnumerable<MethodArg> args)
            => CreateConstructor(Accessibility.Public, typeName, args.AsEnumerable());
        public static MethodSignature CreateConstructor(TypeName typeName, params MethodArg[] args)
            => CreateConstructor(Accessibility.Public, typeName, args.AsEnumerable());

        public static MethodSignature CreateConstructor(Accessibility accessibility, TypeName typeName, params MethodArg[] args)
            => CreateConstructor(accessibility, typeName, args.AsEnumerable());

        public static MethodSignature CreateConstructor(Accessibility accessibility, TypeName typeName, IEnumerable<MethodArg> args) {
            if (args.Any(x => x.TypeName.IsGeneric))
                throw new ArgumentException("Arguments should not contain any generic arguments for constructors", nameof(args));
            // Ensure no duplicates
            var duplicateArgs = args.GroupBy(x => x.Name).Where(x => x.Count() > 1).Select(x => x.Key);
            if (duplicateArgs.Any())
                throw new ArgumentException($"Constructor has duplicate argument names : '{string.Join(",", duplicateArgs)}'", nameof(args));
            return new MethodSignature(accessibility, RefKind.None, null, Enumerable.Empty<MethodGenericInfo>(), typeName.ToString(), args, isConstructor: true);
        }

        public MethodSignatureBuilder AddGenerics(string genericName) {
            EnsureNoDuplicateGenericName(genericName, nameof(genericName));
            EnsureValidName(genericName, nameof(genericName));
            _generics.Add(new MethodGenericInfo(genericName));
            return this;
        }

        public MethodSignatureBuilder AddGenerics(string genericName, params TypeName[] constrainedType) {
            EnsureNoDuplicateGenericName(genericName, nameof(genericName));
            EnsureValidName(genericName, nameof(genericName));
            _generics.Add(new MethodGenericInfo(genericName, constrainedType.Distinct()));
            return this;
        }

        public MethodSignatureBuilder AddGenerics(string genericName, TypeKind constrainedKind, params TypeName[] constrainedType) {
            EnsureNoDuplicateGenericName(genericName, nameof(genericName));
            EnsureValidName(genericName, nameof(genericName));
            if (constrainedKind != TypeKind.Class && constrainedKind != TypeKind.Struct)
                throw new ArgumentException("Constrained Type can only be class or struct");
            _generics.Add(new MethodGenericInfo(genericName, constrainedKind, constrainedType.Distinct()));
            return this;
        }

        public MethodSignatureBuilder AddArgument(MethodArg arg) {
            EnsureNoDuplicateArgName(arg.Name);
            _arguments.Add(arg);
            return this;
        }

        public MethodSignatureBuilder AddArgument(params MethodArg[] args) {
            foreach (var arg in args)
                AddArgument(arg);
            return this;
        }

        public MethodSignature WithReturn(ITypeSymbol symbol) => WithReturn(RefKind.None, symbol);

        public MethodSignature WithReturn(RefKind refKind, ITypeSymbol symbol) {
            // Ensure arguments are correct
            var genericSet = new HashSet<string>(_generics.Select(x => x.GenericName));
            var notContainedGeneric = _arguments.Where(x => x.TypeName.IsGeneric).Where(arg => !genericSet.Contains(arg.TypeName));
            if (notContainedGeneric.Any())
                throw new ArgumentException($"Arguments contain generic that is not in the generic list : '{string.Join(",", notContainedGeneric)}'");
            return new MethodSignature(_accessibility, refKind, symbol, _generics, _methodName, _arguments, false);
        }
    }

}
