using SharpMediaInfo.Info;

namespace SharpMediaInfo.Tracks
{
    public partial class VideoTrack
    {
        /// <summary>
        /// Gets the duration of the media, if available.
        /// </summary>
        /// <remarks>The duration is determined based on available metadata. If the duration is not
        /// explicitly set, it may be calculated from the frame count and frame rate, or derived from the timecode. The
        /// value may be null if the duration cannot be determined.</remarks>
        public double? Duration
        {
            get
            {
                if (this.DurationInternal.HasValue)
                {
                    return this.DurationInternal.Value;

                }
                else if (this.FrameCount.HasValue && this.FrameRate.HasValue && this.FrameRate.Value.Value.HasValue && this.FrameRate.Value.Value > 0)
                {
                    return (double?)(this.FrameCount.Value / this.FrameRate.Value.Value) * 1000;
                }
                else if (this.DurationTimecode.TotalMinutes > 0)
                {
                    return (int?)this.DurationTimecode.TotalMinutes;
                } 
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the bit rate as a formatted string suitable for display.
        /// </summary>
        public string? BitRateString
        {    
            get
            {
                if (string.IsNullOrEmpty(this.BitRateStringInternal))
                {
                    if (this.BitRate.HasValue)
                    {
                        return MediaInfo.GetFormattedBitRate(this.BitRate);
                    }
                    else if (this.BitRateNominal.HasValue)
                    {
                        return MediaInfo.GetFormattedBitRate(this.BitRateNominal);
                    }
                }
                return this.BitRateStringInternal;
            }
        }
    }
}
