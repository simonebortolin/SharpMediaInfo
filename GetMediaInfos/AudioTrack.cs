using System;

namespace GetMediaInfos
{
    /// <summary>
    /// Contains properties for a AudioTrack
    /// </summary>
    public class AudioTrack
    {
        private int _Count;
        private int _StreamCount;
        private string _StreamKind;
        private string _StreamKindString;
        private int _StreamKindID;
        private int _StreamKindPos;
        private int _StreamOrder;
        private string _Inform;
        private string _ID;
        private string _IDString;
        private string _UniqueID;
        private string _MenuID;
        private string _MenuIDString;
        private string _Format;
        private string _FormatInfo;
        private string _FormatUrl;
        private string _FormatVersion;
        private string _FormatProfile;
        private string _FormatSettings;
        private string _FormatSettingsSBR;
        private string _FormatSettingsSBRString;
        private string _FormatSettingsPS;
        private string _FormatSettingsPSString;
        private string _FormatSettingsFloor;
        private string _FormatSettingsFirm;
        private string _FormatSettingsEndianness;
        private string _FormatSettingsSign;
        private string _FormatSettingsLaw;
        private string _FormatSettingsITU;
        private string _MuxingMode;
        private int _CodecID;
        private string _CodecIDInfo;
        private string _CodecIDHint;
        private string _CodecIDUrl;
        private string _CodecIDDescription;
        private ulong _DurationMillisec;
        private string _DurationStringFormatted;
        private ulong _BitRate;
        private string _BitRateString;
        private string _BitRateMode;
        private string _BitRateModeString;
        private ulong _BitRateMinimum;
        private string _BitRateMinimumString;
        private ulong _BitRateNominal;
        private string _BitRateNominalString;
        private ulong _BitRateMaximum;
        private string _BitRateMaximumString;
        private int _Channels;
        private string _ChannelsString;
        private string _ChannelPositions;
        private string _ChannelPositionsString2;
        private string _ChannelMode;
        private int _SamplingRate;
        private string _SamplingRateString;
        private int _SamplingCount;
        private int _BitDepth;
        private string _BitDepthString;
        private double _CompressionRatio;
        private int _Delay;
        private string _DelayString;
        private string _DelayStringFormatted;
        private int _VideoDelay;
        private string _VideoDelayString;
        private string _VideoDelayStringFormatted;
        private int _ReplayGainGain;
        private string _ReplayGainGainString;
        private string _ReplayGainPeak;
        private ulong _StreamSize;
        private string _StreamSizeString; //string
        private string _StreamSizeStringRounded; //string1
        private string _StreamSizeStringPrecise; //string4
        private double _StreamSizeProportion;
        private string _Alignment;
        private string _AlignmentString;
        private string _InterleaveVideoFrames;
        private string _InterleaveDuration;
        private string _InterleaveDurationString;
        private string _InterleavePreload;
        private string _InterleavePreloadString;
        private string _Title;
        private string _EncodedLibrary;
        private string _EncodedLibraryString;
        private string _EncodedLibraryName;
        private string _EncodedLibraryVersion;
        private string _EncodedLibraryDate;
        private string _EncodedLibrarySettings;
        private string _Language;
        private string _LanguageString;
        private string _LanguageMore;
        private string _EncodedDate;
        private string _TaggedDate;
        private string _Encryption;
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

        ///<summary> Stream name string formated</summary>
        public string StreamKindString
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamKindString))
                    this._StreamKindString = "";
                return _StreamKindString;
            }
            set
            {
                this._StreamKindString = value;
            }
        }

        ///<summary> id of the stream </summary>
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

        ///<summary> When multiple streams, number of the stream </summary>
        public int StreamKindPos
        {
            get
            {
                return _StreamKindPos;
            }
            set
            {
                this._StreamKindPos = value;
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

        ///<summary> A ID for the stream </summary>
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

        ///<summary> A ID for the stream  string formated</summary>
        public string IDString
        {
            get
            {
                if (String.IsNullOrEmpty(this._IDString))
                    this._IDString = "";
                return _IDString;
            }
            set
            {
                this._IDString = value;
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

        ///<summary> the Menu ID for this stream</summary>
        public string MenuID
        {
            get
            {
                if (String.IsNullOrEmpty(this._MenuID))
                    this._MenuID = "";
                return _MenuID;
            }
            set
            {
                this._MenuID = value;
            }
        }

        ///<summary> the Menu ID for this stream string formated</summary>
        public string MenuIDString
        {
            get
            {
                if (String.IsNullOrEmpty(this._MenuIDString))
                    this._MenuIDString = "";
                return _MenuIDString;
            }
            set
            {
                this._MenuIDString = value;
            }
        }

        ///<summary> the Format used</summary>
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

        ///<summary> Info about the Format used</summary>
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

        ///<summary> Webpage of the Format</summary>
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

        ///<summary> the Version of the Format used</summary>
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

        ///<summary> the Profile of the Format used</summary>
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

        ///<summary> the Settings of the Format used</summary>
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

        ///<summary> the SBR flag</summary>
        public string FormatSettingsSBR
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsSBR))
                    this._FormatSettingsSBR = "";
                return _FormatSettingsSBR;
            }
            set
            {
                this._FormatSettingsSBR = value;
            }
        }

        ///<summary> the SBR flag set as string</summary>
        public string FormatSettingsSBRString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsSBRString))
                    this._FormatSettingsSBRString = "";
                return _FormatSettingsSBRString;
            }
            set
            {
                this._FormatSettingsSBRString = value;
            }
        }

        ///<summary> the PS flag</summary>
        public string FormatSettingsPS
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsPS))
                    this._FormatSettingsPS = "";
                return _FormatSettingsPS;
            }
            set
            {
                this._FormatSettingsPS = value;
            }
        }

        ///<summary> the PS flag set as string</summary>
        public string FormatSettingsPSString
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsPSString))
                    this._FormatSettingsPSString = "";
                return _FormatSettingsPSString;
            }
            set
            {
                this._FormatSettingsPSString = value;
            }
        }

        ///<summary> the Floor used in the stream</summary>
        public string FormatSettingsFloor
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsFloor))
                    this._FormatSettingsFloor = "";
                return _FormatSettingsFloor;
            }
            set
            {
                this._FormatSettingsFloor = value;
            }
        }

        ///<summary> the Firm used in the settings</summary>
        public string FormatSettingsFirm
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsFirm))
                    this._FormatSettingsFirm = "";
                return _FormatSettingsFirm;
            }
            set
            {
                this._FormatSettingsFirm = value;
            }
        }

        ///<summary> the Endianness used in the stream</summary>
        public string FormatSettingsEndianness
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsEndianness))
                    this._FormatSettingsEndianness = "";
                return _FormatSettingsEndianness;
            }
            set
            {
                this._FormatSettingsEndianness = value;
            }
        }

        ///<summary> the Sign used in the stream</summary>
        public string FormatSettingsSign
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsSign))
                    this._FormatSettingsSign = "";
                return _FormatSettingsSign;
            }
            set
            {
                this._FormatSettingsSign = value;
            }
        }

        ///<summary> the Law used in the stream</summary>
        public string FormatSettingsLaw
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsLaw))
                    this._FormatSettingsLaw = "";
                return _FormatSettingsLaw;
            }
            set
            {
                this._FormatSettingsLaw = value;
            }
        }

        ///<summary> the ITU Format used in the stream</summary>
        public string FormatSettingsITU
        {
            get
            {
                if (String.IsNullOrEmpty(this._FormatSettingsITU))
                    this._FormatSettingsITU = "";
                return _FormatSettingsITU;
            }
            set
            {
                this._FormatSettingsITU = value;
            }
        }

        ///<summary> how the stream has been muxed </summary>
        public string MuxingMode
        {
            get
            {
                if (String.IsNullOrEmpty(this._MuxingMode))
                    this._MuxingMode = "";
                return _MuxingMode;
            }
            set
            {
                this._MuxingMode = value;
            }
        }

        ///<summary> the ID of the Codec, found in the container </summary>
        public int CodecID
        {
            get
            {
                return _CodecID;
            }
            set
            {
                this._CodecID = value;
            }
        }

        ///<summary> Info about the CodecID </summary>
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

        ///<summary> the Hint/popular name for the CodecID  </summary>
        public string CodecIDHint
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecIDHint))
                    this._CodecIDHint = "";
                return _CodecIDHint;
            }
            set
            {
                this._CodecIDHint = value;
            }
        }

        ///<summary> homepage for more details about the CodecID </summary>
        public string CodecIDUrl
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecIDUrl))
                    this._CodecIDUrl = "";
                return _CodecIDUrl;
            }
            set
            {
                this._CodecIDUrl = value;
            }
        }

        ///<summary> the description of the Codec ID </summary>
        public string CodecIDDescription
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecIDDescription))
                    this._CodecIDDescription = "";
                return _CodecIDDescription;
            }
            set
            {
                this._CodecIDDescription = value;
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

        ///<summary> Bit rate in bps </summary>
        public ulong BitRate
        {
            get
            {
                return _BitRate;
            }
            set
            {
                this._BitRate=value;
            }
        }

        ///<summary> Bit rate (with measurement) </summary>
        public string BitRateString
        {
            get
            {
                if (String.IsNullOrEmpty(this._BitRateString))
                    this._BitRateString="";
                return _BitRateString;
            }
            set
            {
                this._BitRateString=value;
            }
        }

        ///<summary> Bit rate mode (VBR, CBR) </summary>
        public string BitRateMode
        {
            get
            {
                if (String.IsNullOrEmpty(this._BitRateMode))
                    this._BitRateMode="";
                return _BitRateMode;
            }
            set
            {
                this._BitRateMode=value;
            }
        }

        ///<summary> Bit rate mode (VBR, CBR) formated as string </summary>
        public string BitRateModeString
        {
            get
            {
                if (String.IsNullOrEmpty(this._BitRateModeString))
                    this._BitRateModeString = "";
                return _BitRateModeString;
            }
            set
            {
                this._BitRateModeString = value;
            }
        }

        ///<summary> Minimum Bit rate mode </summary>
        public ulong BitRateMinimum
        {
            get
            {
                return _BitRateMinimum;
            }
            set
            {
                this._BitRateMinimum = value;
            }
        }

        ///<summary> Minimum Bit rate mode formated as string </summary>
        public string BitRateMinimumString
        {
            get
            {
                if (String.IsNullOrEmpty(this._BitRateMinimumString))
                    this._BitRateMinimumString = "";
                return _BitRateMinimumString;
            }
            set
            {
                this._BitRateMinimumString = value;
            }
        }

        ///<summary> Nominal Bit rate </summary>
        public ulong BitRateNominal
        {
            get
            {
                return _BitRateNominal;
            }
            set
            {
                this._BitRateNominal = value;
            }
        }

        ///<summary> Nominal Bit rate formated as string </summary>
        public string BitRateNominalString
        {
            get
            {
                if (String.IsNullOrEmpty(this._BitRateNominalString))
                    this._BitRateNominalString = "";
                return _BitRateNominalString;
            }
            set
            {
                this._BitRateNominalString = value;
            }
        }

        ///<summary> Max Bit rate </summary>
        public ulong BitRateMaximum
        {
            get
            {
                return _BitRateMaximum;
            }
            set
            {
                this._BitRateMaximum = value;
            }
        }

        ///<summary> Max Bit rate formated as string </summary>
        public string BitRateMaximumString
        {
            get
            {
                if (String.IsNullOrEmpty(this._BitRateMaximumString))
                    this._BitRateMaximumString = "";
                return _BitRateMaximumString;
            }
            set
            {
                this._BitRateMaximumString = value;
            }
        }

        ///<summary> Number of channels </summary>
        public int Channels
        {
            get
            {
                return _Channels;
            }
            set
            {
                this._Channels=value;
            }
        }

        ///<summary> Number of channels </summary>
        public string ChannelsString
        {
            get
            {
                if (String.IsNullOrEmpty(this._ChannelsString))
                    this._ChannelsString="";
                return _ChannelsString;
            }
            set
            {
                this._ChannelsString=value;
            }
        }

        ///<summary> Positions of channels </summary>
        public string ChannelPositions
        {
            get
            {
                if (String.IsNullOrEmpty(this._ChannelPositions))
                    this._ChannelPositions = "";
                return _ChannelPositions;
            }
            set
            {
                this._ChannelPositions = value;
            }
        }

        ///<summary> Positions of channels (x/y.z format) </summary>
        public string ChannelPositionsString2
        {
            get
            {
                if (String.IsNullOrEmpty(this._ChannelPositionsString2))
                    this._ChannelPositionsString2 = "";
                return _ChannelPositionsString2;
            }
            set
            {
                this._ChannelPositionsString2 = value;
            }
        }

        ///<summary> Channel mode </summary>
        public string ChannelMode
        {
            get
            {
                if (String.IsNullOrEmpty(this._ChannelMode))
                    this._ChannelMode="";
                return _ChannelMode;
            }
            set
            {
                this._ChannelMode=value;
            }
        }

        ///<summary> Sampling rate </summary>
        public int SamplingRate
        {
            get
            {
                return _SamplingRate;
            }
            set
            {
                this._SamplingRate=value;
            }
        }

        ///<summary> in KHz </summary>
        public string SamplingRateString
        {
            get
            {
                if (String.IsNullOrEmpty(this._SamplingRateString))
                    this._SamplingRateString="";
                return _SamplingRateString;
            }
            set
            {
                this._SamplingRateString=value;
            }
        }

        ///<summary> Frame count </summary>
        public int SamplingCount
        {
            get
            {
                return _SamplingCount;
            }
            set
            {
                this._SamplingCount=value;
            }
        }

        ///<summary> BitDepth in bits (8, 16, 20, 24) </summary>
        public int BitDepth
        {
            get
            {
                return _BitDepth;
            }
            set
            {
                this._BitDepth=value;
            }
        }

        ///<summary> BitDepth in bits (8, 16, 20, 24) formated as string </summary>
        public string BitDepthString
        {
            get
            {
                if (String.IsNullOrEmpty(this._BitDepthString))
                    this._BitDepthString = "";
                return _BitDepthString;
            }
            set
            {
                this._BitDepthString = value;
            }
        }

        ///<summary> Current Stream Size divided by uncompressed Stream Size </summary>
        public double CompressionRatio
        {
            get
            {
                return _CompressionRatio;
            }
            set
            {
                this._CompressionRatio = value;
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

        ///<summary> Delay in format : XXx YYy only, YYy omitted if zero </summary>
        public string DelayString
        {
            get
            {
                if (String.IsNullOrEmpty(this._DelayString))
                    this._DelayString = "";
                return _DelayString;
            }
            set
            {
                this._DelayString = value;
            }
        }

        ///<summary> Delay in format : HH:MM:SS.MMM </summary>
        public string DelayStringFormatted
        {
            get
            {
                if (String.IsNullOrEmpty(this._DelayStringFormatted))
                    this._DelayStringFormatted = "";
                return _DelayStringFormatted;
            }
            set
            {
                this._DelayStringFormatted = value;
            }
        }

        ///<summary> Delay (ms) fixed in the stream (absolute / video) </summary>
        public int VideoDelay
        {
            get
            {
                return _VideoDelay;
            }
            set
            {
                this._VideoDelay=value;
            }
        }

        ///<summary> Delay (ms) fixed in the stream (absolute / video) formated as string </summary>
        public string VideoDelayString
        {
            get
            {
                if (String.IsNullOrEmpty(this._VideoDelayString))
                    this._VideoDelayString = "";
                return _VideoDelayString;
            }
            set
            {
                this._VideoDelayString = value;
            }
        }

        ///<summary> Delay (ms) fixed in the stream (absolute / video) formated as string </summary>
        public string VideoDelayStringFormatted
        {
            get
            {
                if (String.IsNullOrEmpty(this._VideoDelayStringFormatted))
                    this._VideoDelayStringFormatted = "";
                return _VideoDelayStringFormatted;
            }
            set
            {
                this._VideoDelayStringFormatted = value;
            }
        }

        ///<summary> Play time of the stream </summary>
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

        ///<summary> Play time in format : HH:MM:SS.MMM </summary>
        public string DurationStringFormatted
        {
            get
            {
                if (String.IsNullOrEmpty(this._DurationStringFormatted))
                    this._DurationStringFormatted = "";
                return _DurationStringFormatted;
            }
            set
            {
                this._DurationStringFormatted = value;
            }
        }

        ///<summary> The gain to apply to reach 89dB SPL on playback </summary>
        public int ReplayGainGain
        {
            get
            {
                return _ReplayGainGain;
            }
            set
            {
                this._ReplayGainGain = value;
            }
        }

        ///<summary> The gain to apply to reach 89dB SPL on playback formated as string </summary>
        public string ReplayGainGainString
        {
            get
            {
                if (String.IsNullOrEmpty(this._ReplayGainGainString))
                    this._ReplayGainGainString = "";
                return _ReplayGainGainString;
            }
            set
            {
                this._ReplayGainGainString = value;
            }
        }

        ///<summary> The maximum absolute peak value of the item </summary>
        public string ReplayGainPeak
        {
            get
            {
                if (String.IsNullOrEmpty(this._ReplayGainPeak))
                    this._ReplayGainPeak = "";
                return _ReplayGainPeak;
            }
            set
            {
                this._ReplayGainPeak = value;
            }
        }

        ///<summary> Streamsize in bytes </summary>
        public ulong StreamSize
        {
            get
            {
                return _StreamSize;
            }
            set
            {
                this._StreamSize = value;
            }
        }

        ///<summary> Streamsize with percentage value </summary>
        public string StreamSizeString
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamSizeString))
                    this._StreamSizeString = "";
                return _StreamSizeString;
            }
            set
            {
                this._StreamSizeString = value;
            }
        }

        ///<summary> Streamsize with percentage value </summary>
        public string StreamSizeStringRounded
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamSizeStringRounded))
                    this._StreamSizeStringRounded = "";
                return _StreamSizeStringRounded;
            }
            set
            {
                this._StreamSizeStringRounded = value;
            }
        }

        ///<summary> Streamsize with percentage value </summary>
        public string StreamSizeStringPrecise
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamSizeStringPrecise))
                    this._StreamSizeStringPrecise = "";
                return _StreamSizeStringPrecise;
            }
            set
            {
                this._StreamSizeStringPrecise = value;
            }
        }

        ///<summary> Stream size divided by file size </summary>
        public double StreamSizeProportion
        {
            get
            {
                return _StreamSizeProportion;
            }
            set
            {
                this._StreamSizeProportion = value;
            }
        }

        ///<summary> How this stream file is aligned in the container </summary>
        public string Alignment
        {
            get
            {
                if (String.IsNullOrEmpty(this._Alignment))
                    this._Alignment = "";
                return _Alignment;
            }
            set
            {
                this._Alignment = value;
            }
        }

        ///<summary> Where this stream file is aligned in the container </summary>
        public string AlignmentString
        {
            get
            {
                if (String.IsNullOrEmpty(this._AlignmentString))
                    this._AlignmentString = "";
                return _AlignmentString;
            }
            set
            {
                this._AlignmentString = value;
            }
        }

        ///<summary> Between how many video frames the stream is inserted </summary>
        public string InterleaveVideoFrames
        {
            get
            {
                if (String.IsNullOrEmpty(this._InterleaveVideoFrames))
                    this._InterleaveVideoFrames = "";
                return _InterleaveVideoFrames;
            }
            set
            {
                this._InterleaveVideoFrames = value;
            }
        }

        ///<summary> Between how much time (ms) the stream is inserted </summary>
        public string InterleaveDuration
        {
            get
            {
                if (String.IsNullOrEmpty(this._InterleaveDuration))
                    this._InterleaveDuration = "";
                return _InterleaveDuration;
            }
            set
            {
                this._InterleaveDuration = value;
            }
        }

        ///<summary> Between how much time and video frames the stream is inserted (with measurement) </summary>
        public string InterleaveDurationString
        {
            get
            {
                if (String.IsNullOrEmpty(this._InterleaveDurationString))
                    this._InterleaveDurationString = "";
                return _InterleaveDurationString;
            }
            set
            {
                this._InterleaveDurationString = value;
            }
        }

        ///<summary> How much time is buffered before the first video frame </summary>
        public string InterleavePreload
        {
            get
            {
                if (String.IsNullOrEmpty(this._InterleavePreload))
                    this._InterleavePreload = "";
                return _InterleavePreload;
            }
            set
            {
                this._InterleavePreload = value;
            }
        }

        ///<summary> How much time is buffered before the first video frame (with measurement) </summary>
        public string InterleavePreloadString
        {
            get
            {
                if (String.IsNullOrEmpty(this._InterleavePreloadString))
                    this._InterleavePreloadString = "";
                return _InterleavePreloadString;
            }
            set
            {
                this._InterleavePreloadString = value;
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

        ///<summary> Software used to create the file formated as string </summary>
        public string EncodedLibraryString
        {
            get
            {
                if (String.IsNullOrEmpty(this._EncodedLibraryString))
                    this._EncodedLibraryString = "";
                return _EncodedLibraryString;
            }
            set
            {
                this._EncodedLibraryString = value;
            }
        }

        ///<summary> Name of the Software used to create the file </summary>
        public string EncodedLibraryName
        {
            get
            {
                if (String.IsNullOrEmpty(this._EncodedLibraryName))
                    this._EncodedLibraryName = "";
                return _EncodedLibraryName;
            }
            set
            {
                this._EncodedLibraryName = value;
            }
        }


        ///<summary> Version of the Software used to create the file </summary>
        public string EncodedLibraryVersion
        {
            get
            {
                if (String.IsNullOrEmpty(this._EncodedLibraryVersion))
                    this._EncodedLibraryVersion = "";
                return _EncodedLibraryVersion;
            }
            set
            {
                this._EncodedLibraryVersion = value;
            }
        }

        ///<summary> Date of the Software used to create the file </summary>
        public string EncodedLibraryDate
        {
            get
            {
                if (String.IsNullOrEmpty(this._EncodedLibraryDate))
                    this._EncodedLibraryDate = "";
                return _EncodedLibraryDate;
            }
            set
            {
                this._EncodedLibraryDate = value;
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

        ///<summary> UTC time that the encoding of this item was completed began. </summary>
        public string EncodedDate
        {
            get
            {
                if (String.IsNullOrEmpty(this._EncodedDate))
                    this._EncodedDate = "";
                return _EncodedDate;
            }
            set
            {
                this._EncodedDate = value;
            }
        }

        ///<summary> UTC time that the tags were done for this item. </summary>
        public string TaggedDate
        {
            get
            {
                if (String.IsNullOrEmpty(this._TaggedDate))
                    this._TaggedDate = "";
                return _TaggedDate;
            }
            set
            {
                this._TaggedDate = value;
            }
        }

        ///<summary> encryption string. </summary>
        public string Encryption
        {
            get
            {
                if (String.IsNullOrEmpty(this._Encryption))
                    this._Encryption = "";
                return _Encryption;
            }
            set
            {
                this._Encryption = value;
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
