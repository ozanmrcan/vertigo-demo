using WheelOfFortune.Data;

namespace WheelOfFortune.Core
{
    public readonly struct SpinResult
    {
        public SpinResult(int sliceIndex, WheelSliceData slice, int scaledAmount)
        {
            SliceIndex = sliceIndex;
            Slice = slice;
            ScaledAmount = scaledAmount;
        }

        public int SliceIndex { get; }
        public WheelSliceData Slice { get; }
        public int ScaledAmount { get; }
    }
}
