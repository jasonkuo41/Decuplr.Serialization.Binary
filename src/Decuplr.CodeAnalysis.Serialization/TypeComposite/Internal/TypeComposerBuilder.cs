using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Serialization.LayoutService;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {

    public interface ITypeComposer {
        /// <summary>
        /// The layout that this composer represents
        /// </summary>
        SchemaLayout CompositeType { get; }
        
        /// <summary>
        /// A full referencable name to the composer
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// A short hand name of the composer, namely the type name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The member composers of this type composer
        /// </summary>
        IReadOnlyList<IMemberComposer> MemberComposers { get; }

        /// <summary>
        /// Methods that this signature presents
        /// </summary>
        IReadOnlyList<MethodSignature> Methods { get; }
    }

    public interface IMemberComposer {

    }

    public class MethodSignature {
        /// <summary>
        /// Is the method ref return, we don't support this feature yet
        /// </summary>
        public bool IsRefReturn { get; }

        /// <summary>
        /// Is this method a constructor
        /// </summary>
        public bool IsConstructor { get; }

        /// <summary>
        /// The name of the method, the full name of the constructor if this is a constructor
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// The returning type, maybe be void, null if constructor
        /// </summary>
        public ITypeSymbol? ReturnType { get; }

        /// <summary>
        /// The arguments this method contains
        /// </summary>
        public IReadOnlyList<MethodArg> Arguments { get; }

        private MethodSignature(string methodName, ITypeSymbol? returnType, IEnumerable<MethodArg> args, bool isConstructor, bool isRefReturn) {
            MethodName = methodName;
            ReturnType = returnType;
            IsConstructor = isConstructor;
            IsRefReturn = isRefReturn;
            Arguments = args.ToList();
        }

        public static MethodSignature CreateMethod(string methodName, ITypeSymbol returnType, IEnumerable<MethodArg> args)
            => new MethodSignature(methodName, returnType, args, isConstructor: false, isRefReturn: false);

        public static MethodSignature CreateMethod(string methodName, ITypeSymbol returnType, params MethodArg[] args)
            => CreateMethod(methodName, returnType, args.AsEnumerable());

        public static MethodSignature CreateConstructor(string fulltypeName, IEnumerable<MethodArg> args)
            => new MethodSignature(fulltypeName, null, args, isConstructor: true, isRefReturn: false);

        public static MethodSignature CreateConstructor(string fulltypeName, params MethodArg[] args)
            => CreateConstructor(fulltypeName, args.AsEnumerable());

        private string GetModifierString(int i) {
            if (i > Arguments.Count)
                throw new ArgumentOutOfRangeException();
            var modifier = Arguments[i].Modifier;
            if (modifier == MethodArgModifier.None)
                return string.Empty;
            return $"{modifier.ToString().ToLowerInvariant()} ";
        }

        /// <summary>
        /// Gets the invocation string, without target object
        /// </summary>
        /// <remarks>
        /// For example for method : <i>int MyMethod(int value, in double data)</i>, this returns : <i>MyMethod(a, in b)</i>
        /// . For constructors, this returns <i>new MyMethod(a, in b)</i>
        /// </remarks>
        /// <returns></returns>
        public string GetInvocationString(IEnumerable<string> argumentNames) {
            var builder = new StringBuilder();
            if (IsConstructor)
                builder.Append("new ");
            builder.Append(MethodName);
            builder.Append('(');
            builder.Append(string.Join(",", GetArgName(argumentNames)));
            builder.Append(')');
            return builder.ToString();

            IEnumerable<string> GetArgName(IEnumerable<string> argNames) {
                var i = 0;
                foreach(var arg in argNames) {
                    yield return $"{GetModifierString(i)}{arg}";
                    ++i;
                }
            }
        }

        public string CreateMethodHeader(Accessibility accessibility) {

        }
    }

    public readonly struct MethodArg {
        public ITypeSymbol Type { get; }
        public string ArgName { get; }
        public MethodArgModifier Modifier { get; }
        public MethodArg(ITypeSymbol type, string argName) : this (type, argName, MethodArgModifier.None) {
            Type = type;
            ArgName = argName;
        }

        public MethodArg(ITypeSymbol type, string argName, MethodArgModifier modifier) {
            Type = type;
            ArgName = argName;
            Modifier = modifier;
        }

        public static implicit operator MethodArg((ITypeSymbol, string) tuple) => new MethodArg(tuple.Item1, tuple.Item2);
        public static implicit operator MethodArg((MethodArgModifier, ITypeSymbol, string) tuple) => new MethodArg(tuple.Item2, tuple.Item3, tuple.Item1);
        public static implicit operator MethodArg((ITypeSymbol, string, MethodArgModifier) tuple) => new MethodArg(tuple.Item1, tuple.Item2, tuple.Item3);
    }

    public enum MethodArgModifier {
        None = 0,
        Ref = 1,
        In = 2,
        Out = 3
    }
}

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {

    internal class TypeComposer : ITypeComposer {
        public SchemaLayout CompositeType { get; }

        public string FullName { get; }
    }

    internal class TypeComposerBuilder {

        private static class Property {
            public static string MemberName(int index) => $"Member_{index}";
        }

        public const string DefaultNamespace = "Decuplr.Serialization.Internal.Parsers";

        private const string discovery = "discovery";
        private const string isSuccess = "isSuccess";

        private readonly SchemaLayout _type;
        private readonly ISourceAddition _sourceAddition;
        private readonly ITypeSymbolProvider _symbolSource;
        private readonly IEnumerable<IConditionResolverProvider> _resolvers;
        private readonly IEnumerable<IMemberDataFormatterProvider> _formatter;

        public TypeComposerBuilder(SchemaLayout layout, ISourceAddition sourceAddition, ITypeSymbolProvider symbolProvider, IEnumerable<IConditionResolverProvider> resolvers, IEnumerable<IMemberDataFormatterProvider> memberFormatter) {
            _type = layout;
            _resolvers = resolvers;
            _formatter = memberFormatter;
            _sourceAddition = sourceAddition;
            _symbolSource = symbolProvider;
        }

        private MethodSignature NonTryPattern(string fullName, IComponentProvider provider) 
            => MethodSignature.CreateConstructor(fullName, (provider.DiscoveryType, discovery));

        private MethodSignature TryPattern(string fullName, IComponentProvider provider) 
            => MethodSignature.CreateConstructor(fullName, (provider.DiscoveryType, discovery), (MethodArgModifier.Out, _symbolSource.GetSymbol<bool>(), isSuccess));

        public ITypeComposer Build(string typeComposerNamespace, IComponentProvider provider) => Build(typeComposerNamespace, _type.Type.UniqueName, provider);
        public ITypeComposer Build(IComponentProvider provider) => Build("Decuplr.Serialization.Internal.Parsers", provider);
        public ITypeComposer Build(string typeComposerNamespace, string typeComposerName, IComponentProvider provider) {

            var composers = _type.Members.Select(member => new MemberComposerBuilder(member, _resolvers, _formatter).CreateStruct(provider)).ToList();

            var builder = new CodeSourceFileBuilder(typeComposerNamespace);
            builder.Using("System");

            builder.DenoteHideEditor();
            builder.DenoteGenerated(typeof(TypeComposerBuilder).Assembly);
            builder.AddNode($"internal readonly struct {typeComposerName}", node => {
                for (var i = 0; i < composers.Count; ++i) {
                    node.State($"public {composers[i].Name} {Property.MemberName(i)} {{ get; }}");
                }

                node.NewLine();
                node.Comment("Non-try Pattern");
                node.AddNode($"public {typeComposerName} ({provider.DiscoveryType} {discovery})", node => {
                    for (var i = 0; i < composers.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {composers[i].Name} ({discovery})");
                    }
                }).NewLine();

                node.Comment("Try Pattern");
                node.AddNode($"public {typeComposerName} ({provider.DiscoveryType} {discovery}, out bool {isSuccess})", node => {
                    for (var i = 0; i < composers.Count; ++i) {
                        node.State($"{Property.MemberName(i)} = new {composers[i].Name} ({discovery}, out {isSuccess})");
                        node.If($"!{isSuccess}", node => {
                            node.Return();
                        }).NewLine();
                    }
                });
            });

            _sourceAddition.AddSource($"{typeComposerName.Replace('.', '_')}_{typeComposerName}.cs", builder.ToString());

            return new TypeComposer(typeComposerNamespace, typeComposerName);
        }
    }
}