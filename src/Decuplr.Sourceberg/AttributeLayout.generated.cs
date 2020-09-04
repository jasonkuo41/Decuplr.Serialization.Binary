using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
	public partial class AttributeLayout<TAttribute> {
		public bool TryGetConstructorArgument<T0>(Expression<Func<T0, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1>(Expression<Func<T0, T1, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2>(Expression<Func<T0, T1, T2, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3>(Expression<Func<T0, T1, T2, T3, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4>(Expression<Func<T0, T1, T2, T3, T4, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5>(Expression<Func<T0, T1, T2, T3, T4, T5, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6, [MaybeNullWhen(false)] out T7 value7) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[7]], out value7);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
			Debug.Assert(value7 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6, [MaybeNullWhen(false)] out T7 value7, [MaybeNullWhen(false)] out T8 value8) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[7]], out value7);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[8]], out value8);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
			Debug.Assert(value7 != null);
			Debug.Assert(value8 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6, [MaybeNullWhen(false)] out T7 value7, [MaybeNullWhen(false)] out T8 value8, [MaybeNullWhen(false)] out T9 value9) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[7]], out value7);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[8]], out value8);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[9]], out value9);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
			Debug.Assert(value7 != null);
			Debug.Assert(value8 != null);
			Debug.Assert(value9 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
				value9 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6, [MaybeNullWhen(false)] out T7 value7, [MaybeNullWhen(false)] out T8 value8, [MaybeNullWhen(false)] out T9 value9, [MaybeNullWhen(false)] out T10 value10) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[7]], out value7);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[8]], out value8);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[9]], out value9);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[10]], out value10);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
			Debug.Assert(value7 != null);
			Debug.Assert(value8 != null);
			Debug.Assert(value9 != null);
			Debug.Assert(value10 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
				value9 = default;
				value10 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6, [MaybeNullWhen(false)] out T7 value7, [MaybeNullWhen(false)] out T8 value8, [MaybeNullWhen(false)] out T9 value9, [MaybeNullWhen(false)] out T10 value10, [MaybeNullWhen(false)] out T11 value11) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[7]], out value7);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[8]], out value8);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[9]], out value9);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[10]], out value10);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[11]], out value11);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
			Debug.Assert(value7 != null);
			Debug.Assert(value8 != null);
			Debug.Assert(value9 != null);
			Debug.Assert(value10 != null);
			Debug.Assert(value11 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
				value9 = default;
				value10 = default;
				value11 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6, [MaybeNullWhen(false)] out T7 value7, [MaybeNullWhen(false)] out T8 value8, [MaybeNullWhen(false)] out T9 value9, [MaybeNullWhen(false)] out T10 value10, [MaybeNullWhen(false)] out T11 value11, [MaybeNullWhen(false)] out T12 value12) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[7]], out value7);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[8]], out value8);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[9]], out value9);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[10]], out value10);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[11]], out value11);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[12]], out value12);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
			Debug.Assert(value7 != null);
			Debug.Assert(value8 != null);
			Debug.Assert(value9 != null);
			Debug.Assert(value10 != null);
			Debug.Assert(value11 != null);
			Debug.Assert(value12 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
				value9 = default;
				value10 = default;
				value11 = default;
				value12 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6, [MaybeNullWhen(false)] out T7 value7, [MaybeNullWhen(false)] out T8 value8, [MaybeNullWhen(false)] out T9 value9, [MaybeNullWhen(false)] out T10 value10, [MaybeNullWhen(false)] out T11 value11, [MaybeNullWhen(false)] out T12 value12, [MaybeNullWhen(false)] out T13 value13) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[7]], out value7);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[8]], out value8);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[9]], out value9);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[10]], out value10);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[11]], out value11);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[12]], out value12);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[13]], out value13);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
			Debug.Assert(value7 != null);
			Debug.Assert(value8 != null);
			Debug.Assert(value9 != null);
			Debug.Assert(value10 != null);
			Debug.Assert(value11 != null);
			Debug.Assert(value12 != null);
			Debug.Assert(value13 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
				value9 = default;
				value10 = default;
				value11 = default;
				value12 = default;
				value13 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6, [MaybeNullWhen(false)] out T7 value7, [MaybeNullWhen(false)] out T8 value8, [MaybeNullWhen(false)] out T9 value9, [MaybeNullWhen(false)] out T10 value10, [MaybeNullWhen(false)] out T11 value11, [MaybeNullWhen(false)] out T12 value12, [MaybeNullWhen(false)] out T13 value13, [MaybeNullWhen(false)] out T14 value14) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[7]], out value7);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[8]], out value8);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[9]], out value9);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[10]], out value10);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[11]], out value11);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[12]], out value12);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[13]], out value13);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[14]], out value14);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
			Debug.Assert(value7 != null);
			Debug.Assert(value8 != null);
			Debug.Assert(value9 != null);
			Debug.Assert(value10 != null);
			Debug.Assert(value11 != null);
			Debug.Assert(value12 != null);
			Debug.Assert(value13 != null);
			Debug.Assert(value14 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
				value9 = default;
				value10 = default;
				value11 = default;
				value12 = default;
				value13 = default;
				value14 = default;
		        return false;
		    }
		}

		public bool TryGetConstructorArgument<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TAttribute>> expression, [MaybeNullWhen(false)] out T0 value0, [MaybeNullWhen(false)] out T1 value1, [MaybeNullWhen(false)] out T2 value2, [MaybeNullWhen(false)] out T3 value3, [MaybeNullWhen(false)] out T4 value4, [MaybeNullWhen(false)] out T5 value5, [MaybeNullWhen(false)] out T6 value6, [MaybeNullWhen(false)] out T7 value7, [MaybeNullWhen(false)] out T8 value8, [MaybeNullWhen(false)] out T9 value9, [MaybeNullWhen(false)] out T10 value10, [MaybeNullWhen(false)] out T11 value11, [MaybeNullWhen(false)] out T12 value12, [MaybeNullWhen(false)] out T13 value13, [MaybeNullWhen(false)] out T14 value14, [MaybeNullWhen(false)] out T15 value15) {
			if (!ValidateConstructorArguments(expression, out var locations))
		        goto ReturnError;

		    var isSuccess = true;
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[0]], out value0);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[1]], out value1);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[2]], out value2);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[3]], out value3);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[4]], out value4);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[5]], out value5);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[6]], out value6);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[7]], out value7);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[8]], out value8);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[9]], out value9);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[10]], out value10);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[11]], out value11);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[12]], out value12);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[13]], out value13);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[14]], out value14);
			isSuccess &= GetValue(AttributeData.ConstructorArguments[locations[15]], out value15);
		    
		    if (!isSuccess)
		        goto ReturnError;

			Debug.Assert(value0 != null);
			Debug.Assert(value1 != null);
			Debug.Assert(value2 != null);
			Debug.Assert(value3 != null);
			Debug.Assert(value4 != null);
			Debug.Assert(value5 != null);
			Debug.Assert(value6 != null);
			Debug.Assert(value7 != null);
			Debug.Assert(value8 != null);
			Debug.Assert(value9 != null);
			Debug.Assert(value10 != null);
			Debug.Assert(value11 != null);
			Debug.Assert(value12 != null);
			Debug.Assert(value13 != null);
			Debug.Assert(value14 != null);
			Debug.Assert(value15 != null);
		    return true;

		    ReturnError:
		    {
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
				value9 = default;
				value10 = default;
				value11 = default;
				value12 = default;
				value13 = default;
				value14 = default;
				value15 = default;
		        return false;
		    }
		}

	}
	
}