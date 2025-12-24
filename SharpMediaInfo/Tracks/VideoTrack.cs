using SharpMediaInfo.Types;

namespace SharpMediaInfo.Tracks
{
    public partial class VideoTrack
    {
        public bool HDR
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.HDRFormat) == false)
                {
                    return true;
                } 
                if (this.BitDepth.HasValue && this.BitDepth.Value >= 10)
                {
                    if ((!string.IsNullOrEmpty(this.FormatCommercial) && this.FormatCommercial.Contains("HDR")) ||
                    (!string.IsNullOrEmpty(this.Format) && this.Format.Contains("HEVC")))
                    {
                        if (!string.IsNullOrEmpty(this.ColourPrimaries) && this.ColourPrimaries.Contains("BT.2020") ||
                            !string.IsNullOrEmpty(this.HDRFormat))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
