using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {

    public partial class AttributeLayout<TAttribute> : AttributeLayout where TAttribute : Attribute {

        public AttributeLayout(AttributeLayout source)
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
            _ = AsInfo<ObsoleteAttribute>().TryGetConstructorArgument(x => new ObsoleteAttribute(x), out string? result);
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
                var expressArgType = newExpress.Arguments[i].Type;
                if (arg.Kind == TypedConstantKind.Array && arg.Values.GetType().Equals(expressArgType))
                    continue;
                Debug.Assert(arg.Value is { });
                if (arg.Value.GetType().Equals(expressArgType))
                    continue;
                goto ReturnError;
            }

            // Initialize the list with -1, so we can see if any is not found later
            var locs = new List<int>(Enumerable.Repeat(-1, expression.Parameters.Count));
            for (var i = 0; i < newExpress.Arguments.Count; ++i) {
                // If it's a parameter just ignore
                if (!(newExpress.Arguments[i] is ParameterExpression parameter))
                    continue;
                // Loop through the expression to look for matching name location
                for (var j = 0; j < expression.Parameters.Count; ++j) {
                    // When the x => new T(0,x), x equals
                    if (expression.Parameters[j].Name == parameter.Name)
                        locs[j] = i;
                }
            }

            // Check if we have duplicate in the arguments and any have -1
            var nonDuplicateCount = new HashSet<int>(locs);
            if (locs.Any(x => x == -1 || !nonDuplicateCount.Add(x)))
                goto ReturnError;

            locations = locs;
            return true;

            ReturnError:
            {
                locations = Array.Empty<int>();
                return false;
            }
        }

    }

    public class AttributeLayout : IComparable<AttributeLayout> {

        private class StrictComparingRules : IComparer<AttributeLayout> {
            public int Compare(AttributeLayout x, AttributeLayout y) => x.OrderComparedTo(y) ?? throw new InvalidOperationException($"Unable to compare the order due to two attributes located in different source locations");
        }

        internal AttributeLayout<TAttribute> AsInfo<TAttribute>() where TAttribute : Attribute => new AttributeLayout<TAttribute>(this);

        private readonly Location? _symbolLocation;

        protected AttributeData AttributeData { get; }

        public static IComparer<AttributeLayout> StrictOrderComparer { get; } = new StrictComparingRules();

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

        public AttributeLayout(AttributeData attributeData, Location? symbolLocation) {
            AttributeData = attributeData;
            _symbolLocation = symbolLocation;
        }

        protected AttributeLayout(AttributeLayout sourceInfo) {
            AttributeData = sourceInfo.AttributeData;
            _symbolLocation = sourceInfo._symbolLocation;
        }

        public int? OrderComparedTo(AttributeLayout attribute) {
            if (attribute is null)
                throw new ArgumentNullException(nameof(attribute));
            if (_symbolLocation is null || attribute._symbolLocation is null || !_symbolLocation.Equals(attribute._symbolLocation))
                return null;
            return ApplicationSyntaxReference.Span.CompareTo(attribute.ApplicationSyntaxReference.Span);
        }

        int IComparable<AttributeLayout>.CompareTo(AttributeLayout other)
            => OrderComparedTo(other) ?? 0;

        public static implicit operator AttributeData(AttributeLayout info) => info.AttributeData;
    }

}
