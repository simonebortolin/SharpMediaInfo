using System;
using System.Collections.Generic;
using System.Linq;

namespace GetMediaInfoDDLV2
{
    /// <summary>
    /// Contains properties for a GeneralTrack
    /// </summary>
    public class GeneralTrack
    {
        private List<byte[]> _coverData;
        internal bool CanGetCover = false;

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
        ///<summary>  Number of general streams </summary>
        public int? GeneralCount { get; set; }
        ///<summary>  Number of video streams </summary>
        public int? VideoCount { get; set; }
        ///<summary>  Number of audio streams </summary>
        public int? AudioCount { get; set; }
        ///<summary>  Number of text streams </summary>
        public int? TextCount { get; set; }
        ///<summary>  Number of other streams </summary>
        public int? OtherCount { get; set; }
        ///<summary>  Number of image streams </summary>
        public int? ImageCount { get; set; }
        ///<summary>  Number of menu streams </summary>
        public int? MenuCount { get; set; }
        ///<summary>  Video Codecs in this file, separated by / </summary>
        public List<string> VideoFormatList { get; set; }
        ///<summary>  Video Codecs in this file with popular name (hint), separated by / </summary>
        public List<string> VideoFormatWithHintList { get; set; }
        ///<summary>  Video languagesin this file, full names, separated by / </summary>
        public List<string> VideoLanguageList { get; set; }
        ///<summary>  Audio Codecs in this file,separated by / </summary>
        public List<string> AudioFormatList { get; set; }
        ///<summary>  Audio Codecs in this file with popular name (hint), separated by / </summary>
        public List<string> AudioFormatWithHintList { get; set; }
        ///<summary>  Audio languages in this file separated by / </summary>
        public List<string> AudioLanguageList { get; set; }
        ///<summary>  Text Codecs in this file, separated by / </summary>
        public List<string> TextFormatList { get; set; }
        ///<summary>  Text Codecs in this file with popular name (hint),separated by / </summary>
        public List<string> TextFormatWithHintList { get; set; }
        ///<summary>  Text languages in this file, separated by / </summary>
        public List<string> TextLanguageList { get; set; }
        ///<summary>  Other formats in this file, separated by / </summary>
        public List<string> OtherFormatList { get; set; }
        ///<summary>  Other formats in this file with popular name (hint), separated by / </summary>
        public List<string> OtherFormatWithHintLis { get; set; }
        ///<summary>  Chapters languages in this file, separated by / </summary>
        public List<string> OtherLanguageList { get; set; }
        ///<summary>  Image Codecs in this file, separated by / </summary>
        public List<string> ImageFormatList { get; set; }
        ///<summary>  Image Codecs in this file with popular name (hint), separated by / </summary>
        public List<string> ImageFormatWithHintLis { get; set; }
        ///<summary>  Image languages in this file, separated by / </summary>
        public List<string> ImageLanguageList { get; set; }
        ///<summary>  Menu Codecsin this file, separated by / </summary>
        public List<string> MenuFormatList { get; set; }
        ///<summary>  Menu Codecs in this file with popular name (hint),separated by / </summary>
        public List<string> MenuFormatWithHintList { get; set; }
        ///<summary>  Menu languages in this file, separated by / </summary>
        public List<string> MenuLanguageList { get; set; }
        ///<summary>  Complete name (Folder+Name+Extension) </summary>
        public string CompleteName { get; set; }
        ///<summary>  Folder name only </summary>
        public string FolderName { get; set; }
        ///<summary>  File name and extension </summary>
        public string FileNameExtension { get; set; }
        ///<summary>  File name only </summary>
        public string FileName { get; set; }
        ///<summary>  File extension only </summary>
        public string FileExtension { get; set; }
        ///<summary>  Complete name (Folder+Name+Extension) of the last file (in the case of a sequence of files) </summary>
        public string CompleteNameLast { get; set; }
        ///<summary>  Folder name only of the last file (in the case of a sequence of files) </summary>
        public string FolderNameLast { get; set; }
        ///<summary>  File name and extension of the last file (in the case of a sequence of files) </summary>
        public string FileNameExtensionLast { get; set; }
        ///<summary>  File name only of the last file (in the case of a sequence of files) </summary>
        public string FileNameLast { get; set; }
        ///<summary>  File extension only of the last file (in the case of a sequence of files) </summary>
        public string FileExtensionLast { get; set; }
        ///<summary>  Format used </summary>
        public string Format { get; set; }
        ///<summary>  Info about this Format </summary>
        public string FormatInfo { get; set; }
        ///<summary>  Link to a description of this format </summary>
        public string FormatUrl { get; set; }
        ///<summary>  Known extensions of this format </summary>
        public string FormatExtensions { get; set; }
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
        ///<summary>  Settings needed for decoder used </summary>
        public string FormatSettings { get; set; }
        ///<summary>  Internet Media Type (aka MIME Type, Content-Type) </summary>
        public string InternetMediaType { get; set; }
        ///<summary>  Codec ID (found in some containers) </summary>
        public string CodecID { get; set; }
        ///<summary>  Codec ID (found in some containers) </summary>
        public string CodecIDString { get; set; }
        ///<summary>  Info about this codec </summary>
        public string CodecIDInfo { get; set; }
        ///<summary>  A hint/popular name for this codec </summary>
        public string CodecIDHint { get; set; }
        ///<summary>  A link to more details about this codec ID </summary>
        public string CodecIDUrl { get; set; }
        ///<summary>  Manual description given by the container </summary>
        public string CodecIDDescription { get; set; }
        ///<summary>  Version of the CodecID </summary>
        public string CodecIDVersion { get; set; }
        ///<summary>  Compatible CodecIDs </summary>
        public string CodecIDCompatible { get; set; }
        ///<summary>  If Audio and video are muxed </summary>
        public string Interleaved { get; set; }
        ///<summary>  File size in bytes </summary>
        public ulong? FileSize { get; set; }
        ///<summary>  File size (with measure) </summary>
        public string FileSizeString { get; set; }
        ///<summary>  File size (with measure, 1 digit mini) </summary>
        internal string FileSizeString1 { get; set; }
        ///<summary>  File size (with measure, 2 digit mini) </summary>
        internal string FileSizeString2 { get; set; }
        ///<summary>  File size (with measure, 3 digit mini) </summary>
        internal string FileSizeString3 { get; set; }
        ///<summary>  File size (with measure, 4 digit mini) </summary>
        public string FileSizeFormatted { get; set; }
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
        ///<summary>   </summary>
        public string DurationStart { get; set; }
        ///<summary>   </summary>
        public string DurationEnd { get; set; }
        ///<summary>  Bit rate mode of all streams (VBR, CBR) </summary>
        public string OverallBitRateMode { get; set; }
        ///<summary>  Bit rate mode of all streams (Variable, Constant) </summary>
        public string OverallBitRateModeString { get; set; }
        ///<summary>  Bit rate of all streams in bps </summary>
        public double? OverallBitRate { get; set; }
        ///<summary>  Bit rate of all streams (with measure) </summary>
        public string OverallBitRateString { get; set; }
        ///<summary>  Minimum Bit rate in bps </summary>
        public double? OverallBitRateMinimum { get; set; }
        ///<summary>  Minimum Bit rate (with measurement) </summary>
        public string OverallBitRateMinimumString { get; set; }
        ///<summary>  Nominal Bit rate in bps </summary>
        public double? OverallBitRateNominal { get; set; }
        ///<summary>  Nominal Bit rate (with measurement) </summary>
        public string OverallBitRateNominalString { get; set; }
        ///<summary>  Maximum Bit rate in bps </summary>
        public double? OverallBitRateMaximum { get; set; }
        ///<summary>  Maximum Bit rate (with measurement) </summary>
        public string OverallBitRateMaximumString { get; set; }
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
        ///<summary>  format HH:MM:SS.MMM </summary>
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
        ///<summary>   </summary>
        public string HeaderSize { get; set; }
        ///<summary>   </summary>
        public string DataSize { get; set; }
        ///<summary>   </summary>
        public string FooterSize { get; set; }
        ///<summary>   </summary>
        public string IsStreamable { get; set; }
        ///<summary>  The gain to apply to reach 89dB SPL on playback </summary>
        public string AlbumReplayGainGain { get; set; }
        ///<summary>   </summary>
        public string AlbumReplayGainGainString { get; set; }
        ///<summary>  The maximum absolute peak value of the item </summary>
        public string AlbumReplayGainPeak { get; set; }
        ///<summary>   </summary>
        public string Encryption { get; set; }
        ///<summary>   </summary>
        public string EncryptionFormat { get; set; }
        ///<summary>   </summary>
        public string EncryptionLength { get; set; }
        ///<summary>   </summary>
        public string EncryptionMethod { get; set; }
        ///<summary>   </summary>
        public string EncryptionMode { get; set; }
        ///<summary>   </summary>
        public string EncryptionPadding { get; set; }
        ///<summary>   </summary>
        public string EncryptionInitializationVector { get; set; }
        ///<summary>  Universal Ad ID, see https://ad-id.org </summary>
        public string UniversalAdIDString { get; set; }
        ///<summary>  Universal Ad ID registry </summary>
        public string UniversalAdIDRegistry { get; set; }
        ///<summary>  Universal Ad ID value </summary>
        public string UniversalAdIDValue { get; set; }
        ///<summary>  (Generic)Title of file </summary>
        public string Title { get; set; }
        ///<summary>  (Generic)More info about the title of file </summary>
        public string TitleMore { get; set; }
        ///<summary>  (Generic)Url </summary>
        public string TitleUrl { get; set; }
        ///<summary>  Univers movies belong to, e.g. Starwars, Stargate, Buffy, Dragonballs </summary>
        public string Domain { get; set; }
        ///<summary>  Name of the series, e.g. Starwars movies, Stargate SG-1, Stargate Atlantis, Buffy, Angel </summary>
        public string Collection { get; set; }
        ///<summary>  Name of the season, e.g. Strawars first Trilogy, Season 1 </summary>
        public string Season { get; set; }
        ///<summary>  Number of the Season </summary>
        public int? SeasonPosition { get; set; }
        ///<summary>  Place of the season e.g. 2 of 7 </summary>
        public string SeasonPositionTotal { get; set; }
        ///<summary>  Name of the movie. Eg </summary>
        public string Movie { get; set; }

        ///<summary>  More infos about the movie </summary>
        public string MovieMore { get; set; }
        ///<summary>  Country, where the movie was procuced </summary>
        public int? MovieCountry { get; set; }
        ///<summary>  Homepage for the movie </summary>
        public string MovieUrl { get; set; }
        ///<summary>  Name of an audio-album. Eg </summary>
        public string Album { get; set; }
        ///<summary>   </summary>
        public string Thejoshuatree { get; set; }
        ///<summary>  More infos about the album </summary>
        public string AlbumMore { get; set; }
        ///<summary>   </summary>
        public string AlbumSort { get; set; }
        ///<summary>  Album performer/artist of this file </summary>
        public string AlbumPerformer { get; set; }
        ///<summary>   </summary>
        public string AlbumPerformerSort { get; set; }
        ///<summary>  Homepage of the album performer/artist </summary>
        public string AlbumPerformerUrl { get; set; }
        ///<summary>  Name of the comic. </summary>
        public string Comic { get; set; }
        ///<summary>   </summary>
        public string ComicMore { get; set; }
        ///<summary>   </summary>
        public string ComicPositionTotal { get; set; }
        ///<summary>  Name of the part. e.g. CD1, CD2 </summary>
        public string Part { get; set; }
        ///<summary>  Number of the part </summary>
        public int? PartPosition { get; set; }
        ///<summary>  Place of the part e.g. 2 of 3 </summary>
        public string PartPositionTotal { get; set; }
        ///<summary>  Name of the track. e.g. track1, track 2 </summary>
        public string Track { get; set; }
        ///<summary>  More infos about the track </summary>
        public string TrackMore { get; set; }
        ///<summary>  Link to a site about this track </summary>
        public string TrackUrl { get; set; }
        ///<summary>   </summary>
        public string TrackSort { get; set; }
        ///<summary>  Number of this track </summary>
        public int? TrackPosition { get; set; }
        ///<summary>  Place of this track, e.g. 3 of 15 </summary>
        public string TrackPositionTotal { get; set; }
        ///<summary>  Package name i.e. technical flavor of the content </summary>
        public string PackageName { get; set; }
        ///<summary>  iTunes grouping </summary>
        public string Grouping { get; set; }
        ///<summary>  Name of the chapter. </summary>
        public string Chapter { get; set; }
        ///<summary>  Name of the subtrack. </summary>
        public string SubTrack { get; set; }
        ///<summary>  Original name of album, serie... </summary>
        public string OriginalAlbum { get; set; }
        ///<summary>  Original name of the movie </summary>
        public string OriginalMovie { get; set; }
        ///<summary>  Original name of the part in the original support </summary>
        public string OriginalPart { get; set; }
        ///<summary>  Original name of the track in the original support </summary>
        public string OriginalTrack { get; set; }
        ///<summary>  iTunes compilation </summary>
        public string Compilation { get; set; }
        ///<summary>  iTunes compilation </summary>
        public string CompilationString { get; set; }
        ///<summary>  Main performer/artist of this file </summary>
        public string Performer { get; set; }
        ///<summary>   </summary>
        public string PerformerSort { get; set; }
        ///<summary>  Homepage of the performer/artist </summary>
        public string PerformerUrl { get; set; }
        ///<summary>  Original artist(s)/performer(s). </summary>
        public string OriginalPerformer { get; set; }
        ///<summary>  Band/orchestra/accompaniment/musician. </summary>
        public string Accompaniment { get; set; }
        ///<summary>  Name of the original composer. </summary>
        public string Composer { get; set; }
        ///<summary>  Nationality of the main composer of the item, mostly for classical music. </summary>
        public string ComposerNationality { get; set; }
        ///<summary>   </summary>
        public string ComposerSort { get; set; }
        ///<summary>  The person who arranged the piece. e.g. Ravel. </summary>
        public string Arranger { get; set; }
        ///<summary>  The person who wrote the lyrics for a musical item. </summary>
        public string Lyricist { get; set; }
        ///<summary>  Original lyricist(s)/text writer(s). </summary>
        public string OriginalLyricist { get; set; }
        ///<summary>  The artist(s) who performed the work. In classical music this would be the conductor, orchestra, soloists. </summary>
        public string Conductor { get; set; }
        ///<summary>  Name of the director. </summary>
        public string Director { get; set; }
        ///<summary>  Name of the codirector. </summary>
        public string CoDirector { get; set; }
        ///<summary>  Name of the assistant director. </summary>
        public string AssistantDirector { get; set; }
        ///<summary>  The name of the director of photography, also known as cinematographer. </summary>
        public string DirectorOfPhotography { get; set; }
        ///<summary>  The name of the sound engineer or sound recordist. </summary>
        public string SoundEngineer { get; set; }
        ///<summary>  The person who oversees the artists and craftspeople who build the sets. </summary>
        public string ArtDirector { get; set; }
        ///<summary>  The person responsible for designing the Overall visual appearance of a movie. </summary>
        public string ProductionDesigner { get; set; }
        ///<summary>  The name of the choregrapher. </summary>
        public string Choregrapher { get; set; }
        ///<summary>  The name of the costume designer. </summary>
        public string CostumeDesigner { get; set; }
        ///<summary>  Real name of an actor or actress playing a role in the movie. </summary>
        public string Actor { get; set; }
        ///<summary>  Name of the character an actor or actress plays in this movie. </summary>
        public string ActorCharacter { get; set; }
        ///<summary>  The author of the story or script. </summary>
        public string WrittenBy { get; set; }
        ///<summary>  The author of the screenplay or scenario (used for movies and TV shows). </summary>
        public string ScreenplayBy { get; set; }
        ///<summary>  Editors name </summary>
        public string EditedBy { get; set; }
        ///<summary>  name of the person or organization that commissioned the subject of the file </summary>
        public string CommissionedBy { get; set; }
        ///<summary>  Name of the producer of the movie. </summary>
        public string Producer { get; set; }
        ///<summary>  The name of a co-producer. </summary>
        public string CoProducer { get; set; }
        ///<summary>  The name of an executive producer. </summary>
        public string ExecutiveProducer { get; set; }
        ///<summary>  Main music-artist for a movie </summary>
        public string MusicBy { get; set; }
        ///<summary>  Company the item is mainly distributed by </summary>
        public string DistributedBy { get; set; }
        ///<summary>  The engineer who mastered the content for a physical medium or for digital distribution. </summary>
        public string MasteredBy { get; set; }
        ///<summary>  Name of the person or organisation that encoded/ripped the audio file. </summary>
        public string EncodedBy { get; set; }
        ///<summary>  Name of the artist(s), that interpreted, remixed, or otherwise modified the item. </summary>
        public string RemixedBy { get; set; }
        ///<summary>  Main production studio </summary>
        public string ProductionStudio { get; set; }
        ///<summary>  A very general tag for everyone else that wants to be listed. </summary>
        public string ThanksTo { get; set; }
        ///<summary>  Name of the organization publishing the album (i.e. the 'record label') or movie. </summary>
        public string Publisher { get; set; }
        ///<summary>  Publishers official webpage. </summary>
        public string PublisherURL { get; set; }
        ///<summary>  Brand or trademark associated with the marketing of music recordings and music videos. </summary>
        public string Label { get; set; }
        ///<summary>  The main genre of the audio or video. e.g. classical, ambient-house, synthpop, sci-fi, drama, etc. </summary>
        public string Genre { get; set; }
        ///<summary>  Podcast category </summary>
        public string PodcastCategory { get; set; }
        ///<summary>  Intended to reflect the mood of the item with a few keywords, e.g. Romantic, Sad, Uplifting, etc. </summary>
        public string Mood { get; set; }
        ///<summary>  The type of the item. e.g. Documentary, Feature Film, Cartoon, Music Video, Music, Sound FX, etc. </summary>
        public string ContentType { get; set; }
        ///<summary>  Describes the topic of the file, such as Aerial view of Seattle.. </summary>
        public string Subject { get; set; }
        ///<summary>  A short description of the contents, such as Two birds flying. </summary>
        public string Description { get; set; }
        ///<summary>  Keywords to the item separated by a comma, used for searching. </summary>
        public string Keywords { get; set; }
        ///<summary>  A plot outline or a summary of the story. </summary>
        public string Summary { get; set; }
        ///<summary>  A description of the story line of the item. </summary>
        public string Synopsis { get; set; }
        ///<summary>  Describes the period that the piece is from or about. e.g. Renaissance. </summary>
        public string Period { get; set; }
        ///<summary>  Depending on the country it's the format of the rating of a movie (P, R, X in the USA, an age in other countries or a URI defining a logo). </summary>
        public int? LawRating { get; set; }
        ///<summary>  Reason for the law rating </summary>
        public string LawRatingReason { get; set; }
        ///<summary>  The ICRA rating. (Previously RSACi) </summary>
        public string ICRA { get; set; }
        ///<summary>  The date/year that the item was released. </summary>
        public string ReleasedDate { get; set; }
        ///<summary>  The date/year that the item was originaly released. </summary>
        public string OriginalReleasedDate { get; set; }
        ///<summary>  The time/date/year that the recording began. </summary>
        public string RecordedDate { get; set; }
        ///<summary>  The time/date/year that the encoding of this item was completed began. </summary>
        public string EncodedDate { get; set; }
        ///<summary>  The time/date/year that the tags were done for this item. </summary>
        public string TaggedDate { get; set; }
        ///<summary>  The time/date/year that the composition of the music/script began. </summary>
        public string WrittenDate { get; set; }
        ///<summary>  The time/date/year that the item was tranfered to a digitalmedium. </summary>
        public string MasteredDate { get; set; }
        ///<summary>  The time that the file was created on the file system </summary>
        public string FileCreatedDate { get; set; }
        ///<summary>  The time that the file was created on the file system (Warning: this field depends of local configuration, do not use it in an international database) </summary>
        public string FileCreatedDateLocal { get; set; }
        ///<summary>  The time that the file was modified on the file system </summary>
        public string FileModifiedDate { get; set; }
        ///<summary>  The time that the file was modified on the file system (Warning: this field depends of local configuration, do not use it in an international database) </summary>
        public string FileModifiedDateLocal { get; set; }
        ///<summary>  Location where track was recorded. (See COMPOSITION_LOCATION for format) </summary>
        public string RecordedLocation { get; set; }
        ///<summary>  Location that the item was originaly designed/written. Information should be stored in the following format: country code, state/province, city where the coutry code is the same 2 octets as in Internet domains, or possibly ISO-3166. e.g. US, Texas, Austin or US, , Austin. </summary>
        public int? WrittenLocation { get; set; }
        ///<summary>  Location, where an item is archived, e.eg. Louvre,Paris,France </summary>
        public string ArchivalLocation { get; set; }
        ///<summary>  Name of the software package used to create the file, such as Microsoft WaveEdiTY </summary>
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
        ///<summary>  Describes whether an image has been cropped and, if so, how it was cropped. </summary>
        public string Cropped { get; set; }
        ///<summary>  Specifies the size of the original subject of the file. eg 8.5 in h, 11 in w </summary>
        public string Dimensions { get; set; }
        ///<summary>  Stores dots per inch setting of the digitizer used to produce the file </summary>
        public string DotsPerInch { get; set; }
        ///<summary>  Describes the changes in lightness settings on the digitizer required to produce the file </summary>
        public string Lightness { get; set; }
        ///<summary>  Original medium of the material, e.g. vinyl, Audio-CD, Super8 or BetaMax </summary>
        public string OriginalSourceMedium { get; set; }
        ///<summary>  Original form of the material, e.g. slide, paper, map </summary>
        public string OriginalSourceForm { get; set; }
        ///<summary>  Name of the product the file was originally intended for </summary>
        public string OriginalSourceFormName { get; set; }
        ///<summary>  Software used to tag this file </summary>
        public string TaggedApplication { get; set; }
        ///<summary>  Average number of beats per minute </summary>
        public int? BPM { get; set; }
        ///<summary>  International Standard Recording Code, excluding the ISRC prefix and including hyphens. </summary>
        public string ISRC { get; set; }
        ///<summary>  International Standard Book Number. </summary>
        public int? ISBN { get; set; }
        ///<summary>  EAN-13 (13-digit European Article Numbering) or UPC-A (12-digit Universal Product Code) bar code identifier. </summary>
        public int? BarCode { get; set; }
        ///<summary>  Library of Congress Control Number. </summary>
        public int? LCCN { get; set; }
        ///<summary>  A label-specific catalogue number used to identify the release. e.g. TIC 01. </summary>
        public int? CatalogNumber { get; set; }
        ///<summary>  A 4-digit or 5-digit number to identify the record label, typically printed as (LC) xxxx or (LC) 0xxxx on CDs medias or covers, with only the number being stored. </summary>
        public int? LabelCode { get; set; }
        ///<summary>  Owner of the file </summary>
        public string Owner { get; set; }
        ///<summary>  Copyright attribution. </summary>
        public string Copyright { get; set; }
        ///<summary>  Link to a site with copyright/legal information. </summary>
        public string CopyrightUrl { get; set; }
        ///<summary>  The copyright information as per the productioncopyright holder. </summary>
        public string ProducerCopyright { get; set; }
        ///<summary>  License information, e.g., All Rights Reserved,Any Use Permitted. </summary>
        public ulong? TermsOfUse { get; set; }
        ///<summary>   </summary>
        public string ServiceName { get; set; }
        ///<summary>   </summary>
        public string ServiceChannel { get; set; }
        ///<summary>   </summary>
        public string ServiceUrl { get; set; }
        ///<summary>   </summary>
        public string ServiceProvider { get; set; }
        ///<summary>   </summary>
        public string ServiceProviderUrl { get; set; }
        ///<summary>   </summary>
        public string ServiceType { get; set; }
        ///<summary>   </summary>
        public string NetworkName { get; set; }
        ///<summary>   </summary>
        public string OriginalNetworkName { get; set; }
        ///<summary>   </summary>
        public string Country { get; set; }
        ///<summary>   </summary>
        public string TimeZone { get; set; }
        ///<summary>  Is there a cover </summary>
        internal List<bool?> Cover { get; set; }
        ///<summary>  short descriptio, e.g. Earth in space </summary>
        public List<string> CoverDescription { get; set; }
        ///<summary>   </summary>
        public List<string> CoverType { get; set; }
        ///<summary>   </summary>
        public List<string >CoverMime { get; set; }

        ///<summary>  Cover, in binary format encoded BASE64 </summary>
        public List<byte[]> CoverData
        {
            get
            {
                if(!CanGetCover) throw new Exception("Unable to get cover");
                return _coverData;
            }
            set => _coverData = value;
        }

        ///<summary>  Text of a song </summary>
        public string Lyrics { get; set; }
        ///<summary>  Any comment related to the content. </summary>
        public string Comment { get; set; }
        ///<summary>  A numeric value defining how much a person likes the song/movie. The number is between 0 and 5 with decimal values possible (e.g. 2.7), 5(.0) being the highest possible rating. </summary>
        public int? Rating { get; set; }
        ///<summary>  Date/year the item was added to the owners collection </summary>
        public string AddedDate { get; set; }
        ///<summary>  The date, the owner first played an item </summary>
        public string PlayedFirstDate { get; set; }
        ///<summary>  The date, the owner last played an item </summary>
        public string PlayedLastDate { get; set; }
        ///<summary>  Number of times an item was played </summary>
        public int? PlayedCount { get; set; }
        ///<summary>   </summary>
        public string EPGPositionsBegin { get; set; }
        ///<summary>   </summary>
        public string EPGPositionsEnd { get; set; }
        ///<summary> (Generic)Performer of the file </summary>
        public string Author
        { get; set; }
        ///<summary> Duplicate of Performer </summary>
        public string Artist
        { get; set; }
        ///<summary> Name of the character an actor or actress plays in this movie. </summary>
        public string MusicianInstrument
        { get; set; }
        ///<summary> Technician. Identifies the technician who digitized the subject file. For example, Smith, John. </summary>
        public string Technician
        { get; set; }
        ///<summary> Source. Identifies the name of the person or organization who supplied the original subject of the file. For example, Trey Research. </summary>
        public string EncodedOriginalDistributedBy
        { get; set; }
        ///<summary> Contains the name of the internet radio station from which the audio is streamed. </summary>
        public string RadioStation
        { get; set; }
        ///<summary> Contains the name of the owner of the internet radio station from which the audio is streamed. </summary>
        public string RadioStationOwner
        { get; set; }
        /// <summary> Official internet radio station homepage. </summary>
        public string RadioStationURL

        { get; set; }
        ///<summary> A description of the story line of the item. </summary>
        public string Synopsys
        { get; set; }
        ///<summary> The ICRA rating. (Previously RSACi) </summary>
        public string IRCA
        { get; set; }
        ///<summary> Language(s) of the item in the bibliographic ISO-639-2 form. </summary>
        public string Language
        { get; set; }
        ///<summary> Medium. Describes the original subject of the file, such as, computer image, drawing, lithograph, and so forth. Not necessarily the same as ISRF. </summary>
        public string Medium
        { get; set; }
        ///<summary> Product. Specifies the name of the title the file was originally intended for, such as Encyclopedia of Pacific Northwest Geography. </summary>
        public string Product
        { get; set; }
        ///<summary> Contains a timestamp describing when the original recording of the audio was recorded. </summary>
        public string OriginalRecordedDate
        { get; set; }
        ///<summary> Identifies the original recording media form from which the material originated, such as CD, cassette, LP, radio broadcast, slide, paper, etc. </summary>
        public string EncodedOriginal
        { get; set; }
        ///<summary> Official audio source webpage. e.g. a movie. </summary>
        public string EncodedOriginalUrl
        { get; set; }
        ///<summary> This is a binary dump of the TOC of the CDROM that this item was taken from. </summary>
        public string MSDI
        { get; set; }
        /// <summary>
        /// Is there a cover
        /// </summary>
        public bool HasCover => (Cover?.Contains(true)).GetValueOrDefault();
        /// <summary>
        /// Count of cover
        /// </summary>
        public int CoverCount => Cover?.Count(i => i == true) ?? 0;

        // Other

        /// <summary>
        /// Bit rate of all streams in bps
        /// </summary>
        [Obsolete("OveralBitRate is deprecated, please use OverallBitRate instead.")]
        public double? OveralBitRate
        {
            get => OverallBitRate;
            set => OverallBitRate = value;
        }

        ///<summary>  Bit rate of all streams (with measure) </summary>
        [Obsolete("OveralBitRate is deprecated, please use OverallBitRate instead.")]
        public string OveralBitRateString
        {
            get => OverallBitRateString;
            set => OverallBitRateString = value;
        }
        /// <summary>
        /// 
        /// </summary>
        [Obsolete("PlayTimeMillisec is deprecated.")]
        public double? PlayTimeMillisec
        {
            get => Duration;
            set => Duration = value;
        }
        ///<summary> Video Codecs separated by _ </summary>
        [Obsolete("VideoCodecList is deprecated.")]
        public string VideoCodecList
        { get; set; }

        ///<summary> Audio Codecs separated by _ </summary>
        [Obsolete("AudioCodecList is deprecated by MediaInfo.")]
        public string AudioCodecList
        { get; set; }

        ///<summary> Text Codecs separated by _ </summary>
        [Obsolete("TextCodecList is deprecated by MediaInfo.")]
        public string TextCodecList
        { get; set; }

        ///<summary> Chapters Codecs separated by _ </summary>
        [Obsolete("ChaptersCodecList is deprecated.")]
        public string ChaptersCodecList
        { get; set; }
        ///<summary> Chapters languages separated by _ </summary>
        [Obsolete("ChaptersLanguageList is deprecated.")]
        public string ChaptersLanguageList
        { get; set; }

        ///<summary> Image Codecs separated by _ </summary>
        [Obsolete("ImageCodecList is deprecated by MediaInfo.")]
        public string ImageCodecList
        { get; set; }

        ///<summary> Other tags not known... </summary>
        [Obsolete("Other is deprecated.")]
        public string Other
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("FileSizePrecise is deprecated.")]
        public string FileSizePrecise
        {
            get => FileSizeFormatted;
            set => FileSizeFormatted = value;
        }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("FileSizeRounded is deprecated.")]
        public string FileSizeRounded
        {
            get => FileSizeString;
            set => FileSizeString = value;
        }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("PlayTimeFormatted is deprecated.")]
        public string PlayTimeFormatted
        {
            get => PlayTimeString3;
            set => PlayTimeString3 = value;
        }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("PlayTimeString3 is deprecated.")]
        public string PlayTimeString3 { get; set; }
    }
}
