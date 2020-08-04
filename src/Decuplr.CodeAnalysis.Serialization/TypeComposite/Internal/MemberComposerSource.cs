using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {

    // WARN : Don't add this to the DI, use MemberComposerFactory!
    internal class MemberComposerSource {

        private class PublicParsingMethodBuilder : ParsingMethodBuilder {

            private const string parent = "parent";
            private readonly MemberComposerSource _builder;

            public override IReadOnlyList<string> PrependArguments { get; }

            public PublicParsingMethodBuilder(MemberComposerSource builder)
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
        private readonly GeneratingTypeName _typeName;
        private readonly IReadOnlyList<IConditionResolver> _conditions;
        private readonly IMemberDataFormatter _format;
        private readonly ITypeSymbolProvider _symbols;
        private readonly ComponentCollection _componentCollection = new ComponentCollection();
        private readonly ThrowCollection _throwCollection = new ThrowCollection("ThrowHelper");

        // Maybe we can change use to DI to resolve all this
        public MemberComposerSource(MemberMetaInfo layoutMember, GeneratingTypeName typeName,
                                    IEnumerable<IConditionResolverProvider> conditions,
                                    IEnumerable<IMemberDataFormatterProvider> formatters,
                                    ITypeSymbolProvider symbolProvider) {
            if (_member.ReturnType is null)
                throw new ArgumentException("Invalid layout member (No Return Type)");
            _member = layoutMember;
            _typeName = typeName;
            _symbols = symbolProvider;
            _conditions = conditions.Select(x => x.GetResolver(layoutMember, _throwCollection)).ToList();
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
                builder.State($"private {components[i].Type} {Field.Component(i)}");
            }
            return builder;
        }

        private CodeNodeBuilder AddComponentInitializers(CodeNodeBuilder builder, INamedTypeSymbol discoveryType, IReadOnlyList<IComponentTypeInfo> components) {
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

        private CodeNodeBuilder AddConstructor(CodeNodeBuilder builder, INamedTypeSymbol discoveryType, IReadOnlyList<IComponentTypeInfo> components) {
            // Create Constructor
            const string parser = "parser";
            return builder.AddNode($"public {_typeName.TypeName}({discoveryType} {parser}) : this()", node => {
                for (var i = 0; i < components.Count; ++i)
                    node.State($"{Field.Component(i)} = {Method.InitializeComponent(i)} ( {parser} )");
            });
        }

        private CodeNodeBuilder AddTryConstructor(CodeNodeBuilder builder, INamedTypeSymbol discoveryType, IReadOnlyList<IComponentTypeInfo> components) {
            // Arguments
            const string parser = "parser";
            const string isSuccess = "isSuccess";

            // Create Constructor with try pattern
            builder.AddNode($"public {_typeName.TypeName}({discoveryType} {parser}, out bool {isSuccess}) : this()", node => {
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

        //private MethodSignature TryConstructor => MethodSignatureBuilder.CreateConstructor(Accessibility.Public, _typeName, )

        public IMemberComposer CreateStruct(ITypeComposer typeComposer, IComponentProvider provider, Func<GeneratingTypeName, string, INamedTypeSymbol> symbolProvider) {
            var components = _componentCollection.Components.Select(x => provider.ProvideComponent(x)).ToList();

            var builder = new CodeSourceFileBuilder(_typeName.Namespace);
            builder.Using("System");

            builder.DenoteHideEditor().DenoteGenerated(typeof(MemberComposerSource).Assembly);
            builder.NestType(_typeName, $"internal readonly struct {_typeName.TypeName} {GetGenericArgs()}", node => {

                // Fields & Field Initialization
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

            var methods = new MethodSignature[] {
                MethodSignatureBuilder.CreateConstructor(_typeName, (provider.DiscoveryType, "parser")),
                MethodSignatureBuilder.CreateConstructor(_typeName, (provider.DiscoveryType, "parser"), (RefKind.Out, _symbols.GetSymbol<bool>(), "isSuccess")),
                MethodSignatureBuilder.CreateMethod(_typeName, Method.TryDeserializeState(0)).AddArgument().WithReturn(_symbols.GetSymbol<bool>())
            };

            return new MemberComposer(typeComposer, _member, symbolProvider(_typeName, builder.ToString()), methods);
        }
    }
}
