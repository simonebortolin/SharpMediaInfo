using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetMediaInfos
{
    /// <summary>
    /// Contains properties for a ChapterTrack
    /// </summary>
    public class ChapterTrack
    {
        private int _Index;
        private string _Timecode;
        private string _Title;
        private string _Language;

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int Index
        {
            get
            {
                return _Index;
            }

            set
            {
                _Index = value;
            }
        }

        /// <summary>
        /// Gets or sets the timecode.
        /// </summary>
        public string Timecode
        {
            get
            {
                return _Timecode;
            }

            set
            {
                _Timecode = value;
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get
            {
                return _Title;
            }

            set
            {
                _Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language
        {
            get
            {
                return _Language;
            }

            set
            {
                _Language = value;
            }
        }
    }
}
