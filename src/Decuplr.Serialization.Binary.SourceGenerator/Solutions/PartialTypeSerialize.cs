﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator.Solutions {
    class PartialTypeSerialize : ISerializeSolution {

        private readonly TypeFormatInfo TypeInfo;

        private INamedTypeSymbol TypeSymbol => TypeInfo.TypeSymbol;
        private IReadOnlyList<MemberFormatInfo> Members => TypeInfo.Members;
        
        private string OutArgs => string.Join(",", Enumerable.Range(0, TypeInfo.Members.Count).Select(i => $"out s_{i}"));
        private string OutArgsWithType => string.Join(",", Enumerable.Range(0, TypeInfo.Members.Count).Select(i => $"out {Members[i].MemberTypeSymbol} s_{i}"));

        public PartialTypeSerialize(TypeFormatInfo typeInfo) {
            TypeInfo = typeInfo;
        }

        internal static string DefaultDeserializePoint(INamedTypeSymbol symbol) => $"___generated__no_invoke_{symbol.Name}_Derializer";

        public GeneratedSourceCode[] GetAdditionalFiles() {
            var builder = new CodeSnippetBuilder(TypeSymbol.ContainingNamespace.ToString());
            builder.Using("System");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");
            builder.Using("Decuplr.Serialization");

            builder.AddPlain("// This file is automatically generated by Decuplr.Serilization.Binary library");
            builder.AddPlain("// For more information, see https://decuplr.dev/serialization/binary");
            builder.AddPlain("");
            builder.AddPlain($"// Debug Info : Built by {nameof(PartialTypeSerialize)}");

            // {public} {partial} {class/ struct} Name {
            builder.AddPartialClass(TypeSymbol, node => {

                node.AddAttribute($"[GeneratedCode (\"{Assembly.GetExecutingAssembly().GetName().Name}\", \"{Assembly.GetExecutingAssembly().GetName().Version}\")]");
                node.AddAttribute("[EditorBrowsable(EditorBrowsableState.Never)]");

                node.AddNode($"internal static void {DefaultDeserializePoint(TypeSymbol)} ({TypeSymbol} value, {OutArgsWithType})", node => {
                    for(var i = 0; i < Members.Count; ++i)
                        node.AddStatement($"s_{i} = value.{Members[i].MemberSymbol.Name}");
                });

            });
            return new GeneratedSourceCode[] { ($"{TypeInfo.TypeSymbol.Name}.Generated.PartialSerialize.cs", builder.ToString()) };
        }

        public GeneratedFormatFunction GetSerializeFunction() {
            var builder = new CodeNodeBuilder();
            builder.AddNode($"private static void DeconstructType({TypeSymbol} value, {OutArgsWithType})", node => {
                node.AddStatement($"{TypeSymbol}.{DefaultDeserializePoint(TypeSymbol)} (value, {OutArgs});");
            });
            return new GeneratedFormatFunction("DeconstructType", builder.ToString());
        }
    }
}
