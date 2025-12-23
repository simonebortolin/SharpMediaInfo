namespace SharpMediaInfo.Info
{
    /// <summary>
    /// Represents bit rate information with automatic unit conversion.
    /// </summary>
    public readonly struct BitRateInfo
    {
        /// <summary>
        /// Gets the bit rate in bits per second.
        /// </summary>
        public long? BitsPerSecond { get; }

        /// <summary>
        /// Gets the bit rate in kilobits per second.
        /// </summary>
        public double? KiloBitsPerSecond => BitsPerSecond.HasValue ? BitsPerSecond.Value / 1000.0 : null;

        /// <summary>
        /// Gets the bit rate in megabits per second.
        /// </summary>
        public double? MegaBitsPerSecond => BitsPerSecond.HasValue ? BitsPerSecond.Value / 1_000_000.0 : null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitRateInfo"/> struct.
        /// </summary>
        /// <param name="bitsPerSecond">The bit rate in bits per second</param>
        public BitRateInfo(long? bitsPerSecond)
        {
            BitsPerSecond = bitsPerSecond;
        }

        /// <summary>
        /// Returns the bit rate with appropriate unit (bps, kb/s, Mb/s).
        /// </summary>
        public override string ToString()
        {
            if (!BitsPerSecond.HasValue)
                return string.Empty;

            var bps = BitsPerSecond.Value;

            if (bps >= 1_000_000)
                return $"{bps / 1_000_000.0:F2} Mb/s";
            else if (bps >= 1_000)
                return $"{bps / 1_000.0:F0} kb/s";
            else
                return $"{bps} bps";
        }

        /// <summary>
        /// Implicitly converts to nullable long (bits per second).
        /// </summary>
        public static implicit operator long?(BitRateInfo bitRate) => bitRate.BitsPerSecond;
    }
}
