using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.CodeGeneration.Internal.ParserGroup;
using Decuplr.Serialization.CodeGeneration.ParserGroup;
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

            public ParserMethodGroup AddComponent(ITypeSymbol symbol) {
                _symbols.Add(symbol);
            }
        }

        private static class Method {
            public static string InitializeComponent(int count) => $"{nameof(InitializeComponent)}_{count}";
            public static string TryInitializeComponent(int count) => $"{nameof(TryInitializeComponent)}_{count}";
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
        public DependencyStructBuilder(MemberMetaInfo layoutMember, IEnumerable<IConditionResolverProvider> conditions, IEnumerable<IMemberDataFormatterProvider> formatters) {
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

                // Data Condition Methods
                for (int i = 0; i < _conditions.Count; i++)
                    builder.AddFormatterParserGroup(_conditions[i], _member, i).NewLine();

                // Data Format Method
                builder.AddFormatterParserGroup(_format, _member, _conditions.Count).NewLine();

                // Data Resolve Method
                "Resolve Data pls";

                // Nested Throw Helpers
                _throwCollection.AddThrowClass(builder);
            });
        }
    }
}
