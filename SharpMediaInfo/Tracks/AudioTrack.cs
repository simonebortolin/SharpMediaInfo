namespace SharpMediaInfo.Tracks
{
    public partial class AudioTrack
    {
        /// <summary>
        /// Gets the duration of the media in milliseconds, if available.
        /// </summary>
        /// <remarks>The duration is determined based on available metadata. If an explicit duration is
        /// not set, it may be calculated from the frame count and frame rate, or derived from the timecode. If
        /// insufficient information is available, the property returns null.</remarks>
        public double? Duration
        {
            get
            {
                if (this.DurationInternal.HasValue)
                {
                    return this.DurationInternal.Value;

                }
                else if (this.FrameCount.HasValue && this.FrameRate.HasValue && this.FrameRate.Value > 0)
                {
                    return (double)(this.FrameCount.Value / this.FrameRate.Value) * 1000;
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
        /// Gets the number of audio channels, if available.
        /// </summary>
        /// <remarks>The value is determined from internal metadata if present; otherwise, it is
        /// calculated from channel position information if available. If neither source is available, the property
        /// returns null.</remarks>
        public long? Channels
        {
            get
            {
                if (this.ChannelsInternal.HasValue)
                {
                    return this.ChannelsInternal.Value;
                }
                else if (!string.IsNullOrEmpty(this.ChannelPositions))
                {
                    return MediaInfo.GetChannelsCount(this.ChannelPositions);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
