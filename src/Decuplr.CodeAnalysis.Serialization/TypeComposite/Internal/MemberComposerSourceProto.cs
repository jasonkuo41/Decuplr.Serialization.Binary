using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {

    internal class MemberComposerInfo {
        public GeneratingTypeName Name { get; }
        public IReadOnlyList<MethodSignature> ChainingMethods { get; }
    }

    internal class MemberComposerSourceProto {

        private static class Field { public static string Component(int count) => $"component_{count}"; }

        private readonly MemberMetaInfo _member;
        private readonly MemberComposerInfo _info;
        private readonly GeneratingTypeName _typeName;
        private readonly IReadOnlyList<IMemberComposingMethod> _features;
        private readonly ITypeSymbolProvider _symbols;
        private readonly ISourceAddition _sourceAddition;
        private readonly MemberComponentCollection _componentCollection = new MemberComponentCollection();
        private readonly ThrowCollection _throwCollection = new ThrowCollection("ThrowHelper");

        public MemberComposerSourceProto(MemberMetaInfo layoutMember, GeneratingTypeName typeName, IEnumerable<IMemberComposingFeature> features, ITypeSymbolProvider symbolProvider) {
            if (_member.ReturnType is null)
                throw new ArgumentException("Invalid layout member (No Return Type)");
            _member = layoutMember;
            _typeName = typeName;
            _symbols = symbolProvider;
            _features = features.Where(x => x.ShouldFormat(layoutMember)).Select(x => x.GetComposingMethods(layoutMember, _componentCollection, _throwCollection)).ToList();
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

        private void AddFeaturingMethods(CodeNodeBuilder builder) {
            if (_features.Count == 0)
                throw new InvalidOperationException("Feature count cannot be zero!");

            foreach (var method in _info.ChainingMethods) {
                var methodBodies = GetMethodBodies(method);
                for (var i = 0; i < methodBodies.Count; ++i) {
                    var currentMethod = method;
                    if (i != 0)
                        currentMethod = method.Rename(Accessibility.Private, GetNextMethodName(method, i));
                    builder.AddMethod(currentMethod, node => node.AddPlain(methodBodies[i]));
                }
            }

            IReadOnlyList<string> GetMethodBodies(MethodSignature method) {
                var list = new List<string>(_features.Count);

                for (var i = 0; i < _features.Count; i++) {
                    var feature = _features[i];
                    var isLast = i == _features.Count - 1;
                    var methodProvider = new ChainingMethodProvider(method.Arguments, isLast ? null : GetNextMethodName(method, i) );
                    // We need to evaluate it first
                    list.Add(feature.GetMethodBody(method.MethodName, methodProvider));
                    if (!methodProvider.HasInvokedNextMethod)
                        break;
                }
                return list;
            }

            static string GetNextMethodName(MethodSignature method, int stateCount) => $"{method.MethodName}_State{stateCount - 1}";
        }

        public IMemberComposer CreateStruct(ITypeComposer typeComposer, IComponentProvider provider) {
            var genericArgs = _member.ReturnType!.Symbol is ITypeParameterSymbol symbol ? $"<{symbol}>" : "";
            var components = _componentCollection.Components.Select(type => provider.ProvideComponent(type)).ToList();
            var builder = new CodeSourceFileBuilder(_typeName.Namespace);
            builder.Using("System");

            builder.DenoteHideEditor().DenoteGenerated(typeof(MemberComposerSourceProto).Assembly);
            builder.NestType(_typeName, $"internal readonly struct {_typeName.TypeName} {genericArgs}", node => {

                // Add Components for each serializing members
                for(var i = 0; i < components.Count; ++i)
                    node.State($"private {components[i].Type} {Field.Component(i)}");

                // Add Constructors
                AddConstructor(node, provider.DiscoveryType, components).NewLine();
                AddTryConstructor(node, provider.DiscoveryType, components).NewLine();

                // Initialize Component
                AddComponentInitializers(node, provider.DiscoveryType, components).NewLine();

                AddFeaturingMethods(node);
                AddComponentComposerMethod(node, provider, components);
            });

            _sourceAddition.AddSource($"{_typeName}.generated.cs", builder.ToString());
            var memberComposerSymbol = _symbols.GetSymbol(_typeName.ToString());

            return new MemberComposer(typeComposer, _member, memberComposerSymbol);
        }
    }
}
