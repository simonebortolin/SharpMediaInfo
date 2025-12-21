using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMediaInfo.Tracks
{
    public partial  class ChapterTrack
    {
        /// <summary>
        /// The absolute index of the track.
        /// </summary>
        public long? Index { get; set; }

        /// <summary>
        /// The timecode of the track.
        /// </summary>
        public string? Timecode { get; set; }

        /// <summary>
        /// The title of the track.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The language of the track.
        /// </summary>
        public string? Language { get; set; }
    }
}
