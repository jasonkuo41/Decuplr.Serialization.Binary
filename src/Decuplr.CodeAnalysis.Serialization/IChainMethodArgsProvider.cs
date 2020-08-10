using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IChainMethodArgsProvider {
        void AssertReturn<TReturn>();
        void AssertReturn(Type type);
        void AssertReturn(ITypeSymbol symbol);
        string this [Type type] { get; }
        string this [Type type, int index] { get; }
        string this [ITypeSymbol symbol] { get; }
        string this [ITypeSymbol symbol, int index] { get; }

        string InvokeNextMethod();
        string InvokeNextMethod(Action<IChainMethodInvokeAction> action);
    }

    public interface IChainMethodInvokeAction {
        string this[Type type] { get; set;  }
        string this[Type type, int index] { get; set; }
        string this[ITypeSymbol symbol] { get; set; }
        string this[ITypeSymbol symbol, int index] { get; set; }
    }
}
