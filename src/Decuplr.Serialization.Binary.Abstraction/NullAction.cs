using System;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// The action to take when serializer encounters a null value
    /// </summary>
    public enum NullAction {
        /// <summary>
        /// The default action
        /// </summary>
        Default = 0,

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> wrapped in <see cref="SerializationFaultException"/>
        /// </summary>
        Throw,

        /// <summary>
        /// Always appends a fixed <see cref="bool"/> to any member that can be <see cref="null"/>, for example reference types and <see cref="Nullable{T}"/>
        /// </summary>
        AppendFixedBool,

        /// <summary>
        /// Treating null as if it's type member are all set to <see cref="default"/> value. When deserialized, they become a default object and not null, the null info will be lost
        /// </summary>
        DefaultValue
    }
}
