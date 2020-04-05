namespace GetMediaInfoDDLV2
{
    /// <summary>
    /// Contains properties for a ImageTrack
    /// </summary>
    public class ImageTrack
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
        ///<summary>  Name of the track </summary>
        public string Title { get; set; }
        ///<summary>  Format used </summary>
        public string Format { get; set; }
        ///<summary>  Info about Format </summary>
        public string FormatInfo { get; set; }
        ///<summary>  Link </summary>
        public string FormatUrl { get; set; }
        ///<summary>  Commercial name used by vendor for theses setings or Format field if there is no difference </summary>
        public string FormatCommercial { get; set; }
        ///<summary>  Commercial name used by vendor for theses setings if there is one </summary>
        public string FormatCommercialIfAny { get; set; }
        ///<summary>  Version of this format </summary>
        public string FormatVersion { get; set; }
        ///<summary>  Profile of the Format </summary>
        public string FormatProfile { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsEndianness { get; set; }
        ///<summary>   </summary>
        public string FormatSettingsPacking { get; set; }
        ///<summary>  Compression method used </summary>
        public string FormatCompression { get; set; }
        ///<summary>  Settings needed for decoder used </summary>
        public string FormatSettings { get; set; }
        ///<summary>  Wrapping mode (Frame wrapped or Clip wrapped) </summary>
        public string FormatSettingsWrapping { get; set; }
        ///<summary>  Internet Media Type (aka MIME Type, Content-Type) </summary>
        public string InternetMediaType { get; set; }
        ///<summary>  Codec ID (found in some containers) </summary>
        public string CodecID { get; set; }
        ///<summary>  Codec ID (found in some containers) </summary>
        public string CodecIDString { get; set; }
        ///<summary>  Info about codec ID </summary>
        public string CodecIDInfo { get; set; }
        ///<summary>  A hint for this codec ID </summary>
        public string CodecIDHint { get; set; }
        ///<summary>  A link for more details about this codec ID </summary>
        public string CodecIDUrl { get; set; }
        ///<summary>  Manual description given by the container </summary>
        public string CodecIDDescription { get; set; }
        ///<summary>  Width (aperture size if present) in pixel </summary>
        public string Width { get; set; }
        ///<summary>  Width (aperture size if present) with measurement (pixel) </summary>
        public string WidthString { get; set; }
        ///<summary>  Offset between original width and displayed width (aperture size) in pixel </summary>
        public string WidthOffset { get; set; }
        ///<summary>  Offset between original width and displayed width (aperture size)  in pixel </summary>
        public string WidthOffsetString { get; set; }
        ///<summary>  Original (in the raw stream) width in pixel </summary>
        public string WidthOriginal { get; set; }
        ///<summary>  Original (in the raw stream) width with measurement (pixel) </summary>
        public string WidthOriginalString { get; set; }
        ///<summary>  Height (aperture size if present) in pixel </summary>
        public string Height { get; set; }
        ///<summary>  Height (aperture size if present) with measurement (pixel) </summary>
        public string HeightString { get; set; }
        ///<summary>  Offset between original height and displayed height (aperture size) in pixel </summary>
        public string HeightOffset { get; set; }
        ///<summary>  Offset between original height and displayed height (aperture size)  in pixel </summary>
        public string HeightOffsetString { get; set; }
        ///<summary>  Original (in the raw stream) height in pixel </summary>
        public string HeightOriginal { get; set; }
        ///<summary>  Original (in the raw stream) height with measurement (pixel) </summary>
        public string HeightOriginalString { get; set; }
        ///<summary>  Pixel Aspect ratio </summary>
        public double? PixelAspectRatio { get; set; }
        ///<summary>  Pixel Aspect ratio </summary>
        public string PixelAspectRatioString { get; set; }
        ///<summary>  Original (in the raw stream) Pixel Aspect ratio </summary>
        public double? PixelAspectRatioOriginal { get; set; }
        ///<summary>  Display Aspect ratio </summary>
        public double? DisplayAspectRatio { get; set; }
        ///<summary>  Display Aspect ratio </summary>
        public string DisplayAspectRatioString { get; set; }
        ///<summary>  Original (in the raw stream) Display Aspect ratio </summary>
        public double? DisplayAspectRatioOriginal { get; set; }
        ///<summary>   </summary>
        public string ColorSpace { get; set; }
        ///<summary>   </summary>
        public string ChromaSubsampling { get; set; }
        ///<summary>   </summary>
        public string BitDepth { get; set; }
        ///<summary>   </summary>
        public string BitDepthString { get; set; }
        ///<summary>  Compression mode (Lossy or Lossless) </summary>
        public string CompressionMode { get; set; }
        ///<summary>  Compression mode (Lossy or Lossless) </summary>
        public string CompressionModeString { get; set; }
        ///<summary>  Current stream size divided by uncompressed stream size </summary>
        public string CompressionRatio { get; set; }
        ///<summary>  Stream size in bytes </summary>
        public ulong? StreamSize { get; set; }
        ///<summary>   </summary>
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
        public ulong? StreamSizeProportion { get; set; }
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
        ///<summary>  Software used to create the file </summary>
        public string EncodedLibrary { get; set; }
        ///<summary>  Software used to create the file </summary>
        public string EncodedLibraryString { get; set; }
        ///<summary>  Info from the software </summary>
        public string EncodedLibraryName { get; set; }
        ///<summary>  Version of software </summary>
        public string EncodedLibraryVersion { get; set; }
        ///<summary>  Release date of software </summary>
        public string EncodedLibraryDate { get; set; }
        ///<summary>  Parameters used by the software </summary>
        public string EncodedLibrarySettings { get; set; }
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
        ///<summary>   </summary>
        public string Summary { get; set; }
        ///<summary>  The time that the encoding of this item was completed began. </summary>
        public string EncodedDate { get; set; }
        ///<summary>  The time that the tags were done for this item. </summary>
        public string TaggedDate { get; set; }
        ///<summary>   </summary>
        public string Encryption { get; set; }
        ///<summary>  Chromaticity coordinates of the source primaries </summary>
        public string Colourprimaries { get; set; }
        ///<summary>  Opto-electronic transfer characteristic of the source picture </summary>
        public string Transfercharacteristics { get; set; }
        ///<summary>  Matrix coefficients used in deriving luma and chroma signals from the green, blue, and red primaries </summary>
        public string Matrixcoefficients { get; set; }


    }
}
