namespace SharpMediaInfo.Info
{
    public readonly struct FrameRateInfo
    {
        public double? Value { get; }
        public long? Numerator { get; }
        public long? Denominator { get; }

        // Full constructor (existing)
        public FrameRateInfo(double? value, long? numerator, long? denominator)
        {
            Value = value;
            Numerator = numerator;
            Denominator = denominator;
        }

        // Constructor for value-only (e.g., 25.000)
        public FrameRateInfo(double? value)
        {
            Value = value;
            Numerator = null;
            Denominator = null;
        }

        // Constructor for rational-only (e.g., 30000/1001)
        public FrameRateInfo(long? numerator, long? denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
            Value = (numerator.HasValue && denominator.HasValue && denominator.Value != 0)
                ? (double)numerator.Value / denominator.Value
                : null;
        }

        public override string ToString()
        {
            if (Value.HasValue && Numerator.HasValue && Denominator.HasValue && Denominator.Value != 0)
            {
                return $"{Value:0.###} ({Numerator}/{Denominator}) FPS";
            }
            if (Value.HasValue)
            {
                return $"{Value:0.###} FPS";
            }
            if (Numerator.HasValue && Denominator.HasValue && Denominator.Value != 0)
            {
                var val = (double)Numerator.Value / Denominator.Value;
                return $"{val:0.###} ({Numerator}/{Denominator}) FPS";
            }
            return string.Empty;
        }
    }
}
