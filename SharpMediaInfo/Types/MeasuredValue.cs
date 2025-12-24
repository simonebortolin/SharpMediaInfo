namespace SharpMediaInfo.Types
{
    /// <summary>
    /// Represents a measured value with its unit of measurement.
    /// </summary>
    /// <typeparam name="T">The numeric type of the value</typeparam>
    public readonly struct MeasuredValue<T> where T : struct
    {
        /// <summary>
        /// Gets the numeric value.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Gets the unit of measurement (e.g., "pixel", "Hz", "bps").
        /// </summary>
        public string Unit { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasuredValue{T}"/> struct.
        /// </summary>
        /// <param name="value">The numeric value</param>
        /// <param name="unit">The unit of measurement</param>
        public MeasuredValue(T? value, string unit)
        {
            Value = value;
            Unit = unit ?? string.Empty;
        }

        /// <summary>
        /// Returns a string representation of the measured value.
        /// </summary>
        /// <returns>A string in the format "value unit" or empty if no value</returns>
        public override string ToString()
        {
            if (!Value.HasValue)
                return string.Empty;

            return string.IsNullOrEmpty(Unit)
                ? Value.ToString()!
                : $"{Value} {Unit}";
        }

        /// <summary>
        /// Implicitly converts to the underlying value type.
        /// </summary>
        public static implicit operator T?(MeasuredValue<T> measured) => measured.Value;
    }
}
