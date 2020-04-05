using System;

namespace GetMediaInfo
{
    /// <summary>
    /// Contains properties for a VideoTrack
    /// </summary>
    public class VideoTrack
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
        private string _CodecInfo;
        private string _CodecUrl;
        private string _CodecID;
        private string _CodecIDInfo;
        private ulong _BitRate;
        private string _BitRateString;
        private ulong _MinimumBitRate;
        private string _MinimumBitRateString;
        private ulong _NominalBitRate;
        private string _NominalBitRateString;
        private ulong _MaximumBitRate;
        private string _MaximumBitRateString;
        private string _BitRateMode;
        private string _EncodedLibrary;
        private string _EncodedLibrarySettings;
        private int _Width;
        private int _Height;
        private double _AspectRatio;
        private string _AspectRatioString;
        private double _PixelAspectRatio;
        private string _PixelAspectRatioString;
        private double _FrameRate;
        private string _FrameRateString;
        private double _FrameRateOriginal;
        private string _FrameRateOriginalString;
        private string _FrameRateMode;
        private string _FrameRateModeString;
        private int _FrameCount;
        private int _BitDepth;
        private double _BitsPixelFrame;
        private int _Delay;
        private ulong _DurationMillisec;
        private string _DurationFormatted;
        private string _Language;
        private string _LanguageString;
        private string _LanguageMore;
        private string _Format;
        private string _FormatInfo;
        private string _FormatUrl;
        private string _FormatVersion;
        private string _FormatProfile;
        private int _MultiView_Count;
        private string _MultiView_Layout;
        private string _FormatSettings;
        private string _FormatSettingsBVOP;
        private string _FormatSettingsBVOPString;
        private string _FormatSettingsQPel;
        private string _FormatSettingsQPelString;
        private string _FormatSettingsGMC;
        private string _FormatSettingsGMCString;
        private string _FormatSettingsMatrix;
        private string _FormatSettingsMatrixString;
        private string _FormatSettingsMatrixData;
        private string _FormatSettingsCABAC;
        private string _FormatSettingsCABACString;
        private int _FormatSettingsRefFrames;
        private string _FormatSettingsRefFramesString;
        private string _FormatSettingsPulldown;
        private string _ScanType;
        private string _ScanTypeString;
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
                this._Count = value;
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
                this._StreamCount = value;
            }
        }

        ///<summary> Stream name </summary>
        public string StreamKind
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamKind))
                    this._StreamKind = "";
                return _StreamKind;
            }
            set
            {
                this._StreamKind = value;
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
                this._StreamKindID = value;
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
                    this._Inform = "";
                return _Inform;
            }
            set
            {
                this._Inform = value;
            }
        }

        ///<summary> A ID for this stream in this file </summary>
        public string ID
        {
            get
            {
                if (String.IsNullOrEmpty(this._ID))
                    this._ID = "";
                return _ID;
            }
            set
            {
                this._ID = value;
            }
        }

        ///<summary> A unique ID for this stream, should be copied with stream copy </summary>
        public string UniqueID
        {
            get
            {
                if (String.IsNullOrEmpty(this._UniqueID))
                    this._UniqueID = "";
                return _UniqueID;
            }
            set
            {
                this._UniqueID = value;
            }
        }

        ///<summary> Name of the track </summary>
        public string Title
        {
            get
            {
                if (String.IsNullOrEmpty(this._Title))
                    this._Title = "";
                return _Title;
            }
            set
            {
                this._Title = value;
            }
        }

        ///<summary> Codec used </summary>
        public string Codec
        {
            get
            {
                if (String.IsNullOrEmpty(this._Codec))
                    this._Codec = "";
                return _Codec;
            }
            set
            {
                this._Codec = value;
            }
        }
        
        ///<summary> Codec used (text) </summary>
        public string CodecString
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecString))
                    this._CodecString = "";
                return _CodecString;
            }
            set
            {
                this._CodecString = value;
            }
        }

        ///<summary> Info about codec </summary>
        public string CodecInfo
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecInfo))
                    this._CodecInfo = "";
                return _CodecInfo;
            }
            set
            {
                this._CodecInfo = value;
            }
        }

        ///<summary> Link </summary>
        public string CodecUrl
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecUrl))
                    this._CodecUrl = "";
                return _CodecUrl;
            }
            set
            {
                this._CodecUrl = value;
            }
        }

        ///<summary> Codec ID used </summary>
        public string CodecID
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecID))
                    this._CodecID = "";
                return _CodecID;
            }
            set
            {
                this._CodecID = value;
            }
        }

        ///<summary> Info about codec ID </summary>
        public string CodecIDInfo
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecIDInfo))
                    this._CodecIDInfo = "";
                return _CodecIDInfo;
            }
            set
            {
                this._CodecIDInfo = value;
            }
        }

        ///<summary> Bit rate in bps </summary>
        public ulong BitRate
        {
            get
            {
                return _BitRate;
            }
            set
            {
                this._BitRate = value;
            }
        }

        ///<summary> Bit rate (with measurement) </summary>
        public string BitRateString
        {
            get
            {
                if (String.IsNullOrEmpty(this._BitRateString))
                    this._BitRateString = "";
                return _BitRateString;
            }
            set
            {
                this._BitRateString = value;
            }
        }

        ///<summary> Minimum Bit rate in bps </summary>
        public ulong MinimumBitRate
        {
            get
            {
                return _MinimumBitRate;
            }
            set
            {
                this._MinimumBitRate = value;
            }
        }

        ///<summary> Minimum Bit rate (with measurement) </summary>
        public string MinimumBitRateString
        {
            get
            {
                if (String.IsNullOrEmpty(this._MinimumBitRateString))
                    this._MinimumBitRateString = "";
                return _MinimumBitRateString;
            }
            set
            {
                this._MinimumBitRateString = value;
            }
        }

        ///<summary> Nominal Bit rate in bps </summary>
        public ulong NominalBitRate
        {
            get
            {
                return _NominalBitRate;
            }
            set
            {
                this._NominalBitRate = value;
            }
        }

        ///<summary> Nominal Bit rate (with measurement) </summary>
        public string NominalBitRateString
        {
            get
            {
                if (String.IsNullOrEmpty(this._NominalBitRateString))
                    this._NominalBitRateString = "";
                return _NominalBitRateString;
            }
            set
            {
                this._NominalBitRateString = value;
            }
        }

        ///<summary> Maximum Bit rate in bps </summary>
        public ulong MaximumBitRate
        {
            get
            {
                return _MaximumBitRate;
            }
            set
            {
                this._MaximumBitRate = value;
            }
        }

        ///<summary> Maximum Bit rate (with measurement) </summary>
        public string MaximumBitRateString
        {
            get
            {
                if (String.IsNullOrEmpty(this._MaximumBitRateString))
                    this._MaximumBitRateString = "";
                return _MaximumBitRateString;
            }
            set
            {
                this._MaximumBitRateString = value;
            }
        }

        ///<summary> Bit rate mode (VBR, CBR) </summary>
        public string BitRateMode
        {
            get
            {
                if (String.IsNullOrEmpty(this._BitRateMode))
                    this._BitRateMode = "";
                return _BitRateMode;
            }
            set
            {
                this._BitRateMode = value;
            }
        }

        ///<summary> Software used to create the file </summary>
        public string EncodedLibrary
        {
            get
            {
                if (String.IsNullOrEmpty(this._EncodedLibrary))
                    this._EncodedLibrary = "";
                return _EncodedLibrary;
            }
            set
            {
                this._EncodedLibrary = value;
            }
        }

        ///<summary> Parameters used by the software </summary>
        public string EncodedLibrarySettings
        {
            get
            {
                if (String.IsNullOrEmpty(this._EncodedLibrarySettings))
                    this._EncodedLibrarySettings = "";
                return _EncodedLibrarySettings;
            }
            set
            {
                this._EncodedLibrarySettings = value;
            }
        }

        ///<summary> Width </summary>
        public int Width
        {
            get
            {
                return _Width;
            }
            set
            {
                this._Width = value;
            }
        }

        ///<summary> Height </summary>
        public int Height
        {
            get
            {
                return _Height;
            }
            set
            {
                this._Height = value;
            }
        }

        ///<summary> Aspect ratio </summary>
        public double AspectRatio
        {
            get
            {
                return _AspectRatio;
            }
            set
            {
                this._AspectRatio = value;
            }
        }

        ///<summary> Aspect ratio </summary>
        public string AspectRatioString
        {
            get
            {
                if (String.IsNullOrEmpty(this._AspectRatioString))
                    this._AspectRatioString = "";
                return _AspectRatioString;
            }
            set
            {
                this._AspectRatioString = value;
            }
        }

        ///<summary> Pixel Aspect Ratio </summary>
        public double PixelAspectRatio
        {
            get
            {
                return _PixelAspectRatio;
            }
            set
            {
                this._PixelAspectRatio = value;
            }
        }

        ///<summary> Pixel Aspect Ratio </summary>
        public string PixelAspectRatioString
        {
            get
            {
                if (String.IsNullOrEmpty(this._PixelAspectRatioString))
                    this._PixelAspectRatioString = "";
                return _PixelAspectRatioString;
            }
            set
            {
                this._PixelAspectRatioString = value;
            }
        }

        ///<summary> Frame rate </summary>
        public double FrameRate
        {
            get
            {
                return _FrameRate;
            }
            set
            {
                this._FrameRate = value;
            }
        }

        ///<summary> Frame rate </summary>
        public string FrameRateString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FrameRateString))
                    this._FrameRateString = "";
                return _FrameRateString;
            }
            set
            {
                this._FrameRateString = value;
            }
        }

        ///<summary> Frame rate original </summary>
        public double FrameRateOriginal
        {
            get
            {
                return _FrameRateOriginal;
            }
            set
            {
                this._FrameRateOriginal = value;
            }
        }

        ///<summary> Frame rate original</summary>
        public string FrameRateOriginalString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FrameRateOriginalString))
                    this._FrameRateOriginalString = "";
                return _FrameRateOriginalString;
            }
            set
            {
                this._FrameRateOriginalString = value;
            }
        }

        ///<summary> Frame rate mode</summary>
        public string FrameRateMode
        {
            get
            {
                if (String.IsNullOrEmpty(this._FrameRateMode))
                    this._FrameRateMode = "";
                return _FrameRateMode;
            }
            set
            {
                this._FrameRateMode = value;
            }
        }

        ///<summary> Frame rate mode</summary>
        public string FrameRateModeString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FrameRateModeString))
                    this._FrameRateModeString = "";
                return _FrameRateModeString;
            }
            set
            {
                this._FrameRateModeString = value;
            }
        }

        ///<summary> Frame count </summary>
        public int FrameCount
        {
            get
            {
                return _FrameCount;
            }
            set
            {
                this._FrameCount = value;
            }
        }

        ///<summary> example for MP3 : 16 bits </summary>
        public int BitDepth
        {
            get
            {
                return _BitDepth;
            }
            set
            {
                this._BitDepth = value;
            }
        }

        ///<summary> bits_(Pixel Frame) (like Gordian Knot) </summary>
        public double BitsPixelFrame
        {
            get
            {
                return _BitsPixelFrame;
            }
            set
            {
                this._BitsPixelFrame = value;
            }
        }

        ///<summary> Delay fixed in the stream (relative) IN MS </summary>
        public int Delay
        {
            get
            {
                return _Delay;
            }
            set
            {
                this._Delay = value;
            }
        }

        ///<summary> Duration of the stream in milliseconds </summary>
        public ulong DurationMillisec
        {
            get
            {
                return _DurationMillisec;
            }
            set
            {
                this._DurationMillisec = value;
            }
        }

        ///<summary> Duration in format : HH:MM:SS.MMM </summary>
        public string DurationString
        {
            get
            {
                if (String.IsNullOrEmpty(this._DurationFormatted))
                    this._DurationFormatted = "";
                return _DurationFormatted;
            }
            set
            {
                this._DurationFormatted = value;
            }
        }

        ///<summary> Language (2 letters) </summary>
        public string Language
        {
            get
            {
                if (String.IsNullOrEmpty(this._Language))
                    this._Language = "";
                return _Language;
            }
            set
            {
                this._Language = value;
            }
        }

        ///<summary> Language (full) </summary>
        public string LanguageString
        {
            get
            {
                if (String.IsNullOrEmpty(this._LanguageString))
                    this._LanguageString = "";
                return _LanguageString;
            }
            set
            {
                this._LanguageString = value;
            }
        }

        ///<summary> More info about Language (director s comment...) </summary>
        public string LanguageMore
        {
            get
            {
                if (String.IsNullOrEmpty(this._LanguageMore))
                    this._LanguageMore = "";
                return _LanguageMore;
            }
            set
            {
                this._LanguageMore = value;
            }
        }

        ///<summary> Format </summary>
        public string Format
        {
            get
            {
                if (String.IsNullOrEmpty(this._Format))
                    this._Format = "";
                return _Format;
            }
            set
            {
                this._Format = value;
            }
        }

        ///<summary> Info about Format </summary>
        public string FormatInfo
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatInfo))
                    this._FormatInfo = "";
                return _FormatInfo;
            }
            set
            {
                this._FormatInfo = value;
            }
        }

        ///<summary> Url about Format </summary>
        public string FormatUrl
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatUrl))
                    this._FormatUrl = "";
                return _FormatUrl;
            }
            set
            {
                this._FormatUrl = value;
            }
        }

        ///<summary> Version of the Format </summary>
        public string FormatVersion
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatVersion))
                    this._FormatVersion = "";
                return _FormatVersion;
            }
            set
            {
                this._FormatVersion = value;
            }
        }

        ///<summary> Profile of the Format </summary>
        public string FormatProfile
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatProfile))
                    this._FormatProfile = "";
                return _FormatProfile;
            }
            set
            {
                this._FormatProfile = value;
            }
        }

        ///<summary> MultiView Count </summary>
        public int MultiViewCount
        {
            get
            {
                return _MultiView_Count;
            }
            set
            {
                this._MultiView_Count = value;
            }
        }

        ///<summary> Multi View Layout </summary>
        public string MultiViewLayout
        {
            get
            {
                if (String.IsNullOrEmpty(this._MultiView_Layout))
                    this._MultiView_Layout = "";
                return _MultiView_Layout;
            }
            set
            {
                this._MultiView_Layout = value;
            }
        }

        ///<summary> Settings of the Format </summary>
        public string FormatSettings
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettings))
                    this._FormatSettings = "";
                return _FormatSettings;
            }
            set
            {
                this._FormatSettings = value;
            }
        }

        ///<summary> BVOP Info </summary>
        public string FormatSettingsBVOP
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsBVOP))
                    this._FormatSettingsBVOP = "";
                return _FormatSettingsBVOP;
            }
            set
            {
                this._FormatSettingsBVOP = value;
            }
        }

        ///<summary> BVOP Info (string format)</summary>
        public string FormatSettingsBVOPString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsBVOPString))
                    this._FormatSettingsBVOPString = "";
                return _FormatSettingsBVOPString;
            }
            set
            {
                this._FormatSettingsBVOPString = value;
            }
        }

        ///<summary> Qpel Info </summary>
        public string FormatSettingsQPel
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsQPel))
                    this._FormatSettingsQPel = "";
                return _FormatSettingsQPel;
            }
            set
            {
                this._FormatSettingsQPel = value;
            }
        }

        ///<summary> Qpel Info (string format)</summary>
        public string FormatSettingsQPelString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsQPelString))
                    this._FormatSettingsQPelString = "";
                return _FormatSettingsQPelString;
            }
            set
            {
                this._FormatSettingsQPelString = value;
            }
        }

        ///<summary> GMC Info </summary>
        public string FormatSettingsGMC
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsGMC))
                    this._FormatSettingsGMC = "";
                return _FormatSettingsGMC;
            }
            set
            {
                this._FormatSettingsGMC = value;
            }
        }

        ///<summary> GMC Info (string format)</summary>
        public string FormatSettingsGMCString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsGMCString))
                    this._FormatSettingsGMCString = "";
                return _FormatSettingsGMCString;
            }
            set
            {
                this._FormatSettingsGMCString = value;
            }
        }

        ///<summary> Matrix Info </summary>
        public string FormatSettingsMatrix
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsMatrix))
                    this._FormatSettingsMatrix = "";
                return _FormatSettingsMatrix;
            }
            set
            {
                this._FormatSettingsMatrix = value;
            }
        }

        ///<summary> Matrix Info (string format)</summary>
        public string FormatSettingsMatrixString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsMatrixString))
                    this._FormatSettingsMatrixString = "";
                return _FormatSettingsMatrixString;
            }
            set
            {
                this._FormatSettingsMatrixString = value;
            }
        }

        ///<summary> Matrix Info (data format)</summary>
        public string FormatSettingsMatrixData
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsMatrixData))
                    this._FormatSettingsMatrixData = "";
                return _FormatSettingsMatrixData;
            }
            set
            {
                this._FormatSettingsMatrixData = value;
            }
        }

        ///<summary> CABAC Info </summary>
        public string FormatSettingsCABAC
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsCABAC))
                    this._FormatSettingsCABAC = "";
                return _FormatSettingsCABAC;
            }
            set
            {
                this._FormatSettingsCABAC = value;
            }
        }

        ///<summary> CABAC Info (string format)</summary>
        public string FormatSettingsCABACString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsCABACString))
                    this._FormatSettingsCABACString = "";
                return _FormatSettingsCABACString;
            }
            set
            {
                this._FormatSettingsCABACString = value;
            }
        }

        ///<summary> RefFrames Info </summary>
        public int FormatSettingsRefFrames
        {
            get
            {
                return _FormatSettingsRefFrames;
            }
            set
            {
                this._FormatSettingsRefFrames = value;
            }
        }

        ///<summary> RefFrames Info (string format)</summary>
        public string FormatSettingsRefFramesString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsRefFramesString))
                    this._FormatSettingsRefFramesString = "";
                return _FormatSettingsRefFramesString;
            }
            set
            {
                this._FormatSettingsRefFramesString = value;
            }
        }

        ///<summary> Pulldown Info </summary>
        public string FormatSettingsPulldown
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsPulldown))
                    this._FormatSettingsPulldown = "";
                return _FormatSettingsPulldown;
            }
            set
            {
                this._FormatSettingsPulldown = value;
            }
        }

        ///<summary> ScanType Info </summary>
        public string ScanType
        {
            get
            {
                if (String.IsNullOrEmpty(this._ScanType))
                    this._ScanType = "";
                return _ScanType;
            }
            set
            {
                this._ScanType = value;
            }
        }

        ///<summary> ScanType Info (string format)</summary>
        public string ScanTypeString
        {
            get
            {
                if (String.IsNullOrEmpty(this._ScanTypeString))
                    this._ScanTypeString = "";
                return _ScanTypeString;
            }
            set
            {
                this._ScanTypeString = value;
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
