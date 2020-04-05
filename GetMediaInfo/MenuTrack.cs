using System;
using System.Collections.Generic;
using GetMediaInfoDDLV2;

namespace GetMediaInfo
{
    /// <summary>
    /// Contains properties for a MenuTrack
    /// </summary>
    public class MenuTrack
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
        ///<summary>  Raw data of string </summary>
        internal string Inform { get; set; }
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
        ///<summary>  Compression method used </summary>
        public string FormatCompression { get; set; }
        ///<summary>  Settings needed for decoder used </summary>
        public string FormatSettings { get; set; }
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
        ///<summary>  List of programs available </summary>
        public ulong? ListStreamKind { get; set; }
        ///<summary>  List of programs available </summary>
        public ulong? ListStreamPos { get; set; }
        ///<summary>  List of programs available </summary>
        public ulong? List { get; set; }
        ///<summary>  List of programs available </summary>
        public string ListString { get; set; }
        ///<summary>  Name of this menu </summary>
        public string Title { get; set; }
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
        public string Countries { get; set; }
        ///<summary>   </summary>
        public string TimeZones { get; set; }
        ///<summary>  Depending on the country it's the format of the rating of a movie (P, R, X in the USA, an age in other countries or a URI defining a logo). </summary>
        public int? LawRating { get; set; }
        ///<summary>  Reason for the law rating </summary>
        public string LawRatingReason { get; set; }
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
        ///<summary>  Used by third-party developers to know about the beginning of the chapters list, to be used by Get(Stream_Menu, x, Pos), where Pos is an Integer between Chapters_Pos_Begin and Chapters_Pos_End </summary>
        public string ChaptersPosBegin { get; set; }
        ///<summary>  Used by third-party developers to know about the end of the chapters list (this position excluded) </summary>
        public string ChaptersPosEnd { get; set; }



        /// <summary>
        /// Chapters List.
        /// </summary>
        public List<ChapterTrack> Chapters { get; set; } = new List<ChapterTrack>();

        ///<summary> ChaptersCount of objects available in this stream </summary>
        public int ChaptersCount { get; set; }        
        ///<summary> The unprocessed raw data from mediainfo </summary>
        public string RawData
        {
            get => Inform;
            set => Inform = value;
        }
    }
}
