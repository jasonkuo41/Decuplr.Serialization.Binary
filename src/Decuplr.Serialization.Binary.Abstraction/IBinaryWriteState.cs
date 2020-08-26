namespace Decuplr.Serialization.Binary {

    /// <summary>
    /// Represents a high performance write state that captures info of what has been serialized.
    /// </summary>
    /// <typeparam name="TRef">The self referencing generic</typeparam>
    public interface IBinaryWriteState<TRef> where TRef : IBinaryWriteState<TRef> {
        /// <summary>
        /// Inform the serializing object to the state and generates the next state
        /// </summary>
        /// <typeparam name="TKind">The serializing object type</typeparam>
        /// <param name="item">The serializing object</param>
        /// <param name="nextState">The next state</param>
        /// <returns>If the next state can be successfully generated and the serialization should continue</returns>
        bool TryGetNextState<TKind>(in TKind item, out TRef nextState);
    }

}
