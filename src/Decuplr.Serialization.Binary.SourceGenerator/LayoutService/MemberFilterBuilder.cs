using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Binary.AnalysisService;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal class MemberFilterBuilder {

        private class FilterImpl : IMemberFilter {
            private readonly Func<MemberMetaInfo, bool>? Filters;

            public IEnumerable<MemberMetaInfo> SelectMembers(IEnumerable<MemberMetaInfo> source) {
                foreach (var filter in Filters?.GetInvocationList() as Func<MemberMetaInfo, bool>[] ?? Enumerable.Empty<Func<MemberMetaInfo, bool>>()) {
                    source = source.Where(filter);
                }
                return source;
            }

            public FilterImpl(MemberFilterBuilder builder) {
                Filters = builder.Filters;
            }
        }

        private Func<MemberMetaInfo, bool>? Filters;

        public MemberFilterBuilder SelectMember(Func<MemberMetaInfo, bool> filter) {
            Filters += filter;
            return this;
        }

        public IMemberFilter Build() => new FilterImpl(this);

    }
}
