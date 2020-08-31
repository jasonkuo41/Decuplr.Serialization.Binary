using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {


    public interface INamedTypeInfo {



    }

    public interface IValueMemberInfo {

    }

    public interface IReturnValueInfo {
        ISymbol OriginalSymbol { get; }
        TypeName TypeName { get; }
    }

    public interface IAttributeDataCollection {

    }

    internal class NamedTypeInfo {

        public INamedTypeSymbol OriginalSymbol { get; }

        public IReadOnlyList<IValueMemberInfo> Members { get; }

        public IAttributeDataCollection Attributes { get; }

        public bool IsPartial => OriginalSymbol.Locations.Length > 1;

        public NamedTypeInfo(INamedTypeSymbol symbol) {
            OriginalSymbol = symbol;
        }
    }

    internal class ValueMemberInfo : IValueMemberInfo {
        public ISymbol OriginalSymbol { get; }

        public string Name { get; }

        public bool IsStatic => OriginalSymbol.IsStatic;

        public NamedTypeInfo ContainingType { get; }

        public Location Location => OriginalSymbol.Locations[0];

        public IAttributeDataCollection Attributes { get; }

        public IReturnValueInfo ReturnValue { get; }
    }

    internal class AttributeDataCollection {

    }

}
