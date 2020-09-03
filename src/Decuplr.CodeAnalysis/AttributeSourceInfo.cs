using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {

    public class AttributeSourceInfo<TAttribute> : AttributeSourceInfo where TAttribute : Attribute {

        public AttributeSourceInfo(AttributeSourceInfo source) 
            : base(source) {
        }

        private static string GetMemberName<T>(Expression<T> expression) => expression.Body switch
        {
            MemberExpression memberExpression => memberExpression.Member.Name,
            UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression memberExpression => memberExpression.Member.Name,
            _ => throw new NotImplementedException(expression.GetType().ToString())
        };

        [return: MaybeNull]
        private bool GetValue<TValue>(TypedConstant constValue, [MaybeNullWhen(false)] out TValue value) {
            if (constValue.Value is null) {
                value = default;
                return false;
            }
            if (constValue.Kind == TypedConstantKind.Array)
                value = (TValue)(object)constValue.Values.Select(x => x.Value).ToArray();
            else
                value = (TValue)constValue.Value;
            return true;
        }

        [return: MaybeNull]
        public TValue GetNamedArgument<TValue>(Expression<Func<TAttribute, TValue>> expression) {
            var propertyName = GetMemberName(expression); 
            GetValue<TValue>(AttributeData.NamedArguments.FirstOrDefault(x => x.Key == propertyName).Value, out var value);
            return value;
        }

        private bool ValidateConstructorArguments(LambdaExpression expression, out IReadOnlyList<int> locations) {
            var constArgs = AttributeData.ConstructorArguments;
            var newExpress = expression.Body as NewExpression ?? throw new ArgumentException("Expression is not a new expression", nameof(expression));
            if (newExpress.Arguments.Count != constArgs.Length)
                goto ReturnError;

            for (var i = 0; i < newExpress.Arguments.Count; ++i) {
                var arg = constArgs[i];
                if (arg.Kind == TypedConstantKind.Array && arg.Values[0].Value!.GetType().MakeArrayType().Equals(newExpress.Arguments[i].Type))
                    continue;
                Debug.Assert(arg.Value is { });
                if (arg.Value.GetType().Equals(newExpress.Arguments[i].Type))
                    continue;
                goto ReturnError;
            }

            var locs = new List<int>();
            for (var i = 0; i < newExpress.Arguments.Count; ++i) {
                if (newExpress.Arguments[i] is ParameterExpression) {
                    locs.Add(i);
                }
            }
            if (locs.Count < 0)
                goto ReturnError;

            locations = locs;
            return true;

            ReturnError:
            {
                locations = Array.Empty<int>();
                return false;
            }
        }

        public bool TryGetConstructorArgument<T>(Expression<Func<T, TAttribute>> expression, [MaybeNullWhen(false)] out T value) {
            ValidateConstructorArguments(expression, out var locations);
            return GetValue(AttributeData.ConstructorArguments[locations[0]], out value);
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
    }

    public class AttributeSourceInfo : IComparable<AttributeSourceInfo> {

        private class StrictComparingRules : IComparer<AttributeSourceInfo> {
            public int Compare(AttributeSourceInfo x, AttributeSourceInfo y) => x.OrderComparedTo(y) ?? throw new InvalidOperationException($"Unable to compare the order due to two attributes located in different source locations");
        }

        internal AttributeSourceInfo<TAttribute> AsInfo<TAttribute>() where TAttribute : Attribute => new AttributeSourceInfo<TAttribute>(this);

        private readonly Location? _symbolLocation;

        protected AttributeData AttributeData { get; }

        public static IComparer<AttributeSourceInfo> StrictOrderComparer { get; } = new StrictComparingRules();

        public INamedTypeSymbol? AttributeClass => AttributeData.AttributeClass;

        public IMethodSymbol? AttributeConstructor => AttributeData.AttributeConstructor;

        public SyntaxReference ApplicationSyntaxReference { 
            get {
                Debug.Assert(AttributeData.ApplicationSyntaxReference is { });
                return AttributeData.ApplicationSyntaxReference;
            } 
        }

        public ImmutableArray<TypedConstant> ConstructorArguments => AttributeData.ConstructorArguments;

        public ImmutableArray<KeyValuePair<string, TypedConstant>> NamedArguments => AttributeData.NamedArguments;

        public AttributeSourceInfo(AttributeData attributeData, Location? symbolLocation) {
            AttributeData = attributeData;
            _symbolLocation = symbolLocation;
        }

        protected AttributeSourceInfo(AttributeSourceInfo sourceInfo) {
            AttributeData = sourceInfo.AttributeData;
            _symbolLocation = sourceInfo._symbolLocation;
        }

        public int? OrderComparedTo(AttributeSourceInfo attribute) {
            if (attribute is null)
                throw new ArgumentNullException(nameof(attribute));
            if (_symbolLocation is null || attribute._symbolLocation is null || !_symbolLocation.Equals(attribute._symbolLocation))
                return null;
            return ApplicationSyntaxReference.Span.CompareTo(attribute.ApplicationSyntaxReference.Span);
        }

        int IComparable<AttributeSourceInfo>.CompareTo(AttributeSourceInfo other) 
            => OrderComparedTo(other) ?? 0;

        public static implicit operator AttributeData(AttributeSourceInfo info) => info.AttributeData;
    }

}
