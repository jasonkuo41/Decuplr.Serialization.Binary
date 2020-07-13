using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.LayoutService;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal {

    interface IDependencySource {
        /// <summary>
        /// In Decuplr.Serialization.Binary it should be IParserDiscovery
        /// </summary>
        Type DiscoveryType { get; }

        IReadOnlyList<IComponentTypeInfo> ComponentTypes { get; }
    }

    interface IDependencySourceProvider {
        /// <summary>
        /// The name of the source that we can refer as
        /// </summary>
        string Name { get; }

        IDependencySource ProvideSource(IComponentCollection collection);
    }

    interface IComponentTypeInfo {
        string FullName { get; }

        string ProvideInitialize(string parserName);

        string ProvideTryInitialize(string parserName, OutArgs<bool> isSuccess);
    }

    class DependencyStructBuilder {

        private class ComponentCollection : IComponentCollection {

            private readonly List<ITypeSymbol> _symbols = new List<ITypeSymbol>();

            public IReadOnlyList<ITypeSymbol> Components => _symbols;

            public string AddComponent(ITypeSymbol symbol) {
                _symbols.Add(symbol);
                return Field.Component(_symbols.Count - 1);
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
        private readonly IReadOnlyList<IConditionResolver> _conditions;
        private readonly IFormatResolver _format;
        private readonly ComponentCollection _componentCollection;

        public DependencyStructBuilder(MemberMetaInfo layoutMember, IEnumerable<IConditionResolverProvider> conditions, IEnumerable<IFormatResolverProvider> formatResolvers) {
            _member = layoutMember;
            _conditions = conditions.Select(x => x.GetResolver(layoutMember)).ToList();
            _structName = $"{_member.ContainingFullType.UniqueName}_{_member.Name}_Depedency";
            _format = GetFormatResolver(out _componentCollection);

            IFormatResolver GetFormatResolver(out ComponentCollection provider) {
                foreach (var formatResolver in formatResolvers) {
                    provider = new ComponentCollection();
                    if (formatResolver.TryGetResolver(layoutMember, provider, out var format))
                        return format;
                }
                throw new ArgumentException($"The target member {layoutMember.Name} cannot be resolved by any of the provided {nameof(IFormatResolverProvider)}");
            }
        }

        private CodeNodeBuilder AddComponents(CodeNodeBuilder builder, IDependencySource provider) {
            for (var i = 0; i < provider.ComponentTypes.Count; ++i) {
                builder.State($"private {provider.ComponentTypes[i].FullName} {Field.Component(i)}");
            }
            return builder;
        }

        private CodeNodeBuilder AddComponentInitializers(CodeNodeBuilder builder, IDependencySource provider) {
            for (var i = 0; i < provider.ComponentTypes.Count; ++i) {
                const string parserName = "parser";
                const string isSuccess = "isSuccess";

                builder.AddNode($"private {Method.InitializeComponent(i)}({provider.DiscoveryType} {parserName})", node => {
                    node.Add(provider.ComponentTypes[i].ProvideInitialize(parserName));
                });

                builder.AddNode($"private {Method.TryInitializeComponent(i)}({provider.DiscoveryType} {parserName}, out bool {isSuccess})", node => {
                    node.Add(provider.ComponentTypes[i].ProvideTryInitialize(parserName, isSuccess));
                });
            }
            return builder;
        }

        private CodeNodeBuilder AddConstructor(CodeNodeBuilder builder, IDependencySource provider) {
            // Create Constructor
            const string parser = "parser";
            return builder.AddNode($"public {_structName}({provider.DiscoveryType} {parser}) : this()", node => {
                for (var i = 0; i < provider.ComponentTypes.Count; ++i)
                    node.State($"{Field.Component(i)} = {Method.InitializeComponent(i)} ( {parser} )");
            });
        }

        private CodeNodeBuilder AddTryConstructor(CodeNodeBuilder builder, IDependencySource provider) {
            // Arguments
            const string parser = "parser";
            const string isSuccess = "isSuccess";

            // Create Constructor with try pattern
            builder.AddNode($"public {_structName}({provider.DiscoveryType} parser, out bool {isSuccess}) : this()", node => {
                for (var i = 0; i < provider.ComponentTypes.Count; ++i) {
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

        private CodeNodeBuilder AddCondition(CodeNodeBuilder builder, IConditionResolver resolver, int index) {
            builder.Comment($"Condition Generated By : {resolver.ResolverName} (Order: {index})");

            Debug.Assert(!(_member.ReturnType is null));

            const string parent = "parent";
            const string span = "span";
            const string cursor = "cursor";
            const string readBytes = "readBytes";
            const string writtenBytes = "writtenBytes";
            const string target = "target";

            var targetSymbol = _member.ReturnType!.Symbol;
            var parentSymbol = _member.ContainingFullType.Symbol;
            
            // TryDeserialize (ReadOnlySpan<byte>)
            builder.AddNode($"private {nameof(DeserializeResult)} {Method.TryDeserializeState(index)}(in {parentSymbol} {parent}, {ReadOnlySpanByte} {span}, out int {readBytes}, out {targetSymbol} {target})",
                node => {
                    resolver.GetFunctionBody(Method.TryDeserializeState(index), new TryDeserializeSpanArgs<TypeSourceArgs>(parent, span, readBytes, target));
                });

            // TryDeserialize (Sequence)
            builder.AddNode($"private {nameof(DeserializeResult)} {Method.TryDeserializeState(index)}(in {parentSymbol} {parent}, {SequenceCursor} {cursor}, out  {targetSymbol} {target})",
                node => {
                    resolver.GetFunctionBody(Method.TryDeserializeState(index), new TryDeserializeSequenceArgs<TypeSourceArgs>(parent, cursor, target));
                });

            // Deserialize (ReadOnlySpan<byte>)
            builder.AddNode($"private {targetSymbol} {Method.DeserializeState(index)}(in {parentSymbol} {parent}, {ReadOnlySpanByte} {span}, out int {readBytes})",
                node => {
                    resolver.GetFunctionBody(Method.DeserializeState(index), new DeserializeSpanArgs<TypeSourceArgs>(parent, span, readBytes));
                });

            // Deserialize (Sequence)
            builder.AddNode($"private int {Method.DeserializeState(index)}(in {parentSymbol} {parent}, {SequenceCursor} {cursor})",
                node => {
                    resolver.GetFunctionBody(Method.DeserializeState(index), new DeserializeSequenceArgs<TypeSourceArgs>(parent, cursor));
                });


            // TrySerialize (ReadOnlySpan<byte>)
            builder.AddNode($"private bool {Method.TrySerializeState(index)}(in {parentSymbol} {parent}, in {targetSymbol} {target}, {SpanByte} {span}, out int {writtenBytes})",
                node => {
                    resolver.GetFunctionBody(Method.TrySerializeState(index), new TrySerializeArgs<TypeSourceArgs>(parent, target, span, writtenBytes));
                });

            // Serialize (ReadOnlySpan<byte>)
            builder.AddNode($"private int {Method.SerializeState(index)}(in {parentSymbol} {parent}, in {targetSymbol} {target}, {SpanByte} {span})",
                node => {
                    resolver.GetFunctionBody(Method.SerializeState(index), new SerializeArgs<TypeSourceArgs>(parent, target, span));
                });

            // GetLength
            builder.AddNode($"private int {Method.GetLengthState(index)}(in {parentSymbol} {parent}, in {targetSymbol} {target})",
                node => {
                    resolver.GetFunctionBody(Method.GetLengthState(index), new GetLengthArgs<TypeSourceArgs>(parent, target));
                });

            return builder;
        }

        private void CreateStruct(IDependencySourceProvider provider) {
            var source = provider.ProvideSource(_componentCollection);
            var builder = new CodeNodeBuilder();

            builder.DenoteHideEditor().DenoteGenerated(typeof(DependencyStructBuilder).Assembly);
            builder.AddNode($"internal readonly struct {_structName}", node => {

                // Fields & Field Initialization
                builder.Comment($"Depedency provided by {provider.Name}");
                AddComponents(builder, source).NewLine();
                AddComponentInitializers(builder, source).NewLine();

                // Construtor
                AddConstructor(builder, source).NewLine();
                AddTryConstructor(builder, source).NewLine();

                // Condition Methods
                for (int i = 0; i < _conditions.Count; i++)
                    AddCondition(builder, _conditions[i], i).NewLine();

                // DataResolving Method
                AddDataResolving(builder, ).NewLine();

                // DataFormatting Method

            });
        }
    }

    struct ParserMethodNames {
        public string TryDeserializeSpan { get; set; }
        public string TryDeserializeSequence { get; set; }
        public string DeserializeSpan { get; set; }
        public string DeserializeSequence { get; set; }
        public string TrySerialize { get; set; }
        public string Serialize { get; set; }
        public string GetLength { get; set; }
    }

    interface ITypeParserGroup {
        ParserMethodNames MethodNames { get; }
        void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult);
        void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult);
        void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes);
        void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor);
        void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes);
        void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan);
        void GetLength(CodeNodeBuilder node, InArgs<object> target);
    }

    internal static class CodeNodeExtensions {
        public static CodeNodeBuilder AddTypeParserGroup(this CodeNodeBuilder builder, ITypeParserGroup group, ITypeSymbol targetSymbol, params string[] prependArguments) {
            const string ReadOnlySpanByte = "ReadOnlySpan<byte>";
            const string SpanByte = "Span<byte>";
            const string SequenceCursor = "SequenceCursor<byte>";

            const string span = "span";
            const string cursor = "cursor";
            const string readBytes = "readBytes";
            const string writtenBytes = "writtenBytes";
            const string target = "target";

            var strBuilder = new StringBuilder();
            foreach (var arg in prependArguments) {
                strBuilder.Append(arg);
                strBuilder.Append(", ");
            }
            var prependString = strBuilder.ToString();

            // TryDeserialize (ReadOnlySpan<byte>)
            builder.AddNode($"private {nameof(DeserializeResult)} {group.MethodNames.TryDeserializeSpan}({prependString}{ReadOnlySpanByte} {span}, out int {readBytes}, out {targetSymbol} {target})",
                node => group.TryDeserializeSpan(node, span, readBytes, target));

            // TryDeserialize (Sequence)
            builder.AddNode($"private {nameof(DeserializeResult)} {group.MethodNames.TryDeserializeSequence}({prependString}{SequenceCursor} {cursor}, out  {targetSymbol} {target})",
                node => {
                    resolver.GetFunctionBody(Method.TryDeserializeState(index), new TryDeserializeSequenceArgs<TypeSourceArgs>(parent, cursor, target));
                });

            // Deserialize (ReadOnlySpan<byte>)
            builder.AddNode($"private {targetSymbol} {Method.DeserializeState(index)}({prependString}{ReadOnlySpanByte} {span}, out int {readBytes})",
                node => {
                    resolver.GetFunctionBody(Method.DeserializeState(index), new DeserializeSpanArgs<TypeSourceArgs>(parent, span, readBytes));
                });

            // Deserialize (Sequence)
            builder.AddNode($"private int {Method.DeserializeState(index)}({prependString}{SequenceCursor} {cursor})",
                node => {
                    resolver.GetFunctionBody(Method.DeserializeState(index), new DeserializeSequenceArgs<TypeSourceArgs>(parent, cursor));
                });


            // TrySerialize (ReadOnlySpan<byte>)
            builder.AddNode($"private bool {Method.TrySerializeState(index)}({prependString}in {targetSymbol} {target}, {SpanByte} {span}, out int {writtenBytes})",
                node => {
                    resolver.GetFunctionBody(Method.TrySerializeState(index), new TrySerializeArgs<TypeSourceArgs>(parent, target, span, writtenBytes));
                });

            // Serialize (ReadOnlySpan<byte>)
            builder.AddNode($"private int {Method.SerializeState(index)}({prependString}in {targetSymbol} {target}, {SpanByte} {span})",
                node => {
                    resolver.GetFunctionBody(Method.SerializeState(index), new SerializeArgs<TypeSourceArgs>(parent, target, span));
                });

            // GetLength
            builder.AddNode($"private int {Method.GetLengthState(index)}(in {parentSymbol} {parent}, in {targetSymbol} {target})",
                node => {
                    resolver.GetFunctionBody(Method.GetLengthState(index), new GetLengthArgs<TypeSourceArgs>(parent, target));
                });

            return builder;
        }
    }
}
