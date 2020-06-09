using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.Analyzers;
using Decuplr.Serialization.Binary.Annotations;
using Decuplr.Serialization.Binary.Annotations.Namespaces;
using Decuplr.Serialization.Binary.SourceGenerator.Schemas;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator.Providers {

    /*   [BinaryParser] comes with two flavor
     *   1. Native Base Type : Inherits TypeParser, these are natively consider "sealed" as they cannot be modified, unless it accepts `IParserDiscovery`
     *   2. Native Type : Implements IParserProvider or Inherits GenericParserProvider, they can never be sealed.
     *   2. Interpret Type : Accepts a certain type as constructor, and has ConvertTo() method that returns the same type, these are not sealed unless specified
     *
     */

    internal class BinaryParserSourceProvider : IParserGenerateSource {

        private struct BinaryParserInfo {
            public AnalyzedAttribute Attribute { get; set; }
            public IReadOnlyList<ITypeSymbol> TargetTypes { get; set; }
            public IReadOnlyList<string> Namespaces { get; set; }
        }

        public bool TryGenerateParser(IEnumerable<AnalyzedType> types, SourceGeneratorContext context, out IEnumerable<GeneratedParser> parsers) {
            parsers = Array.Empty<GeneratedParser>();
            var interestedType = types.Where(type => type.ContainsAttribute<BinaryParserAttribute>());
            var resultParsers = new List<GeneratedParser>();
            foreach (var type in interestedType) {

                var parserInfo = new BinaryParserInfo {
                    Attribute = type.GetAttributes<BinaryParserAttribute>().First(),
                    Namespaces = type.GetAttributes<BinaryParserNamespaceAttribute>().Select(x => (string)x.Data.ConstructorArguments[0].Value!).ToList(),
                };
                parserInfo.TargetTypes = parserInfo.Attribute.Data.ConstructorArguments[0].Values.Select(x => (ITypeSymbol)x.Value!).ToList();

                // Native Type
                if (type.Implements(typeof(IParserProvider<>)) || type.InheritFrom<GenericParserProvider>()) {
                    if (!TryGetNativeTypeParser(type, context, parserInfo, out var parser))
                        return false;
                    resultParsers.Add(parser);
                    continue;
                }
                // Since it inheriteds from TypeParser it's a native base type
                if (type.InheritFrom<TypeParser>()) {
                    if (!TryGetSealedTypeParser(type, context, parserInfo, out var parser))
                        return false;
                    resultParsers.Add(parser);
                    continue;
                }
                // Finally we think this is a schema type based parser
                if (!TryGetSchemaTypeParser(type, context, parserInfo, out var schemaParsers))
                    return false;
                resultParsers.AddRange(schemaParsers);
            }
            parsers = resultParsers;
            return true;
        }

        private bool TryReportUnwantedTypeStatement(SourceGeneratorContext context, BinaryParserInfo parserInfo, ITypeSymbol providedParserType, INamedTypeSymbol parser) {
            var targetTypes = parserInfo.TargetTypes;
            var attributeData = parserInfo.Attribute;
            if (targetTypes.Count != 0) {
                // It can't be target type with count over 1
                if (targetTypes.Count > 1) {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ParserProviderCountTooMuch, attributeData.Location, parser.Name));
                    return false;
                }
                // However if user explicitly state the wrong type of symbol, we dump error
                if (!targetTypes[0].Equals(providedParserType, SymbolEqualityComparer.Default)) {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ParserProviderTypeMismatch, attributeData.Location, providedParserType.Name, targetTypes[0]));
                    return false;
                }
                // Otherwise it's just uneccessary
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ParserProviderImplicitReport, attributeData.Location, providedParserType.Name));
            }
            return true;
        }

        private bool TryGetNativeTypeParser(AnalyzedType type, SourceGeneratorContext context, BinaryParserInfo parserInfo, out GeneratedParser parser) {
            parser = default;
            // So they provided IParserProvider / GenericParserProvider then we just need to specifiy the namespace
            // And also what type they are looking for
            var targetTypes = parserInfo.TargetTypes;
            var attributeData = parserInfo.Attribute;

            ITypeSymbol providedParserType;
            var isGenericParser = type.InheritFrom<GenericParserProvider>();
            if (isGenericParser) {
                if (targetTypes.Count != 1) {
                    if (targetTypes.Count == 0)
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ShouldStateTypeWithGenericParser, attributeData.Location, type.TypeSymbol.Name));
                    else
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ParserProviderCountTooMuch, attributeData.Location, type.TypeSymbol.Name));
                    return false;
                }
                providedParserType = targetTypes[0];
            }
            // Then we must be IParserProvider, in this case we just need to get the type from the generic
            else {
                providedParserType = type.TypeSymbol.TypeParameters[0];
                if (!TryReportUnwantedTypeStatement(context, parserInfo, providedParserType, type.TypeSymbol))
                    return false;
            }

            IParserKindProvider provider = isGenericParser ? (IParserKindProvider)new GenericParserKindProvider(providedParserType, type.TypeSymbol.ToString()) : new ParserProviderKindProvider(providedParserType, type.TypeSymbol.ToString());

            parser = new GeneratedParser {
                AdditionalSourceFiles = Array.Empty<GeneratedSourceCode>(),
                EmbeddedCode = default,
                ParserKinds = new IParserKindProvider[] { provider },
                ParserNamespaces = parserInfo.Namespaces.ToList(),
                ParserTypeName = providedParserType.Name
            };
            return true;
        }

        // Slightly native
        private bool TryGetSealedTypeParser(AnalyzedType type, SourceGeneratorContext context, BinaryParserInfo parserInfo, out GeneratedParser parser) {
            parser = default;
            var providedParserType = GetTypeParameter(type.TypeSymbol);
            // Sealed Native Type can only have default constructor and they are always sealed
            if (!type.TypeSymbol.Constructors.Any(x => x.Parameters.Length == 0)) {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.TypeParserRequiresDefaultConstructor, type.Declarations[0].DeclaredLocation, type.TypeSymbol.Name));
                return false;
            }
            // Sealed Native Type has the same limitation as IParserProvider when it comes to designating typeprovidedParserType = type.TypeSymbol.TypeParameters[0];
            if (!TryReportUnwantedTypeStatement(context, parserInfo, providedParserType, type.TypeSymbol))
                return false;
            parser = new GeneratedParser {
                ParserKinds = new IParserKindProvider[] { new SealedParserKindProvider(type.TypeSymbol.ToString(), false) },
                ParserNamespaces = parserInfo.Namespaces,
                ParserTypeName = type.TypeSymbol.Name
            };
            return true;

            ITypeSymbol GetTypeParameter(ITypeSymbol symbol) {
                while (symbol.BaseType != null) {
                    if (symbol.BaseType.IsGenericType 
                        && symbol.BaseType.ConstructUnboundGenericType().Equals(type.Analyzer.GetSymbol(typeof(TypeParser<>))!.ConstructUnboundGenericType(), SymbolEqualityComparer.Default))
                        return symbol.BaseType.TypeParameters[0];
                    symbol = symbol.BaseType;
                }
                throw new ArgumentException("Symbol doesn't inherit TypeParser<>");
            }
        }

        private bool TryGetSchemaTypeParser(AnalyzedType type, SourceGeneratorContext context, BinaryParserInfo parserInfo, out IEnumerable<GeneratedParser> parsers) {
            parsers = Array.Empty<GeneratedParser>();
            // First we need to capture all the types we wanted to be parsed
            // It much come with either combination, a constructor taking a type and with the type we have CovertTo (or implement ITypeConvertibale), or we have implicit operators

            // In common, we have constructors that take only one type, so we select that
            var explicitStateType = parserInfo.TargetTypes.Count != 0;
            if (!TryGetInputTypes(out var inputTypes))
                return false;
            
            bool TryGetInputTypes(out IReadOnlyList<ITypeSymbol> symbols) {
                var constructorEnumerable = type.TypeSymbol.Constructors
                    .Where(x => x.Parameters.Length == 1)
                    .Select(x => GetUnconstraintOrNormalType(x.Parameters[0].Type));
                var constructorTypes = new HashSet<ITypeSymbol>(constructorEnumerable);
                if (!explicitStateType) {
                    symbols = constructorTypes.ToList();
                    return true;
                }
                foreach (var designatedType in parserInfo.TargetTypes) {
                    if (!constructorTypes.Contains(designatedType)) {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ExplicitTypeMushHaveConstructor, parserInfo.Attribute.Location, designatedType));
                        symbols = Array.Empty<ITypeSymbol>();
                        return false;
                    }
                }
                symbols = parserInfo.TargetTypes;
                return true;
            }

            // Then we look if it
            //  1) Implements ITypeConvertable, if so, we just simply use that (and use casting to invoke, since it's always the first priority to use it)
            //  2) Implements ConvertTo"Something"
            // TODO:  3) Implements implict conversion operators (Not Implemented in this version)

            // Type, ConvertFunction combo
            var outputTypes = new Dictionary<ITypeSymbol, (Location Location, ITypeSymbol SourceSymbol, Func<string, string> Conversion)>();
            {
                // 1) we look for types that implement interface
                foreach (var interfaceType in type.TypeSymbol.AllInterfaces
                                                            .Where(x => x.IsGenericType)
                                                            .Where(x => x.ConstructUnboundGenericType().Equals(type.Analyzer.GetSymbol(typeof(ITypeConvertible<>)), SymbolEqualityComparer.Default))) {
                    outputTypes[GetUnconstraintOrNormalType(interfaceType.TypeArguments[0])] = (interfaceType.Locations[0], interfaceType.TypeArguments[0], itemName => $"(({interfaceType}){itemName}).ConvertTo()");
                }

                // 2) we look for functions that has "ConvertTo" signature
                foreach (var functionTypes in type.TypeSymbol.GetMembers()
                                                            .Where(x => x is IMethodSymbol && x.Name.StartsWith("ConvertTo"))
                                                            .Select(x => (IMethodSymbol)x)) {
                    // Warn user that this function would be ignored if the interface is implemented
                    var normalizedType = GetUnconstraintOrNormalType(functionTypes.ReturnType);
                    if (outputTypes.ContainsKey(normalizedType)) {
                        if (functionTypes.Name != "ConvertTo")
                            context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ConvertToIsIgnoredIfHasInterface, functionTypes.Locations[0], functionTypes.Name));
                        continue;
                    }
                    outputTypes[normalizedType] = (functionTypes.Locations[0], functionTypes.ReturnType, itemName => $"{itemName}.{functionTypes.Name}()");
                }

                // 3) we look for functions that has implicit conversion operators
                // Not implement by now, since I can't find a better way to understand / locate it
                /*
                var conversionOp = type.TypeSymbol.GetMembers().Where(x => x is IMethodSymbol)
                                                  .Select(x => (IMethodSymbol)x)
                                                  .Where(x => x.MethodKind == MethodKind.UserDefinedOperator)
                                                  .ToList();
                */
            }

            var finalTypes = new Dictionary<ITypeSymbol, (ITypeSymbol SourceSymbol, Func<string, string> Conversion)>();
            foreach(var inputType in inputTypes) {
                if (!outputTypes.ContainsKey(inputType) && explicitStateType) {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ExplicitTypeMushHaveConstructor, parserInfo.Attribute.Location, inputType.Name));
                    return false;
                }
                var (_, sourceSymbol, conversion) = outputTypes[inputType];
                var (key, value) = (inputType, (sourceSymbol, conversion));
                finalTypes.Add(key, value);
            }
            // Check if some outputTypes are actually missing out
            foreach(var outputType in outputTypes) {
                // Just a warning
                if (!finalTypes.ContainsKey(outputType.Key))
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.DeconstructIsIgnoredDueToNoConstructor, outputType.Value.Item1, outputType.Key));
            }
            // Check if there's actually qualified type
            if (finalTypes.Count == 0) {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.CannotFindMatchingParserType, type.Declarations[0].DeclaredLocation, type.TypeSymbol.Name));
                return false;
            }

            ////
            // Finally,
            // We fire up schema to type parser, then we would wrap it around our own flavor
            ////
            var attribute = parserInfo.Attribute;
            var schemaPrecusor = new SchemaPrecusor {
                IsSealed = attribute.Data.GetNamedArgumentValue<bool>(nameof(BinaryParserAttribute.Sealed)) ?? false,
                RequestLayout = attribute.Data.GetNamedArgumentValue<BinaryLayout>(nameof(BinaryParserAttribute.Layout)) ?? BinaryLayout.Auto,
                // We don't provide, "please don't deserialize option", you throw or deny deserialization at that level
                NeverDeserialize = false,
                // BinaryParser's namespace is provided by `ParserNamespace`
                TargetNamespaces = parserInfo.Namespaces
            };
            bool result = SchemaParserConverter.TryConvert(type, context.Compilation, schemaPrecusor, out IList<Diagnostic>? diagnostics, out var helperParser);
            foreach(Diagnostic? diagnostic in diagnostics) {
                context.ReportDiagnostic(diagnostic);
            }
            if (!result)
                return false;
            var parserlist = new List<GeneratedParser>{ helperParser };

            // Then we wrap the parser around so we can actually provider parser for our type
            foreach(var finalType in finalTypes) {
                var (className, sourceCode) = ParserProviderWrapper(type.TypeSymbol, finalType.Value.SourceSymbol, finalType.Value.Conversion);

                IParserKindProvider kindProvider;
                var embeddedCode = new EmbeddedCode {
                    CodeNamespaces = Array.Empty<string>(),
                    SourceCode = sourceCode
                };

                // If it's a generic type we wrap it with generic parser
                if (finalType.Key is INamedTypeSymbol namedType && namedType.IsUnboundGenericType) {
                    var provider = new GenericParserProviderWrapper(namedType, className);
                    embeddedCode = provider.Provide(embeddedCode, out kindProvider);
                }
                else {
                    var provider = new ParserProviderWrapper(finalType.Key, className);
                    embeddedCode = provider.Provide(embeddedCode, out kindProvider);
                }

                parserlist.Add(new GeneratedParser {
                    EmbeddedCode = embeddedCode,
                    ParserKinds = new IParserKindProvider[] { kindProvider },
                    ParserNamespaces = parserInfo.Namespaces,
                    ParserTypeName = finalType.Key.ToString()
                });
            }
            parsers = parserlist;
            return true;
        }

        private (string ClassName, string SourceCode) ParserProviderWrapper(ITypeSymbol parserProviderType, ITypeSymbol parsedType, Func<string, string> convertFunction) {
            var wrapperName = $"TypeParserWrapper_{parsedType.GetEmbedName()}_As_{parserProviderType.GetEmbedName()}";
            var node = new CodeNodeBuilder();

            node.AddNode($"private sealed class {wrapperName} : TypeParser<{parsedType}>", node => {
                node.AddStatement($"private readonly TypeParser<{parserProviderType}> Parser");
                node.AddNode($"public {wrapperName} (IParserDiscovery discovery)", node => {
                    node.AddStatement($"Parser = discovery.GetParser<{parserProviderType}>()");
                });

                node.AddNode($"public {wrapperName} (IParserDiscovery discovery, out bool isSuccess)", node => {
                    node.AddStatement($"isSuccess = discovery.TryGetParser<{parserProviderType}>(out Parser)");
                });

                node.AddStatement($"public override int? FixedSize => Parser.FixedSize");
                node.AddStatement($"public override bool TrySerialize({parsedType} value, Span<byte> destination, out int writtenBytes) => Parser.TrySerialize(new {parserProviderType}(value), destination, out writtenBytes)");
                node.AddStatement($"public override int Serialize({parsedType} value, Span<byte> destination) => Parser.Serialize(new {parserProviderType}(value), destination)");
                node.AddStatement($"public override int GetBinaryLength({parsedType} value) => Parser.GetBinaryLength(new {parserProviderType}(value))");
                node.AddNode($"public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out {parsedType} result)", node => {
                    node.AddStatement("var deserializeResult = Parser.TryDeserialize(span, out readBytes, out var ogResult)");
                    node.AddStatement($"result = {convertFunction("ogResult")}");
                    node.AddStatement("return deserializeResult");
                });
            });

            return (wrapperName, node.ToString());
        }

        private ITypeSymbol GetUnconstraintOrNormalType(ITypeSymbol symbol) {
            if (symbol is INamedTypeSymbol namedSymbol && namedSymbol.IsGenericType)
                return namedSymbol.ConstructUnboundGenericType();
            return symbol;
        }

    }
}
