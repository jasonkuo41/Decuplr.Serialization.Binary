using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class ChainingMethodProvider : IChainMethodArgsProvider {

        private class ChainMethodInvokeAction : IChainMethodInvokeAction {

            private readonly IReadOnlyDictionary<string, List<(MethodArg Args, int Index)>> _methods;

            public ChainMethodInvokeAction(Dictionary<string, List<(MethodArg, int)>> args) {
                _methods = args;
            }

            public string this[Type type] { get => this[type, 0]; set => this[type, 0] = value; }
            public string this[ITypeSymbol symbol] { get => this[symbol, 0]; set => this[symbol, 0] = value; }
            public string this[Type type, int index] { get => this[type.FullName, index]; set => this[type.FullName, index] = value; }
            public string this[ITypeSymbol symbol, int index] { get => this[symbol.ToString(), index]; set => this[symbol.ToString(), index] = value; }

            private string this[string typeName, int index] {
                get => _methods[typeName][index].Args.Name;
                set {
                    var (args, argIndex) = _methods[typeName][index];
                    args = args.Rename(value);
                    _methods[typeName][index] = (args, argIndex);
                }
            }

            public IReadOnlyList<MethodArg> GetArgsList() => ChainingMethodProvider.GetArgsList(_methods);
        }

        private readonly IReadOnlyDictionary<string, List<(MethodArg Arg, int Index)>> _args;
        private readonly string? _nextMethodName;

        public string this[Type type] => this[type, 0];
        public string this[ITypeSymbol symbol] => this[symbol, 0];
        public string this[Type type, int index] => _args[type.FullName][index].Arg.Name;
        public string this[ITypeSymbol symbol, int index] => _args[symbol.ToString()][index].Arg.Name;

        public bool HasChainedMethod => !(_nextMethodName is null);
        public bool HasInvokedNextMethod { get; private set; }

        public ChainingMethodProvider(IReadOnlyList<MethodArg> args, string? nextMethodName) {
            _args = GetArgs(args).GroupBy(x => x.Args.TypeName).ToDictionary(x => x.Key, x => x.ToList());
            _nextMethodName = nextMethodName;

            static IEnumerable<(MethodArg Args, int Index)> GetArgs(IReadOnlyList<MethodArg> args) {
                for (var i = 0; i < args.Count; ++i)
                    yield return (args[i], i);
            }
        }

        private static IReadOnlyList<MethodArg> GetArgsList(IReadOnlyDictionary<string, List<(MethodArg Args, int Index)>> args) 
            => args.Values.SelectMany(x => x).OrderBy(x => x.Index).Select(x => x.Args).ToList();

        private string GetNextMethodString(IReadOnlyList<MethodArg> argList) => $"{_nextMethodName}({string.Join(",", argList.Select(x => x.ToParamString()))})";

        private void EnsureHasNextMethod() {
            if (!HasChainedMethod)
                throw new InvalidOperationException("Cannot get next method because this is the end of the chain");
        }

        public string InvokeNextMethod() {
            EnsureHasNextMethod();
            HasInvokedNextMethod = true;
            return GetNextMethodString(GetArgsList(_args));
        }

        public string InvokeNextMethod(Action<IChainMethodInvokeAction> action) {
            EnsureHasNextMethod();
            HasInvokedNextMethod = true;
            // Clone our arguments and build an invocation list for the next method
            var args = new ChainMethodInvokeAction(_args.ToDictionary(x => x.Key, x => x.Value.ToList()));
            action.Invoke(args);
            return GetNextMethodString(args.GetArgsList());
        }
    }
}
