using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Decuplr.Serialization;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    
    // WARN : Don't add this to the DI, use MemberComposerFactory!
    // Note to future : since the method structures are hard coded, you may want to use MethodSignature to represent it in the future
    internal class MemberComposerSource {

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
        private readonly MemberComponentCollection _componentCollection = new MemberComponentCollection();
        private readonly ThrowCollection _throwCollection = new ThrowCollection("ThrowHelper");

        public MemberComposerSource(MemberMetaInfo layoutMember, GeneratingTypeName typeName,
                                    IEnumerable<IConditionResolverProvider> conditions,
                                    IEnumerable<IMemberDataFormatterProvider> formatters,
                                    ITypeSymbolProvider symbolProvider) {
            if (_member.ReturnType is null)
                throw new ArgumentException("Invalid layout member (No Return Type)");
            _member = layoutMember;
            _typeName = typeName;
            _symbols = symbolProvider;
            // Watch out for order(?
            _conditions = conditions.SelectMany(x => x.GetResolvers(layoutMember, _throwCollection)).ToList();
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
                TryDeserializeSequence = MemberMethod.TryDeserializeState(index),
                TryDeserializeSpan = MemberMethod.TryDeserializeState(index),
                DeserializeSequence = MemberMethod.TryDeserializeState(index),
                DeserializeSpan = MemberMethod.TryDeserializeState(index),
                TrySerialize = MemberMethod.TrySerializeState(index),
                Serialize = MemberMethod.SerializeState(index),
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

                builder.AddNode($"private {components[i].Type} {MemberMethod.InitializeComponent(i)}({discoveryType} {parserName})", node => {
                    components[i].ProvideInitialize(node, parserName);
                });

                builder.AddNode($"private {components[i].Type} {MemberMethod.TryInitializeComponent(i)}({discoveryType} {parserName}, out bool {isSuccess})", node => {
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
                    node.State($"{Field.Component(i)} = {MemberMethod.InitializeComponent(i)} ( {parser} )");
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
                    node.State($"{Field.Component(i)} = {MemberMethod.InitializeComponent(i)} ( {parser}, out {isSuccess} )");

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
                builder.AddParsingMethods(new ParsingMethodWithTypeBuilder(_member));

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

            return new MemberComposer(typeComposer, _member, symbolProvider(_typeName, builder.ToString()), GetMethodSignatures(provider));
        }

        private IReadOnlyList<MethodSignature> GetMethodSignatures(IComponentProvider provider) {
            var boolSymbol = _symbols.GetSymbol<bool>();
            var intSymbol = _symbols.GetSymbol<int>();
            var memberType = _member.ReturnType!.Symbol;

            // Incoming Data type
            var discovery = new MethodArg(RefKind.In, provider.DiscoveryType, "parent");
            var sourceType = new MethodArg(RefKind.In, _member.ContainingFullType.Symbol, "source");

            // Buffers
            var readOnlySpan = new MethodArg(_symbols.GetSymbol(typeof(ReadOnlySpan<byte>)), "readOnlySpan");
            var span = new MethodArg(_symbols.GetSymbol(typeof(Span<byte>)), "span");
            var cursor = new MethodArg(RefKind.Ref, _symbols.GetSymbol(typeof(SequenceCursor<byte>)), "cursor");

            // Return Data
            var readBytes = new MethodArg(RefKind.Out, intSymbol, "readBytes");
            var writtenBytes = new MethodArg(RefKind.Out, intSymbol, "writtenBytes");
            var result = new MethodArg(RefKind.Out, memberType, "result");

            return new MethodSignature[] {
                MethodSignatureBuilder.CreateConstructor(_typeName, (provider.DiscoveryType, "parser")),
                MethodSignatureBuilder.CreateConstructor(_typeName, (provider.DiscoveryType, "parser"), (RefKind.Out, boolSymbol, "isSuccess")),

                MethodSignatureBuilder.CreateMethod(_typeName, MemberMethod.TryDeserializeState(0))
                                      .AddArgument(discovery, readOnlySpan, readBytes, result)
                                      .WithReturn<bool>(),

                MethodSignatureBuilder.CreateMethod(_typeName, MemberMethod.TryDeserializeState(0))
                                      .AddArgument(discovery, cursor, result)
                                      .WithReturn<bool>(),

                MethodSignatureBuilder.CreateMethod(_typeName, MemberMethod.DeserializeState(0))
                                      .AddArgument(discovery, readOnlySpan, result)
                                      .WithReturn(memberType),

                MethodSignatureBuilder.CreateMethod(_typeName, MemberMethod.DeserializeState(0))
                                      .AddArgument(discovery, cursor)
                                      .WithReturn(memberType),

                MethodSignatureBuilder.CreateMethod(_typeName, MemberMethod.TrySerializeState(0))
                                      .AddArgument(discovery, sourceType, span, writtenBytes)
                                      .WithReturn(boolSymbol),

                MethodSignatureBuilder.CreateMethod(_typeName, MemberMethod.SerializeState(0))
                                      .AddArgument(discovery, sourceType, span)
                                      .WithReturn(intSymbol),

                MethodSignatureBuilder.CreateMethod(_typeName, MemberMethod.GetLengthState(0))
                                      .AddArgument(discovery, sourceType)
                                      .WithReturn(intSymbol)
            };
        }
    }
}
