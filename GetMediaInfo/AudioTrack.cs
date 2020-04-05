using System;

namespace GetMediaInfoDDLV2
{
    /// <summary>
    /// Contains properties for a AudioTrack
    /// </summary>
    public class AudioTrack
    {
        ///<summary>  Count of objects available in this stream </summary>
        public int? Count { get; set; }
        ///<summary>  bit field (0=IsAccepted, 1=IsFilled, 2=IsUpdated, 3=IsFinished) </summary>
        public string Status { get; set; }
        ///<summary>  Count of streams of that kind available </summary>
        public int? StreamCount { get; set; }
        ///<summary>  Stream type name </summary>
        public string StreamKind { get; set; }
        ///<summary>  Stream type name </summary>
        public string StreamKindString { get; set; }
        ///<summary>  Number of the stream (base=0) </summary>
        public int? StreamKindID { get; set; }
        ///<summary>  When multiple streams, number of the stream (base=1) </summary>
        public int? StreamKindPos { get; set; }
        ///<summary>  Stream order in the file, whatever is the kind of stream (base=0) </summary>
        public int? StreamOrder { get; set; }
        ///<summary>  Order of the first fully decodable packet met in the file, whatever is the kind of stream (base=0) </summary>
        public int? FirstPacketOrder { get; set; }
        ///<summary>  Last **Inform** call </summary>
        public string Inform { get; set; }
        ///<summary>  The ID for this stream in this file </summary>
        public string ID { get; set; }
        ///<summary>  The ID for this stream in this file </summary>
        public string IDString { get; set; }
        ///<summary>  The ID for this stream in the original medium of the material </summary>
        public string OriginalSourceMediumID { get; set; }
        ///<summary>  The ID for this stream in the original medium of the material </summary>
        public string OriginalSourceMediumIDString { get; set; }
        ///<summary>  The unique ID for this stream, should be copied with stream copy </summary>
        public string UniqueID { get; set; }
        ///<summary>  The unique ID for this stream, should be copied with stream copy </summary>
        public string UniqueIDString { get; set; }
        ///<summary>  The menu ID for this stream in this file </summary>
        public string MenuID { get; set; }
        ///<summary>  The menu ID for this stream in this file </summary>
        public string MenuIDString { get; set; }
        ///<summary>  Format used </summary>
        public string Format { get; set; }
        ///<summary>  Info about the format </summary>
        public string FormatInfo { get; set; }
        ///<summary>  Homepage of this format </summary>
        public string FormatUrl { get; set; }
        ///<summary>  Commercial name used by vendor for theses setings or Format field if there is no difference </summary>
        public string FormatCommercial { get; set; }
        ///<summary>  Commercial name used by vendor for theses setings if there is one </summary>
        public string FormatCommercialIfAny { get; set; }
        ///<summary>  Version of this format </summary>
        public string FormatVersion { get; set; }
        ///<summary>  Profile of the Format (old XML: 'Profile@Level' format </summary>
        public string FormatProfile { get; set; }
        ///<summary>  Level of the Format (only MIXML) </summary>
        public string FormatLevel { get; set; }
        ///<summary>  Compression method used </summary>
        public string FormatCompression { get; set; }
        ///<summary>  Settings needed for decoder used, summary </summary>
        public string FormatSettings { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsSBR { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsSBRString { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsPS { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsPSString { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsMode { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsModeExtension { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsEmphasis { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsFloor { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsFirm { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsEndianness { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsSign { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsLaw { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsITU { get; set; }
        ///<summary>  Wrapping mode (Frame wrapped or Clip wrapped) </summary>
        public string FormatSettingsWrapping { get; set; }
        ///<summary>  Matrix format (e.g. DTS Neural) </summary>
        public string MatrixFormat { get; set; }
        ///<summary>  Internet Media Type (aka MIME Type, Content-Type) </summary>
        public string InternetMediaType { get; set; }
        ///<summary>  How this stream is muxed in the container </summary>
        public string MuxingMode { get; set; }
        ///<summary>  More info (text) about the muxing mode </summary>
        public string MuxingModeMoreInfo { get; set; }
        ///<summary>  Codec ID (found in some containers) </summary>
        public string CodecID { get; set; }
        ///<summary>  Codec ID (found in some containers) </summary>
        public string CodecIDString { get; set; }
        ///<summary>  Info about codec ID </summary>
        public string CodecIDInfo { get; set; }
        ///<summary>  Hint/popular name for this codec ID </summary>
        public string CodecIDHint { get; set; }
        ///<summary>  Homepage for more details about this codec ID </summary>
        public string CodecIDUrl { get; set; }
        ///<summary>  Manual description given by the container </summary>
        public string CodecIDDescription { get; set; }
        ///<summary>  Play time of the stream </summary>
        public double? Duration { get; set; }
        ///<summary>  Play time of the stream in MilliSecond </summary>
        public ulong? DurationMilliSecond => Duration.HasValue ? (ulong?)Duration.Value  : null;
        ///<summary>  Play time in format XXx YYy only, YYy omited if zero </summary>
        internal string DurationString { get; set; }
        ///<summary>  Play time in format HHh MMmn SSs MMMms, XX omited if zero </summary>
        internal string DurationString1 { get; set; }
        ///<summary>  Play time in format XXx YYy only, YYy omited if zero </summary>
        internal string DurationString2 { get; set; }
        ///<summary>  Play time in format HH:MM:SS.MMM </summary>
        public string DurationFormatted { get; set; }
        ///<summary>  Play time in format HH:MM:SS:FF, last colon replaced by semicolon for drop frame if available </summary>
        internal string DurationString4 { get; set; }
        ///<summary>  Play time in format HH:MM:SS.mmm (HH:MM:SS:FF) </summary>
        internal string DurationString5 { get; set; }
        ///<summary>  Duration of the first frame if it is longer than others </summary>
        public double? DurationFirstFrame { get; set; }
        ///<summary>  Duration of the first frame if it is longer than others, in MilliSecond </summary>
        public ulong? DurationFirstFrameMilliSecond => DurationFirstFrame.HasValue ? (ulong?)DurationFirstFrame.Value  : null;
        ///<summary>  Duration of the first frame if it is longer than others, in format XXx YYy only, YYy omited if zero </summary>
        internal string DurationFirstFrameString { get; set; }
        ///<summary>  Duration of the first frame if it is longer than others, in format HHh MMmn SSs MMMms, XX omited if zero </summary>
        internal string DurationFirstFrameString1 { get; set; }
        ///<summary>  Duration of the first frame if it is longer than others, in format XXx YYy only, YYy omited if zero </summary>
        internal string DurationFirstFrameString2 { get; set; }
        ///<summary>  Duration of the first frame if it is longer than others, in format HH:MM:SS.MMM </summary>
        public string DurationFirstFrameFormatted { get; set; }
        ///<summary>  Play time in format HH:MM:SS:FF, last colon replaced by semicolon for drop frame if available </summary>
        internal string DurationFirstFrameString4 { get; set; }
        ///<summary>  Play time in format HH:MM:SS.mmm (HH:MM:SS:FF) </summary>
        internal string DurationFirstFrameString5 { get; set; }
        ///<summary>  Duration of the last frame if it is longer than others </summary>
        public double? DurationLastFrame { get; set; }
        ///<summary>  Duration of the last frame if it is longer than others, in MilliSecond </summary>
        public ulong? DurationLastFrameMilliSecond => DurationLastFrame.HasValue ? (ulong?)DurationLastFrame.Value  : null;
        ///<summary>  Duration of the last frame if it is longer than others, in format XXx YYy only, YYy omited if zero </summary>
        internal string DurationLastFrameString { get; set; }
        ///<summary>  Duration of the last frame if it is longer than others, in format HHh MMmn SSs MMMms, XX omited if zero </summary>
        internal string DurationLastFrameString1 { get; set; }
        ///<summary>  Duration of the last frame if it is longer than others, in format XXx YYy only, YYy omited if zero </summary>
        internal string DurationLastFrameString2 { get; set; }
        ///<summary>  Duration of the last frame if it is longer than others, in format HH:MM:SS.MMM </summary>
        public string DurationLastFrameFormatted { get; set; }
        ///<summary>  Play time in format HH:MM:SS:FF, last colon replaced by semicolon for drop frame if available </summary>
        internal string DurationLastFrameString4 { get; set; }
        ///<summary>  Play time in format HH:MM:SS.mmm (HH:MM:SS:FF) </summary>
        internal string DurationLastFrameString5 { get; set; }
        ///<summary>  Source Play time of the stream </summary>
        public double? SourceDuration { get; set; }
        ///<summary>  Source Play time of the stream, in MilliSecond </summary>
        public ulong? SourceDurationMilliSecond => SourceDuration.HasValue ? (ulong?)SourceDuration.Value  : null;
        ///<summary>  Source Play time in format XXx YYy only, YYy omited if zero </summary>
        internal string SourceDurationString { get; set; }
        ///<summary>  Source Play time in format HHh MMmn SSs MMMms, XX omited if zero </summary>
        internal string SourceDurationString1 { get; set; }
        ///<summary>  Source Play time in format XXx YYy only, YYy omited if zero </summary>
        internal string SourceDurationString2 { get; set; }
        ///<summary>  Source Play time in format HH:MM:SS.MMM </summary>
        public string SourceDurationFormatted { get; set; }
        ///<summary>  Play time in format HH:MM:SS:FF, last colon replaced by semicolon for drop frame if available </summary>
        internal string SourceDurationString4 { get; set; }
        ///<summary>  Play time in format HH:MM:SS.mmm (HH:MM:SS:FF) </summary>
        internal string SourceDurationString5 { get; set; }
        ///<summary>  Source Duration of the first frame if it is longer than others </summary>
        public double? SourceDurationFirstFrame { get; set; }
        ///<summary>  Source Duration of the first frame if it is longer than others, in MilliSecond </summary>
        public ulong? SourceDurationFirstFrameMilliSecond => SourceDurationFirstFrame.HasValue ? (ulong?)SourceDurationFirstFrame.Value  : null;
        ///<summary>  Source Duration of the first frame if it is longer than others, in format XXx YYy only, YYy omited if zero </summary>
        internal string SourceDurationFirstFrameString { get; set; }
        ///<summary>  Source Duration of the first frame if it is longer than others, in format HHh MMmn SSs MMMms, XX omited if zero </summary>
        internal string SourceDurationFirstFrameString1 { get; set; }
        ///<summary>  Source Duration of the first frame if it is longer than others, in format XXx YYy only, YYy omited if zero </summary>
        internal string SourceDurationFirstFrameString2 { get; set; }
        ///<summary>  Source Duration of the first frame if it is longer than others, in format HH:MM:SS.MMM </summary>
        public string SourceDurationFirstFrameFormatted { get; set; }
        ///<summary>  Play time in format HH:MM:SS:FF, last colon replaced by semicolon for drop frame if available </summary>
        internal string SourceDurationFirstFrameString4 { get; set; }
        ///<summary>  Play time in format HH:MM:SS.mmm (HH:MM:SS:FF) </summary>
        internal string SourceDurationFirstFrameString5 { get; set; }
        ///<summary>  Source Duration of the last frame if it is longer than others </summary>
        public double? SourceDurationLastFrame { get; set; }
        ///<summary>  Source Duration of the last frame if it is longer than others, in MilliSecond </summary>
        public ulong? SourceDurationLastFrameMilliSecond => SourceDurationLastFrame.HasValue ? (ulong?)SourceDurationLastFrame.Value  : null;
        ///<summary>  Source Duration of the last frame if it is longer than others, in format XXx YYy only, YYy omited if zero </summary>
        internal string SourceDurationLastFrameString { get; set; }
        ///<summary>  Source Duration of the last frame if it is longer than others, in format HHh MMmn SSs MMMms, XX omited if zero </summary>
        internal string SourceDurationLastFrameString1 { get; set; }
        ///<summary>  Source Duration of the last frame if it is longer than others, in format XXx YYy only, YYy omited if zero </summary>
        internal string SourceDurationLastFrameString2 { get; set; }
        ///<summary>  Source Duration of the last frame if it is longer than others, in format HH:MM:SS.MMM </summary>
        public string SourceDurationLastFrameFormatted { get; set; }
        ///<summary>  Play time in format HH:MM:SS:FF, last colon replaced by semicolon for drop frame if available </summary>
        internal string SourceDurationLastFrameString4 { get; set; }
        ///<summary>  Play time in format HH:MM:SS.mmm (HH:MM:SS:FF) </summary>
        internal string SourceDurationLastFrameString5 { get; set; }
        ///<summary>  Bit rate mode (VBR, CBR) </summary>
        public string BitRateMode { get; set; }
        ///<summary>  Bit rate mode (Constant, Variable) </summary>
        public string BitRateModeString { get; set; }
        ///<summary>  Bit rate in bps </summary>
        public double? BitRate { get; set; }
        ///<summary>  Bit rate (with measurement) </summary>
        public string BitRateString { get; set; }
        ///<summary>  Minimum Bit rate in bps </summary>
        public double? BitRateMinimum { get; set; }
        ///<summary>  Minimum Bit rate (with measurement) </summary>
        public string BitRateMinimumString { get; set; }
        ///<summary>  Nominal Bit rate in bps </summary>
        public double? BitRateNominal { get; set; }
        ///<summary>  Nominal Bit rate (with measurement) </summary>
        public string BitRateNominalString { get; set; }
        ///<summary>  Maximum Bit rate in bps </summary>
        public double? BitRateMaximum { get; set; }
        ///<summary>  Maximum Bit rate (with measurement) </summary>
        public string BitRateMaximumString { get; set; }
        ///<summary>  Encoded (with forced padding) bit rate in bps, if some container padding is present </summary>
        public double? BitRateEncoded { get; set; }
        ///<summary>  Encoded (with forced padding) bit rate (with measurement), if some container padding is present </summary>
        public string BitRateEncodedString { get; set; }
        ///<summary>  Number of channels </summary>
        public int? Channels { get; set; }
        ///<summary>  Number of channels (with measurement) </summary>
        public string ChannelsString { get; set; }
        ///<summary>  Number of channels </summary>
        public int? ChannelsOriginal { get; set; }
        ///<summary>  Number of channels (with measurement) </summary>
        public string ChannelsOriginalString { get; set; }
        ///<summary>  Number of channels after matrix decoding </summary>
        public int? MatrixChannels { get; set; }
        ///<summary>  Number of channels after matrix decoding (with measurement) </summary>
        public string MatrixChannelsString { get; set; }
        ///<summary>  Position of channels </summary>
        public string ChannelPositions { get; set; }
        ///<summary>  Position of channels (x/y.z format) </summary>
        public string ChannelPositionsString { get; set; }
        ///<summary>  Position of channels after matrix decoding </summary>
        public string MatrixChannelPositions { get; set; }
        ///<summary>  Position of channels after matrix decoding (x/y.z format) </summary>
        public string MatrixChannelPositionsString { get; set; }
        ///<summary>  Layout of channels (in the stream) </summary>
        public string ChannelLayout { get; set; }
        ///<summary>  ID of layout of channels (e.g. MXF descriptor channel assignment). Warning, sometimes this is not enough for uniquely identifying a layout (e.g. MXF descriptor channel assignment is SMPTE 377-4). For AC-3, the form is x,y with x=acmod and y=lfeon. </summary>
        public string ChannelLayoutID { get; set; }
        ///<summary>  Sampling rate </summary>
        public string SamplesPerFrame { get; set; }
        ///<summary>  Sampling rate </summary>
        public string SamplingRate { get; set; }
        ///<summary>  in KHz </summary>
        public string SamplingRateString { get; set; }
        ///<summary>  Sample count (based on sampling rate) </summary>
        public int? SamplingCount { get; set; }
        ///<summary>  Source Sample count (based on sampling rate) </summary>
        public int? SourceSamplingCount { get; set; }
        ///<summary>  Frames per second </summary>
        public double? FrameRate { get; set; }
        ///<summary>  Frames per second (with measurement) </summary>
        public string FrameRateString { get; set; }
        ///<summary>  Frames per second, numerator </summary>
        public double? FrameRateNum { get; set; }
        ///<summary>  Frames per second, denominator </summary>
        public double? FrameRateDen { get; set; }
        ///<summary>  Frame count (a frame contains a count of samples depends of the format) </summary>
        public int? FrameCount { get; set; }
        ///<summary>  Source Frame count (a frame contains a count of samples depends of the format) </summary>
        public int? SourceFrameCount { get; set; }
        ///<summary>  Resolution in bits (8, 16, 20, 24). Note: significant bits in case the stored bit depth is different </summary>
        public string BitDepth { get; set; }
        ///<summary>  Resolution in bits (8, 16, 20, 24). Note: significant bits in case the stored bit depth is different </summary>
        public string BitDepthString { get; set; }
        ///<summary>  Detected (during scan of the input by the muxer) resolution in bits </summary>
        public string BitDepthDetected { get; set; }
        ///<summary>  Detected (during scan of the input by the muxer) resolution in bits </summary>
        public string BitDepthDetectedString { get; set; }
        ///<summary>  Stored Resolution in bits (8, 16, 20, 24) </summary>
        public string BitDepthStored { get; set; }
        ///<summary>  Stored Resolution in bits (8, 16, 20, 24) </summary>
        public string BitDepthStoredString { get; set; }
        ///<summary>  Compression mode (Lossy or Lossless) </summary>
        public string CompressionMode { get; set; }
        ///<summary>  Compression mode (Lossy or Lossless) </summary>
        public string CompressionModeString { get; set; }
        ///<summary>  Current stream size divided by uncompressed stream size </summary>
        public string CompressionRatio { get; set; }
        ///<summary>  Delay fixed in the stream (relative) </summary>
        public double? Delay { get; set; }
        ///<summary>  Delay fixed in the stream (relative) IN MilliSecond </summary>
        public ulong? DelayMilliSecond => Delay.HasValue ? (ulong?)Delay.Value  : null;
        ///<summary>  Delay with measurement </summary>
        public string DelayString { get; set; }
        ///<summary>  Delay with measurement </summary>
        internal string DelayString1 { get; set; }
        ///<summary>  Delay with measurement </summary>
        internal string DelayString2 { get; set; }
        ///<summary>  Delay in format HH:MM:SS.MMM </summary>
        public string DelayFormatted { get; set; }
        ///<summary>  Delay in format HH:MM:SS:FF, last colon replaced by semicolon for drop frame if available </summary>
        internal string DelayString4 { get; set; }
        ///<summary>  Delay in format HH:MM:SS.mmm (HH:MM:SS:FF) </summary>
        internal string DelayString5 { get; set; }
        ///<summary>  Delay settings (in case of timecode for example) </summary>
        public string DelaySettings { get; set; }
        ///<summary>  Delay drop frame </summary>
        public string DelayDropFrame { get; set; }
        ///<summary>  Delay source (Container or Stream or empty) </summary>
        public string DelaySource { get; set; }
        ///<summary>  Delay source (Container or Stream or empty) </summary>
        public string DelaySourceString { get; set; }
        ///<summary>  Delay fixed in the raw stream (relative) </summary>
        public double? DelayOriginal { get; set; }
        ///<summary>  Delay fixed in the raw stream (relative) IN MilliSecond </summary>
        public ulong? DelayOriginalMilliSecond => DelayOriginal.HasValue ? (ulong?)DelayOriginal.Value  : null;
        ///<summary>  Delay with measurement </summary>
        public string DelayOriginalString { get; set; }
        ///<summary>  Delay with measurement </summary>
        internal string DelayOriginalString1 { get; set; }
        ///<summary>  Delay with measurement </summary>
        internal string DelayOriginalString2 { get; set; }
        ///<summary>  Delay in format: HH:MM:SS.MMM </summary>
        public string DelayOriginalFormatted { get; set; }
        ///<summary>  Delay in format: HH:MM:SS:FF, last colon replaced by semicolon for drop frame if available </summary>
        internal string DelayOriginalString4 { get; set; }
        ///<summary>  Delay in format HH:MM:SS.mmm (HH:MM:SS:FF) </summary>
        internal string DelayOriginalString5 { get; set; }
        ///<summary>  Delay settings (in case of timecode for example) </summary>
        public string DelayOriginalSettings { get; set; }
        ///<summary>  Delay drop frame info </summary>
        public string DelayOriginalDropFrame { get; set; }
        ///<summary>  Delay source (Stream or empty) </summary>
        public string DelayOriginalSource { get; set; }
        ///<summary>  Delay fixed in the stream (absolute / video) </summary>
        public string VideoDelay { get; set; }
        ///<summary>   </summary>
        public string VideoDelayString { get; set; }
        ///<summary>   </summary>
        internal string VideoDelayString1 { get; set; }
        ///<summary>   </summary>
        internal string VideoDelayString2 { get; set; }
        ///<summary>   </summary>
        public string VideoDelayFormatted { get; set; }
        ///<summary>   </summary>
        internal string VideoDelayString4 { get; set; }
        ///<summary>   </summary>
        internal string VideoDelayString5 { get; set; }
        ///<summary>  The gain to apply to reach 89dB SPL on playback </summary>
        public string ReplayGainGain { get; set; }
        ///<summary>   </summary>
        public string ReplayGainGainString { get; set; }
        ///<summary>  The maximum absolute peak value of the item </summary>
        public string ReplayGainPeak { get; set; }
        ///<summary>  Streamsize in bytes </summary>
        public ulong? StreamSize { get; set; }
        ///<summary>  Streamsize in with percentage value </summary>
        public string StreamSizeString { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeString1 { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeString2 { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeString3 { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeString4 { get; set; }
        ///<summary>  Stream size divided by file size </summary>
        public double? StreamSizeProportion { get; set; }
        ///<summary>  StreamSize in bytes of hte stream after demux </summary>
        public ulong? StreamSizeDemuxed { get; set; }
        ///<summary>  StreamSize_Demuxed in with percentage value </summary>
        public string StreamSizeDemuxedString { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeDemuxedString1 { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeDemuxedString2 { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeDemuxedString3 { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeDemuxedString4 { get; set; }
        ///<summary>  Source Streamsize in bytes </summary>
        public ulong? SourceStreamSize { get; set; }
        ///<summary>  Source Streamsize in with percentage value </summary>
        public string SourceStreamSizeString { get; set; }
        ///<summary>   </summary>
        internal string SourceStreamSizeString1 { get; set; }
        ///<summary>   </summary>
        internal string SourceStreamSizeString2 { get; set; }
        ///<summary>   </summary>
        internal string SourceStreamSizeString3 { get; set; }
        ///<summary>   </summary>
        internal string SourceStreamSizeString4 { get; set; }
        ///<summary>  Source Stream size divided by file size </summary>
        public ulong? SourceStreamSizeProport { get; set; }
        ///<summary>  Encoded Streamsize in bytes </summary>
        public ulong? StreamSizeEncoded { get; set; }
        ///<summary>   </summary>
        public string StreamSizeEncodedString { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeEncodedString1 { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeEncodedString2 { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeEncodedString3 { get; set; }
        ///<summary>   </summary>
        internal string StreamSizeEncodedString4 { get; set; }
        ///<summary>  Encoded Streamsize in with percentage value </summary>
        internal string StreamSizeEncodedString5 { get; set; }
        ///<summary>  Source Encoded Streamsize in bytes </summary>
        public ulong? SourceStreamSizeEncoded { get; set; }
        ///<summary>   </summary>
        internal string SourceStreamSizeEncodedString1 { get; set; }
        ///<summary>   </summary>
        internal string SourceStreamSizeEncodedString2 { get; set; }
        ///<summary>   </summary>
        internal string SourceStreamSizeEncodedString3 { get; set; }
        ///<summary>   </summary>
        internal string SourceStreamSizeEncodedString4 { get; set; }
        ///<summary>  How this stream file is aligned in the container </summary>
        public string Alignment { get; set; }
        ///<summary>  Where this stream file is aligned in the container </summary>
        public string AlignmentString { get; set; }
        ///<summary>  Between how many video frames the stream is inserted </summary>
        public string InterleaveVideoFrames { get; set; }
        ///<summary>  Between how much time (ms) the stream is inserted </summary>
        public double? InterleaveDuration { get; set; }
        ///<summary>  Between how much time and video frames the stream is inserted (with measurement) </summary>
        public string InterleaveDurationString { get; set; }
        ///<summary>  How much time is buffered before the first video frame </summary>
        public string InterleavePreload { get; set; }
        ///<summary>  How much time is buffered before the first video frame (with measurement) </summary>
        public string InterleavePreloadString { get; set; }
        ///<summary>  Name of the track </summary>
        public string Title { get; set; }
        ///<summary>  Name of the software package used to create the file, such as Microsoft WaveEdit </summary>
        public string EncodedApplication { get; set; }
        ///<summary>  Name of the software package used to create the file, such as Microsoft WaveEdit, trying to have the format 'CompanyName ProductName (OperatingSystem) Version (Date)' </summary>
        public string EncodedApplicationString { get; set; }
        ///<summary>  Name of the company </summary>
        public string EncodedApplicationCompany { get; set; }
        ///<summary>  Name of the product </summary>
        public string EncodedApplicationName { get; set; }
        ///<summary>  Version of the product </summary>
        public string EncodedApplicationVersion { get; set; }
        ///<summary>  Name of the software package used to create the file, such as Microsoft WaveEdit. </summary>
        public string EncodedApplicationUrl { get; set; }
        ///<summary>  Software used to create the file </summary>
        public string EncodedLibrary { get; set; }
        ///<summary>  Software used to create the file, trying to have the format 'CompanyName ProductName (OperatingSystem) Version (Date)' </summary>
        public string EncodedLibraryString { get; set; }
        ///<summary>  Name of the company </summary>
        public string EncodedLibraryCompanyName { get; set; }
        ///<summary>  Name of the the encoding-software </summary>
        public string EncodedLibraryName { get; set; }
        ///<summary>  Version of encoding-software </summary>
        public string EncodedLibraryVersion { get; set; }
        ///<summary>  Release date of software </summary>
        public string EncodedLibraryDate { get; set; }
        ///<summary>  Parameters used by the software </summary>
        public string EncodedLibrarySettings { get; set; }
        ///<summary>  Operating System of encoding-software </summary>
        public string EncodedOperatingSystem { get; set; }
        ///<summary>  Language (2-letter ISO 639-1 if exists, else 3-letter ISO 639-2, and with optional ISO 3166-1 country separated by a dash if available, e.g. en, en-us, zh-cn) </summary>
        public int? Language { get; set; }
        ///<summary>  Language (full) </summary>
        public string LanguageString { get; set; }
        ///<summary>  Language (full) </summary>
        internal string LanguageString1 { get; set; }
        ///<summary>  Language (2-letter ISO 639-1 if exists, else empty) </summary>
        internal string LanguageString2 { get; set; }
        ///<summary>  Language (3-letter ISO 639-2 if exists, else empty) </summary>
        public string LanguageString3 { get; set; }
        ///<summary>  Language (2-letter ISO 639-1 if exists with optional ISO 3166-1 country separated by a dash if available, e.g. en, en-us, zh-cn, else empty) </summary>
        public string LanguageString4 { get; set; }
        ///<summary>  More info about Language (e.g. Director's Comment) </summary>
        public string LanguageMore { get; set; }
        ///<summary>  Service kind, e.g. visually impaired, commentary, voice over </summary>
        public string ServiceKind { get; set; }
        ///<summary>  Service kind (full) </summary>
        public string ServiceKindString { get; set; }
        ///<summary>  Set if that track should not be used </summary>
        public string Disabled { get; set; }
        ///<summary>  Set if that track should not be used </summary>
        public string DisabledString { get; set; }
        ///<summary>  Set if that track should be used if no language found matches the user preference. </summary>
        public bool? Default { get; set; }
        ///<summary>  Set if that track should be used if no language found matches the user preference. </summary>
        public string DefaultString { get; set; }
        ///<summary>  Set if that track should be used if no language found matches the user preference. </summary>
        public bool? Forced { get; set; }
        ///<summary>  Set if that track should be used if no language found matches the user preference. </summary>
        public string ForcedString { get; set; }
        ///<summary>  Number of a group in order to provide versions of the same track </summary>
        public int? AlternateGroup { get; set; }
        ///<summary>  Number of a group in order to provide versions of the same track </summary>
        public string AlternateGroupString { get; set; }
        ///<summary>  UTC time that the encoding of this item was completed began. </summary>
        public string EncodedDate { get; set; }
        ///<summary>  UTC time that the tags were done for this item. </summary>
        public string TaggedDate { get; set; }
        ///<summary>   </summary>
        public string Encryption { get; set; }

        // Other

        ///<summary> Play time of the stream </summary>
        [Obsolete("DurationMillisec  is deprecated.")]
        public double? DurationMillisec
        { get => Duration;
            set => Duration = value;
        }
        ///<summary> Play time in format : HH:MM:SS.MMM </summary>
        [Obsolete("DurationStringFormatted  is deprecated.")]
        public string DurationStringFormatted
        {
            get => DurationFormatted;
            set => DurationFormatted = value; 
        }
        ///<summary> Channel mode </summary>
        [Obsolete("ChannelMode  is deprecated.")]
        public string ChannelMode
        { get; set; }
        ///<summary> Delay in format : HH:MM:SS.MMM </summary>
        [Obsolete("DelayStringFormatted  is deprecated.")]
        public string DelayStringFormatted
        {
            get => DelayFormatted;
            set => DelayFormatted = value;
        }
        ///<summary> Delay in format : HH:MM:SS.MMM </summary>
        [Obsolete("VideoDelayStringFormatted  is deprecated.")]
        public string VideoDelayStringFormatted
        {
            get => VideoDelayFormatted;
            set => VideoDelayFormatted = value;
        }
        ///<summary> Streamsize with percentage value </summary>
        [Obsolete("StreamSizeStringRounded is deprecated.")]
        public string StreamSizeStringRounded
        {
            get => StreamSizeString1;
            set => StreamSizeString1 = value;
        }
    
        ///<summary> Streamsize with percentage value </summary>
        [Obsolete("StreamSizeStringPrecise is deprecated.")]
        public string StreamSizeStringPrecise
        {
            get => StreamSizeString4;
            set => StreamSizeString4 = value;
        }


    }
}
