using SharpMediaInfo.Types;

namespace SharpMediaInfo.Tracks
{
    public partial class AudioTrack
    {
        /// <summary>
        /// Gets the duration with fallback logic.
        /// </summary>
        /// <remarks>
        /// Returns the duration from the DurationInternal field if available.
        /// If not available, calculates from FrameCount and FrameRate.
        /// </remarks>
        public TimeSpan Duration
        {
            get
            {
                // Primary: use DurationInternal field if available
                if (this.DurationInternal.HasValue)
                {
                    return this.DurationInternal.Value;
                }

                // Fallback: calculate from FrameCount and FrameRate
                if (this.FrameCount.HasValue && this.FrameRate != null && this.FrameRate.Value.Value > 0)
                {
                    var milliseconds = (long)((this.FrameCount.Value / this.FrameRate.Value.Value) * 1000);
                    return TimeSpan.FromMilliseconds(milliseconds);
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Gets the normalized audio format.
        /// </summary>
        /// <remarks>
        /// Applies fixes for E-AC3, TrueHD, and DTS formats based on MediaInfo quirks.
        /// </remarks>
        public string? FormatNormalized
        {
            get
            {
                // E-AC3 fix
                if (this.Format?.Contains("AC-3") == true && this.FormatString?.Equals("E-AC-3") == true)
                {
                    return "EAC-3";
                }

                // TrueHD Atmos + AC3 fix
                if (this.Format?.Contains("AC-3") == true && this.FormatAdditionalFeatures?.Equals("MLP FBA 16-ch") == true)
                {
                    return null; // Cleared in original code
                }

                // TrueHD fix
                if (this.Format?.Contains("MLP FBA") == true)
                {
                    if (!string.IsNullOrEmpty(this.FormatAdditionalFeatures))
                        return "TrueHD Atmos";
                    return "TrueHD";
                }

                // DTS fix
                if (this.Format?.Contains("DTS") == true)
                {
                    if (this.FormatAdditionalFeatures?.Contains("XLL X") == true)
                        return "DTS-X";
                    else if (this.FormatAdditionalFeatures?.Contains("XLL") == true)
                        return "DTS-HD MA";
                    else if (this.FormatAdditionalFeatures?.Contains("XBR") == true)
                        return "DTS-HD HR";
                    else if (this.FormatAdditionalFeatures?.Contains("ES") == true)
                        return "DTS-ES";
                }

                return this.Format;
            }
        }

        /// <summary>
        /// Gets the normalized format profile.
        /// </summary>
        /// <remarks>
        /// Applies fixes for specific audio formats based on MediaInfo quirks.
        /// </remarks>
        public string? FormatProfileNormalized
        {
            get
            {
                // E-AC3 fix
                if (this.Format?.Contains("AC-3") == true && this.FormatString?.Equals("E-AC-3") == true)
                {
                    return string.Empty;
                }

                // TrueHD Atmos + AC3 fix
                if (this.Format?.Contains("AC-3") == true && this.FormatAdditionalFeatures?.Equals("MLP FBA 16-ch") == true)
                {
                    return "TrueHD Atmos / AC-3";
                }

                // TrueHD fix
                if (this.Format?.Contains("MLP FBA") == true)
                {
                    return string.Empty;
                }

                return this.FormatProfile;
            }
        }

        /// <summary>
        /// Gets a description of the channel configuration with object-based audio notation.
        /// </summary>
        /// <remarks>
        /// Adds "(object-based)" suffix for Atmos and DTS-X formats.
        /// For TrueHD Atmos + AC3, combines both channel configurations.
        /// </remarks>
        public string? ChannelsDescription
        {
            get
            {
                var baseChannels = this.Channels.ToString();
                if (string.IsNullOrEmpty(baseChannels))
                    return null;

                // TrueHD Atmos + AC3 fix
                if (this.Format?.Contains("AC-3") == true && this.FormatAdditionalFeatures?.Equals("MLP FBA 16-ch") == true)
                {
                    return $"{baseChannels} (object-based) / 6 channels";
                }

                // TrueHD Atmos fix
                if (this.Format?.Contains("MLP FBA") == true && !string.IsNullOrEmpty(this.FormatAdditionalFeatures))
                {
                    return $"{baseChannels} (object-based)";
                }

                // DTS-X fix
                if (this.Format?.Contains("DTS") == true && this.FormatAdditionalFeatures?.Contains("XLL X") == true)
                {
                    return $"{baseChannels} (object-based)";
                }

                return baseChannels;
            }
        }

        /// <summary>
        /// Gets the normalized bit depth description.
        /// </summary>
        /// <remarks>
        /// For TrueHD Atmos + AC3, combines both bit depths.
        /// </remarks>
        public string? BitDepthDescription
        {
            get
            {
                // TrueHD Atmos + AC3 fix
                if (this.Format?.Contains("AC-3") == true && this.FormatAdditionalFeatures?.Equals("MLP FBA 16-ch") == true)
                {
                    var bitDepth = this.BitDepth?.ToString();
                    if (!string.IsNullOrEmpty(bitDepth))
                        return $"{bitDepth} bits / 16 bits";
                }

                return this.BitDepth?.ToString();
            }
        }

        /// <summary>
        /// Gets the normalized sampling rate description.
        /// </summary>
        /// <remarks>
        /// For TrueHD Atmos + AC3, combines both sampling rates.
        /// </remarks>
        public string? SamplingRateDescription
        {
            get
            {
                var samplingRate = this.SamplingRate?.ToString();

                // TrueHD Atmos + AC3 fix
                if (this.Format?.Contains("AC-3") == true && this.FormatAdditionalFeatures?.Equals("MLP FBA 16-ch") == true)
                {
                    if (!string.IsNullOrEmpty(samplingRate))
                        return $"{samplingRate} / {samplingRate}";
                }

                return samplingRate;
            }
        }

        /// <summary>
        /// Gets the normalized bit rate mode description.
        /// </summary>
        /// <remarks>
        /// For TrueHD Atmos + AC3, combines both bit rate modes.
        /// </remarks>
        public string? BitRateModeDescription
        {
            get
            {
                // TrueHD Atmos + AC3 fix
                if (this.Format?.Contains("AC-3") == true && this.FormatAdditionalFeatures?.Equals("MLP FBA 16-ch") == true)
                {
                    var mode = this.BitRateMode;
                    if (!string.IsNullOrEmpty(mode))
                        return $"{mode} / CBR";
                }

                return this.BitRateMode;
            }
        }

        /// <summary>
        /// Gets the normalized bit rate description.
        /// </summary>
        /// <remarks>
        /// For TrueHD Atmos + AC3, combines BitRateMaximum and BitRate.
        /// </remarks>
        public string? BitRateDescription
        {
            get
            {
                // TrueHD Atmos + AC3 fix
                if (this.Format?.Contains("AC-3") == true && this.FormatAdditionalFeatures?.Equals("MLP FBA 16-ch") == true)
                {
                    var maxBitRate = this.BitRateMaximum.ToString();
                    var bitRate = this.BitRate.ToString();

                    if (!string.IsNullOrEmpty(maxBitRate) && !string.IsNullOrEmpty(bitRate))
                        return $"{maxBitRate} / {bitRate}";
                }

                return this.BitRate.ToString();
            }
        }
    }
}
