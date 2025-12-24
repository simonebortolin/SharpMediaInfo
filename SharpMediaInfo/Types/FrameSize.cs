namespace SharpMediaInfo.Types
{
    /// <summary>
    /// Represents a frame dimension value (width, height, etc.) with optional unit of measurement.
    /// </summary>
    public readonly struct FrameSize
    {
        /// <summary>
        /// Gets the dimension value.
        /// </summary>
        public long? Value { get; }

        /// <summary>
        /// Gets the unit of measurement (e.g., "pixel", "character").
        /// </summary>
        public string Unit { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameSize"/> struct.
        /// </summary>
        /// <param name="value">The dimension value</param>
        /// <param name="unit">The unit of measurement (defaults to "pixel")</param>
        public FrameSize(long? value, string unit = "pixel")
        {
            Value = value;
            Unit = unit ?? "pixel";
        }

        /// <summary>
        /// Returns the dimension in default format with unit and spacing.
        /// </summary>
        public override string ToString() => ToString(FrameSizeFormat.WithUnit);

        /// <summary>
        /// Returns the dimension in the specified format.
        /// </summary>
        /// <param name="format">The format to use</param>
        /// <returns>Formatted dimension string</returns>
        public string ToString(FrameSizeFormat format)
        {
            if (!Value.HasValue)
                return string.Empty;

            var value = Value.Value;

            return format switch
            {
                FrameSizeFormat.ValueOnly => $"{value}",
                FrameSizeFormat.WithUnit => FormatWithUnit(value),
                FrameSizeFormat.WithSpaces => FormatWithSpaces(value),
                _ => FormatWithUnit(value)
            };
        }

        private string FormatWithUnit(long value)
        {
            // Format: "1920 pixels"
            var unitPlural = Unit + (value != 1 ? "s" : "");
            return $"{value} {unitPlural}";
        }

        private string FormatWithSpaces(long value)
        {
            // Format: "1 920 pixels" (thousands separator with spaces)
            var valueStr = string.Format("{0:N0}", value).Replace(",", " ").Replace(".", " ");
            var unitPlural = Unit + (value != 1 ? "s" : "");
            return $"{valueStr} {unitPlural}";
        }

        /// <summary>
        /// Implicitly converts to nullable long.
        /// </summary>
        public static implicit operator long?(FrameSize dimension) => dimension.Value;

        /// <summary>
        /// Implicitly converts from nullable long to FrameSize with default unit (pixel).
        /// </summary>
        public static implicit operator FrameSize(long? value) => new FrameSize(value);
    }

    /// <summary>
    /// Specifies the format for frame size string representation.
    /// </summary>
    public enum FrameSizeFormat
    {
        /// <summary>
        /// Value only without unit (e.g., "1920")
        /// </summary>
        ValueOnly,

        /// <summary>
        /// Value with unit (e.g., "1920 pixels")
        /// </summary>
        WithUnit,

        /// <summary>
        /// Value with spaces as thousands separator and unit (e.g., "1 920 pixels")
        /// </summary>
        WithSpaces
    }
}
