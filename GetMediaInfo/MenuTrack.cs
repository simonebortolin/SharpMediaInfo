using System;
using System.Collections.Generic;

namespace GetMediaInfos
{
    /// <summary>
    /// Contains properties for a MenuTrack
    /// </summary>
    public class MenuTrack
    {
        private int _ChaptersCount;
        private int _StreamCount;
        private string _StreamKind;
        private int _StreamKindID;
        private int _StreamOrder;
        private string _RawData;
        private string _ID;
        private string _UniqueID;
        private string _Title;
        private string _Codec;
        private string _CodecString;
        private string _CodecUrl;
        private string _Language;
        private string _LanguageString;
        private bool _Default;
        private bool _Forced;
        private List<ChapterTrack> _Chapters;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuTrack"/> class.
        /// </summary>
        public MenuTrack()
        {
            _Chapters = new List<ChapterTrack>();
        }

        ///<summary> ChaptersCount of objects available in this stream </summary>
        public int ChaptersCount
        {
            get
            {
                return _ChaptersCount;
            }
            set
            {
                this._ChaptersCount=value;
            }
        }

        ///<summary> Count of streams of that kind available </summary>
        public int StreamCount
        {
            get
            {
                return _StreamCount;
            }
            set
            {
                this._StreamCount=value;
            }
        }

        ///<summary> Stream name </summary>
        public string StreamKind
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamKind))
                    this._StreamKind="";
                return _StreamKind;
            }
            set
            {
                this._StreamKind=value;
            }
        }

        ///<summary> When multiple streams, number of the stream </summary>
        public int StreamKindID
        {
            get
            {
                return _StreamKindID;
            }
            set
            {
                this._StreamKindID=value;
            }
        }

        ///<summary>Stream order in the file, whatever is the kind of stream (base=0)</summary>
        public int StreamOrder
        {
            get
            {
                return _StreamOrder;
            }
            set
            {
                this._StreamOrder = value;
            }
        }

        ///<summary> The unprocessed raw data from mediainfo </summary>
        public string RawData
        {
            get
            {
                if (String.IsNullOrEmpty(this._RawData))
                    this._RawData = "";
                return _RawData;
            }
            set
            {
                this._RawData = value;
            }
        }

        ///<summary> A ID for this stream in this file </summary>
        public string ID
        {
            get
            {
                if (String.IsNullOrEmpty(this._ID))
                    this._ID="";
                return _ID;
            }
            set
            {
                this._ID=value;
            }
        }

        ///<summary> A unique ID for this stream, should be copied with stream copy </summary>
        public string UniqueID
        {
            get
            {
                if (String.IsNullOrEmpty(this._UniqueID))
                    this._UniqueID="";
                return _UniqueID;
            }
            set
            {
                this._UniqueID=value;
            }
        }

        ///<summary> Name of the track </summary>
        public string Title
        {
            get
            {
                if (String.IsNullOrEmpty(this._Title))
                    this._Title="";
                return _Title;
            }
            set
            {
                this._Title=value;
            }
        }

        ///<summary> Codec used </summary>
        public string Codec
        {
            get
            {
                if (String.IsNullOrEmpty(this._Codec))
                    this._Codec="";
                return _Codec;
            }
            set
            {
                this._Codec=value;
            }
        }

        ///<summary> Codec used (test) </summary>
        public string CodecString
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecString))
                    this._CodecString="";
                return _CodecString;
            }
            set
            {
                this._CodecString=value;
            }
        }

        ///<summary> Codec used (test) </summary>
        public string CodecUrl
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecUrl))
                    this._CodecUrl="";
                return _CodecUrl;
            }
            set
            {
                this._CodecUrl=value;
            }
        }

        ///<summary> Language (2 letters) </summary>
        public string Language
        {
            get
            {
                if (String.IsNullOrEmpty(this._Language))
                    this._Language="";
                return _Language;
            }
            set
            {
                this._Language=value;
            }
        }

        ///<summary> Language (full) </summary>
        public string LanguageString
        {
            get
            {
                if (String.IsNullOrEmpty(this._LanguageString))
                    this._LanguageString="";
                return _LanguageString;
            }
            set
            {
                this._LanguageString=value;
            }
        }

        ///<summary> Default Info </summary>
        public bool Default
        {
            get
            {
                return _Default;
            }
            set
            {
                this._Default = value;
            }
        }

        ///<summary> Forced Info </summary>
        public bool Forced
        {
            get
            {
                return _Forced;
            }
            set
            {
                this._Forced = value;
            }
        }

        /// <summary>
        /// Chapters List.
        /// </summary>
        public List<ChapterTrack> Chapters
        {
            get
            {
                return _Chapters;
            }

            set
            {
                this._Chapters = value;
            }
        }
    }
}
