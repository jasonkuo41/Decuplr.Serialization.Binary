using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.Serialization.Annotations;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.ConditionResolver {

    // Handles both IgnoreIf and IgnoreIfNot
    internal class IgnoreIfConditionProvider : IConditionResolverProvider {
        
        private readonly IConditionAnalyzer _analyzer;

        public IgnoreIfConditionProvider(IConditionAnalyzer analyzer) {
            _analyzer = analyzer;
        }

        private static Condition GetAttributeCondition(AttributeData data) {
            // Uses the first constructor (string)
            if (data.ConstructorArguments.Length == 1)
                return new Condition {
                    SourceName = (string)data.ConstructorArguments[0].Value!,
                    Operator = Operator.Equal,
                    ComparedValue = true
                };
            // second constructor (string, object)
            if (data.ConstructorArguments.Length == 2)
                return new Condition {
                    SourceName = (string)data.ConstructorArguments[0].Value!,
                    Operator = Operator.Equal,
                    ComparedValue = data.ConstructorArguments[1].Value!
                };
            // third constructor (string, operator, object)
            if (data.ConstructorArguments.Length == 3)
                return new Condition {
                    SourceName = (string)data.ConstructorArguments[0].Value!,
                    Operator = (Operator)data.ConstructorArguments[1].Value!,
                    ComparedValue = data.ConstructorArguments[2].Value!
                };
            throw new ArgumentException("Invalid attribute data (too much constructor args", nameof(data));
        }

        private IEnumerable<IConditionResolver> GetResolvers<TAttribute>(MemberMetaInfo member, bool isInverted) where TAttribute : Attribute
            => member.GetAttributes<TAttribute>().Select(attr => new IgnoreIfResolver(member, GetAttributeCondition(attr), _analyzer, isInverted));

        public void ConfigureValidation(IFluentTypeGroupValidator validator) {
            validator.SelectedMembers
                     .WhereAttribute<IgnoreIfAttribute>()
                     .VerifyCondition(GetAttributeCondition);

            validator.SelectedMembers
                     .WhereAttribute<IgnoreIfNotAttribute>()
                     .VerifyCondition(GetAttributeCondition);
        }

        public bool ShouldFormat(MemberMetaInfo member) => member.ContainsAttribute<IgnoreIfAttribute>() || member.ContainsAttribute<IgnoreIfNotAttribute>();

        public IEnumerable<IConditionResolver> GetResolvers(MemberMetaInfo member, IThrowCollection throwCollection) {
            var ignoreIf = GetResolvers<IgnoreIfAttribute>(member, isInverted: false);
            var ignoreIfNot = GetResolvers<IgnoreIfNotAttribute>(member, isInverted: true);
            return ignoreIf.Concat(ignoreIfNot);
        }

    }
}
