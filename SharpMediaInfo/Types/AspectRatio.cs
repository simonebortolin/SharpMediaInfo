using System;

namespace SharpMediaInfo.Types
{
    /// <summary>
    /// Represents an aspect ratio value with various formatting options.
    /// </summary>
    public readonly struct AspectRatio
    {
        /// <summary>
        /// Gets the aspect ratio as a decimal value (e.g., 1.777 for 16:9).
        /// </summary>
        public double? Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspectRatio"/> struct.
        /// </summary>
        /// <param name="value">The aspect ratio as decimal value</param>
        public AspectRatio(double? value)
        {
            Value = value;
        }

        /// <summary>
        /// Returns the aspect ratio in default format (ratio notation).
        /// </summary>
        public override string ToString() => ToString(AspectRatioFormat.Ratio);

        /// <summary>
        /// Returns the aspect ratio in the specified format.
        /// </summary>
        /// <param name="format">The format to use</param>
        /// <returns>Formatted aspect ratio string</returns>
        public string ToString(AspectRatioFormat format)
        {
            if (!Value.HasValue)
                return string.Empty;

            var value = Value.Value;

            return format switch
            {
                AspectRatioFormat.Decimal => FormatDecimal(value),
                AspectRatioFormat.Ratio => FormatRatio(value),
                _ => FormatRatio(value)
            };
        }

        private string FormatDecimal(double value)
        {
            // Format: "1.778"
            return $"{value:F3}";
        }

        private string FormatRatio(double value)
        {
            // Format: "16:9" or "4:3" or "2.40:1"
            // Try to detect common aspect ratios
            var ratio = GetCommonRatio(value);
            if (ratio != null)
                return ratio;

            // Otherwise format as X:1
            return $"{value:F2}:1";
        }

        private string? GetCommonRatio(double value)
        {
            const double tolerance = 0.01;

            // Common aspect ratios
            if (Math.Abs(value - 1.333) < tolerance) return "4:3";
            if (Math.Abs(value - 1.778) < tolerance) return "16:9";
            if (Math.Abs(value - 1.6) < tolerance) return "16:10";
            if (Math.Abs(value - 2.35) < tolerance) return "2.35:1";
            if (Math.Abs(value - 2.39) < tolerance) return "2.39:1";
            if (Math.Abs(value - 2.40) < tolerance) return "2.40:1";
            if (Math.Abs(value - 1.85) < tolerance) return "1.85:1";
            if (Math.Abs(value - 1.0) < tolerance) return "1:1";
            if (Math.Abs(value - 0.5625) < tolerance) return "9:16"; // Portrait 16:9

            return null;
        }

        /// <summary>
        /// Implicitly converts to nullable double.
        /// </summary>
        public static implicit operator double?(AspectRatio aspectRatio) => aspectRatio.Value;

        /// <summary>
        /// Implicitly converts from nullable double to AspectRatio.
        /// </summary>
        public static implicit operator AspectRatio(double? value) => new AspectRatio(value);
    }

    /// <summary>
    /// Specifies the format for aspect ratio string representation.
    /// </summary>
    public enum AspectRatioFormat
    {
        /// <summary>
        /// Decimal format (e.g., "1.778")
        /// </summary>
        Decimal,

        /// <summary>
        /// Ratio format (e.g., "16:9")
        /// </summary>
        Ratio
    }
}
