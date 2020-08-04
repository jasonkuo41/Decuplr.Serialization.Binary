using System;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.Serialization.Annotations;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.ConditionResolver {

    // Handles both IgnoreIf and IgnoreIfNot
    internal class IgnoreIfConditionProvider : IConditionResolverProvider {
        private class IgnoreIf : IgnoreIfConditionBase {
            protected override bool IsInverted => false;
        }

        private class IgnoreIfNot : IgnoreIfConditionBase {
            protected override bool IsInverted => true;
        }

        public void ConfigureValidation(IFluentTypeGroupValidator validator) {
            validator.SelectedMembers
                     .WhereAttribute<IgnoreIfAttribute>()
                     .VerifyCondition(GetAttributeCondition);

            validator.SelectedMembers
                     .WhereAttribute<IgnoreIfNotAttribute>()
                     .VerifyCondition(GetAttributeCondition);

            static Condition GetAttributeCondition(AttributeData data) {
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
        }

        public IConditionResolver GetResolver(MemberMetaInfo member, IThrowCollection throwCollection) {
            throw new NotImplementedException();
        }

        public bool ShouldFormat(MemberMetaInfo member) {
            throw new NotImplementedException();
        }
    }
}
