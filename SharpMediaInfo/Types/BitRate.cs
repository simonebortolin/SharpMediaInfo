namespace SharpMediaInfo.Types
{
    /// <summary>
    /// Represents bit rate information with automatic unit conversion.
    /// </summary>
    public readonly struct BitRate
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
        public BitRate(long? bitsPerSecond)
        {
            BitsPerSecond = bitsPerSecond;
        }

        /// <summary>
        /// Returns the bit rate with appropriate unit (Bps, Kbps, Mbps, Gbps, Tbps).
        /// </summary>
        public override string ToString()
        {
            if (!BitsPerSecond.HasValue)
                return string.Empty;

            var bps = BitsPerSecond.Value;

            if (bps >= 1099511627776)
                return $"{bps / 1099511627776.0:0.##} Tbps";
            else if (bps >= 1073741824)
                return $"{bps / 1073741824.0:0.##} Gbps";
            else if (bps >= 1048576)
                return $"{bps / 1048576.0:0.##} Mbps";
            else if (bps >= 1024)
                return $"{bps / 1024.0:0.##} Kbps";
            else
                return $"{bps:0.##} Bps";
        }

        /// <summary>
        /// Implicitly converts to nullable long (bits per second).
        /// </summary>
        public static implicit operator long?(BitRate bitRate) => bitRate.BitsPerSecond;
    }
}
