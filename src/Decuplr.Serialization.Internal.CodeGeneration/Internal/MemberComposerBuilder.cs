using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.CodeGeneration.TypeComposers.Internal;
using Decuplr.Serialization.CodeGeneration.TypeComposers;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal class MemberComposerBuilder {

        private class PublicParsingMethodBuilder : ParsingMethodBuilder {

            private const string parent = "parent";
            private readonly MemberComposerBuilder _builder;

            public override IReadOnlyList<string> PrependArguments { get; }

            public PublicParsingMethodBuilder(MemberComposerBuilder builder)
                : base(builder._member, ComposerMethodNames.DefaultNames) {
                _builder = builder;
                PrependArguments = new[] { $"in {_builder._member.ContainingFullType.Symbol} {parent}" };
            }

            public override void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor) 
                => node.AddPlain($"{Method.TryDeserializeState(0)}({parent}, ref {refSequenceCursor})");

            public override void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes)
                => node.Return($"{Method.TryDeserializeState(0)}({parent}, {readOnlySpan}, out {outReadBytes})");

            public override void GetLength(CodeNodeBuilder node, InArgs<object> target)
                => node.Return($"{Method.GetLengthState(0)}({parent}, {target} )");

            public override void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan)
                => node.Return($"{Method.SerializeState(0)}({parent}, {target}, {readOnlySpan})");

            public override void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult)
                => node.Return($"{Method.TryDeserializeState(0)}({parent}, ref {refSequenceCursor}, out {outResult})");

            public override void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult)
                => node.Return($"{Method.TryDeserializeState(0)}({parent}, {readOnlySpan}, out {outReadBytes}, out {outResult})");

            public override void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes)
                => node.Return($"{Method.TrySerializeState(0)} ({parent}, {target}, {readOnlySpan}, out {outWrittenBytes})");
        }

        private class ComponentCollection : IComponentCollection {

            private readonly List<ITypeSymbol> _symbols = new List<ITypeSymbol>();

            public ComposerMethodNames GetMethodNames(int index)
                => new ComposerMethodNames {
                    TryDeserializeSequence = $"TryDeserialize_Component_{index}",
                    TryDeserializeSpan = $"TryDeserialize_Component_{index}",
                    DeserializeSequence = $"Deserialize_Component_{index}",
                    DeserializeSpan = $"Deserialize_Component_{index}",
                    TrySerialize = $"TrySerialize_Component_{index}",
                    Serialize = $"Serialize_Component_{index}",
                    GetLength = $"GetLength_Component_{index}",
                };

            public IReadOnlyList<ITypeSymbol> Components => _symbols;

            public ComposerMethods AddComponent(ITypeSymbol symbol) {
                _symbols.Add(symbol);
                return new ComposerMethods(GetMethodNames(_symbols.Count - 1));
            }
        }

        private static class Method {
            public static string InitializeComponent(int count) => $"{nameof(InitializeComponent)}_{count}";
            public static string TryInitializeComponent(int count) => $"{nameof(TryInitializeComponent)}_{count}";
            public static string TryDeserializeState(int index) => $"TryDeserializeState_{index}";
            public static string DeserializeState(int index) => $"DeserializeState_{index}";
            public static string TrySerializeState(int index) => $"TrySerializeState_{index}";
            public static string SerializeState(int index) => $"SerializeState_{index}";
            public static string GetLengthState(int index) => $"GetLengthState_{index}";
        }

        private static class Field {
            public static string Component(int count) => $"component_{count}";
        }

        private const string ReadOnlySpanByte = "ReadOnlySpan<byte>";
        private const string SpanByte = "Span<byte>";
        private const string SequenceCursor = "SequenceCursor<byte>";

        private readonly MemberMetaInfo _member;
        private readonly string _structName;
        private readonly IReadOnlyList<IConditionalFormatter> _conditions;
        private readonly IMemberDataFormatter _format;
        private readonly ComponentCollection _componentCollection = new ComponentCollection();
        private readonly ThrowCollection _throwCollection = new ThrowCollection("ThrowHelper");

        // Maybe we can change use to DI to resolve all this
        public MemberComposerBuilder(MemberMetaInfo layoutMember, IEnumerable<IConditionResolverProvider> conditions, IEnumerable<IMemberDataFormatterProvider> formatters) {
            if (_member.ReturnType is null)
                throw new ArgumentException("Invalid layout member (No Return Type)");
            _member = layoutMember;
            _conditions = conditions.Select(x => x.GetResolver(layoutMember, _throwCollection)).ToList();
            _structName = $"{_member.ContainingFullType.UniqueName}_{_member.Name}_Depedency";
            _format = GetFormatResolver();

            IMemberDataFormatter GetFormatResolver() {
                foreach (var formatter in formatters) {
                    if (!formatter.ShouldFormat(layoutMember))
                        continue;
                    return formatter.GetFormatter(layoutMember, _componentCollection, _throwCollection);
                }
                throw new ArgumentException($"The target member {layoutMember.Name} cannot be resolved by any of the provided {nameof(IMemberDataFormatterProvider)}");
            }
        }

        private static ComposerMethodNames GetDefaultNames(int index)
            => new ComposerMethodNames {
                TryDeserializeSequence = Method.TryDeserializeState(index),
                TryDeserializeSpan = Method.TryDeserializeState(index),
                DeserializeSequence = Method.TryDeserializeState(index),
                DeserializeSpan = Method.TryDeserializeState(index),
                TrySerialize = Method.TrySerializeState(index),
                Serialize = Method.SerializeState(index),
            };

        private CodeNodeBuilder AddComponents(CodeNodeBuilder builder, IReadOnlyList<IComponentTypeInfo> components) {
            for (var i = 0; i < components.Count; ++i) {
                builder.State($"private {components[i].FullTypeName} {Field.Component(i)}");
            }
            return builder;
        }

        private CodeNodeBuilder AddComponentInitializers(CodeNodeBuilder builder, Type discoveryType, IReadOnlyList<IComponentTypeInfo> components) {
            for (var i = 0; i < components.Count; ++i) {
                const string parserName = "parser";
                const string isSuccess = "isSuccess";

                builder.AddNode($"private {Method.InitializeComponent(i)}({discoveryType} {parserName})", node => {
                    components[i].ProvideInitialize(node, parserName);
                });

                builder.AddNode($"private {Method.TryInitializeComponent(i)}({discoveryType} {parserName}, out bool {isSuccess})", node => {
                    components[i].ProvideTryInitialize(node, parserName, isSuccess);
                });
            }
            return builder;
        }

        private CodeNodeBuilder AddConstructor(CodeNodeBuilder builder, Type discoveryType, IReadOnlyList<IComponentTypeInfo> components) {
            // Create Constructor
            const string parser = "parser";
            return builder.AddNode($"public {_structName}({discoveryType} {parser}) : this()", node => {
                for (var i = 0; i < components.Count; ++i)
                    node.State($"{Field.Component(i)} = {Method.InitializeComponent(i)} ( {parser} )");
            });
        }

        private CodeNodeBuilder AddTryConstructor(CodeNodeBuilder builder, Type discoveryType, IReadOnlyList<IComponentTypeInfo> components) {
            // Arguments
            const string parser = "parser";
            const string isSuccess = "isSuccess";

            // Create Constructor with try pattern
            builder.AddNode($"public {_structName}({discoveryType} parser, out bool {isSuccess}) : this()", node => {
                for (var i = 0; i < components.Count; ++i) {
                    // Initialize every component
                    node.State($"{Field.Component(i)} = {Method.InitializeComponent(i)} ( {parser}, out {isSuccess} )");

                    // If any fails, we bail out
                    node.If($"!{isSuccess}", node => {
                        node.State($"{isSuccess} = false");
                        node.Return();
                    });
                }

                // Finally we state that we success
                node.State($"{isSuccess} = true");
            });

            return builder;
        }

        private string GetGenericArgs() {
            if (_member.ReturnType!.Symbol is ITypeParameterSymbol symbol) {
                return $"<{symbol}>";
            }
            return string.Empty;
        }

        public MemberComposerPrecusor CreateStruct(IComponentProvider provider) {
            var components = _componentCollection.Components.Select(x => provider.ProvideComponent(x)).ToList();

            var builder = new CodeNodeBuilder();

            builder.DenoteHideEditor().DenoteGenerated(typeof(MemberComposerBuilder).Assembly);
            builder.AddNode($"internal readonly struct {_structName} {GetGenericArgs()}", node => {

                // Fields & Field Initialization
                builder.Comment($"Depedency provided by {provider.Name}");
                AddComponents(builder, components).NewLine();
                AddComponentInitializers(builder, provider.DiscoveryType, components).NewLine();

                // Construtor
                AddConstructor(builder, provider.DiscoveryType, components).NewLine();
                AddTryConstructor(builder, provider.DiscoveryType, components).NewLine();

                // Entry Point
                builder.Comment("Dependency Member Entry Point");
                builder.AddParsingMethods(new PublicParsingMethodBuilder(this));

                // Data Condition Methods
                for (int i = 0; i < _conditions.Count; i++)
                    builder.AddFormatterMethods(_conditions[i], _member, i, GetDefaultNames).NewLine();

                // Data Format Method
                builder.AddFormatterFinalMethods(_format, _member, _conditions.Count, GetDefaultNames).NewLine();

                // Data Resolve Method
                builder.Comment("Data Resolver").NewLine();
                for (var i = 0; i < components.Count; i++) {
                    builder.AddParsingBody(components[i], _member, _componentCollection.GetMethodNames(i)).NewLine();
                }

                // Nested Throw Helpers
                builder.Comment("Throw Helpers (Avoid inlining throw action)");
                _throwCollection.AddThrowClass(builder);
            });

            return new MemberComposerPrecusor(_member, _structName, builder);
        }
    }
}
