using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods;
using Decuplr.Serialization.Namespaces;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal {

    internal class BinaryMemberCompositeBuilderProvider {

        private readonly IMemberFormatNamespaceProvider _namespaceProvider;
        private readonly IEnumerable<IBinaryMemberFeatureProvider> _featureProviders;
        private readonly ISourceAddition _sourceAddition;

        public BinaryMemberCompositeBuilderProvider(IMemberFormatNamespaceProvider namespaceProvider, IEnumerable<IBinaryMemberFeatureProvider> featureProviders, ISourceAddition sourceAddition) {
            _namespaceProvider = namespaceProvider;
            _featureProviders = featureProviders;
            _sourceAddition = sourceAddition;
        }

        public BinaryMemberCompositeStruct AddMemberCompositeStruct(MemberMetaInfo member, GeneratingTypeName typeName, IComponentResolver resolver) {
            return new BinaryMemberCompositeBuilder(member, typeName, _namespaceProvider, _featureProviders, _sourceAddition).AddStructToSource(resolver);
        }
    }


    internal class BinaryMemberCompositeBuilder {

        #region Common Name Definitions
        
        private static class MemberName {
            public static string ComponentField(int i) => $"_component{i}";
            public static string InitializeComponentMethod(int i) => $"InitializeComponent{i}";
            public static string Serialize(int? i) => i.HasValue ? $"SerializeState{i.Value}" : $"Serialize";
            public static string Deserialize(int? i) => i.HasValue ? $"DeserializeState{i.Value}" : $"Deserialize";
            public static string GetSpanLength(int? i) => i.HasValue ? $"GetSpanLengthState{i.Value}" : $"GetSpanLength";
            public static string GetBlockLength(int? i) => i.HasValue ? $"GetBlockLengthState{i.Value}" : $"GetBlockLength";
        }
        
        #endregion

        private static readonly TypeName TStateTypeName = TypeName.FromGenericArgument(SerializeWriterChainedMethods.T_STATE);

        private readonly MemberMetaInfo _member;
        private readonly GeneratingTypeName _typeName;
        private readonly IReadOnlyList<IBinaryMemberComposeMethod> _features;
        private readonly ISourceAddition _sourceAddition;
        private readonly IReadOnlyList<string> _normalNamespaces;
        private readonly IReadOnlyList<string> _prioritizedNamespaces;
        private readonly MemberComponentCollection _memberComponents = new MemberComponentCollection();
        private readonly ThrowCollection _throwCollection = new ThrowCollection("ThrowHelper");

        #region Constructor
        public BinaryMemberCompositeBuilder(MemberMetaInfo member,
                                            GeneratingTypeName typeName,
                                            IMemberFormatNamespaceProvider namespaceProvider,
                                            IEnumerable<IBinaryMemberFeatureProvider> featureProviders,
                                            ISourceAddition sourceAddition) {
            _member = member;
            _typeName = typeName;
            _sourceAddition = sourceAddition;
            _features = featureProviders.Where(feature => feature.ShouldApply(member))
                                        .Select(feature => feature.GetComposingMethods(member, _memberComponents, _throwCollection))
                                        .ToList();

            var namespaceInfo = namespaceProvider.GetUsingNamespaces(member);
            _normalNamespaces = namespaceInfo.WithNamespaces.ToList();
            _prioritizedNamespaces = namespaceInfo.WithPrioritizedNamespaces.ToList();

            Debug.Assert(_member.ReturnType != null);
        }
        #endregion

        private CodeNodeBuilder UseChainedMethods<TChainedMethod>(CodeNodeBuilder builder, TChainedMethod chainedMethod, List<MethodSignature> methods, Action<CodeNodeBuilder> node) where TChainedMethod : MemberChainedMethods<TChainedMethod> {

            builder.AttributeMethodImpl(MethodImplOptions.AggressiveInlining);
            builder.AddMethod(chainedMethod.MethodSignature, node);

            // Maybe we don't need to generate these methods when it's constant length
            for (var i = 0; i < _features.Count; ++i) {
                if (!chainedMethod.HasChainedMethodInvoked)
                    return builder;
                var shouldMoveNext = i == _features.Count - 1; // Check if it's the last feature available
                chainedMethod = chainedMethod.MoveNext(shouldMoveNext);

                builder.AttributeMethodImpl(MethodImplOptions.AggressiveInlining);
                builder.AddMethod(chainedMethod.MethodSignature, node => _features[i].SerializeWriter(node, chainedMethod));
            }

            methods.Add(chainedMethod.MethodSignature);
            return builder;
        }

        /// <returns>Next state arg name</returns>
        private string LoadNextWriteState<TChainedMethod>(CodeNodeBuilder node, TChainedMethod chainedMethod) where TChainedMethod : MemberChainedMethods<TChainedMethod> {
            const string nextState = "nextState";

            var tstateTypeName = TypeName.FromGenericArgument(SerializeWriterChainedMethods.T_STATE);
            var state = chainedMethod[tstateTypeName];
            var tsource = chainedMethod[TypeName.FromType(_member.ContainingFullType.Symbol)];

            node.If($"!{state}.Write({tsource}, out var {nextState})", node => {
                node.Return();
            });

            return nextState;
        }

        private int? GetConstantLength() {
            var lengthInfo = ConstantLengthInfo.NotModified;
            foreach (var feature in _features)
                lengthInfo += feature.ConstantLengthInfo;
            if (!lengthInfo.IsConstant)
                return null;
            if (lengthInfo.AbsoluteLength >= 0)
                return lengthInfo.AbsoluteLength;
            // Normally at this point we have to determinate the length
            // If we don't then we assume we don't know the length of this member
            return null;
        }

        public BinaryMemberCompositeStruct AddStructToSource(IComponentResolver resolver) {
            var builder = new CodeSourceFileBuilder(_typeName.Namespace);
            var genericArgs = _member.ReturnType!.Symbol is ITypeParameterSymbol symbol ? $"<{symbol}>" : "";
            var components = _memberComponents.Components;
            var relyingMembers = _features.SelectMany(x => x.RelyingMembers).ToList();
            var constLength = GetConstantLength();
            var methods = new List<MethodSignature>();

            builder.AttributeHideEditor().AttributeGenerated(typeof(BinaryMemberCompositeBuilder).Assembly);
            builder.NestType(_typeName, $"internal readonly struct {_typeName.TypeName} {genericArgs}", node => {

                node.NewLine().Comment("Component Fields");
                for (var i = 0; i < components.Count; ++i)
                    node.State($"private {components[i]} {MemberName.ComponentField(i)}");

                node.Comment("Member Composer Constructor");
                {
                    const string parser = "parser";
                    node.AddNode($"internal {_typeName.TypeName} ({typeof(IBinaryNamespaceDiscovery).FullName} {parser})", node => {
                        // Get's the namespace for us
                        node.State($"{parser} = {parser}.{nameof(INamespaceDiscovery.WithNamespace)}({JoinedNamespaces(_normalNamespaces)})" +
                                                      $".{nameof(INamespaceDiscovery.WithPrioritizedNamespace)}({JoinedNamespaces(_prioritizedNamespaces)})" +
                                                      $"as {typeof(IBinaryNamespaceDiscovery).FullName}");
                        for (var i = 0; i < components.Count; ++i) {
                            node.State($"{MemberName.ComponentField(i)} = {parser}.{nameof(IBinaryNamespaceDiscovery.GetConverter)}<{components[i]}>()");
                        }
                    }).NewLine();

                    static string JoinedNamespaces(IEnumerable<string> namespaces) => $"new string[] {{ { string.Join(", ", namespaces.Select(n => $@"""{n}""")) } }}";
                }

                // void Serialize<TState, TWriter>(in T item, in TState state, ref TWriter writer);
                // Optimization point : If it's anything smaller then 8 bytes, we don't pass the member value as `in`
                node.Comment($"Serialize Buffer Writer");
                {
                    var serializeWriter = new SerializeWriterChainedMethods(_typeName, _member, MemberName.Serialize);
                    UseChainedMethods(builder, serializeWriter, methods, node => {
                        var nextState = LoadNextWriteState(node, serializeWriter);

                        if (!constLength.HasValue) {
                            node.State(serializeWriter.InvokeNextMethod(args => args[TStateTypeName] = nextState));
                            return;
                        }

                        // If the method is constant we can simply advance and use Span method instead
                        const string writeSpan = "writeSpan";
                        var writer = serializeWriter[TypeName.FromGenericArgument(SerializeWriterChainedMethods.T_WRITER)];
                        var serializeSpanMethod = SerializerSpanChainedMethods.CreateMethodSignature(_typeName, _member, MemberName.Serialize(0));
                        var nextInvokeStr = serializeSpanMethod.GetInvocationString(
                            new[] { SerializeWriterChainedMethods.T_STATE },
                            new[] {
                                serializeWriter[TypeName.FromType(_member.ReturnType.Symbol)].ArgName,           // member
                                serializeWriter[TypeName.FromType(_member.ContainingFullType.Symbol)].ArgName,   // source
                                serializeWriter[TStateTypeName].ArgName,                                         // writeState
                                writeSpan                                                                        // data
                            });

                        node.State($"var {writeSpan} = {writer}.GetSpan({constLength.Value})");
                        node.State($"{writer}.Advance({nextInvokeStr})"); // Call Serialize Span<byte> index 0 method
                    }).NewLine();
                }

                // int Serialize<TState>(in __memberType member, in __type source, TState writeState, Span<byte> data);
                node.Comment($"Serializer Span");
                {
                    var serializeSpan = new SerializerSpanChainedMethods(_typeName, _member, MemberName.Serialize);
                    UseChainedMethods(builder, serializeSpan, methods, node => {
                        var nextState = LoadNextWriteState(node, serializeSpan);
                        node.State(serializeSpan.InvokeNextMethod(args => args[TStateTypeName] = nextState));
                    }).NewLine();
                }

                // int Deserialize(in __type source, ReadOnlySpan<byte> data, out __memberType result)
                node.Comment($"Deserialize Span");
                {
                    var deserializeSpan = new DeserializeSpanChainedMethods(_typeName, _member, MemberName.Deserialize);
                    UseChainedMethods(builder, deserializeSpan, methods, node => node.State(deserializeSpan.InvokeNextMethod()))
                        .NewLine();
                }

                // bool Deserialize(in __type source, ref SequenceCursor<byte> cursor, out __memberType result)
                node.Comment($"Deserialize Cursor");
                {
                    var deserializeCursor = new DeserializeCursorChainedMethods(_typeName, _member, MemberName.Deserialize);
                    UseChainedMethods(builder, deserializeCursor, methods, node => node.State(deserializeCursor.InvokeNextMethod()))
                        .NewLine();
                }

                // int GetSpanLength<TState>(in __memberType member, in __type source, TState writerState)
                node.Comment("Get Span Length");
                {
                    var spanLength = new GetSpanLengthChainedMethods(_typeName, _member, MemberName.GetSpanLength);
                    UseChainedMethods(builder, spanLength, methods, node => {
                        var nextState = LoadNextWriteState(node, spanLength);
                        node.State(spanLength.InvokeNextMethod(args => args[TStateTypeName] = nextState));
                    }).NewLine();
                }

                // int GetBlockLength(ReadOnlySpan<byte> data, in __dependentMember... dependentSource);
                node.Comment("Get Block Length (Span)");
                {
                    var blockSpanLength = new GetBlockLengthSpanChainedMethods(_typeName, _member, relyingMembers, MemberName.GetBlockLength);
                    UseChainedMethods(builder, blockSpanLength, methods, node => node.State(blockSpanLength.InvokeNextMethod()))
                        .NewLine();
                }

                // int GetBlockLength(ref SequenceCursor<byte> cursor, in __dependentMember... dependentSource);
                node.Comment("Get Block Length (Sequence Cursor)");
                {
                    var blockCursorLength = new GetBlockLengthCursorChainedMethods(_typeName, _member, relyingMembers, MemberName.GetBlockLength);
                    UseChainedMethods(builder, blockCursorLength, methods, node => node.State(blockCursorLength.InvokeNextMethod()))
                        .NewLine();
                }
            });

            _sourceAddition.AddSource($"{_typeName}.generated.cs", builder.ToString());
            return new BinaryMemberCompositeStruct(_typeName, _member, relyingMembers, methods, constLength);
        }
    }
}

