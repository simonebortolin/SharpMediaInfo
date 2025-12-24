namespace SharpMediaInfo.Tracks
{
    public partial class GeneralTrack
    {
        /*
        /// <summary>
        /// Gets the calculated overall bitrate with fallback logic.
        /// </summary>
        /// <remarks>
        /// Returns the bitrate from the OverallBitRate field if available.
        /// If not available or zero, calculates from Duration and FileSize.
        /// Returns null if calculation is not possible.
        /// </remarks>
        public long? OverallBitRateCalculated
        {
            get
            {
                // Primary: use OverallBitRate field if available and non-zero
                if (this.OverallBitRate.BitsPerSecond.HasValue && this.OverallBitRate.BitsPerSecond.Value > 0)
                {
                    return this.OverallBitRate.BitsPerSecond.Value;
                }

                // Fallback: calculate from Duration and FileSize
                if (this.Duration.Milliseconds.HasValue && this.Duration.Milliseconds.Value > 0 &&
                    this.FileSize.Bytes.HasValue && this.FileSize.Bytes.Value > 0)
                {
                    // duration is in ms → seconds
                    var durationSeconds = this.Duration.Milliseconds.Value / 1000.0;
                    if (durationSeconds <= 0) return null;

                    // bits = bytes * 8
                    var bits = this.FileSize.Bytes.Value * 8;

                    // Clamp to long range to avoid overflow
                    if (bits > long.MaxValue)
                    {
                        return long.MaxValue;
                    }

                    return (long)(bits / durationSeconds);
                }

                return null;
            }
        }*/
    }
}
