using System;
using System.Collections.Generic;
using Decuplr.Serialization.Analyzer.BinaryFormat;

namespace Decuplr.Serialization.Binary.Templates.ParserSource {
    // Every 
    internal abstract class ParserDependencyPrecusor {

        private class ComponentName : IComponentName {

            public ComponentName(string name) {
                Name = name;
            }

            public string Name { get; }

            public override string ToString() => Name;
            public override bool Equals(object obj) => obj is ComponentName name && name.Name.Equals(Name);
            public override int GetHashCode() => Name.GetHashCode();
        }

        private readonly List<(IParserDependencyComponent Item, IComponentName Name)> Components = new List<(IParserDependencyComponent, IComponentName)>();
        private readonly List<(ParserDependencyPrecusor Dependecy, IComponentName Name)> Dependencies = new List<(ParserDependencyPrecusor Dependecy, IComponentName Name)>();

        protected IComponentName AddComponent(IParserDependencyComponent component) {
            var name = new ComponentName($"component_{Components.Count}");
            Components.Add((component, name));
            return name;
        }

        protected IComponentName AddExternalDependency(ParserDependencyPrecusor dependency) {
            var name = new ComponentName($"dependency_{Dependencies.Count}");
            Dependencies.Add((dependency, name));
            return name;
        }

        #region Serialize / Deserialize Functions

        // bool TrySerializeUnsafe(in T value, Span<byte> dest, out int written);
        // Included as : bool TrySerialize{FieldName}(in {Type} type, in T {fieldName}, Span<byte> {destination}, out int {writtenBytes})
        protected abstract string TrySerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes);

        // int SerializeUnsafe(in T value, Span<byte> writer);
        protected abstract string SerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination);

        // DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int read, out T result);
        protected abstract string TryDeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result);

        // DeserializeResult TryDeserialize(ref SequenceCursor<byte> cursor);
        protected abstract string TryDeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<object> result);

        // T Deserialize(ReadOnlySpan<byte> span, out int readBytes);
        protected abstract string DeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes);

        // T Deserialize(ref SequenceCursor<byte> cursor);
        protected abstract string DeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source);

        // int GetLength(in T value);
        protected abstract string GetLengthFunction(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName);

        #endregion

        // Probably get functions out of it 
        public GeneratedDependencyParser CreateParsers(string uniqueName, MemberFormatInfo memberInfo, params IParsingConditions[] transforms) {
            var main = CreateMainDependency(uniqueName, memberInfo, transforms);

            var additional = new List<IGeneratedType>();
            for(var i = 0; i < Dependencies.Count; ++i) {
                var parsers = Dependencies[i].Dependecy.CreateParsers($"{uniqueName}_{i}", memberInfo);
                additional.Add(parsers.MainParser);
                additional.AddRange(parsers.AdditionalParser);
            }

            return new GeneratedDependencyParser {
                MainParser = main,
                AdditionalParser = additional,
            };
        }

        private IGeneratedType CreateMainDependency(string uniqueName, MemberFormatInfo memberInfo, IParsingConditions[] conditions) {
            var targetTypeSymbol = memberInfo.Analyzed.ContainingFullType.TypeSymbol;
            var memberTypeSymbol = memberInfo.TypeSymbol;
            var template = ParserFunctionTemplate.Create(targetTypeSymbol, memberTypeSymbol);

            var node = new CodeNodeBuilder();
            node.AddAttribute(CommonAttributes.GeneratedCodeAttribute);
            node.AddAttribute(CommonAttributes.HideFromEditor);
            node.AddNode($"internal readonly struct {uniqueName} ", node => {
                // Start adding members
                foreach ((var component, var name) in Components) {
                    node.AddStatement($"private readonly {component.TypeName} {name}");
                }

                // Add dependencies, the rule is always that the new type's name appends the index 
                for (var i = 0; i < Dependencies.Count; ++i) {
                    node.AddStatement($"private readonly {uniqueName}_{i} {Dependencies[i].Name}");
                }

                // Create Constructor : public TypeDep (IParserDiscovery discovery) for non try methods
                node.AddNode($"public {uniqueName}({nameof(IParserDiscovery)} discovery)", node => {
                    // Create assignments
                    // component_0 = GetComponenet_0(discovery);
                    foreach ((var component, var name) in Components)
                        node.AddStatement($"{name} = Get{name}(discovery)");

                    // Initialize our relied dependencies too
                    for (var i = 0; i < Dependencies.Count; ++i) {
                        node.AddStatement($"{Dependencies[i].Name} = new {uniqueName}_{i}(discovery)");
                    }

                    // {TypeComponent} GetComponent_0 (IParserDiscovery discovery)
                    foreach ((var component, var name) in Components)
                        node.AddNode($"{component.TypeName} Get{name}({nameof(IParserDiscovery)} discovery)", node => {
                            node.AddPlain(component.GetInitializer("discovery"));
                        });
                });

                // Create Constructor : public {TypeDep} (IParserDiscovery discovery, out bool isSuccess) for try methods
                node.AddNode($"public {uniqueName}({nameof(IParserDiscovery)} discovery, out bool isSuccess) : this()", node => {
                    // First we state that we aren't success unless we complete the whole condition
                    node.AddStatement("isSuccess = false");

                    foreach ((var component, var name) in Components)
                        node.AddNode($"if (!TryGet{name} (discovery, out {name}))", node => node.AddStatement("return"));

                    // Try to initiate our dependencies too
                    for (var i = 0; i < Dependencies.Count; ++i) {
                        node.AddStatement($"{Dependencies[i].Name} = new {uniqueName}_{i}(discovery, out isSuccess)");
                        node.AddNode($"if (!isSuccess)", node => node.AddStatement("return"));
                    }

                    // Finally if we complete every member's condition we say we succeed
                    node.AddStatement("isSuccess = true");
                    // Complete!

                    // This section covers nested functions for our component fetching process
                    foreach ((var component, var name) in Components)
                        node.AddNode($"bool TryGet{name}({nameof(IParserDiscovery)} discovery, out {component.TypeName} component)", node => {
                            node.AddPlain(component.TryGetInitializer("discovery", "component"));
                        });
                });

                // The actual parsing core "TryDeserializeActual"
                AddActualParsingCore(node, template);
                // Add a common interface for actual target constructor and other dependency to invoke: "TryDeserialize" it might first see the condition first, if ever present
                AddInvokingInterface(node, template, conditions);

                for (int i = 0; i < conditions.Length; i++)
                    AddParsingCondition(node, template, conditions[i], i, conditions.Length);

            });

            return new GeneratedTypeBuilder(targetTypeSymbol, uniqueName, GeneratedTypeKind.Struct, GeneratedPurpose.MemberDependency, GeneratedPlacement.PartialNestedExtension, node.ToString());
        }

        private void AddActualParsingCore(CodeNodeBuilder node, ParserFunctionTemplate template) {

            // public DeserializeResult TryDeserializeStateless(in {TargetType} type, ReadOnlySpan<byte> span, out int readBytes, out {TargetFieldType} data)
            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.TryDeserializeSpan("TryDeserializeActual", "type", "span", "readBytes", "field"), node => {
                node.AddPlain(TryDeserializeSpan("type", "span", "readBytes", "field"));
            });

            // public DeserializeResult TryDeserializeStateless(in {TargetType} type, ref SequenceCursor<byte> cursor, out {TargetFieldType} data)
            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.TryDeserializeSequence("TryDeserializeActual", "type", "cursor", "field"), node => {
                node.AddPlain(TryDeserializeSequence("type", "cursor", "field"));
            });

            // public {TargetFieldType} DeserializeStateless(in {TargetType} type, ReadOnlySpan<byte> span, out int readBytes)
            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.DeserializeSpan("DeserializeActual", "type", "span", "readBytes"), node => {
                node.AddPlain(DeserializeSpan("type", "span", "readBytes"));
            });

            // public {TargetFieldType} DeserializeStateless(in {TargetType} type, ref SequenceCursor<byte> cursor)
            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.DeserializeSequence("DeserializeActual", "type", "cursor"), node => {
                node.AddPlain(DeserializeSequence("type", "cursor"));
            });

            // public bool TrySerializeUnsafeStateless(in {TargetType} type, in {TargetFieldType} data, Span<byte> dest, out int writtenBytes)
            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.TrySerializeUnsafeSpan("TrySerializeUnsafeActual", "type", "field", "dest", "writtenBytes"), node => {
                node.AddPlain(TrySerializeUnsafeSpan("type", "field", "dest", "writtenBytes"));
            });

            // public int SerializeStateless(in {TargetType} type, in {TargetFieldType} data, Span<byte> dest)
            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.SerializeUnsafeSpan("SerializeActual", "type", "field", "dest"), node => {
                node.AddPlain(SerializeUnsafeSpan("type", "field", "dest"));
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.GetLengthFunction("GetLengthActual", "type", "field"), node => {
                node.AddPlain(GetLengthFunction("type", "field"));
            });

        }

        private void AddInvokingInterface(CodeNodeBuilder node, ParserFunctionTemplate template, IParsingConditions[] conditions) {

            string ReturnNextStatement(string name) => conditions.Length == 0 ? $"{name}Actual" : $"{name}_Condition_0";

            // public bool TryDeserialize(in {TargetType} type, ReadOnlySpan<byte> span, out int readBytes, out {TargetFieldType} data, out DeserializeResult result)
            // Note : this function is quite special in itself, 
            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.TryDeserializeSpan("TryDeserialize", "type", "span", "readBytes", "data"), node => {
                node.AddStatement($"return {ReturnNextStatement("TryDeserialize")} (type, span, out readBytes, out data)");
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.TryDeserializeSequence("TryDeserialize", "type", "cursor", "data"), node => {
                node.AddStatement($"return {ReturnNextStatement("TryDeserialize")} (type, ref cursor, out data)");
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.DeserializeSpan("Deserialize", "type", "span", "readBytes"), node => {
                node.AddStatement($"return {ReturnNextStatement("Deserialize")} (type, span, out readBytes)");
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.DeserializeSequence("Deserialize", "type", "cursor"), node => {
                node.AddStatement($"return {ReturnNextStatement("Deserialize")} (type, ref cursor)");
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.TrySerializeUnsafeSpan("TrySerializeUnsafe", "type", "field", "dest", "writtenBytes"), node => {
                node.AddStatement($"return {ReturnNextStatement("SerializeUnsafe")} (type, field, dest, out writtenBytes)");
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.SerializeUnsafeSpan("SerializeUnsafe", "type", "field", "dest"), node => {
                node.AddStatement($"return {ReturnNextStatement("SerializeUnsafe")} (type, field, dest)");
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.GetLengthFunction("GetLength", "type", "field"), node => {
                node.AddStatement($"return {ReturnNextStatement("GetLength")} (type, field)");
            });

        }

        private void AddParsingCondition(CodeNodeBuilder node, ParserFunctionTemplate template, IParsingConditions condition, int index, int totalLength) {

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.TryDeserializeSpan(CurrentName("TryDeserialize"), "type", "span", "readBytes", "data"), node => {
                node.AddPlain(condition.TryDeserializeSpan(NextName("TryDeserialize"), "type", "span", "readBytes", "data"));
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.TryDeserializeSequence(CurrentName("TryDeserialize"), "type", "cursor", "result"), node => {
                node.AddPlain(condition.TryDeserializeSequence(NextName("TryDeserialize"), "type", "cursor", "result"));
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.DeserializeSpan(CurrentName("Deserialize"), "type", "span", "readBytes"), node => {
                node.AddPlain(condition.DeserializeSpan(NextName("TryDeserialize"), "type", "span", "readBytes"));
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.DeserializeSequence(CurrentName("Deserialize"), "type", "cursor"), node => {
                node.AddPlain(condition.DeserializeSequence(NextName("Deserialize"), "type", "cursor"));
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.TrySerializeUnsafeSpan(CurrentName("TrySerializeUnsafe"), "type", "field", "dest", "writtenBytes"), node => {
                node.AddPlain(condition.TrySerializeUnsafeSpan(NextName("TrySerializeUnsafe"), "type", "field", "dest", "writtenBytes"));
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.SerializeUnsafeSpan(CurrentName("SerializeUnsafe"), "type", "field", "dest"), node => {
                node.AddPlain(condition.SerializeUnsafeSpan(NextName("SerializeUnsafe"), "type", "field", "dest"));
            });

            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode(template.GetLengthFunction(CurrentName("GetLength"), "type", "field"), node => {
                node.AddPlain(condition.GetLengthFunction(NextName("GetLength"), "type", "field"));
            });

            string CurrentName(string name) => $"{name}_Condition_{index}";
            string NextName(string name) => index + 1 == totalLength ? $"{name}Actual" : $"{name}_Condition_{index + 1}";
        }
    }
}
