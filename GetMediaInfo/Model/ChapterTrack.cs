using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetMediaInfo.Model
{
    /// <summary>
    /// Contains properties for a ChapterTrack
    /// </summary>
    public class ChapterTrack
    {
        /// <summary>
        /// The absolute index of the track.
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// The timecode of the track.
        /// </summary>
        public string Timecode { get; set; }

        /// <summary>
        /// The title of the track.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The language of the track.
        /// </summary>
        public string Language { get; set; }
    }
}
