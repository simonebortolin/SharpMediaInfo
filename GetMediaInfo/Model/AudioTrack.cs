namespace GetMediaInfo.Model
{
    /// <summary>
    /// Contains properties for a AudioTrack
    /// </summary>
    public class AudioTrack
    {
        #region Properties
        /// <summary>
        /// Count of objects available in this stream.
        /// </summary>
        public int? Count { get; set; } //Count

        /// <summary>
        /// Count of streams of that kind available.
        /// </summary>
        public int? StreamCount { get; set; } //StreamCount

        /// <summary>
        /// Stream type name.
        /// </summary>
        public string StreamKind { get; set; } //StreamKind

        /// <summary>
        /// Number of the stream (base=0).
        /// </summary>
        public int? StreamKindID { get; set; } //StreamKindID

        /// <summary>
        /// When multiple streams, number of the stream (base=1).
        /// </summary>
        public int? StreamKindPos { get; set; } //StreamKindPos

        /// <summary>
        /// Stream order in the file, whatever is the kind of stream (base=0).
        /// </summary>
        public int? StreamOrder { get; set; } //StreamOrder

        /// <summary>
        /// Last **Inform** call.
        /// </summary>
        public string Inform { get; set; } //Inform

        /// <summary>
        /// The ID for this stream in this file.
        /// </summary>
        public int? ID { get; set; } //ID

        /// <summary>
        /// The ID for this stream in the original medium of the material.
        /// </summary>
        public string OriginalSourceMediumID { get; set; } //OriginalSourceMedium_ID

        /// <summary>
        /// The unique ID for this stream, should be copied with stream copy.
        /// </summary>
        public string UniqueID { get; set; } //UniqueID

        /// <summary>
        /// Format used.
        /// </summary>
        public string Format { get; set; } //Format

        /// <summary>
        /// Format used + additional features.
        /// </summary>
        public string FormatWithExtra { get; set; } //Format/String

        /// <summary>
        /// Info about the format.
        /// </summary>
        public string FormatInfo { get; set; } //Format/Info

        /// <summary>
        /// Homepage of this format.
        /// </summary>
        public string FormatUrl { get; set; } //Format/Url

        /// <summary>
        /// Commercial name used by vendor for theses setings or Format field if there is no difference.
        /// </summary>
        public string FormatCommercial { get; set; } //Format_Commercial

        /// <summary>
        /// Version of this format.
        /// </summary>
        public string FormatVersion { get; set; } //Format_Version

        /// <summary>
        /// Profile of this Format.
        /// </summary>
        public string FormatProfile { get; set; } //Format_Profile

        /// <summary>
        /// Compression method used.
        /// </summary>
        public string FormatCompression { get; set; } //Format_Compression

        /// <summary>
        /// Settings needed for decoder used, summary.
        /// </summary>
        public string FormatSettings { get; set; } //Format_Settings

        /// <summary>
        /// Format_Settings_SBR
        /// </summary>
        public string FormatSettingsSBR { get; set; } //Format_Settings_SBR

        /// <summary>
        /// Format_Settings_PS
        /// </summary>
        public string FormatSettingsPS { get; set; } //Format_Settings_PS

        /// <summary>
        /// Format_Settings_Mode
        /// </summary>
        public string FormatSettingsMode { get; set; } //Format_Settings_Mode

        /// <summary>
        /// Format_Settings_ModeExtension
        /// </summary>
        public string FormatSettingsModeExtension { get; set; } //Format_Settings_ModeExtension

        /// <summary>
        /// Format_Settings_Emphasis
        /// </summary>
        public string FormatSettingsEmphasis { get; set; } //Format_Settings_Emphasis

        /// <summary>
        /// Format_Settings_Floor
        /// </summary>
        public string FormatSettingsFloor { get; set; } //Format_Settings_Floor

        /// <summary>
        /// Format_Settings_Firm
        /// </summary>
        public string FormatSettingsFirm { get; set; } //Format_Settings_Firm

        /// <summary>
        /// Format_Settings_Endianness
        /// </summary>
        public string FormatSettingsEndianness { get; set; } //Format_Settings_Endianness

        /// <summary>
        /// Format_Settings_Sign
        /// </summary>
        public string FormatSettingsSign { get; set; } //Format_Settings_Sign

        /// <summary>
        /// Format_Settings_Law
        /// </summary>
        public string FormatSettingsLaw { get; set; } //Format_Settings_Law

        /// <summary>
        /// Format_Settings_ITU
        /// </summary>
        public string FormatSettingsITU { get; set; } //Format_Settings_ITU

        /// <summary>
        /// Wrapping mode (Frame wrapped or Clip wrapped).
        /// </summary>
        public string FormatSettingsWrapping { get; set; } //Format_Settings_Wrapping

        /// <summary>
        /// Format features needed for fully supporting the content.
        /// </summary>
        public string FormatAdditionalFeatures { get; set; } //Format_AdditionalFeatures

        /// <summary>
        /// Matrix format (e.g. DTS Neural).
        /// </summary>
        public string MatrixFormat { get; set; } //Matrix_Format

        /// <summary>
        /// Internet Media Type (aka MIME Type, Content-Type).
        /// </summary>
        public string InternetMediaType { get; set; } //InternetMediaType

        /// <summary>
        /// How this stream is muxed in the container.
        /// </summary>
        public string MuxingMode { get; set; } //MuxingMode

        /// <summary>
        /// More info (text) about the muxing mode.
        /// </summary>
        public string MuxingModeMoreInfo { get; set; } //MuxingMode_MoreInfo

        /// <summary>
        /// Codec ID (found in some containers).
        /// </summary>
        public int? CodecID{ get; set; } //CodecID

        /// <summary>
        /// Info about codec ID.
        /// </summary>
        public string CodecIDInfo { get; set; } //CodecID/Info

        /// <summary>
        /// Hint/popular name for this codec ID.
        /// </summary>
        public string CodecIDHint { get; set; } //CodecID/Hint

        /// <summary>
        /// Homepage for more details about this codec ID.
        /// </summary>
        public string CodecIDUrl { get; set; } //CodecID/Url

        /// <summary>
        /// Manual description given by the container.
        /// </summary>
        public string CodecIDDescription { get; set; } //CodecID_Description

        /// <summary>
        /// Play time of the stream in ms.
        /// </summary>
        public int? Duration { get; set; } //Duration

        /// <summary>
        /// Play time in format : XXx YYy only, YYy omitted if zero.
        /// </summary>
        public string DurationFormatted { get; set; } //Duration/String

        /// <summary>
        /// Play time in format : HHh MMmn SSs MMMms, XX omitted if zero.
        /// </summary>
        public string DurationFormattedComplete { get; set; } //Duration/String1

        /// <summary>
        /// Play time in format : HH:MM:SS.MMM.
        /// </summary>
        public string DurationTimecode { get; set; } //Duration/String3

        /// <summary>
        /// Duration of the first frame if it is longer than others, in ms.
        /// </summary>
        public int? DurationFirstFrame { get; set; } //Duration_FirstFrame

        /// <summary>
        /// Duration of the first frame if it is longer than others, in format : XXx YYy only, YYy omited if zero.
        /// </summary>
        public string DurationFirstFrameFormatted { get; set; } //Duration_FirstFrame/String

        /// <summary>
        /// Duration of the first frame if it is longer than others, in format : HHh MMmn SSs MMMms, XX omited if zero.
        /// </summary>
        public string DurationFirstFrameFormattedComplete { get; set; } //Duration_FirstFrame/String1

        /// <summary>
        /// Play time in format : HH:MM:SS.MMM.
        /// </summary>
        public string DurationFirstFrameTimecode { get; set; } //Duration_FirstFrame/String3

        /// <summary>
        /// Duration of the last frame if it is longer than others, in ms.
        /// </summary>
        public int? DurationLastFrame { get; set; } //Duration_LastFrame

        /// <summary>
        /// Duration of the last frame if it is longer than others, in format : XXx YYy only, YYy omited if zero.
        /// </summary>
        public string DurationLastFrameFormatted { get; set; } //Duration_LastFrame/String

        /// <summary>
        /// Duration of the last frame if it is longer than others, in format : HHh MMmn SSs MMMms, XX omited if zero.
        /// </summary>
        public string DurationLastFrameFormattedComplete { get; set; } //Duration_LastFrame/String1

        /// <summary>
        /// Play time in format : HH:MM:SS.MMM.
        /// </summary>
        public string DurationLastFrameTimecode { get; set; } //Duration_LastFrame/String3

        /// <summary>
        /// Source Play time of the stream, in ms.
        /// </summary>
        public int? SourceDuration { get; set; } //Source_Duration

        /// <summary>
        /// Source Play time in format : XXx YYy only, YYy omited if zero.
        /// </summary>
        public string SourceDurationFormatted { get; set; } //Source_Duration/String

        /// <summary>
        /// Source Play time in format : HHh MMmn SSs MMMms, XX omited if zero.
        /// </summary>
        public string SourceDurationFormattedComplete { get; set; } //Source_Duration/String1

        /// <summary>
        /// Play time in format : HH:MM:SS.MMM.
        /// </summary>
        public string SourceDurationTimecode { get; set; } //Source_Duration/String3

        /// <summary>
        /// Source Duration of the first frame if it is longer than others, in ms.
        /// </summary>
        public int? SourceDurationFirstFrame { get; set; } //Source_Duration_FirstFrame

        /// <summary>
        /// Source Duration of the first frame if it is longer than others, in format : XXx YYy only, YYy omited if zero.
        /// </summary>
        public string SourceDurationFirstFrameFormatted { get; set; } //Source_Duration_FirstFrame/String

        /// <summary>
        /// Source Duration of the first frame if it is longer than others, in format : HHh MMmn SSs MMMms, XX omited if zero.
        /// </summary>
        public string SourceDurationFirstFrameFormattedComplete { get; set; } //Source_Duration_FirstFrame/String1

        /// <summary>
        /// Play time in format : HH:MM:SS.MMM.
        /// </summary>
        public string SourceDurationFirstFrameTimecode { get; set; } //Source_Duration_FirstFram/String3

        /// <summary>
        /// Source Duration of the last frame if it is longer than others, in ms.
        /// </summary>
        public int? SourceDurationLastFrame { get; set; } //Source_Duration_LastFrame

        /// <summary>
        /// Source Duration of the last frame if it is longer than others, in format : XXx YYy only, YYy omited if zero.
        /// </summary>
        public string SourceDurationLastFrameFormatted { get; set; } //Source_Duration_LastFrame/String

        /// <summary>
        /// Source Duration of the last frame if it is longer than others, in format : HHh MMmn SSs MMMms, XX omited if zero.
        /// </summary>
        public string SourceDurationLastFrameFormattedComplete { get; set; } //Source_Duration_LastFrame/String1

        /// <summary>
        /// Play time in format : HH:MM:SS.MMM.
        /// </summary>
        public string SourceDurationLastFrameTimecode { get; set; } //Source_Duration_LastFrame/String3

        /// <summary>
        /// Bit rate mode (VBR, CBR).
        /// </summary>
        public string BitRateMode { get; set; } //BitRate_Mode

        /// <summary>
        /// Bit rate in bps.
        /// </summary>
        public ulong? BitRate { get; set; } //BitRate

        /// <summary>
        /// Bit rate (with measurement).
        /// </summary>
        public string BitRateFormatted { get; set; } //BitRate/String

        /// <summary>
        /// Minimum Bit rate in bps.
        /// </summary>
        public ulong? BitRateMinimum { get; set; } //BitRate_Minimum

        /// <summary>
        /// Minimum Bit rate (with measurement).
        /// </summary>
        public string BitRateMinimumFormatted { get; set; } //BitRate_Minimum/String

        /// <summary>
        /// Nominal Bit rate in bps.
        /// </summary>
        public ulong? BitRateNominal { get; set; } //BitRate_Nominal

        /// <summary>
        /// Nominal Bit rate (with measurement).
        /// </summary>
        public string BitRateNominalFormatted { get; set; } //BitRate_Nominal/String

        /// <summary>
        /// Maximum Bit rate in bps.
        /// </summary>
        public ulong? BitRateMaximum { get; set; } //BitRate_Maximum

        /// <summary>
        /// Maximum Bit rate (with measurement).
        /// </summary>
        public string BitRateMaximumFormatted { get; set; } //BitRate_Maximum/String

        /// <summary>
        /// Encoded (with forced padding) bit rate in bps, if some container padding is present.
        /// </summary>
        public ulong? BitRateEncoded { get; set; } //BitRate_Encoded

        /// <summary>
        /// Encoded (with forced padding) bit rate (with measurement), if some container padding is present.
        /// </summary>
        public string BitRateEncodedFormatted { get; set; } //BitRate_Encoded/String

        /// <summary>
        /// Number of channels.
        /// </summary>
        public int? Channels { get; set; } //Channel(s)

        /// <summary>
        /// Number of channels (with measurement).
        /// </summary>
        public string ChannelsFormatted { get; set; } //Channel(s)/String

        /// <summary>
        /// Number of channels.
        /// </summary>
        public int? ChannelsOriginal { get; set; } //Channel(s)_Original

        /// <summary>
        /// Number of channels (with measurement).
        /// </summary>
        public string ChannelsOriginalFormatted { get; set; } //Channel(s)_Original/String

        /// <summary>
        /// Number of channels after matrix decoding.
        /// </summary>
        public int? MatrixChannels { get; set; } //Matrix_Channel(s)

        /// <summary>
        /// Number of channels after matrix decoding (with measurement).
        /// </summary>
        public string MatrixChannelsString { get; set; } //Matrix_Channel(s)/String

        /// <summary>
        /// Position of channels.
        /// </summary>
        public string ChannelPositions { get; set; } //ChannelPositions

        /// <summary>
        /// Position of channels.
        /// </summary>
        public string ChannelPositionsOriginal { get; set; } //ChannelPositions_Original

        /// <summary>
        /// Position of channels (x/y.z format).
        /// </summary>
        public string ChannelPositionsFormatted { get; set; } //ChannelPositions/String2

        /// <summary>
        /// Position of channels (x/y.z format).
        /// </summary>
        public string ChannelPositionsOriginalFormatted { get; set; } //ChannelPositions_Original/String2

        /// <summary>
        /// Position of channels after matrix decoding.
        /// </summary>
        public string MatrixChannelPositions { get; set; } //Matrix_ChannelPositions

        /// <summary>
        /// Position of channels after matrix decoding (x/y.z format).
        /// </summary>
        public string MatrixChannelPositionsFormatted { get; set; } //Matrix_ChannelPositions/String

        /// <summary>
        /// Layout of channels (in the stream).
        /// </summary>
        public string ChannelLayout { get; set; } //ChannelLayout

        /// <summary>
        /// Layout of channels (in the stream).
        /// </summary>
        public string ChannelLayoutOriginal { get; set; } //ChannelLayout_Original

        /// <summary>
        /// ID of layout of channels (e.g. MXF descriptor channel assignment). Warning, sometimes this is not enough for uniquely identifying a layout (e.g. MXF descriptor channel assignment is SMPTE 377-4). For AC-3, the form is x,y with x=acmod and y=lfeon.
        /// </summary>
        public string ChannelLayoutID { get; set; } //ChannelLayoutID

        /// <summary>
        /// Sampling rate.
        /// </summary>
        public int? SamplesPerFrame { get; set; } //SamplesPerFrame

        /// <summary>
        /// Sampling rate.
        /// </summary>
        public int? SamplingRate { get; set; } //SamplingRate

        /// <summary>
        /// Sampling rate with measurement (in KHz).
        /// </summary>
        public string SamplingRateFormatted { get; set; } //SamplingRate/String

        /// <summary>
        /// Sample count (based on sampling rate).
        /// </summary>
        public int? SamplingCount { get; set; } //SamplingCount

        /// <summary>
        /// Source Sample count (based on sampling rate).
        /// </summary>
        public int? SourceSamplingCount { get; set; } //Source_SamplingCount

        /// <summary>
        /// Frames per second.
        /// </summary>
        public double? FrameRate { get; set; } //FrameRate

        /// <summary>
        /// Frames per second (with measurement).
        /// </summary>
        public string FrameRateFormatted { get; set; } //FrameRate/String

        /// <summary>
        /// Frames per second, numerator.
        /// </summary>
        public int? FrameRateNum { get; set; } //FrameRate_Num

        /// <summary>
        /// Frames per second, denominator.
        /// </summary>
        public int? FrameRateDen { get; set; } //FrameRate_Den

        /// <summary>
        /// Frame count (a frame contains a count of samples depends of the format).
        /// </summary>
        public int? FrameCount { get; set; } //FrameCount

        /// <summary>
        /// Source Frame count (a frame contains a count of samples depends of the format).
        /// </summary>
        public int? SourceFrameCount { get; set; } //Source_FrameCount

        /// <summary>
        /// Resolution in bits (8, 16, 20, 24). Note: significant bits in case the stored bit depth is different.
        /// </summary>
        public int? BitDepth { get; set; } //BitDepth

        /// <summary>
        /// Resolution in bits (8, 16, 20, 24). Note: significant bits in case the stored bit depth is different.
        /// </summary>
        public string BitDepthFormatted { get; set; } //BitDepth/String

        /// <summary>
        /// Detected (during scan of the input by the muxer) resolution in bits.
        /// </summary>
        public int? BitDepthDetected { get; set; } //BitDepth_Detected

        /// <summary>
        /// Detected (during scan of the input by the muxer) resolution in bits.
        /// </summary>
        public string BitDepthDetectedFormatted { get; set; } //BitDepth_Detected/String

        /// <summary>
        /// Stored Resolution in bits (8, 16, 20, 24).
        /// </summary>
        public int? BitDepthStored { get; set; } //BitDepth_Stored

        /// <summary>
        /// Stored Resolution in bits (8, 16, 20, 24).
        /// </summary>
        public string BitDepthStoredFormatted { get; set; } //BitDepth_Stored/String

        /// <summary>
        /// Compression mode (Lossy or Lossless).
        /// </summary>
        public string CompressionMode { get; set; } //Compression_Mode

        /// <summary>
        /// Current stream size divided by uncompressed stream size.
        /// </summary>
        public double? CompressionRatio { get; set; } //Compression_Ratio

        /// <summary>
        /// Delay fixed in the stream (relative) IN MS.
        /// </summary>
        public int? Delay { get; set; } //Delay

        /// <summary>
        /// Delay with measurement.
        /// </summary>
        public string DelayFormatted { get; set; } //Delay/String

        /// <summary>
        /// Delay with measurement.
        /// </summary>
        public string DelayFormattedComplete { get; set; } //Delay/String1

        /// <summary>
        /// Delay in format : HH:MM:SS.mmm (HH:MM:SS:FF).
        /// </summary>
        public string DelayTimecode { get; set; } //Delay/String5

        /// <summary>
        /// Delay settings (in case of timecode for example).
        /// </summary>
        public string DelaySettings { get; set; } //Delay_Settings

        /// <summary>
        /// Delay drop frame.
        /// </summary>
        public string DelayDropFrame { get; set; } //Delay_DropFrame

        /// <summary>
        /// Delay source (Container or Stream or empty).
        /// </summary>
        public string DelaySource { get; set; } //Delay_Source

        /// <summary>
        /// Delay fixed in the raw stream (relative) IN MS.
        /// </summary>
        public int? DelayOriginal { get; set; } //Delay_Original

        /// <summary>
        /// Delay with measurement.
        /// </summary>
        public string DelayOriginalFormatted { get; set; } //Delay_Original/String

        /// <summary>
        /// Delay with measurement.
        /// </summary>
        public string DelayOriginalFormattedComplete { get; set; } //Delay_Original/String1

        /// <summary>
        /// Delay in format : HH:MM:SS.mmm (HH:MM:SS:FF).
        /// </summary>
        public string DelayOriginalTimecode { get; set; } //Delay_Original/String5

        /// <summary>
        /// Delay settings (in case of timecode for example).
        /// </summary>
        public string DelayOriginalSettings { get; set; } //Delay_Original_Settings

        /// <summary>
        /// Delay drop frame info.
        /// </summary>
        public string DelayOriginalDropFrame { get; set; } //Delay_Original_DropFrame

        /// <summary>
        /// Delay source (Stream or empty).
        /// </summary>
        public string DelayOriginalSource { get; set; } //Delay_Original_Source

        /// <summary>
        /// Delay fixed in the stream (absolute / video).
        /// </summary>
        public int? VideoDelay { get; set; } //Video_Delay

        /// <summary>
        /// Video_Delay/String
        /// </summary>
        public string VideoDelayFormatted { get; set; } //Video_Delay/String

        /// <summary>
        /// Video_Delay/String1
        /// </summary>
        public string VideoDelayFormattedComplete { get; set; } //Video_Delay/String1

        /// <summary>
        /// Video_Delay/String5
        /// </summary>
        public string VideoDelayTimecode { get; set; } //Video_Delay/String5

        /// <summary>
        /// The gain to apply to reach 89dB SPL on playback.
        /// </summary>
        public string ReplayGainGain { get; set; } //ReplayGain_Gain

        /// <summary>
        /// The maximum absolute peak value of the item.
        /// </summary>
        public string ReplayGainPeak { get; set; } //ReplayGain_Peak

        /// <summary>
        /// Stream size in bytes.
        /// </summary>
        public ulong? StreamSize { get; set; } //StreamSize

        /// <summary>
        /// Stream size with proportion (1.66 MiB (0%)).
        /// </summary>
        public string StreamSizeFormatted { get; set; } //StreamSize/String

        /// <summary>
        /// Stream size with proportion (1.678 MiB).
        /// </summary>
        public string StreamSizePrecise { get; set; } //StreamSize/String4

        /// <summary>
        /// Stream size divided by file size.
        /// </summary>
        public double? StreamSizeProportion { get; set; } //StreamSize_Proportion

        /// <summary>
        /// Source Streamsize in bytes.
        /// </summary>
        public ulong? SourceStreamSize { get; set; } //Source_StreamSize

        /// <summary>
        /// Source Streamsize in with percentage value (1.66 MiB (0%)).
        /// </summary>
        public string SourceStreamSizeFormatted { get; set; } //Source_StreamSize/String

        /// <summary>
        /// Source streamsize with proportion (1.678 MiB).
        /// </summary>
        public string SourceStreamSizePrecise { get; set; } //StreamSize/String4

        /// <summary>
        /// Source Stream size divided by file size.
        /// </summary>
        public string SourceStreamSizeProportion { get; set; } //Source_StreamSize_Proportion

        /// <summary>
        /// Encoded Streamsize in bytes.
        /// </summary>
        public ulong? StreamSizeEncoded { get; set; } //StreamSize_Encoded

        /// <summary>
        /// Encoded Streamsize in with percentage value (1.66 MiB (0%)).
        /// </summary>
        public string StreamSizeEncodedFormatted { get; set; } //StreamSize_Encoded/String

        /// <summary>
        /// Encoded Streamsize in with percentage value (1.678 MiB).
        /// </summary>
        public string StreamSizeEncodedPrecise { get; set; } //StreamSize_Encoded/String4

        /// <summary>
        /// Encoded Stream size divided by file size.
        /// </summary>
        public double? StreamSizeEncodedProportion { get; set; } //StreamSize_Encoded_Proportion

        /// <summary>
        /// Source Encoded Streamsize in bytes.
        /// </summary>
        public ulong? SourceStreamSizeEncoded { get; set; } //Source_StreamSize_Encoded

        /// <summary>
        /// Source Encoded Streamsize in with percentage value (1.66 MiB (0%)).
        /// </summary>
        public string SourceStreamSizeEncodedFormatted { get; set; } //Source_StreamSize_Encoded/String

        /// <summary>
        /// Source Encoded Streamsize in with percentage value (1.678 MiB).
        /// </summary>
        public string SourceStreamSizeEncodedPrecise { get; set; } //Source_StreamSize_Encoded/String4

        /// <summary>
        /// Source Encoded Stream size divided by file size.
        /// </summary>
        public double? SourceStreamSizeEncodedProportion { get; set; } //Source_StreamSize_Encoded_Proportion

        /// <summary>
        /// How this stream file is aligned in the container.
        /// </summary>
        public string Alignment { get; set; } //Alignment

        /// <summary>
        /// Where this stream file is aligned in the container.
        /// </summary>
        public string AlignmentFormatted { get; set; } //Alignment/String

        /// <summary>
        /// Between how many video frames the stream is inserted.
        /// </summary>
        public int? InterleaveVideoFrames { get; set; } //Interleave_VideoFrames

        /// <summary>
        /// Between how much time (ms) the stream is inserted.
        /// </summary>
        public int? InterleaveDuration { get; set; } //Interleave_Duration

        /// <summary>
        /// Between how much time and video frames the stream is inserted (with measurement).
        /// </summary>
        public string InterleaveDurationFormatted { get; set; } //Interleave_Duration/String

        /// <summary>
        /// How much time is buffered before the first video frame.
        /// </summary>
        public int? InterleavePreload { get; set; } //Interleave_Preload

        /// <summary>
        /// How much time is buffered before the first video frame (with measurement).
        /// </summary>
        public string InterleavePreloadFormatted { get; set; } //Interleave_Preload/String

        /// <summary>
        /// Name of the track.
        /// </summary>
        public string Title { get; set; } //Title

        /// <summary>
        /// Name of the software package used to create the file, such as Microsoft WaveEdit.
        /// </summary>
        public string EncodedApplication { get; set; } //Encoded_Application

        /// <summary>
        /// Name of the software package used to create the file, such as Microsoft WaveEdit, trying to have the format 'CompanyName ProductName (OperatingSystem) Version (Date)'.
        /// </summary>
        public string EncodedApplicationFormatted { get; set; } //Encoded_Application/String

        /// <summary>
        /// Name of the company.
        /// </summary>
        public string EncodedApplicationCompany { get; set; } //Encoded_Application_Company

        /// <summary>
        /// Name of the product.
        /// </summary>
        public string EncodedApplicationName { get; set; } //Encoded_Application_Name

        /// <summary>
        /// Version of the product.
        /// </summary>
        public string EncodedApplicationVersion { get; set; } //Encoded_Application_Version

        /// <summary>
        /// Name of the software package used to create the file, such as Microsoft WaveEdit.
        /// </summary>
        public string EncodedApplicationUrl { get; set; } //Encoded_Application_Url

        /// <summary>
        /// Software used to create the file.
        /// </summary>
        public string EncodedLibrary { get; set; } //Encoded_Library

        /// <summary>
        /// Software used to create the file, trying to have the format 'CompanyName ProductName (OperatingSystem) Version (Date)'.
        /// </summary>
        public string EncodedLibraryFormatted { get; set; } //Encoded_Library/String

        /// <summary>
        /// Name of the company.
        /// </summary>
        public string EncodedLibraryCompanyName { get; set; } //Encoded_Library_CompanyName

        /// <summary>
        /// Name of the the encoding-software.
        /// </summary>
        public string EncodedLibraryName { get; set; } //Encoded_Library_Name

        /// <summary>
        /// Version of encoding-software.
        /// </summary>
        public string EncodedLibraryVersion { get; set; } //Encoded_Library_Version

        /// <summary>
        /// Release date of software.
        /// </summary>
        public string EncodedLibraryDate { get; set; } //Encoded_Library_Date

        /// <summary>
        /// Parameters used by the software.
        /// </summary>
        public string EncodedLibrarySettings { get; set; } //Encoded_Library_Settings

        /// <summary>
        /// Operating System of encoding-software.
        /// </summary>
        public string EncodedOperatingSystem { get; set; } //Encoded_OperatingSystem

        /// <summary>
        /// Language (2-letter ISO 639-1 if exists, else 3-letter ISO 639-2, and with optional ISO 3166-1 country separated by a dash if available, e.g. en, en-us, zh-cn).
        /// </summary>
        public string Language { get; set; } //Language

        /// <summary>
        /// Language (full).
        /// </summary>
        public string LanguageFull { get; set; } //Language/String

        /// <summary>
        /// Language (2-letter ISO 639-1 if exists, else empty)
        /// </summary>
        public string Language2Letter { get; set; } //Language/String2

        /// <summary>
        /// Language(3-letter ISO 639-2 if exists, else empty).
        /// </summary>
        public string Language3Letter { get; set; } //Language/String3

        /// <summary>
        /// More info about Language (Ex. Director's Comment).
        /// </summary>
        public string LanguageMore { get; set; } //Language_More

        /// <summary>
        /// Set if that track should not be used.
        /// </summary>
        public bool? Disabled { get; set; } //Disabled

        /// <summary>
        /// Set if that track should not be used.
        /// </summary>
        public string DisabledString { get; set; } //Disabled/String

        /// <summary>
        /// Set if that track should be used if no language found matches the user preference.
        /// </summary>
        public bool? Default { get; set; } //Default

        /// <summary>
        /// Set if that track should be used if no language found matches the user preference.
        /// </summary>
        public string DefaultString { get; set; } //Default/String

        /// <summary>
        /// Set if that track should be used if no language found matches the user preference.
        /// </summary>
        public bool? Forced { get; set; } //Forced

        /// <summary>
        /// Set if that track should be used if no language found matches the user preference.
        /// </summary>
        public string ForcedString { get; set; } //Forced/String

        /// <summary>
        /// Number of a group in order to provide versions of the same track.
        /// </summary>
        public int? AlternateGroup { get; set; } //AlternateGroup

        /// <summary>
        /// UTC time that the encoding of this item was completed began.
        /// </summary>
        public string EncodedDate { get; set; } //Encoded_Date

        /// <summary>
        /// UTC time that the tags were done for this item.
        /// </summary>
        public string TaggedDate { get; set; } //Tagged_Date

        /// <summary>
        /// Encryption
        /// </summary>
        public string Encryption { get; set; } //Encryption
        #endregion
    }
}
