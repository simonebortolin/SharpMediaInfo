namespace SharpMediaInfo.Tracks
{
    public partial class GeneralTrack
    {
        public long? OverallBitRate
        {
            get
            {
                if (this.OverallBitRateInternal.GetValueOrDefault() == 0)
                {
                    if (this.Duration.GetValueOrDefault() > 0 && this.FileSize.HasValue && this.FileSize.Value > System.Numerics.BigInteger.Zero)
                    {
                        // duration is in ms → seconds
                        var durationSeconds = (long)(this.Duration!.Value / 1000);
                        if (durationSeconds <= 0) return null;

                        // bits = bytes * 8
                        var bits = this.FileSize.Value * 8;

                        // Clamp to long range to avoid overflow
                        var max = new System.Numerics.BigInteger(long.MaxValue);
                        if (bits > max)
                        {
                            // If extremely large, cap to long.MaxValue to avoid overflow
                            return long.MaxValue;
                        }

                        return (long)(bits / durationSeconds);
                    }
                    return null;
                }
                return this.OverallBitRateInternal.GetValueOrDefault();
            }
        }

        public string? OverallBitRateString
        {
            get
            {
                if (string.IsNullOrEmpty(this.OverallBitRateStringInternal) && this.OverallBitRate.GetValueOrDefault() > 0)
                {
                    return MediaInfo.GetFormattedBitRate(this.OverallBitRate);
                }
                return this.OverallBitRateStringInternal ?? null;
            }
        }
    }
}
