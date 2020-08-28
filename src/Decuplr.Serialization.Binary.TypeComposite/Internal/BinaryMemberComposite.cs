using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Decuplr.Serialization.Namespaces;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal {
    internal class BinaryMemberComposite {

        private static class MemberName {
            public static string ComponentField(int i) => $"_component{i}";
            public static string InitializeComponentMethod(int i) => $"InitializeComponent{i}";
            public static string Serialize(int? i) => i.HasValue ? $"SerializeState{i.Value}" : $"Serialize";
        }

        private readonly MemberMetaInfo _member;
        private readonly GeneratingTypeName _typeName;
        private readonly IReadOnlyList<IBinaryMemberComposeMethod> _features;
        private readonly ITypeSymbolProvider _symbolProvider;
        private readonly ISourceAddition _sourceAddition;
        private readonly IReadOnlyList<string> _normalNamespaces;
        private readonly IReadOnlyList<string> _prioritizedNamespaces;
        private readonly MemberComponentCollection _memberComponents = new MemberComponentCollection();
        private readonly ThrowCollection _throwCollection = new ThrowCollection("ThrowHelper");

        public BinaryMemberComposite(MemberMetaInfo member,
                                     GeneratingTypeName typeName,
                                     IMemberFormatNamespaceProvider namespaceProvider,
                                     IEnumerable<IBinaryMemberFeatureProvider> featureProviders,
                                     ITypeSymbolProvider symbolProvider,
                                     ISourceAddition sourceAddition) {
            _member = member;
            _typeName = typeName;
            _symbolProvider = symbolProvider;
            _sourceAddition = sourceAddition;
            _features = featureProviders.Where(feature => feature.ShouldApply(member))
                                        .Select(feature => feature.GetComposingMethods(member, _memberComponents, _throwCollection))
                                        .ToList();

            var namespaceInfo = namespaceProvider.GetUsingNamespaces(member);
            _normalNamespaces = namespaceInfo.WithNamespaces.ToList();
            _prioritizedNamespaces = namespaceInfo.WithPrioritizedNamespaces.ToList();

            Debug.Assert(_member.ReturnType != null);
        }

        public void CreateStruct() {
            var builder = new CodeSourceFileBuilder(_typeName.Namespace);
            var genericArgs = _member.ReturnType!.Symbol is ITypeParameterSymbol symbol ? $"<{symbol}>" : "";
            var components = _memberComponents.Components;

            builder.AttributeHideEditor().AttributeGenerated(typeof(BinaryMemberComposite).Assembly);
            builder.NestType(_typeName, $"internal readonly struct {_typeName.TypeName} {genericArgs}", node => {

                builder.NewLine().Comment("Component Fields");
                for (var i = 0; i < components.Count; ++i)
                    builder.State($"private {components[i]} {MemberName.ComponentField(i)}");

                builder.NewLine().Comment("Member Composer Constructor");
                {
                    const string parser = "parser";
                    builder.AddNode($"internal {_typeName.TypeName} ({typeof(IBinaryNamespaceDiscovery).FullName} {parser})", node => {
                        // Get's the namespace for us
                        node.State($"{parser} = {parser}.{nameof(INamespaceDiscovery.WithNamespace)}({JoinedNamespaces(_normalNamespaces)})" +
                                                      $".{nameof(INamespaceDiscovery.WithPrioritizedNamespace)}({JoinedNamespaces(_prioritizedNamespaces)})" +
                                                      $"as {typeof(IBinaryNamespaceDiscovery).FullName}");
                        for (var i = 0; i < components.Count; ++i) {
                            node.State($"{MemberName.ComponentField(i)} = {parser}.{nameof(IBinaryNamespaceDiscovery.GetConverter)}<{components[i]}>()");
                        }
                    });

                    static string JoinedNamespaces(IEnumerable<string> namespaces) => $"new string[] {{ { string.Join(", ", namespaces.Select(n => $@"""{n}""")) } }}";
                }


                // void Serialize<TState, TWriter>(in T item, in TState state, ref TWriter writer);
                // Optimization point : If it's anything smaller then 8 bytes, we don't pass the member value as `in`
                builder.NewLine().Comment($"Serialize Buffer Writer");
                {
                    var serializeWriter = new SerializeWriterChainedMethods(_typeName, _member, true);

                    builder.AttributeMethodImpl(MethodImplOptions.AggressiveInlining);
                    builder.AddNode(serializeWriter.MethodSignature.GetDeclarationString(), node => {
                        const string nextState = "nextState";

                        var tstateTypeName = TypeName.FromGenericArgument(SerializeWriterChainedMethods.T_STATE);
                        var state = serializeWriter[tstateTypeName];
                        var tsource = serializeWriter[TypeName.FromType(_member.ContainingFullType.Symbol)];

                        node.If($"!{state}.Write({tsource}, out var {nextState})", node => {
                            node.Return();
                        });

                        if (!isConstant) {
                            var nextMethod = serializeWriter.InvokeNextMethod(args => {
                                args[tstateTypeName] = nextState;
                            });
                            node.State(nextMethod);
                            return;
                        }
                        // If the method is constant we can simply advance and use Span method instead
                        var writer = serializeWriter[TypeName.FromGenericArgument(SerializeWriterChainedMethods.T_WRITER)];
                        node.State($"var writeSpan = {writer}.GetSpan({constLength})");
                        node.State($"{writer}.Advance({MemberName.Serialize(0)}({}, {}, {}, {}))"); // Call Serialize Span<byte> index 0 method
                    });

                    for(var i = 0; i < _features.Count; ++i) {
                        if (serializeWriter.HasChainedMethodInvoked) {
                            serializeWriter = serializeWriter.MoveNext(i == _features.Count - 1); // Check if it's the last feature available
                            _features[i].SerializeWriter(builder, serializeWriter);
                        }
                    }
                }

                // int Serialize<TState>(in T item, in TState writeState, Span<byte> data);

                // int Deserialize(in __memberType member, in __type source, ReadOnlySpan<byte> data, out T result)

                // bool Deserialize(in __memberType member, in __type source, ref SequenceCursor<byte> cursor, out T result)

                // int GetSpanLength(in __memberType member, in __type source)

                // int GetBlockLength(ReadOnlySpan<byte> data);

                // int GetBlockLength(ref SequenceCursor<byte> cursor);
            });
        }
    }

    internal abstract class ChainedMethods<T> : IChainedMethods where T : ChainedMethods<T> {

        private readonly bool _hasChainedMethod;
        private readonly int? _currentIndex;
        private readonly Func<int?, MethodSignature> _nextMethod;

        public MethodSignature MethodSignature { get; }
        
        public MethodArg this[TypeName typeName] => MethodSignature.Arguments.First(x => x.TypeName.Equals(typeName));

        public MethodArg this[TypeName typeName, int index] => MethodSignature.Arguments.ElementAt(index);

        public IReadOnlyList<MethodTypeParams> TypeParameters => MethodSignature.TypeParameters;

        public IReadOnlyList<MethodArg> Arguments => MethodSignature.Arguments;

        public bool HasChainedMethodInvoked { get; private set; }

        bool IChainedMethods.HasChainedMethod => _hasChainedMethod;

        public ChainedMethods(Func<int?, MethodSignature> nextMethodSignature) {
            _hasChainedMethod = true;
            _currentIndex = null;
            _nextMethod = nextMethodSignature;
            MethodSignature = nextMethodSignature(null);
        }

        protected ChainedMethods(T sourceMethods, bool hasNextMethod) {
            _hasChainedMethod = hasNextMethod;
            _currentIndex = sourceMethods._currentIndex.HasValue ? sourceMethods._currentIndex + 1 : 0;
            _nextMethod = sourceMethods._nextMethod;
            MethodSignature = _nextMethod(_currentIndex);
        }

        private void EnsureHasChainedMethod() {
            if (!_hasChainedMethod)
                throw new InvalidOperationException("There's no chained method");
        }

        private string InvokeNextMethodCore(Action<IChainMethodInvokeAction>? action) {

        }

        protected abstract T MoveNext(T sourceMethod, bool hasNextMethod);

        public string InvokeNextMethod() => InvokeNextMethodCore(null);

        public string InvokeNextMethod(Action<IChainMethodInvokeAction> action) => InvokeNextMethodCore(action);

        public T MoveNext(bool hasNextMethod) {
            var us = this as T;
            Debug.Assert(us is { });
            return MoveNext(us, hasNextMethod);
        }
    }

    internal class SerializeWriterChainedMethods : ChainedMethods<SerializeWriterChainedMethods>, IChainedMethods {

        public const string T_STATE = "TState";
        public const string T_WRITER = "TWriter";

        public static MethodSignature CreateMethodSignature(TypeName memberCompositeName, MemberMetaInfo member, string methodName) {
            Debug.Assert(member.ReturnType != null);
            return MethodSignatureBuilder.CreateMethod(memberCompositeName, methodName)
                                         .AddGenerics(T_STATE, GenericConstrainKind.Struct, new TypeName("Decuplr.Serialization.Binary", "IBinaryWriteState<TState>"))
                                         .AddGenerics(T_WRITER, GenericConstrainKind.Struct, TypeName.FromType<IBufferWriter<byte>>())
                                         .AddArgument((RefKind.In, member.ReturnType.Symbol, "member"))
                                         .AddArgument((RefKind.In, member.ContainingFullType.Symbol, "source"))
                                         .AddArgument((TypeName.FromGenericArgument(T_STATE), "state"))
                                         .AddArgument((RefKind.Ref, TypeName.FromGenericArgument(T_WRITER), "writer"))
                                         .WithReturn(TypeName.Void);
        }

        public SerializeWriterChainedMethods(TypeName memberCompositeName, MemberMetaInfo member)
                : base(id => CreateMethodSignature(memberCompositeName, member, $"Serialize{(id.HasValue ? $"State{id}" : "")}")) {
        }

        private SerializeWriterChainedMethods(SerializeWriterChainedMethods sourceMethod, bool hasNextMethod)
            : base(sourceMethod, hasNextMethod) { }

        protected override SerializeWriterChainedMethods MoveNext(SerializeWriterChainedMethods sourceMethod, bool hasNextMethod) {
            throw new NotImplementedException();
        }
    }
}
