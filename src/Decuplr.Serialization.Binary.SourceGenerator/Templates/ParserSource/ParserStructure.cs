using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.Templates.ParserSource {

    internal interface IParserDependencyComponent {
        string TypeName { get; }

        string GetInitializer(ParserDiscoveryArgs args);

        string TryGetInitializer(ParserDiscoveryArgs args, string parserName);
    }

    internal interface IComponentName {
        string Name { get; }
        string ToString();
    }

    internal struct GeneratedDependencyParser {
        public GeneratedType MainParser;
        public IReadOnlyList<GeneratedType> AdditionalParser;
    }

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
        public GeneratedDependencyParser CreateParsers(string uniqueName, MemberFormatInfo memberInfo, params IParserTransformBody[] transforms) {
            // Create the main depenedency
            // We wrap the functions from inner to outer
            Array.Reverse(transforms);
            if (transforms.Length == 0)
                transforms = NullTransform;
            var main = CreateMainDependency(uniqueName, memberInfo, transforms);
        }

        private GeneratedParser CreateMainDependency(string uniqueName, MemberFormatInfo memberInfo, IParserTransformBody[] reversedTransforms) {
            var targetTypeSymbol = memberInfo.Analyzed.ContainingFullType.TypeSymbol;
            var memberTypeSymbol = memberInfo.TypeSymbol;

            var node = new CodeNodeBuilder();
            node.AddAttribute(CommonAttributes.GeneratedCodeAttribute);
            node.AddAttribute(CommonAttributes.HideFromEditor);
            node.AddNode($"internal readonly struct {uniqueName} ", node => {
                // Start adding members
                foreach ((var component, var name) in Components) {
                    node.AddStatement($"private readonly {component.TypeName} {name}");
                }

                // Add dependencies
                // TODO

                // Create Constructor : public TypeDep (IParserDiscovery discovery) for non try methods
                node.AddNode($"public {uniqueName}({nameof(IParserDiscovery)} discovery)", node => {
                    // Create assignments
                    // component_0 = GetComponenet_0(discovery);
                    foreach ((var component, var name) in Components)
                        node.AddStatement($"{name} = Get{name}(discovery)");

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

                    // Finally if we complete every member's condition we say we succeed
                    node.AddStatement("isSuccess = true");
                    // Complete!

                    // This section covers nested functions for our component fetching process
                    foreach ((var component, var name) in Components)
                        node.AddNode($"bool TryGet{name}({nameof(IParserDiscovery)} discovery, out {component.TypeName} component)", node => {
                            node.AddPlain(component.TryGetInitializer("discovery", "component"));
                        });
                });

                #region Stateless Serialization / Deserialization Functions

                // public DeserializeResult TryDeserializeStateless(in {TargetType} type, ReadOnlySpan<byte> span, out int readBytes, out {TargetFieldType} data)
                node.AddAttribute(CommonAttributes.Inline);
                node.AddNode($"public {nameof(DeserializeResult)} TryDeserializeStateless(in {targetTypeSymbol} type, ReadOnlySpan<byte> span, out int readBytes, out {memberTypeSymbol} data)", node => {
                    node.AddPlain(TryDeserializeSpan("type", "span", "readBytes", "data"));
                });

                // public DeserializeResult TryDeserializeStateless(in {TargetType} type, ref SequenceCursor<byte> cursor, out {TargetFieldType} data)
                node.AddAttribute(CommonAttributes.Inline);
                node.AddNode($"public {nameof(DeserializeResult)} TryDeserializeStateless(in {targetTypeSymbol} type, ref SequenceCursor<byte> cursor, out {memberTypeSymbol} data)", node => {
                    node.AddPlain(TryDeserializeSequence("type", "cursor", "data"));
                });

                // public {TargetFieldType} DeserializeStateless(in {TargetType} type, ReadOnlySpan<byte> span, out int readBytes)
                node.AddAttribute(CommonAttributes.Inline);
                node.AddNode($"public {memberTypeSymbol} DeserializeStateless(in {targetTypeSymbol} type, ReadOnlySpan<byte> span, out int readBytes", node => {
                    node.AddPlain(DeserializeSpan("type", "span", "readBytes"));
                });

                // public {TargetFieldType} DeserializeStateless(in {TargetType} type, ref SequenceCursor<byte> cursor)
                node.AddAttribute(CommonAttributes.Inline);
                node.AddNode($"public {memberTypeSymbol} DeserializeStateless(in {targetTypeSymbol} type, ref SequenceCursor<byte> cursor)", node => {
                    node.AddPlain(DeserializeSequence("type", "cursor"));
                });

                // public bool TrySerializeUnsafeStateless(in {TargetType} type, in {TargetFieldType} data, Span<byte> dest, out int writtenBytes)
                node.AddAttribute(CommonAttributes.Inline);
                node.AddNode($"public bool TrySerializeUnsafeStateless(in {targetTypeSymbol} type, in {memberTypeSymbol} data, Span<byte> dest, out int writtenBytes)", node => {
                    node.AddPlain(TrySerializeUnsafeSpan("type", "data", "dest", "writtenBytes"));
                });

                // public int SerializeStateless(in {TargetType} type, in {TargetFieldType} data, Span<byte> dest)
                node.AddAttribute(CommonAttributes.Inline);
                node.AddNode($"public int SerializeStateless(in {targetTypeSymbol} type, in {memberTypeSymbol} data, Span<byte> dest)", node => {
                    node.AddPlain(SerializeUnsafeSpan("type", "data", "dest"));
                });

                #endregion

                // public bool TryDeserialize(in {TargetType} type, ReadOnlySpan<byte> span, out int readBytes, out {TargetFieldType} data, out DeserializeResult result)
                node.AddAttribute(CommonAttributes.Inline);
                node.AddNode($"public bool TryDeserialize(in {targetTypeSymbol} type, ReadOnlySpan<byte> span, out int readBytes, out {memberTypeSymbol} data, out {nameof(DeserializeResult)} result)", node => { 
                    
                });
            });
        }
    }
}
