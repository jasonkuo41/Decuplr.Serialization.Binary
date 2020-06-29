using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService.Internal {
    static class ImplicitConvertible {


        // These don't support implicit conversion : bool, Type, string
        private static readonly Dictionary<SpecialType, HashSet<Type>> ConvertibleTable = new Dictionary<SpecialType, HashSet<Type>> {
            [SpecialType.System_Boolean]
            = CreateSet(typeof(bool)),

            [SpecialType.System_Char]
            = CreateSet(typeof(char), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)),

            [SpecialType.System_SByte]
            = CreateSet(typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal)),

            [SpecialType.System_Byte]
            = CreateSet(typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)),

            [SpecialType.System_Int16]
            = CreateSet(typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal)),

            [SpecialType.System_UInt16]
            = CreateSet(typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)),

            [SpecialType.System_Int32]
            = CreateSet(typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal)),

            [SpecialType.System_UInt32]
            = CreateSet(typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)),

            [SpecialType.System_Int64]
            = CreateSet(typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)),

            [SpecialType.System_UInt64]
            = CreateSet(typeof(ulong), typeof(float), typeof(double), typeof(decimal)),

            [SpecialType.System_Single]
            = CreateSet(typeof(float), typeof(double)),

            [SpecialType.System_Double]
            = CreateSet(typeof(double)),
        };

        private static HashSet<Type> CreateSet(params Type[] types) => new HashSet<Type>(types.Concat(types.Select(x => typeof(Nullable<>).MakeGenericType(x))));

        public static bool IsImplicitConvertibleFrom(this ITypeSymbol symbol, Type target) {
            if (!ConvertibleTable.TryGetValue(symbol.SpecialType, out var set))
                return false;
            return set.Contains(target);
        }
    }
}
