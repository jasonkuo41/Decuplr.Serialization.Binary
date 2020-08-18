using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary {

    public interface IBinaryWriteState<TRef> where TRef : IBinaryWriteState<TRef> {
        bool Write<TKind>(in TKind item, out TRef state);
    }

    public struct BinaryWriteState : IBinaryWriteState<BinaryWriteState> {
        private readonly int MaxDepthCount;
        private readonly ImmutableHashSet<object>? TraversedObjects;

        private int Depth;

        private BinaryWriteState(int maxDepthCount) {
            if (maxDepthCount < 1)
                throw new ArgumentOutOfRangeException(nameof(maxDepthCount), maxDepthCount, "Max Depth Count cannot be smaller then 1");
            MaxDepthCount = maxDepthCount;
            Depth = 0;
            TraversedObjects = null;
        }

        private BinaryWriteState(ImmutableHashSet<object> set, bool ignoreDuplicate) {
            MaxDepthCount = ignoreDuplicate ? 1 : 0;
            Depth = 0;
            TraversedObjects = set;
        }

        private BinaryWriteState(ImmutableHashSet<object> set, int maxDepthCount) {
            MaxDepthCount = maxDepthCount;
            Depth = 0;
            TraversedObjects = set;
        }

        private BinaryWriteState? WriteSet(object item) {

            Debug.Assert(TraversedObjects != null);

            var newSet = TraversedObjects.Add(item);
            if (!TraversedObjects.Equals(newSet))
                return new BinaryWriteState(newSet, MaxDepthCount);
            // Ignore Duplicate
            if (MaxDepthCount == 1)
                return null;
            // Throw
            ThrowHelper.ThrowCircularRef(item);
            return this;
        }

        private BinaryWriteState? WriteDepth() {
            if (Depth++ > MaxDepthCount)
                throw new Exception();
            return this;
        }

        public BinaryWriteState? Write(object item) {
            if (TraversedObjects != null)
                return WriteSet(item);
            if (MaxDepthCount > 0)
                return WriteDepth();
            return this;
        }

        public static BinaryWriteState None => default;
        public static BinaryWriteState DepthOnly(int maxDepth) => new BinaryWriteState(maxDepth);

        public bool Write(out IBinaryWriteState<BinaryWriteState> state) {
            throw new NotImplementedException();
        }

        public bool Write(object item, out BinaryWriteState state) {
            throw new NotImplementedException();
        }

        public static BinaryWriteState ThrowDuplicate => new BinaryWriteState(ImmutableHashSet.Create<object>(), ignoreDuplicate: false);
        public static BinaryWriteState IgnoreDuplicate => new BinaryWriteState(ImmutableHashSet.Create<object>(), ignoreDuplicate: true);

        private class ThrowHelper {
            [DoesNotReturn]
            public static void ThrowCircularRef(object item) => throw CreateCircularRefException(item);

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static Exception CreateCircularRefException(object item) => new BinarySerializationException();
        }
    }

}
