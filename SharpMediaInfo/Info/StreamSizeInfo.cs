namespace SharpMediaInfo.Info
{
    /// <summary>
    /// Represents stream size information with various formatting options.
    /// </summary>
    public readonly struct StreamSizeInfo
    {
        /// <summary>
        /// Gets the size in bytes.
        /// </summary>
        public long? Bytes { get; }

        /// <summary>
        /// Gets the size proportion (0.0 to 1.0) relative to total file size.
        /// </summary>
        public double? Proportion { get; }

        /// <summary>
        /// Gets the size in kilobytes.
        /// </summary>
        public double? KiloBytes => Bytes.HasValue ? Bytes.Value / 1024.0 : null;

        /// <summary>
        /// Gets the size in megabytes.
        /// </summary>
        public double? MegaBytes => Bytes.HasValue ? Bytes.Value / (1024.0 * 1024.0) : null;

        /// <summary>
        /// Gets the size in gigabytes.
        /// </summary>
        public double? GigaBytes => Bytes.HasValue ? Bytes.Value / (1024.0 * 1024.0 * 1024.0) : null;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamSizeInfo"/> struct.
        /// </summary>
        /// <param name="bytes">The size in bytes</param>
        /// <param name="proportion">The proportion relative to total file size (optional)</param>
        public StreamSizeInfo(long? bytes, double? proportion = null)
        {
            Bytes = bytes;
            Proportion = proportion;
        }

        /// <summary>
        /// Returns the size in the default format with appropriate unit and percentage.
        /// </summary>
        public override string ToString() => ToString(StreamSizeFormat.AutoWithPercentage);

        /// <summary>
        /// Returns the size in the specified format.
        /// </summary>
        /// <param name="format">The format to use</param>
        /// <returns>Formatted size string</returns>
        public string ToString(StreamSizeFormat format)
        {
            if (!Bytes.HasValue)
                return string.Empty;

            var bytes = Bytes.Value;

            return format switch
            {
                StreamSizeFormat.Auto => FormatAuto(bytes, false),
                StreamSizeFormat.AutoWithPercentage => FormatAuto(bytes, true),
                StreamSizeFormat.Bytes => $"{bytes} bytes",
                StreamSizeFormat.KiloBytes => $"{bytes / 1024.0:F2} KiB",
                StreamSizeFormat.MegaBytes => $"{bytes / (1024.0 * 1024.0):F2} MiB",
                StreamSizeFormat.GigaBytes => $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GiB",
                _ => FormatAuto(bytes, true)
            };
        }

        private string FormatAuto(long bytes, bool includePercentage)
        {
            string sizeStr;

            if (bytes >= 1024L * 1024L * 1024L)
                sizeStr = $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GiB";
            else if (bytes >= 1024L * 1024L)
                sizeStr = $"{bytes / (1024.0 * 1024.0):F1} MiB";
            else if (bytes >= 1024L)
                sizeStr = $"{bytes / 1024.0:F1} KiB";
            else
                sizeStr = $"{bytes} bytes";

            if (includePercentage && Proportion.HasValue)
            {
                var percentage = Proportion.Value * 100.0;
                sizeStr += $" ({percentage:F0}%)";
            }

            return sizeStr;
        }

        /// <summary>
        /// Implicitly converts to nullable long (bytes).
        /// </summary>
        public static implicit operator long?(StreamSizeInfo size) => size.Bytes;
    }

    /// <summary>
    /// Specifies the format for stream size string representation.
    /// </summary>
    public enum StreamSizeFormat
    {
        /// <summary>
        /// Automatic format with appropriate unit (e.g., "10.5 MiB")
        /// </summary>
        Auto,

        /// <summary>
        /// Automatic format with percentage (e.g., "10.5 MiB (98%)")
        /// </summary>
        AutoWithPercentage,

        /// <summary>
        /// Format in bytes
        /// </summary>
        Bytes,

        /// <summary>
        /// Format in kilobytes (KiB)
        /// </summary>
        KiloBytes,

        /// <summary>
        /// Format in megabytes (MiB)
        /// </summary>
        MegaBytes,

        /// <summary>
        /// Format in gigabytes (GiB)
        /// </summary>
        GigaBytes
    }
}
