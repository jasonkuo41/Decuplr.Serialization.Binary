using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Decuplr.Serialization.CodeGeneration.TypeComposers;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    class ThrowCollection : IThrowCollection {

        private readonly List<string> exceptions = new List<string>();
        private readonly string _className;

        private static string ThrowExceptionName(int count) => $"ThrowException_{count}";

        public ThrowCollection(string className) {
            _className = className;
        }

        public void AddThrowClass(CodeNodeBuilder builder, Accessibility accessbility = Accessibility.Private, bool shouldShowInEditor = false) {
            var accessStr = accessbility.ToString().ToLower();
            if (!shouldShowInEditor)
                builder.DenoteHideEditor();
            builder.AddNode($"{accessStr} static class {_className} ", node => {
                for (var i = 0; i < exceptions.Count; i++) {
                    node.Attribute("MethodImpl(MethodImplOptions.NoInlining)");
                    node.State($"public static {ThrowExceptionName(i)}() => throw {exceptions[i]}");
                }
            });
        }

        public string AddException(Expression<Func<Exception>> expression) {
            var body = (expression.Body as NewExpression)?.ToString() ?? throw new ArgumentException("Invalid expression for exception");
            exceptions.Add(body);
            return $"{_className}.{ThrowExceptionName(exceptions.Count - 1)}()";
        }
    }
}
