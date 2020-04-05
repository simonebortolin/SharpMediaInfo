namespace GetMediaInfo
{
    /// <summary>
    /// Contains properties for a VideoTrack
    /// </summary>
    public class VideoTrack
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
        /// Info about Format.
        /// </summary>
        public string FormatInfo { get; set; } //Format/Info

        /// <summary>
        /// Link.
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
        /// Profile of the Format.
        /// </summary>
        public string FormatProfile { get; set; } //Format_Profile

        /// <summary>
        /// Level of the Format (only MIXML).
        /// </summary>
        public string FormatLevel { get; set; } //Format_Level

        /// <summary>
        /// Tier of the Format (only MIXML).
        /// </summary>
        public string FormatTier { get; set; } //Format_Tier

        /// <summary>
        /// Compression mode of the Format.
        /// </summary>
        public string FormatCompression { get; set; } //Format_Compression 
        
        /// <summary>
        /// Format features needed for fully supporting the content.
        /// </summary>
        public string FormatAdditionalFeatures { get; set; } //Format_AdditionalFeatures

        /// <summary>
        /// Multiview, profile of the base stream.
        /// </summary>
        public string MultiViewBaseProfile { get; set; } //MultiView_BaseProfile

        /// <summary>
        /// Multiview, count of views.
        /// </summary>
        public int? MultiViewCount { get; set; } //MultiView_Count

        /// <summary>
        /// Multiview, how views are muxed in the container in case of it is not muxing in the stream.
        /// </summary>
        public string MultiViewLayout { get; set; } //MultiView_Layout

        /// <summary>
        /// Dolby Vision info.
        /// </summary>
        public string DolbyVision { get; set; } //DolbyVision/String

        /// <summary>
        /// Dolby Vision version.
        /// </summary>
        public string DolbyVisionVersion { get; set; } //DolbyVision_Version

        /// <summary>
        /// Dolby Vision profile.
        /// </summary>
        public string DolbyVisionProfile { get; set; } //DolbyVision_Profile 

        /// <summary>
        /// Dolby Vision level.
        /// </summary>
        public string DolbyVisionLevel { get; set; } //DolbyVision_Level         

        /// <summary>
        /// Dolby Vision layers.
        /// </summary>
        public string DolbyVisionLayers { get; set; } //DolbyVision_Layers

        /// <summary>
        /// Settings needed for decoder used, summary.
        /// </summary>
        public string FormatSettings { get; set; } //Format_Settings

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsBVOP { get; set; } //Format_Settings_BVOP

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsQPel { get; set; } //Format_Settings_QPel

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsGMC { get; set; } //Format_Settings_GMC

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsMatrix { get; set; } //Format_Settings_Matrix

        /// <summary>
        /// Matrix, in binary format encoded BASE64. Order = intra, non-intra, gray intra, gray non-intra.
        /// </summary>
        public byte[] FormatSettingsMatrixData { get; set; } //Format_Settings_Matrix_Data

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsCABAC { get; set; } //Format_Settings_CABAC

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsRefFrames { get; set; } //Format_Settings_RefFrames

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsPulldown { get; set; } //Format_Settings_Pulldown

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsEndianness { get; set; } //Format_Settings_Endianness

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsPacking { get; set; } //Format_Settings_Packing

        /// <summary>
        /// Settings needed for decoder used, detailed.
        /// </summary>
        public string FormatSettingsFrameMode { get; set; } //Format_Settings_FrameMode

        /// <summary>
        /// Settings needed for decoder used, detailed (M=x N=y).
        /// </summary>
        public string FormatSettingsGOP { get; set; } //Format_Settings_GOP

        /// <summary>
        /// Settings needed for decoder used, detailed (Type of frame, and field/frame info).
        /// </summary>
        public string FormatSettingsPictureString { get; set; } //Format_Settings_PictureString

        /// <summary>
        /// Wrapping mode (Frame wrapped or Clip wrapped).
        /// </summary>
        public string FormatSettingsWrapping { get; set; } //Format_Settings_Wrapping

        /// <summary>
        /// Internet Media Type (aka MIME Type, Content-Type).
        /// </summary>
        public string InternetMediaType { get; set; } //InternetMediaType

        /// <summary>
        /// How this file is muxed in the container.
        /// </summary>
        public string MuxingMode { get; set; } //MuxingMode

        /// <summary>
        /// Codec ID (found in some containers).
        /// </summary>
        public int? CodecID { get; set; } //CodecID

        /// <summary>
        /// Info on the codec.
        /// </summary>
        public string CodecIDInfo { get; set; } //CodecID/Info

        /// <summary>
        /// Hint/popular name for this codec.
        /// </summary>
        public string CodecIDHint { get; set; } //CodecID/Hint

        /// <summary>
        /// Homepage for more details about this codec.
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
        /// Width (aperture size if present) in pixel.
        /// </summary>
        public int? Width { get; set; } //Width

        /// <summary>
        /// Width (aperture size if present) with measurement (pixel).
        /// </summary>
        public string WidthFormatted { get; set; } //Width/String

        /// <summary>
        /// Offset between original width and displayed width in pixel.
        /// </summary>
        public int? WidthOffset { get; set; } //Width_Offset

        /// <summary>
        /// Offset between original width and displayed width in pixel.
        /// </summary>
        public string WidthOffsetFormatted { get; set; } //Width_Offset/String

        /// <summary>
        /// Original (in the raw stream) width in pixel.
        /// </summary>
        public int? WidthOriginal { get; set; } //Width_Original

        /// <summary>
        /// Original (in the raw stream) width with measurement (pixel).
        /// </summary>
        public string WidthOriginalFormatted { get; set; } //Width_Original/String

        /// <summary>
        /// Clean Aperture width in pixel.
        /// </summary>
        public int? WidthCleanAperture { get; set; } //Width_CleanAperture

        /// <summary>
        /// Clean Aperture width with measurement (pixel).
        /// </summary>
        public string WidthCleanApertureFormatted { get; set; } //Width_CleanAperture/String

        /// <summary>
        /// Height in pixel.
        /// </summary>
        public int? Height { get; set; } //Height

        /// <summary>
        /// Height with measurement (pixel).
        /// </summary>
        public string HeightFormatted { get; set; } //Height/String

        /// <summary>
        /// Offset between original height and displayed height in pixel.
        /// </summary>
        public int? HeightOffset { get; set; } //Height_Offset

        /// <summary>
        /// Offset between original height and displayed height  in pixel.
        /// </summary>
        public string HeightOffsetFormatted { get; set; } //Height_Offset/String

        /// <summary>
        /// Original (in the raw stream) height in pixel.
        /// </summary>
        public int? HeightOriginal { get; set; } //Height_Original

        /// <summary>
        /// Original (in the raw stream) height with measurement (pixel).
        /// </summary>
        public string HeightOriginalFormatted { get; set; } //Height_Original/String

        /// <summary>
        /// Clean Aperture height in pixel.
        /// </summary>
        public int? HeightCleanAperture { get; set; } //Height_CleanAperture

        /// <summary>
        /// Clean Aperture height with measurement (pixel).
        /// </summary>
        public string HeightCleanApertureFormatted { get; set; } //Height_CleanAperture/String

        /// <summary>
        /// Stored width.
        /// </summary>
        public int? StoredWidth { get; set; } //Stored_Width

        /// <summary>
        /// Stored height.
        /// </summary>
        public int? StoredHeight { get; set; } //Stored_Height

        /// <summary>
        /// Sampled width.
        /// </summary>
        public int? SampledWidth { get; set; } //Sampled_Width

        /// <summary>
        /// Sampled height.
        /// </summary>
        public int? SampledHeight { get; set; } //Sampled_Height

        /// <summary>
        /// Pixel Aspect ratio.
        /// </summary>
        public double? PixelAspectRatio { get; set; } //PixelAspectRatio

        /// <summary>
        /// Pixel Aspect ratio.
        /// </summary>
        public string PixelAspectRatioFormatted { get; set; } //PixelAspectRatio/String

        /// <summary>
        /// Original (in the raw stream) Pixel Aspect ratio.
        /// </summary>
        public double? PixelAspectRatioOriginal { get; set; } //PixelAspectRatio_Original

        /// <summary>
        /// Original (in the raw stream) Pixel Aspect ratio.
        /// </summary>
        public string PixelAspectRatioOriginalFormatted { get; set; } //PixelAspectRatio_Original/String

        /// <summary>
        /// Clean Aperture Pixel Aspect ratio.
        /// </summary>
        public double? PixelAspectRatioCleanAperture { get; set; } //PixelAspectRatio_CleanAperture

        /// <summary>
        /// Clean Aperture Pixel Aspect ratio.
        /// </summary>
        public string PixelAspectRatioCleanApertureFormatted { get; set; } //PixelAspectRatio_CleanAperture/String

        /// <summary>
        /// Display Aspect ratio.
        /// </summary>
        public double? DisplayAspectRatio { get; set; } //DisplayAspectRatio

        /// <summary>
        /// Display Aspect ratio.
        /// </summary>
        public string DisplayAspectRatioFormatted { get; set; } //DisplayAspectRatio/String

        /// <summary>
        /// Original (in the raw stream) Display Aspect ratio.
        /// </summary>
        public double? DisplayAspectRatioOriginal { get; set; } //DisplayAspectRatio_Original

        /// <summary>
        /// Original (in the raw stream) Display Aspect ratio.
        /// </summary>
        public string DisplayAspectRatioOriginalFormatted { get; set; } //DisplayAspectRatio_Original/String

        /// <summary>
        /// Clean Aperture Display Aspect ratio.
        /// </summary>
        public double? DisplayAspectRatioCleanAperture { get; set; } //DisplayAspectRatio_CleanAperture

        /// <summary>
        /// Clean Aperture Display Aspect ratio.
        /// </summary>
        public string DisplayAspectRatioCleanApertureFormatted { get; set; } //DisplayAspectRatio_CleanAperture/String

        /// <summary>
        /// Active Format Description (AFD value).
        /// </summary>
        public string ActiveFormatDescription { get; set; } //ActiveFormatDescription

        /// <summary>
        /// Active Format Description (AFD value) muxing mode (Ancillary or Raw stream).
        /// </summary>
        public string ActiveFormatDescriptionMuxingMode { get; set; } //ActiveFormatDescription_MuxingMode

        /// <summary>
        /// Rotation.
        /// </summary>
        public int? Rotation { get; set; } //Rotation //TOCHECK

        /// <summary>
        /// Rotation (if not horizontal).
        /// </summary>
        public string RotationFormatted { get; set; } //Rotation/String //TOCHECK

        /// <summary>
        /// Frame rate mode (CFR, VFR).
        /// </summary>
        public string FrameRateMode { get; set; } //FrameRate_Mode

        /// <summary>
        /// Original frame rate mode (CFR, VFR).
        /// </summary>
        public string FrameRateModeOriginal { get; set; } //FrameRate_Mode_Original

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
        /// Minimum Frames per second.
        /// </summary>
        public double? FrameRateMinimum { get; set; } //FrameRate_Minimum

        /// <summary>
        /// Minimum Frames per second (with measurement).
        /// </summary>
        public string FrameRateMinimumFormatted { get; set; } //FrameRate_Minimum/String

        /// <summary>
        /// Nominal Frames per second.
        /// </summary>
        public double? FrameRateNominal { get; set; } //FrameRate_Nominal

        /// <summary>
        /// Nominal Frames per second (with measurement).
        /// </summary>
        public string FrameRateNominalFormatted { get; set; } //FrameRate_Nominal/String

        /// <summary>
        /// Maximum Frames per second.
        /// </summary>
        public double? FrameRateMaximum { get; set; } //FrameRate_Maximum

        /// <summary>
        /// Maximum Frames per second (with measurement).
        /// </summary>
        public string FrameRateMaximumFormatted { get; set; } //FrameRate_Maximum/String

        /// <summary>
        /// Original (in the raw stream) frames per second.
        /// </summary>
        public double? FrameRateOriginal { get; set; } //FrameRate_Original

        /// <summary>
        /// Original (in the raw stream) frames per second (with measurement).
        /// </summary>
        public string FrameRateOriginalFormatted { get; set; } //FrameRate_Original/String

        /// <summary>
        /// Frames per second, numerator.
        /// </summary>
        public double? FrameRateOriginalNum { get; set; } //FrameRate_Original_Num

        /// <summary>
        /// Frames per second, denominator.
        /// </summary>
        public double? FrameRateOriginalDen { get; set; } //FrameRate_Original_Den

        /// <summary>
        /// Number of frames.
        /// </summary>
        public int? FrameCount { get; set; } //FrameCount

        /// <summary>
        /// Source Number of frames.
        /// </summary>
        public int? SourceFrameCount { get; set; } //Source_FrameCount

        /// <summary>
        /// NTSC or PAL.
        /// </summary>
        public string Standard { get; set; } //Standard

        /// <summary>
        /// ColorSpace
        /// </summary>
        public string ColorSpace { get; set; } //ColorSpace

        /// <summary>
        /// ChromaSubsampling
        /// </summary>
        public string ChromaSubsampling { get; set; } //ChromaSubsampling

        /// <summary>
        /// ChromaSubsampling_Position
        /// </summary>
        public int? ChromaSubsamplingPosition { get; set; } //ChromaSubsampling_Position //TOCHECK

        /// <summary>
        /// 16/24/32.
        /// </summary>
        public int? BitDepth { get; set; } //BitDepth

        /// <summary>
        /// 16/24/32 bits.
        /// </summary>
        public string BitDepthFormatted { get; set; } //BitDepth/String

        /// <summary>
        /// ScanType
        /// </summary>
        public string ScanType { get; set; } //ScanType

        /// <summary>
        /// ScanType_Original
        /// </summary>
        public string ScanTypeOriginal { get; set; } //ScanType_Original

        /// <summary>
        /// ScanType_StoreMethod
        /// </summary>
        public string ScanTypeStoreMethod { get; set; } //ScanType_StoreMethod

        /// <summary>
        /// ScanType_StoreMethod_FieldsPerBlock
        /// </summary>
        public string ScanTypeStoreMethodFieldsPerBlock { get; set; } //ScanType_StoreMethod_FieldsPerBlock

        /// <summary>
        /// ScanOrder
        /// </summary>
        public string ScanOrder { get; set; } //ScanOrder

        /// <summary>
        /// ScanOrder_Stored
        /// </summary>
        public string ScanOrderStored { get; set; } //ScanOrder_Stored

        /// <summary>
        /// ScanOrder_StoredDisplayedInverted
        /// </summary>
        public string ScanOrderStoredDisplayedInverted { get; set; } //ScanOrder_StoredDisplayedInverted

        /// <summary>
        /// ScanOrder_Original
        /// </summary>
        public string ScanOrderOriginal { get; set; } //ScanOrder_Original

        /// <summary>
        /// Compression mode (Lossy or Lossless).
        /// </summary>
        public string CompressionMode { get; set; } //Compression_Mode

        /// <summary>
        /// Current stream size divided by uncompressed stream size.
        /// </summary>
        public double? CompressionRatio { get; set; } //Compression_Ratio

        /// <summary>
        /// bits/(Pixel*Frame) (like Gordian Knot).
        /// </summary>
        public double? BitsPixelFrame { get; set; } //Bits-(Pixel*Frame)

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
        /// Delay in format : HH:MM:SS.MMM.
        /// </summary>
        public string DelayTimecode { get; set; } //Delay/String3

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
        /// TimeStamp fixed in the stream (relative) IN MS.
        /// </summary>
        public int? TimeStampFirstFrame { get; set; } //TimeStamp_FirstFrame

        /// <summary>
        /// TimeStamp with measurement.
        /// </summary>
        public string TimeStampFirstFrameFormatted { get; set; } //TimeStamp_FirstFrame/String

        /// <summary>
        /// TimeStamp with measurement.
        /// </summary>
        public string TimeStampFirstFrameFormattedComplete { get; set; } //TimeStamp_FirstFrame/String1

        /// <summary>
        /// TimeStamp in format : HH:MM:SS.mmm (HH:MM:SS:FF).
        /// </summary>
        public string TimeStampFirstFrameTimecode { get; set; } //TimeStamp_FirstFrame/String5

        /// <summary>
        /// Time code in HH:MM:SS:FF, last colon replaced by semicolon for drop frame if available format.
        /// </summary>
        public string TimeCodeFirstFrame { get; set; } //TimeCode_FirstFrame

        /// <summary>
        /// Time code settings.
        /// </summary>
        public string TimeCodeSettings { get; set; } //TimeCode_Settings

        /// <summary>
        /// Time code source (Container, Stream, SystemScheme1, SDTI, ANC...).
        /// </summary>
        public string TimeCodeSource { get; set; } //TimeCode_Source

        /// <summary>
        /// Time code information about Open/Closed.
        /// </summary>
        public string GopOpenClosed { get; set; } //Gop_OpenClosed

        /// <summary>
        /// Time code information about Open/Closed of first frame if GOP is Open for the other GOPs.
        /// </summary>
        public string GopOpenClosedFirstFrame { get; set; } //Gop_OpenClosed_FirstFrame

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
        /// Stream size with proportion (1.678 MiB).
        /// </summary>
        public string SourceStreamSizePrecise { get; set; } //Source_StreamSize/String4

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
        /// Stream size with proportion (1.678 MiB).
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
        /// Stream size with proportion (1.678 MiB).
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
        public string EncodedApplicationCompa { get; set; } //Encoded_Application_Company

        /// <summary>
        /// Name of the product.
        /// </summary>
        public string EncodedApplicationName { get; set; } //Encoded_Application_Name

        /// <summary>
        /// Version of the product.
        /// </summary>
        public string EncodedApplicationVersi { get; set; } //Encoded_Application_Version

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
        /// Service kind, e.g. visually impaired, commentary, voice over.
        /// </summary>
        public string ServiceKind { get; set; } //ServiceKind

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

        /// <summary>
        /// Defines the size of the buffer needed to decode the sequence.
        /// </summary>
        public string BufferSize { get; set; } //BufferSize

        /// <summary>
        /// Colour range for YUV colour space.
        /// </summary>
        public string ColourRange { get; set; } //colour_range

        /// <summary>
        /// Presence of colour description.
        /// </summary>
        public string ColourDescriptionPresent { get; set; } //colour_description_present

        /// <summary>
        /// Chromaticity coordinates of the source primaries.
        /// </summary>
        public string ColourPrimaries { get; set; } //colour_primaries

        /// <summary>
        /// Opto-electronic transfer characteristic of the source picture.
        /// </summary>
        public string TransferCharacteristics { get; set; } //transfer_characteristics

        /// <summary>
        /// Matrix coefficients used in deriving luma and chroma signals from the green, blue, and red primaries.
        /// </summary>
        public string MatrixCoefficients { get; set; } //matrix_coefficients

        /// <summary>
        /// Presence of colour description.
        /// </summary>
        public string ColourDescriptionPresentOriginal { get; set; } //colour_description_present_Original

        /// <summary>
        /// Chromaticity coordinates of the source primaries.
        /// </summary>
        public string ColourPrimariesOriginal { get; set; } //colour_primaries_Original

        /// <summary>
        /// Opto-electronic transfer characteristic of the source picture.
        /// </summary>
        public string TransferCharacteristicsOriginal { get; set; } //transfer_characteristics_Original

        /// <summary>
        /// Matrix coefficients used in deriving luma and chroma signals from the green, blue, and red primaries.
        /// </summary>
        public string MatrixCoefficientsOriginal { get; set; } //matrix_coefficients_Original

        /// <summary>
        /// Mastering display color primaries.
        /// </summary>
        public string MasteringDisplayColorPrimaries { get; set; } //MasteringDisplay_ColorPrimaries

        /// <summary>
        /// Mastering display luminance.
        /// </summary>
        public string MasteringDisplayLuminance { get; set; } //MasteringDisplay_Luminance

        /// <summary>
        /// Maximum Content Light Level, the brightest pixel in the entire stream (in nits).
        /// </summary>
        public string MaximumContentLightLevel { get; set; } //MaxCLL

        /// <summary>
        /// Maximum Frame Average Light Level, the highest frame average brightness per frame in the entire stream (in nits).
        /// </summary>
        public string MaximumFrameAverageLightLevel { get; set; } //MaxFALL
        #endregion
    }
}
