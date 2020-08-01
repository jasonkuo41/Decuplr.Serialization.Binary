using System;
using System.Linq;
using System.Linq.Expressions;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary {
    internal class AttributeInspector<TAttribute> where TAttribute : Attribute {

        private readonly AttributeData _attribute;

        public AttributeInspector(AttributeData data) {
            _attribute = data;
        }

        private static string GetMemberName<T>(Expression<T> expression) => expression.Body switch
        {
            MemberExpression memberExpression => memberExpression.Member.Name,
            UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression memberExpression => memberExpression.Member.Name,
            _ => throw new NotImplementedException(expression.GetType().ToString())
        };

        public TValue GetSingleValue<TValue>(Expression<Func<TAttribute, TValue>> expression) where TValue : struct {
            var propertyName = GetMemberName(expression);
            var value = _attribute.NamedArguments.FirstOrDefault(x => x.Key == propertyName).Value.Value;
            return (TValue?)value ?? default;
        }

        public TValue? GetSingleObject<TValue>(Expression<Func<TAttribute, TValue>> expression) where TValue : class {
            var propertyName = GetMemberName(expression);
            var value = _attribute.NamedArguments.FirstOrDefault(x => x.Key == propertyName).Value.Value;
            if (value is null)
                return default;
            return (TValue)value;
        }

    }

    internal static class InspectorExtensions {
        public static AttributeInspector<TAttribute> ToInspector<TAttribute>(this AttributeData data) where TAttribute : Attribute => new AttributeInspector<TAttribute>(data);
        public static AttributeInspector<TAttribute>? GetAttributeInspector<TAttribute>(this NamedTypeMetaInfo data) where TAttribute : Attribute {
            return data.GetAttributes<TAttribute>().FirstOrDefault()?.ToInspector<TAttribute>();
        }
    }
}
