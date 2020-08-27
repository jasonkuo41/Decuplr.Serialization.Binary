using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.CodeAnalysis.SourceBuilder;
using Decuplr.Serialization.Binary.TypeComposite.Internal;
using Decuplr.Serialization.Namespaces;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IChainedMethods {
        bool HasChainedMethod { get; }

        string this[TypeName type] { get; }
        string this[TypeName type, int index] { get; }

        string InvokeNextMethod();
        string InvokeNextMethod(Action<IChainMethodInvokeAction> action);
    }

    internal interface IChainMethodInvokeAction {
        string this[TypeName type] { get; set; }
        string this[TypeName type, int index] { get; set; }
    }

    public interface IBinaryMemberComposeMethod {
        void SerializeWriter(CodeNodeBuilder builder, IChainedMethods chained);
        void SerializeSpan(CodeNodeBuilder builder, IChainedMethods chained);

        void GetSpanLength(CodeNodeBuilder builder, IChainedMethods chained);

        void DeserializeSpan(CodeNodeBuilder builder, IChainedMethods chained);
        void DeserializeCursor(CodeNodeBuilder builder, IChainedMethods chained);

        void GetBlockLengthSpan(CodeNodeBuilder builder, IChainedMethods chained);
        void GetBlockLengthCursor(CodeNodeBuilder builder, IChainedMethods chained);
    }

    public interface IBinaryMemberFeatureProvider {
        bool ShouldApply(MemberMetaInfo member);
        IBinaryMemberComposeMethod GetComposingMethods(MemberMetaInfo member, IComponentCollection components, IThrowCollection throwCollection);
    }

    public interface IMemberFormatNamespaceProvider {
        FormatNamespaceInfo GetUsingNamespaces(MemberMetaInfo member);
    }

    public readonly struct FormatNamespaceInfo {

        private readonly IEnumerable<string> _withNamespaces;
        private readonly IEnumerable<string> _withPrioritizedNamespaces;

        public FormatNamespaceInfo(IEnumerable<string> withNamespaces, IEnumerable<string> withPrioritizedNamespaces) {
            _withNamespaces = withNamespaces;
            _withPrioritizedNamespaces = withPrioritizedNamespaces;
        }

        public IEnumerable<string> WithNamespaces => _withNamespaces ?? Enumerable.Empty<string>();
        public IEnumerable<string> WithPrioritizedNamespaces => _withPrioritizedNamespaces ?? Enumerable.Empty<string>();
    }

    internal class BinaryMemberComposite {

        private static class MemberName {
            public static string ComponentField(int i) => $"_component{i}";
            public static string InitializeComponentMethod(int i) => $"InitializeComponent{i}";
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
        }

        public void CreateStruct() {
            var builder = new CodeSourceFileBuilder(_typeName.Namespace);
            var genericArgs = _member.ReturnType!.Symbol is ITypeParameterSymbol symbol ? $"<{symbol}>" : "";
            var components = _memberComponents.Components;

            builder.AttributeHideEditor().AttributeGenerated(typeof(BinaryMemberComposite).Assembly);
            builder.NestType(_typeName, $"internal readonly struct {_typeName.TypeName} {genericArgs}", node => {

                builder.Comment("Component Fields");
                for (var i = 0; i < components.Count; ++i)
                    builder.State($"private {components[i]} {MemberName.ComponentField(i)}");

                builder.Comment("Member Composer Constructor");
                {
                    const string parser = "parser";
                    builder.AddNode($"internal {_typeName.TypeName} ({typeof(IBinaryNamespaceDiscovery).FullName} {parser})", node => {
                        node.State($"{parser} = {parser}.{nameof(INamespaceDiscovery.WithNamespace)}({JoinedNamespaces(_normalNamespaces)})" +
                                                      $".{nameof(INamespaceDiscovery.WithPrioritizedNamespace)}({JoinedNamespaces(_prioritizedNamespaces)})" +
                                                      $"as {typeof(IBinaryNamespaceDiscovery).FullName}");
                        for (var i = 0; i < components.Count; ++i) {
                            node.State($"{MemberName.ComponentField(i)} = {parser}.{nameof(IBinaryNamespaceDiscovery.GetConverter)}<{components[i]}>()");
                        }
                    });

                    static string JoinedNamespaces(IEnumerable<string> namespaces) => $"new string[] {{ { string.Join(", ", namespaces.Select(n => $@"""{n}""")) } }}";
                }

            });
        }
    }
}
