using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Decuplr.Serialization {
    public readonly struct DeserializeResult {

        public DeserializeResult(DeserializeConclusion conculsion, string faultedReason) {
            Conclusion = conculsion;
            FaultedReason = faultedReason;
        }

        public DeserializeConclusion Conclusion { get; }

        public string FaultedReason { get; }

        public bool IsSuccess => Conclusion == DeserializeConclusion.Success;

        /// <summary>
        /// The serialization was a success
        /// </summary>
        public static DeserializeResult Success { get; } = new DeserializeResult(DeserializeConclusion.Success, string.Empty);
        public static DeserializeResult Faulted { get; } = new DeserializeResult(DeserializeConclusion.Faulted, string.Empty);
        public static DeserializeResult InsufficientSize { get; } = new DeserializeResult(DeserializeConclusion.InsufficientSize, string.Empty);

        public static DeserializeResult FaultedFrom(string result) => new DeserializeResult(DeserializeConclusion.Faulted, result);

        public bool Equals(DeserializeConclusion conclusion) => Conclusion == conclusion;

        public bool Equals(DeserializeResult result) => Conclusion == result.Conclusion;

        public override bool Equals(object obj) => (obj is DeserializeResult result && Equals(result)) || (obj is DeserializeConclusion conclusion && Equals(conclusion));

        public override int GetHashCode() => Conclusion.GetHashCode();

        public override string ToString() => $"{Conclusion}{(string.IsNullOrEmpty(FaultedReason) ? null : $" {FaultedReason}")}";

        internal string ToDisplayString() => Conclusion switch
        {
            DeserializeConclusion.Success => $"{nameof(DeserializeResult)}.{nameof(Success)}",
            DeserializeConclusion.Faulted => $"{nameof(DeserializeResult)}.{nameof(Faulted)}",
            DeserializeConclusion.InsufficientSize => $"{nameof(DeserializeResult)}.{nameof(InsufficientSize)}",
            _ => throw new ArgumentException("Invalid conclusion type")
        };

        public static bool operator ==(DeserializeResult first, DeserializeResult second) => first.Equals(second);
        public static bool operator !=(DeserializeResult first, DeserializeResult second) => !first.Equals(second);
    }
}
