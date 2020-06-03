using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {
    public readonly struct DeserializeResult {

        public DeserializeResult(DeserializeConclusion conculsion, string faultedReason) {
            Conculsion = conculsion;
            FaultedReason = faultedReason;
        }

        public DeserializeConclusion Conculsion { get; }

        public string FaultedReason { get; }

        public bool IsSuccess => Conculsion == DeserializeConclusion.Success;

        /// <summary>
        /// The serialization was a success
        /// </summary>
        public static DeserializeResult Success { get; } = new DeserializeResult(DeserializeConclusion.Success, string.Empty);
        public static DeserializeResult Faulted { get; } = new DeserializeResult(DeserializeConclusion.Faulted, string.Empty);
        public static DeserializeResult InsufficientSize { get; } = new DeserializeResult(DeserializeConclusion.InsufficientSize, string.Empty);

        public static DeserializeResult FaultedFrom(string result) => new DeserializeResult(DeserializeConclusion.Faulted, result);

        public bool Equals(DeserializeConclusion conclusion) => Conculsion == conclusion;

        public bool Equals(DeserializeResult result) => Conculsion == result.Conculsion;

        public override bool Equals(object obj) => (obj is DeserializeResult result && Equals(result)) || (obj is DeserializeConclusion conclusion && Equals(conclusion));

        public override int GetHashCode() => Conculsion.GetHashCode();

        public override string ToString() => $"{Conculsion}{(string.IsNullOrEmpty(FaultedReason) ? null : $" {FaultedReason}")}";

        public static bool operator ==(DeserializeResult first, DeserializeResult second) => first.Equals(second);
        public static bool operator !=(DeserializeResult first, DeserializeResult second) => !first.Equals(second);
    }
}
