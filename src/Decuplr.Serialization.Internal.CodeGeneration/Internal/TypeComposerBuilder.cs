using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Serialization.LayoutService;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.Serialization.CodeGeneration.Internal {

    class TypeComposerPrecusor {

        public SchemaLayout Type { get; }

        public IReadOnlyList<MemberComposerPrecusor> MemberComposers { get; }
    }

    class TypeComposerBuilder {

        private static class Property {
            public static string MemberName(int index) => $"Member_{index}";
        }

        public const string DefaultNamespace = "Decuplr.Serialization.Internal.Parsers";

        private readonly SchemaLayout _type;
        private readonly IEnumerable<IConditionResolverProvider> _resolvers;
        private readonly IEnumerable<IMemberDataFormatterProvider> _formatter;

        public TypeComposerBuilder(SchemaLayout layout, IEnumerable<IConditionResolverProvider> resolvers, IEnumerable<IMemberDataFormatterProvider> memberFormatter) {
            _type = layout;
            _resolvers = resolvers;
            _formatter = memberFormatter;
        }

        public TypeComposerPrecusor Build(string typeComposerNamespace, IComponentProvider provider) => Build(typeComposerNamespace, _type.Type.UniqueName, provider);
        public TypeComposerPrecusor Build(IComponentProvider provider) => Build("Decuplr.Serialization.Internal.Parsers", provider);
        public TypeComposerPrecusor Build(string typeComposerNamespace, string typeComposerName, IComponentProvider provider) {

            var composers = _type.Layouts.Select(member => new MemberComposerBuilder(member, _resolvers, _formatter).CreateStruct(provider)).ToList();

            var builder = new CodeSourceFileBuilder(typeComposerNamespace);
            builder.Using("System");

            builder.AddNode($"internal readonly struct {typeComposerName}", node => {
                for (var i = 0; i < composers.Count; ++i) {
                    node.State($"public {composers[i].Name} {Property.MemberName(i)} {{ get; }}");
                }

                const string discovery = "discovery";
                const string isSuccess = "isSuccess";

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
        }
    }
}
