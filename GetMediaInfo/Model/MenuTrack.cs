using System;
using System.Collections.Generic;

namespace GetMediaInfo.Model
{
    /// <summary>
    /// Contains properties for a MenuTrack
    /// </summary>
    public class MenuTrack
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
        /// Compression method used.
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
        /// Codec ID (found in some containers).
        /// </summary>
        public int? CodecID{ get; set; } //CodecID

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
        /// ChaptersCount of objects available in this stream
        /// </summary>
        public int? ChaptersCount { get; set; }

        /// <summary>
        /// Chapters List.
        /// </summary>
        public List<ChapterTrack> Chapters { get; set; }

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
        /// Duration Start
        /// </summary>
        public int? DurationStart { get; set; } //Duration_Start //TOCHECK

        /// <summary>
        /// Duration End
        /// </summary>
        public int? DurationEnd { get; set; } //Duration_End //TOCHECK

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
        /// List of Streams' Kinds.
        /// </summary>
        public string ListStreamKind { get; set; } //List_StreamKind

        /// <summary>
        /// List of Streams' Positions.
        /// </summary>
        public string ListStreamPos { get; set; } //List_StreamPos

        /// <summary>
        /// List of programs available.
        /// </summary>
        public string List { get; set; } //List

        /// <summary>
        /// Name of this menu.
        /// </summary>
        public string Title { get; set; } //Title

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
        /// ServiceName
        /// </summary>
        public string ServiceName { get; set; } //ServiceName

        /// <summary>
        /// ServiceChannel
        /// </summary>
        public string ServiceChannel { get; set; } //ServiceChannel

        /// <summary>
        /// Service/Url
        /// </summary>
        public string ServiceUrl { get; set; } //Service/Url

        /// <summary>
        /// ServiceProvider
        /// </summary>
        public string ServiceProvider { get; set; } //ServiceProvider

        /// <summary>
        /// ServiceProvider/Url
        /// </summary>
        public string ServiceProviderUrl { get; set; } //ServiceProvider/Url

        /// <summary>
        /// ServiceType
        /// </summary>
        public string ServiceType { get; set; } //ServiceType

        /// <summary>
        /// NetworkName
        /// </summary>
        public string NetworkName { get; set; } //NetworkName

        /// <summary>
        /// Original/NetworkName
        /// </summary>
        public string OriginalNetworkName { get; set; } //Original/NetworkName

        /// <summary>
        /// Countries
        /// </summary>
        public string Countries { get; set; } //Countries

        /// <summary>
        /// TimeZones
        /// </summary>
        public string TimeZones { get; set; } //TimeZones

        /// <summary>
        /// Depending on the country it's the format of the rating of a movie (P, R, X in the USA, an age in other countries or a URI defining a logo).
        /// </summary>
        public string LawRating { get; set; } //LawRating

        /// <summary>
        /// Reason for the law rating.
        /// </summary>
        public string LawRatingReason { get; set; } //LawRating_Reason

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
        #endregion
    }
}
