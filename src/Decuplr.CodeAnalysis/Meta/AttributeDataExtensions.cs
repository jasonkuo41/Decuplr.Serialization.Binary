using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Meta {
    public static class AttributeDataExtensions {

        private static string GetMemberName<T>(Expression<T> expression) => expression.Body switch
        {
            MemberExpression memberExpression => memberExpression.Member.Name,
            UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression memberExpression => memberExpression.Member.Name,
            _ => throw new NotImplementedException(expression.GetType().ToString())
        };

        public static TValue? GetMember<TAttribute, TValue>(this AttributeData attribute, Expression<Func<TAttribute, TValue>> expression) where TValue : struct {
            var propertyName = GetMemberName(expression);
            return (TValue?)attribute.NamedArguments.FirstOrDefault(x => x.Key == propertyName).Value.Value;
        }

        public static object? GetMember<TAttribute>(this AttributeData attribute, Expression<Func<TAttribute, object>> expression) {
            var propertyName = GetMemberName(expression);
            return attribute.NamedArguments.FirstOrDefault(x => x.Key == propertyName).Value.Value;
        }

        public static Type? GetMember<TAttribute>(this AttributeData attribute, Expression<Func<TAttribute, Type>> expression) {
            var propertyName = GetMemberName(expression);
            return (Type?)attribute.NamedArguments.FirstOrDefault(x => x.Key == propertyName).Value.Value;
        }

    }
}
