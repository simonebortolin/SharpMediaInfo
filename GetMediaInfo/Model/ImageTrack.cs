namespace GetMediaInfo.Model
{
    /// <summary>
    /// Image Stream Class
    /// </summary>
    public class ImageTrack
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
        /// Number of the stream(base=0).
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
        /// Name of the track.
        /// </summary>
        public string Title { get; set; } //Title

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
        /// Compression mode of the Format.
        /// </summary>
        public string FormatCompression { get; set; } //Format_Compression

        /// <summary>
        /// Settings needed for decoder used.
        /// </summary>
        public string FormatSettings { get; set; } //Format_Settings

        /// <summary>
        /// Format features needed for fully supporting the content.
        /// </summary>
        public string FormatAdditionalFeatures { get; set; } //Format_AdditionalFeatures

        /// <summary>
        /// Wrapping mode (Frame wrapped or Clip wrapped).
        /// </summary>
        public string FormatSettingsWrapping { get; set; } //Format_Settings_Wrapping

        /// <summary>
        /// Internet Media Type(aka MIME Type, Content-Type).
        /// </summary>
        public string InternetMediaType { get; set; } //InternetMediaType

        /// <summary>
        /// Codec ID(found in some containers).
        /// </summary>
        public int? CodecID { get; set; } //CodecID

        /// <summary>
        /// Info about codec ID.
        /// </summary>
        public string CodecIDInfo { get; set; } //CodecID/Info

        /// <summary>
        /// A hint? for this codec ID.
        /// </summary>
        public string CodecIDHint { get; set; } //CodecID/Hint

        /// <summary>
        /// A link for more details about this codec ID.
        /// </summary>
        public string CodecIDUrl { get; set; } //CodecID/Url

        /// <summary>
        /// Manual description given by the container.
        /// </summary>
        public string CodecIDDescription { get; set; } //CodecID_Description

        /// <summary>
        /// Width (aperture size if present) in pixel.
        /// </summary>
        public int? Width { get; set; } //Width

        /// <summary>
        /// Width(aperture size if present) with measurement (pixel).
        /// </summary>
        public string WidthFormatted { get; set; } //Width/String

        /// <summary>
        /// Offset between original width and displayed width (aperture size) in pixel.
        /// </summary>
        public int? WidthOffset { get; set; } //Width_Offset

        /// <summary>
        /// Offset between original width and displayed width(aperture size) with measurement (pixel).
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
        /// Height (aperture size if present) in pixel.
        /// </summary>
        public int? Height { get; set; } //Height

        /// <summary>
        /// Height (aperture size if present) with measurement (pixel).
        /// </summary>
        public string HeightFormatted { get; set; } //Height/String

        /// <summary>
        /// Offset between original height and displayed height (aperture size) in pixel.
        /// </summary>
        public int? HeightOffset { get; set; } //Height_Offset

        /// <summary>
        /// Offset between original height and displayed height(aperture size) with measurement (pixel).
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
        /// Pixel Aspect ratio.
        /// </summary>
        public double? PixelAspectRatio { get; set; } //PixelAspectRatio

        /// <summary>
        /// Pixel Aspect ratio string.
        /// </summary>
        public string PixelAspectRatioFormatted { get; set; } //PixelAspectRatio/String

        /// <summary>
        /// Original (in the raw stream) Pixel Aspect ratio.
        /// </summary>
        public double? PixelAspectRatioOriginal { get; set; } //PixelAspectRatio_Original

        /// <summary>
        /// Original (in the raw stream) Pixel Aspect ratio string.
        /// </summary>
        public string PixelAspectRatioOriginalFormatted { get; set; } //PixelAspectRatio_Original/String

        /// <summary>
        /// Display Aspect ratio.
        /// </summary>
        public double? DisplayAspectRatio { get; set; } //DisplayAspectRatio

        /// <summary>
        /// Display Aspect ratio string.
        /// </summary>
        public string DisplayAspectRatioFormatted { get; set; } //DisplayAspectRatio/String

        /// <summary>
        /// Original (in the raw stream) Display Aspect ratio.
        /// </summary>
        public double? DisplayAspectRatioOriginal { get; set; } //DisplayAspectRatio_Original

        /// <summary>
        /// Original (in the raw stream) Display Aspect ratio string.
        /// </summary>
        public string DisplayAspectRatioOriginalFormatted { get; set; } //DisplayAspectRatio_Original/String

        /// <summary>
        /// Color Space.
        /// </summary>
        public string ColorSpace { get; set; } //ColorSpace

        /// <summary>
        /// Chroma Subsampling.
        /// </summary>
        public string ChromaSubsampling { get; set; } //ChromaSubsampling

        /// <summary>
        /// Bit Depth.
        /// </summary>
        public int? BitDepth { get; set; } //BitDepth

        /// <summary>
        /// Bit Depth string.
        /// </summary>
        public string BitDepthFormatted { get; set; } //BitDepth/String

        /// <summary>
        /// Compression mode (Lossy or Lossless).
        /// </summary>
        public string CompressionMode { get; set; } //Compression_Mode

        /// <summary>
        /// Current stream size divided by uncompressed stream size.
        /// </summary>
        public double? CompressionRatio { get; set; } //Compression_Ratio

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
        /// Software used to create the file.
        /// </summary>
        public string EncodedLibrary { get; set; } //Encoded_Library

        /// <summary>
        /// Software used to create the file (raw value).
        /// </summary>
        public string EncodedLibraryFormatted { get; set; } //Encoded_Library/String

        /// <summary>
        /// Info from the software.
        /// </summary>
        public string EncodedLibraryName { get; set; } //Encoded_Library_Name

        /// <summary>
        /// Version of software.
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
        /// Language (2-letter ISO 639-1 if exists, else 3-letter ISO 639-2, and with optional ISO 3166-1 country separated by a dash if available, e.g.en, en-us, zh-cn).
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
        /// Set if that track should not be used (raw value).
        /// </summary>
        public string DisabledString { get; set; } //Disabled/String

        /// <summary>
        /// Set if that track should be used if no language found matches the user preference.
        /// </summary>
        public bool? Default { get; set; } //Default

        /// <summary>
        /// Set if that track should be used if no language found matches the user preference (raw value).
        /// </summary>
        public string DefaultString { get; set; } //Default/String

        /// <summary>
        /// Set if that track should be used if no language found matches the user preference.
        /// </summary>
        public bool? Forced { get; set; } //Forced

        /// <summary>
        /// Set if that track should be used if no language found matches the user preference (raw value).
        /// </summary>
        public string ForcedString { get; set; } //Forced/String

        /// <summary>
        /// Number of a group in order to provide versions of the same track.
        /// </summary>
        public int? AlternateGroup { get; set; } //AlternateGroup

        /// <summary>
        /// Summary.
        /// </summary>
        public string Summary { get; set; } //Summary

        /// <summary>
        /// The time that the encoding of this item was completed began.
        /// </summary>
        public string EncodedDate { get; set; } //Encoded_Date

        /// <summary>
        /// The time that the tags were done for this item.
        /// </summary>
        public string TaggedDate { get; set; } //Tagged_Date

        /// <summary>
        /// The Encryption.
        /// </summary>
        public string Encryption { get; set; } //Encryption

        /// <summary>
        /// Presence of colour description.
        /// </summary>
        public bool? ColourDescriptionPresent { get; set; } //colour_description_present

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
        /// Presence of colour description in the original stream.
        /// </summary>
        public bool? ColourDescriptionPresentOriginal { get; set; } //colour_description_present_Original

        /// <summary>
        /// Chromaticity coordinates of the source primaries in the original stream.
        /// </summary>
        public string ColourPrimariesOriginal { get; set; } //colour_primaries_Original

        /// <summary>
        /// Opto-electronic transfer characteristic of the source picture in the original stream.
        /// </summary>
        public string TransferCharacteristicsOriginal { get; set; } //transfer_characteristics_Original

        /// <summary>
        /// Matrix coefficients used in deriving luma and chroma signals from the green, blue, and red primaries in the original stream.
        /// </summary>
        public string MatrixCoefficientsOriginal { get; set; } //matrix_coefficients_Original

        #endregion
    }
}
