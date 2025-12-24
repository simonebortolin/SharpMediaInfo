namespace SharpMediaInfo.Types
{
    /// <summary>
    /// Represents audio channel configuration including count, positions, and layout.
    /// </summary>
    public readonly struct ChannelConfig
    {
        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        public long? Count { get; }

        /// <summary>
        /// Gets the position of channels (e.g., "Front: L C R, Side: L R, Back: L R, LFE").
        /// </summary>
        public string? Positions { get; }

        /// <summary>
        /// Gets the layout of channels (e.g., "L R C LFE Ls Rs Lb Rb").
        /// </summary>
        public string? Layout { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelConfig"/> struct.
        /// </summary>
        /// <param name="count">The number of channels</param>
        /// <param name="positions">The position of channels</param>
        /// <param name="layout">The layout of channels</param>
        public ChannelConfig(long? count, string? positions = null, string? layout = null)
        {
            Positions = string.IsNullOrWhiteSpace(positions) ? null : positions;
            Layout = string.IsNullOrWhiteSpace(layout) ? null : layout;

            // If count is provided, use it
            if (count.HasValue)
            {
                Count = count;
            }
            // Otherwise, try to parse from positions string
            else if (!string.IsNullOrWhiteSpace(positions))
            {
                Count = ParseChannelCountFromPositions(positions);
            }
            // Or from layout string
            else if (!string.IsNullOrWhiteSpace(layout))
            {
                Count = ParseChannelCountFromLayout(layout);
            }
            else
            {
                Count = null;
            }
        }

        /// <summary>
        /// Parses channel count from a position string like "Front: L C R, Side: L R, Back: L R, LFE".
        /// </summary>
        private static long? ParseChannelCountFromPositions(string positions)
        {
            if (string.IsNullOrWhiteSpace(positions))
                return null;

            // Count individual channel letters (L, R, C, LFE, etc.)
            var channelCount = 0;
            var tokens = positions.Split(new[] { ',', ':', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {
                // Skip position prefixes like "Front", "Side", "Back"
                if (token.Equals("Front", System.StringComparison.OrdinalIgnoreCase) ||
                    token.Equals("Side", System.StringComparison.OrdinalIgnoreCase) ||
                    token.Equals("Back", System.StringComparison.OrdinalIgnoreCase) ||
                    token.Equals("Rear", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                // Count channel identifiers
                if (token.Length <= 3 && (token.All(c => char.IsLetter(c)) || token == "LFE"))
                {
                    channelCount++;
                }
            }

            return channelCount > 0 ? channelCount : null;
        }

        /// <summary>
        /// Parses channel count from a layout string like "L R C LFE Ls Rs Lb Rb".
        /// </summary>
        private static long? ParseChannelCountFromLayout(string layout)
        {
            if (string.IsNullOrWhiteSpace(layout))
                return null;

            // Split by spaces and count non-empty tokens
            var channels = layout.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            return channels.Length > 0 ? channels.Length : null;
        }

        /// <summary>
        /// Returns the channel configuration in a readable format.
        /// </summary>
        public override string ToString()
        {
            if (!Count.HasValue)
                return string.Empty;

            var result = $"{Count.Value} channel{(Count.Value != 1 ? "s" : "")}";

            if (!string.IsNullOrEmpty(Positions))
                result += $" ({Positions})";
            else if (!string.IsNullOrEmpty(Layout))
                result += $" ({Layout})";

            return result;
        }

        /// <summary>
        /// Returns the position in x/y.z format if available.
        /// </summary>
        /// <returns>Position string in x/y.z format (e.g., "3/2/0.1")</returns>
        public string? GetPositionFormatted()
        {
            // This would need parsing logic from ChannelPositions to x/y.z format
            // For now, return the raw positions
            return Positions;
        }

        /// <summary>
        /// Implicitly converts to nullable long (returns channel count).
        /// </summary>
        public static implicit operator long?(ChannelConfig config) => config.Count;

        /// <summary>
        /// Implicitly converts from nullable long to ChannelConfig (count only).
        /// </summary>
        public static implicit operator ChannelConfig(long? count) => new ChannelConfig(count);
    }
}
