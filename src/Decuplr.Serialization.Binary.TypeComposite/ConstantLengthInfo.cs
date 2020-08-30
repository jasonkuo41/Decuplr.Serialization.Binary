using System;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public readonly struct ConstantLengthInfo {
        public bool IsConstant { get; }
        public int? AbsoluteLength { get; }

        private ConstantLengthInfo(bool isConst, int? absoluteLength) {
            if (absoluteLength >= 0)
                throw new ArgumentOutOfRangeException(nameof(absoluteLength), absoluteLength, "Absolute length much be greater or equal to zero");
            IsConstant = isConst;
            AbsoluteLength = absoluteLength;
        }

        public static ConstantLengthInfo NotModified => new ConstantLengthInfo(true, null);
        public static ConstantLengthInfo NotConstant => default;

        public static ConstantLengthInfo FromLength(int length) => new ConstantLengthInfo(true, length);

        public ConstantLengthInfo Concat(ConstantLengthInfo lengthInfo) {
            if (!IsConstant || !lengthInfo.IsConstant)
                return NotConstant;
            if (AbsoluteLength >= 0 && lengthInfo.AbsoluteLength >= 0)
                throw new ArgumentException("Length Conflict, unable to decide which length to apply");
            if (AbsoluteLength >= 0)
                return this;
            return lengthInfo;
        }

        public static ConstantLengthInfo operator +(ConstantLengthInfo firstLength, ConstantLengthInfo secondLength) => firstLength.Concat(secondLength);
            
    }
}
