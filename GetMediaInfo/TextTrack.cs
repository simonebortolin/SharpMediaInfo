using System;

namespace GetMediaInfos
{
    /// <summary>
    /// Contains properties for a TextTrack
    /// </summary>
    public class TextTrack
    {
        private int _Count;
        private int _StreamCount;
        private string _StreamKind;
        private int _StreamKindID;
        private int _StreamOrder;
        private string _Inform;
        private string _ID;
        private string _UniqueID;
        private string _Title;
        private string _Codec;
        private string _CodecString;
        private string _CodecUrl;
        private int _Delay;
        private int _Video0Delay;
        private ulong _PlayTimeMillisec;
        private string _PlayTimeStringFormatted;
        private string _Language;
        private string _LanguageString;
        private string _LanguageMore;
        private bool _Default;
        private string _DefaultString;
        private bool _Forced;
        private string _ForcedString;

        ///<summary> Count of objects available in this stream </summary>
        public int Count
        {
            get
            {
                return _Count;
            }
            set
            {
                this._Count=value;
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

        ///<summary> Last   Inform   call </summary>
        public string Inform
        {
            get
            {
                if (String.IsNullOrEmpty(this._Inform))
                    this._Inform="";
                return _Inform;
            }
            set
            {
                this._Inform=value;
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

        ///<summary> Delay fixed in the stream (relative) </summary>
        public int Delay
        {
            get
            {
                return _Delay;
            }
            set
            {
                this._Delay=value;
            }
        }

        ///<summary> Delay fixed in the stream (absolute _ video0) </summary>
        public int Video0Delay
        {
            get
            {
                return _Video0Delay;
            }
            set
            {
                this._Video0Delay=value;
            }
        }

        ///<summary> Play time of the stream </summary>
        public ulong PlayTimeMillisec
        {
            get
            {
                return _PlayTimeMillisec;
            }
            set
            {
                this._PlayTimeMillisec = value;
            }
        }

        ///<summary> Play time in format : HH:MM:SS.MMM </summary>
        public string PlayTimeStringFormatted
        {
            get
            {
                if (String.IsNullOrEmpty(this._PlayTimeStringFormatted))
                    this._PlayTimeStringFormatted = "";
                return _PlayTimeStringFormatted;
            }
            set
            {
                this._PlayTimeStringFormatted = value;
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

        ///<summary> More info about Language (director s comment...) </summary>
        public string LanguageMore
        {
            get
            {
                if (String.IsNullOrEmpty(this._LanguageMore))
                    this._LanguageMore="";
                return _LanguageMore;
            }
            set
            {
                this._LanguageMore=value;
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

        ///<summary> Default Info (string format)</summary>
        public string DefaultString
        {
            get
            {
                if (String.IsNullOrEmpty(this._DefaultString))
                    this._DefaultString = "";
                return _DefaultString;
            }
            set
            {
                this._DefaultString = value;
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

        ///<summary> Forced Info (string format)</summary>
        public string ForcedString
        {
            get
            {
                if (String.IsNullOrEmpty(this._ForcedString))
                    this._ForcedString = "";
                return _ForcedString;
            }
            set
            {
                this._ForcedString = value;
            }
        }
    }
}
