using System;
using System.Collections.Generic;

namespace GetMediaInfo
{
    /// <summary>
    /// Contains properties for a GeneralTrack
    /// </summary>
    public class GeneralTrack
    {
        #region PrivateFields
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
        /// Number of general streams.
        /// </summary>
        public int? GeneralCount { get; set; } //GeneralCount
        /// <summary>
        /// Number of video streams.
        /// </summary>
        public int? VideoCount { get; set; } //VideoCount
        /// <summary>
        /// Number of audio streams.
        /// </summary>
        public int? AudioCount { get; set; } //AudioCount
        /// <summary>
        /// Number of text streams.
        /// </summary>
        public int? TextCount { get; set; } //TextCount
        /// <summary>
        /// Number of image streams.
        /// </summary>
        public int? ImageCount { get; set; } //ImageCount
        /// <summary>
        /// Number of menu streams.
        /// </summary>
        public int? MenuCount { get; set; } //MenuCount
        /// <summary>
        /// List of Video Codecs in this file.
        /// </summary>
        public List<string> VideoFormatList { get; set; } //Video_Format_List
        /// <summary>
        /// List of Video Codecs in this file with popular name (hint).
        /// </summary>
        public List<string> VideoFormatListWithHint { get; set; } //Video_Format_WithHint_List
        /// <summary>
        /// List of Video languages in this file.
        /// </summary>
        public List<string> VideoLanguageList { get; set; } //Video_Language_List
        /// <summary>
        /// List of Audio Codecs in this file.
        /// </summary>
        public List<string> AudioFormatList { get; set; } //Audio_Format_List
        /// <summary>
        /// List of Audio Codecs in this file with popular name (hint).
        /// </summary>
        public List<string> AudioFormatListWithHint { get; set; } //Audio_Format_WithHint_List
        /// <summary>
        /// List of Audio languages in this file.
        /// </summary>
        public List<string> AudioLanguageList { get; set; } //Audio_Language_List
        /// <summary>
        /// List of Text Codecs in this file.
        /// </summary>
        public List<string> TextFormatList  { get; set; } //Text_Format_List
        /// <summary>
        /// List of Text Codecs in this file with popular name (hint).
        /// </summary>
        public List<string> TextFormatListWithHint { get; set; } //Text_Format_WithHint_List
        /// <summary>
        /// List of Text languages in this file.
        /// </summary>
        public List<string> TextLanguageList { get; set; } //Text_Language_List
        /// <summary>
        /// List of Image Codecs in this file.
        /// </summary>
        public List<string> ImageFormatList { get; set; } //Image_Format_List
        /// <summary>
        /// List of Image Codecs in this file with popular name (hint).
        /// </summary>
        public List<string> ImageFormatListWithHint { get; set; } //Image_Format_WithHint_Lis
        /// <summary>
        /// List of Image languages in this file.
        /// </summary>
        public List<string> ImageLanguageList { get; set; } //Image_Language_List
        /// <summary>
        /// List of Menu Codecs in this file.
        /// </summary>
        public List<string> MenuFormatList { get; set; } //Menu_Format_List
        /// <summary>
        /// List of Menu Codecs in this file with popular name (hint).
        /// </summary>
        public List<string> MenuFormatListWithHint { get; set; } //Menu_Format_WithHint_List
        /// <summary>
        /// List of Menu languages in this file.
        /// </summary>
        public List<string> MenuLanguageList { get; set; } //Menu_Language_List
        /// <summary>
        /// Complete name (Folder+Name+Extension).
        /// </summary>
        public string CompleteName { get; set; } //CompleteName
        /// <summary>
        /// Folder name only.
        /// </summary>
        public string FolderName { get; set; } //FolderName
        /// <summary>
        /// File name with extension.
        /// </summary>                                                                                  
        public string FileNameExtension { get; set; } //FileNameExtension
        /// <summary>
        /// File name only.
        /// </summary>
        public string FileName { get; set; } //FileName
        /// <summary>
        /// File extension only.
        /// </summary>
        public string FileExtension { get; set; } //FileExtension
        /// <summary>
        /// Format used.
        /// </summary>                                           
        public string Format { get; set; } //Format
        /// <summary>
        /// Format used + additional features.
        /// </summary>
        public string FormatWithExtra { get; set; } //Format/String
        /// <summary>
        /// Info about this Format.
        /// </summary>
        public string FormatInfo { get; set; } //Format/Info
        /// <summary>
        /// Link to a description of this format.
        /// </summary>
        public string FormatUrl { get; set; } //Format/Url
        /// <summary>
        /// Known extensions of this format.
        /// </summary>
        public string FormatExtensions { get; set; } //Format/Extensions
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
        /// Compression method used.
        /// </summary>
        public string FormatCompression { get; set; } //Format_Compression
        /// <summary>
        /// Settings needed for decoder used.
        /// </summary>
        public string FormatSettings { get; set; } //Format_Settings        
        /// <summary>
        /// Format features needed for fully supporting the content
        /// </summary>
        public string FormatAdditionalFeatures { get; set; } //Format_AdditionalFeatures
        /// <summary>
        /// Internet Media Type(aka MIME Type, Content-Type).
        /// </summary>
        public string InternetMediaType { get; set; } //InternetMediaType

        /// <summary>
        /// Codec ID (found in some containers).
        /// </summary>
        public int? CodecID { get; set; } //CodecID

        /// <summary>
        /// Info about this codec.
        /// </summary>
        public string CodecIDInfo { get; set; } //CodecID/Info

        /// <summary>
        /// A hint/popular name for this codec.
        /// </summary>
        public string CodecIDHint { get; set; } //CodecID/Hint

        /// <summary>
        /// A link to more details about this codec ID.
        /// </summary>
        public string CodecIDUrl { get; set; } //CodecID/Url

        /// <summary>
        /// Manual description given by the container.
        /// </summary>
        public string CodecIDDescription { get; set; } //CodecID_Description

        /// <summary>
        /// Version of the CodecID.
        /// </summary>
        public string CodecIDVersion { get; set; } //CodecID_Version

        /// <summary>
        /// Compatible CodecIDs.
        /// </summary>
        public string CodecIDCompatible { get; set; } //CodecID_Compatible

        /// <summary>
        /// If Audio and video are muxed.
        /// </summary>
        public bool? Interleaved { get; set; } //Interleaved

        /// <summary>
        /// File size in bytes.
        /// </summary>
        public ulong? FileSize { get; set; } //FileSize

        /// <summary>
        /// File size (with measure). Ex: 39.0 MiB
        /// </summary>
        public string FileSizeString { get; set; } //FileSize/String

        /// <summary>
        /// File size (with measure, 2 digit mini). Ex: 39 MiB
        /// </summary>
        public string FileSizeStringRounded { get; set; } //FileSize/String3

        /// <summary>
        /// File size (with measure, 4 digit mini). Ex: 38.97 MiB
        /// </summary>
        public string FileSizeStringAccurate { get; set; } //FileSize/String4

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
        /// Bit rate mode of all streams (VBR, CBR).
        /// </summary>
        public string OverallBitRateMode { get; set; } //OverallBitRate_Mode

        /// <summary>
        /// Bit rate of all streams in bps.
        /// </summary>
        public ulong? OverallBitRate { get; set; } //OverallBitRate

        /// <summary>
        /// Bit rate of all streams (with measure).
        /// </summary>
        public string OverallBitRateFormatted { get; set; } //OverallBitRate/String

        /// <summary>
        /// Minimum Bit rate in bps.
        /// </summary>
        public ulong? OverallBitRateMinimum { get; set; } //OverallBitRate_Minimum

        /// <summary>
        /// Minimum Bit rate (with measurement).
        /// </summary>
        public string OverallBitRateMinimumFormatted { get; set; } //OverallBitRate_Minimum/String

        /// <summary>
        /// Nominal Bit rate in bps.
        /// </summary>
        public ulong? OverallBitRateNominal { get; set; } //OverallBitRate_Nominal

        /// <summary>
        /// Nominal Bit rate (with measurement).
        /// </summary>
        public string OverallBitRateNominalFormatted { get; set; } //OverallBitRate_Nominal/String

        /// <summary>
        /// Maximum Bit rate in bps.
        /// </summary>
        public ulong? OverallBitRateMaximum { get; set; } //OverallBitRate_Maximum

        /// <summary>
        /// Maximum Bit rate (with measurement).
        /// </summary>
        public string OverallBitRateMaximumFormatted { get; set; } //OverallBitRate_Maximum/String

        /// <summary>
        /// Frames per second.
        /// </summary>
        public double? FrameRate { get; set; } //FrameRate

        /// <summary>
        /// Frames per second (with measurement).
        /// </summary>
        public string FrameRateString { get; set; } //FrameRate/String

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
        /// The gain to apply to reach 89dB SPL on playback.
        /// </summary>
        public double? AlbumReplayGainGain { get; set; } //Album_ReplayGain_Gain

        /// <summary>
        /// The maximum absolute peak value of the item.
        /// </summary>
        public double? AlbumReplayGainPeak { get; set; } //Album_ReplayGain_Peak

        /// <summary>
        /// (Generic)Title of file.
        /// </summary>
        public string Title { get; set; } //Title

        /// <summary>
        /// (Generic)More info about the title of file.
        /// </summary>
        public string TitleMore { get; set; } //Title_More

        /// <summary>
        /// (Generic)Url.
        /// </summary>
        public string TitleUrl { get; set; } //Title/Url

        /// <summary>
        /// Univers movies belong to, e.g. Starwars, Stargate, Buffy, Dragonballs.
        /// </summary>
        public string Domain { get; set; } //Domain

        /// <summary>
        /// Name of the series, e.g. Starwars movies, Stargate SG-1, Stargate Atlantis, Buffy, Angel.
        /// </summary>
        public string Collection { get; set; } //Collection

        /// <summary>
        /// Name of the season, e.g. Strawars first Trilogy, Season 1.
        /// </summary>
        public string Season { get; set; } //Season

        /// <summary>
        /// Number of the Season.
        /// </summary>
        public int? SeasonPosition { get; set; } //Season_Position

        /// <summary>
        /// Place of the season e.g. 2 of 7.
        /// </summary>
        public string SeasonPositionTotal { get; set; } //Season_Position_Total

        /// <summary>
        /// Name of the movie. Eg : Starwars, a new hope.
        /// </summary>
        public string Movie { get; set; } //Movie

        /// <summary>
        /// More infos about the movie.
        /// </summary>
        public string MovieMore { get; set; } //Movie_More

        /// <summary>
        /// Country, where the movie was procuced.
        /// </summary>
        public string MovieCountry { get; set; } //Movie/Country

        /// <summary>
        /// Homepage for the movie.
        /// </summary>
        public string MovieUrl { get; set; } //Movie/Url

        /// <summary>
        /// Name of an audio-album. Eg : The joshua tree.
        /// </summary>
        public string Album { get; set; } //Album

        /// <summary>
        /// More infos about the album.
        /// </summary>
        public string AlbumInfo { get; set; } //Album_More

        /// <summary>
        /// Album performer/artist of this file.
        /// </summary>
        public string AlbumPerformer { get; set; } //Album/Performer

        /// <summary>
        /// Homepage of the album performer/artist.
        /// </summary>
        public string AlbumPerformerUrl { get; set; } //Album/Performer/Url

        /// <summary>
        /// Name of the comic.
        /// </summary>
        public string Comic { get; set; } //Comic

        /// <summary>
        /// Name of the part. e.g. CD1, CD2.
        /// </summary>
        public string Part { get; set; } //Part

        /// <summary>
        /// Number of the part.
        /// </summary>
        public int? PartPosition { get; set; } //Part/Position

        /// <summary>
        /// Place of the part e.g. 2 of 3.
        /// </summary>
        public string PartPositionTotal { get; set; } //Part/Position_Total

        /// <summary>
        /// Name of the track. e.g. track1, track 2.
        /// </summary>
        public string Track { get; set; } //Track

        /// <summary>
        /// More infos about the track.
        /// </summary>
        public string TrackInfo { get; set; } //Track_More

        /// <summary>
        /// Link to a site about this track.
        /// </summary>
        public string TrackUrl { get; set; } //Track/Url

        /// <summary>
        /// Number of this track.
        /// </summary>
        public int? TrackPosition { get; set; } //Track/Position

        /// <summary>
        /// Place of this track, e.g. 3 of 15.
        /// </summary>
        public string TrackPositionTotal { get; set; } //Track/Position_Total

        /// <summary>
        /// Package name i.e. technical flavor of the content.
        /// </summary>
        public string PackageName { get; set; } //PackageName

        /// <summary>
        /// iTunes grouping.
        /// </summary>
        public string Grouping { get; set; } //Grouping

        /// <summary>
        /// Name of the chapter.
        /// </summary>
        public string Chapter { get; set; } //Chapter

        /// <summary>
        /// Name of the subtrack.
        /// </summary>
        public string SubTrack { get; set; } //SubTrack

        /// <summary>
        /// Original name of album, serie...
        /// </summary>
        public string OriginalAlbum { get; set; } //Original/Album

        /// <summary>
        /// Original name of the movie.
        /// </summary>
        public string OriginalMovie { get; set; } //Original/Movie

        /// <summary>
        /// Original name of the part in the original support.
        /// </summary>
        public string OriginalPart { get; set; } //Original/Part

        /// <summary>
        /// Original name of the track in the original support.
        /// </summary>
        public string OriginalTrack { get; set; } //Original/Track

        /// <summary>
        /// iTunes compilation.
        /// </summary>
        public string Compilation { get; set; } //Compilation

        /// <summary>
        /// iTunes compilation.
        /// </summary>
        public string CompilationString { get; set; } //Compilation/String

        /// <summary>
        /// Main performer/artist of this file.
        /// </summary>
        public string Performer { get; set; } //Performer

        /// <summary>
        /// Homepage of the performer/artist.
        /// </summary>
        public string PerformerUrl { get; set; } //Performer/Url

        /// <summary>
        /// Original artist(s)/performer(s).
        /// </summary>
        public string OriginalPerformer { get; set; } //Original/Performer

        /// <summary>
        /// Band/orchestra/accompaniment/musician.
        /// </summary>
        public string Accompaniment { get; set; } //Accompaniment

        /// <summary>
        /// Name of the original composer.
        /// </summary>
        public string Composer { get; set; } //Composer

        /// <summary>
        /// Nationality of the main composer of the item, mostly for classical music.
        /// </summary>
        public string ComposerNationality { get; set; } //Composer/Nationality

        /// <summary>
        /// The person who arranged the piece. e.g. Ravel.
        /// </summary>
        public string Arranger { get; set; } //Arranger

        /// <summary>
        /// The person who wrote the lyrics for a musical item.
        /// </summary>
        public string Lyricist { get; set; } //Lyricist

        /// <summary>
        /// Original lyricist(s)/text writer(s).
        /// </summary>
        public string OriginalLyricist { get; set; } //Original/Lyricist

        /// <summary>
        /// The artist(s) who performed the work. In classical music this would be the conductor, orchestra, soloists.
        /// </summary>
        public string Conductor { get; set; } //Conductor

        /// <summary>
        /// Name of the director.
        /// </summary>
        public string Director { get; set; } //Director

        /// <summary>
        /// Name of the codirector.
        /// </summary>
        public string CoDirector { get; set; } //CoDirector

        /// <summary>
        /// Name of the assistant director.
        /// </summary>
        public string AssistantDirector { get; set; } //AssistantDirector

        /// <summary>
        /// The name of the director of photography, also known as cinematographer.
        /// </summary>
        public string DirectorOfPhotography { get; set; } //DirectorOfPhotography

        /// <summary>
        /// The name of the sound engineer or sound recordist.
        /// </summary>
        public string SoundEngineer { get; set; } //SoundEngineer

        /// <summary>
        /// The person who oversees the artists and craftspeople who build the sets.
        /// </summary>
        public string ArtDirector { get; set; } //ArtDirector

        /// <summary>
        /// The person responsible for designing the Overall visual appearance of a movie.
        /// </summary>
        public string ProductionDesigner { get; set; } //ProductionDesigner

        /// <summary>
        /// The name of the choreographer.
        /// </summary>
        public string Choreographer { get; set; } //Choreographer

        /// <summary>
        /// The name of the costume designer.
        /// </summary>
        public string CostumeDesigner { get; set; } //CostumeDesigner

        /// <summary>
        /// Real name of an actor or actress playing a role in the movie.
        /// </summary>
        public string Actor { get; set; } //Actor

        /// <summary>
        /// Name of the character an actor or actress plays in this movie.
        /// </summary>
        public string ActorCharacter { get; set; } //Actor_Character

        /// <summary>
        /// The author of the story or script.
        /// </summary>
        public string WrittenBy { get; set; } //WrittenBy

        /// <summary>
        /// The author of the screenplay or scenario (used for movies and TV shows).
        /// </summary>
        public string ScreenplayBy { get; set; } //ScreenplayBy

        /// <summary>
        /// Editors name.
        /// </summary>
        public string EditedBy { get; set; } //EditedBy

        /// <summary>
        /// name of the person or organization that commissioned the subject of the file.
        /// </summary>
        public string CommissionedBy { get; set; } //CommissionedBy

        /// <summary>
        /// Name of the producer of the movie.
        /// </summary>
        public string Producer { get; set; } //Producer

        /// <summary>
        /// The name of a co-producer.
        /// </summary>
        public string CoProducer { get; set; } //CoProducer

        /// <summary>
        /// The name of an executive producer.
        /// </summary>
        public string ExecutiveProducer { get; set; } //ExecutiveProducer

        /// <summary>
        /// Main music-artist for a movie.
        /// </summary>
        public string MusicBy { get; set; } //MusicBy

        /// <summary>
        /// Company the item is mainly distributed by.
        /// </summary>
        public string DistributedBy { get; set; } //DistributedBy

        /// <summary>
        /// Name of the person or organization who supplied the original subject.
        /// </summary>
        public string OriginalSourceFormDistribution { get; set; } //OriginalSourceForm/Distribution

        /// <summary>
        /// The engineer who mastered the content for a physical medium or for digital distribution.
        /// </summary>
        public string MasteredBy { get; set; } //MasteredBy

        /// <summary>
        /// Name of the person or organisation that encoded/ripped the audio file.
        /// </summary>
        public string EncodedBy { get; set; } //EncodedBy

        /// <summary>
        /// Name of the artist(s), that interpreted, remixed, or otherwise modified the item.
        /// </summary>
        public string RemixedBy { get; set; } //RemixedBy

        /// <summary>
        /// Main production studio.
        /// </summary>
        public string ProductionStudio { get; set; } //ProductionStudio

        /// <summary>
        /// A very general tag for everyone else that wants to be listed.
        /// </summary>
        public string ThanksTo { get; set; } //ThanksTo

        /// <summary>
        /// Name of the organization publishing the album (i.e. the 'record label') or movie.
        /// </summary>
        public string Publisher { get; set; } //Publisher
        
        /// <summary>
        /// Publishers official webpage.
        /// </summary>
        public string PublisherURL { get; set; } //Publisher/URL

        /// <summary>
        /// Brand or trademark associated with the marketing of music recordings and music videos.
        /// </summary>
        public string Label { get; set; } //Label

        /// <summary>
        /// The main genre of the audio or video. e.g. classical, ambient-house, synthpop, sci-fi, drama, etc.
        /// </summary>
        public string Genre { get; set; } //Genre

        /// <summary>
        /// Intended to reflect the mood of the item with a few keywords, e.g. Romantic, Sad, Uplifting, etc.
        /// </summary>
        public string Mood { get; set; } //Mood

        /// <summary>
        /// The type of the item. e.g. Documentary, Feature Film, Cartoon, Music Video, Music, Sound FX, etc.
        /// </summary>
        public string ContentType { get; set; } //ContentType

        /// <summary>
        /// Describes the topic of the file, such as Aerial view of Seattle..
        /// </summary>
        public string Subject { get; set; } //Subject

        /// <summary>
        /// A short description of the contents, such as Two birds flying.
        /// </summary>
        public string Description { get; set; } //Description

        /// <summary>
        /// Keywords to the item separated by a comma, used for searching.
        /// </summary>
        public string Keywords { get; set; } //Keywords

        /// <summary>
        /// A plot outline or a summary of the story.
        /// </summary>
        public string Summary { get; set; } //Summary

        /// <summary>
        /// A description of the story line of the item.
        /// </summary>
        public string Synopsis { get; set; } //Synopsis

        /// <summary>
        /// Describes the period that the piece is from or about. e.g. Renaissance.
        /// </summary>
        public string Period { get; set; } //Period

        /// <summary>
        /// Depending on the country it's the format of the rating of a movie (P, R, X in the USA, an age in other countries or a URI defining a logo).
        /// </summary>
        public string LawRating { get; set; } //LawRating

        /// <summary>
        /// Reason for the law rating.
        /// </summary>
        public string LawRatingReason { get; set; } //LawRating_Reason

        /// <summary>
        /// The ICRA rating. (Previously RSACi).
        /// </summary>
        public string ICRA { get; set; } //ICRA

        /// <summary>
        /// The date/year that the item was released.
        /// </summary>
        public string ReleasedDate { get; set; } //Released_Date

        /// <summary>
        /// The date/year that the item was originaly released.
        /// </summary>
        public string OriginalReleasedDate { get; set; } //Original/Released_Date

        /// <summary>
        /// The time/date/year that the recording began.
        /// </summary>
        public string RecordedDate { get; set; } //Recorded_Date

        /// <summary>
        /// The time/date/year that the encoding of this item was completed began.
        /// </summary>
        public string EncodedDate { get; set; } //Encoded_Date

        /// <summary>
        /// The time/date/year that the tags were done for this item.
        /// </summary>
        public string TaggedDate { get; set; } //Tagged_Date

        /// <summary>
        /// The time/date/year that the composition of the music/script began.
        /// </summary>
        public string WrittenDate { get; set; } //Written_Date

        /// <summary>
        /// The time/date/year that the item was tranfered to a digitalmedium.
        /// </summary>
        public string MasteredDate { get; set; } //Mastered_Date

        /// <summary>
        /// The time that the file was created on the file system.
        /// </summary>
        public string FileCreatedDate { get; set; } //File_Created_Date

        /// <summary>
        /// The time that the file was created on the file system (Warning: this field depends of local configuration, do not use it in an international database).
        /// </summary>
        public string FileCreatedDateLocal { get; set; } //File_Created_Date_Local

        /// <summary>
        /// The time that the file was modified on the file system.
        /// </summary>
        public string FileModifiedDate { get; set; } //File_Modified_Date

        /// <summary>
        /// The time that the file was modified on the file system (Warning: this field depends of local configuration, do not use it in an international database).
        /// </summary>
        public string FileModifiedDateLocal { get; set; } //File_Modified_Date_Local

        /// <summary>
        /// Location where track was recorded. (See COMPOSITION_LOCATION for format).
        /// </summary>
        public string RecordedLocation { get; set; } //Recorded_Location

        /// <summary>
        /// Location that the item was originaly designed/written. Information should be stored in the following format: country code, state/province, city where the coutry code is the same 2 octets as in Internet domains, or possibly ISO-3166. e.g. US, Texas, Austin or US, , Austin.
        /// </summary>
        public string WrittenLocation { get; set; } //Written_Location

        /// <summary>
        /// Location, where an item is archived, e.eg. Louvre,Paris,France.
        /// </summary>
        public string ArchivalLocation { get; set; } //Archival_Location

        /// <summary>
        /// Name of the software package used to create the file, such as Microsoft WaveEdit.
        /// </summary>
        public string EncodedApplication { get; set; } //Encoded_Application

        /// <summary>
        /// Name of the software package used to create the file, such as Microsoft WaveEdit, trying to have the format 'CompanyName ProductName (OperatingSystem) Version (Date)'.
        /// </summary>
        public string EncodedApplicationString { get; set; } //Encoded_Application/String

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
        public string EncodedLibraryString { get; set; } //Encoded_Library/String

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
        /// Describes whether an image has been cropped and, if so, how it was cropped.
        /// </summary>
        public string Cropped { get; set; } //Cropped

        /// <summary>
        /// Specifies the size of the original subject of the file. eg 8.5 in h, 11 in w.
        /// </summary>
        public string Dimensions { get; set; } //Dimensions

        /// <summary>
        /// Stores dots per inch setting of the digitizer used to produce the file.
        /// </summary>
        public string DotsPerInch { get; set; } //DotsPerInch

        /// <summary>
        /// Describes the changes in lightness settings on the digitizer required to produce the file.
        /// </summary>
        public string Lightness { get; set; } //Lightness

        /// <summary>
        /// Original medium of the material, e.g. vinyl, Audio-CD, Super8 or BetaMax.
        /// </summary>
        public string OriginalSourceMedium { get; set; } //OriginalSourceMedium

        /// <summary>
        /// Original form of the material, e.g. slide, paper, map.
        /// </summary>
        public string OriginalSourceForm { get; set; } //OriginalSourceForm

        /// <summary>
        /// Number of colors requested when digitizing, e.g. 256 for images or 32 bit RGB for video.
        /// </summary>
        public string OriginalSourceFormNumColour { get; set; } //OriginalSourceForm/NumColour

        /// <summary>
        /// Name of the product the file was originally intended for.
        /// </summary>
        public string OriginalSourceFormName { get; set; } //OriginalSourceForm/Name

        /// <summary>
        /// Describes whether an image has been cropped and, if so, how it was cropped. e.g. 16:9 to 4:3, top and bottom.
        /// </summary>
        public string OriginalSourceFormCropped { get; set; } //OriginalSourceForm/Cropped

        /// <summary>
        /// Identifies the changes in sharpness for the digitizer requiered to produce the file.
        /// </summary>
        public string OriginalSourceFormSharpness { get; set; } //OriginalSourceForm/Sharpness

        /// <summary>
        /// Software used to tag this file.
        /// </summary>
        public string TaggedApplication { get; set; } //Tagged_Application

        /// <summary>
        /// Average number of beats per minute.
        /// </summary>
        public int? BPM { get; set; } //BPM

        /// <summary>
        /// International Standard Recording Code, excluding the ISRC prefix and including hyphens.
        /// </summary>
        public string ISRC { get; set; } //ISRC

        /// <summary>
        /// International Standard Book Number.
        /// </summary>
        public string ISBN { get; set; } //ISBN

        /// <summary>
        /// EAN-13 (13-digit European Article Numbering) or UPC-A (12-digit Universal Product Code) bar code identifier.
        /// </summary>
        public string BarCode { get; set; } //BarCode

        /// <summary>
        /// Library of Congress Control Number.
        /// </summary>
        public string LCCN { get; set; } //LCCN

        /// <summary>
        /// A label-specific catalogue number used to identify the release. e.g. TIC 01.
        /// </summary>
        public string CatalogNumber { get; set; } //CatalogNumber

        /// <summary>
        /// A 4-digit or 5-digit number to identify the record label, typically printed as (LC) xxxx or (LC) 0xxxx on CDs medias or covers, with only the number being stored.
        /// </summary>
        public string LabelCode { get; set; } //LabelCode

        /// <summary>
        /// Owner of the file.
        /// </summary>
        public string Owner { get; set; } //Owner

        /// <summary>
        /// Copyright attribution.
        /// </summary>
        public string Copyright { get; set; } //Copyright

        /// <summary>
        /// Link to a site with copyright/legal information.
        /// </summary>
        public string CopyrightUrl { get; set; } //Copyright/Url

        /// <summary>
        /// The copyright information as per the productioncopyright holder.
        /// </summary>
        public string ProducerCopyright { get; set; } //Producer_Copyright

        /// <summary>
        /// License information, e.g., All Rights Reserved,Any Use Permitted.
        /// </summary>
        public string TermsOfUse { get; set; } //TermsOfUse

        /// <summary>
        /// Is there a cover.
        /// </summary>
        public bool? Cover { get; set; } //Cover

        /// <summary>
        /// short descriptio, e.g. Earth in space.
        /// </summary>
        public List<string> CoverDescription { get; set; } = new List<string> { }; //Cover_Description

        /// <summary>
        /// Cover_Mime
        /// </summary>
        public List<string> CoverType { get; set; } = new List<string> { }; //Cover_Mime

        /// <summary>
        /// Cover, in binary format encoded BASE64.
        /// </summary>
        public List<byte[]> CoverData { get; set; } = new List<byte[]> { }; //Cover_Data

        /// <summary>
        /// Text of a song.
        /// </summary>
        public string Lyrics { get; set; } //Lyrics

        /// <summary>
        /// Any comment related to the content.
        /// </summary>
        public string Comment { get; set; } //Comment

        /// <summary>
        /// A numeric value defining how much a person likes the song/movie. The number is between 0 and 5 with decimal values possible (e.g. 2.7), 5(.0) being the highest possible rating.
        /// </summary>
        public double? Rating { get; set; } //Rating

        /// <summary>
        /// Date/year the item was added to the owners collection.
        /// </summary>
        public string AddedDate { get; set; } //Added_Date

        /// <summary>
        /// The date, the owner first played an item.
        /// </summary>
        public string PlayedFirstDate { get; set; } //Played_First_Date

        /// <summary>
        /// The date, the owner last played an item.
        /// </summary>
        public string PlayedLastDate { get; set; } //Played_Last_Date

        /// <summary>
        /// Number of times an item was played.
        /// </summary>
        public int? PlayedCount { get; set; } //Played_Count
        #endregion
    }
}
