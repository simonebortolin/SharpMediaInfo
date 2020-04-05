using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using GetMediaInfo.Exceptions;
using System.Text;

namespace GetMediaInfo
{
    /// <summary>
    /// When called with a proper file target, returns a MediaInfo object filled with list of media tracks containing
    /// every information MediaInfo.dll can collect.
    /// Tracks are accessibles as properties.
    /// </summary>
    public class MediaInfo : IDisposable
    {
        #region Global variables
        /// <summary>
        /// The handle to call mediainfo functions
        /// </summary>
        private IntPtr Handle;

        /// <summary>
        /// The path of the file that it's analyzed.
        /// </summary>
        private string _FileName;
        /// <summary>
        /// The flag to determine if the cover data must be parsed or not.
        /// </summary>
        private bool _ParseCoverData;

        #region Information
        /// <summary>
        /// The list of general streams
        /// </summary>
        private List<GeneralTrack> _General;
        /// <summary>
        /// The list of video streams
        /// </summary>
        private List<VideoTrack> _Video;
        /// <summary>
        /// The list of audio streams
        /// </summary>
        private List<AudioTrack> _Audio;
        /// <summary>
        /// The list of text streams
        /// </summary>
        private List<TextTrack> _Text;
        /// <summary>
        /// The list of image streams
        /// </summary>
        private List<ImageTrack> _Image;
        /// <summary>
        /// The list of menu streams
        /// </summary>
        private List<MenuTrack> _Menu;
        /// <summary>
        /// The general streams count
        /// </summary>
        private int _GeneralCount;
        /// <summary>
        /// The video streams count
        /// </summary>
        private int _VideoCount;
        /// <summary>
        /// The audio streams count
        /// </summary>
        private int _AudioCount;
        /// <summary>
        /// The text streams count
        /// </summary>
        private int _TextCount;
        /// <summary>
        /// The image streams count
        /// </summary>
        private int _ImageCount;
        /// <summary>
        /// The menu streams count
        /// </summary>
        private int _MenuCount;
        /// <summary>
        /// The complete information from Mediainfo
        /// </summary>
        private string _InfoComplete;
        /// <summary>
        /// The standard information from Mediainfo
        /// </summary>
        private string _InfoStandard;
        /// <summary>
        /// The custom information from Mediainfo
        /// </summary>
        private string _InfoCustom;
        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new MediaInfo object instance by copying all information from the input MediaInfo object.
        /// </summary>
        /// <param name="mediaInfo">The MediaInfo object</param>
        public MediaInfo(MediaInfo mediaInfo)
        {
            this._General = new List<GeneralTrack>(mediaInfo._General);
            this._GeneralCount = mediaInfo._GeneralCount;
            this._Video = new List<VideoTrack>(mediaInfo._Video);
            this._VideoCount = mediaInfo._VideoCount;
            this._Audio = new List<AudioTrack>(mediaInfo._Audio);
            this._AudioCount = mediaInfo._AudioCount;
            this._Text = new List<TextTrack>(mediaInfo._Text);
            this._TextCount = mediaInfo._TextCount;
            this._Image = new List<ImageTrack>(mediaInfo._Image);
            this._ImageCount = mediaInfo._ImageCount;
            this._Menu = new List<MenuTrack>(mediaInfo._Menu);
            this._GeneralCount = mediaInfo._GeneralCount;

            this._InfoComplete = mediaInfo._InfoComplete;
            this._InfoStandard = mediaInfo._InfoStandard;
            this._InfoCustom = mediaInfo._InfoCustom;

            this._FileName = mediaInfo._FileName;
            this._ParseCoverData = mediaInfo._ParseCoverData;
        }

        /// <summary>
        /// Creates a new Mediainfo object instance filled with all parsed information from the specified file.
        /// </summary>
        /// <param name="path">The path of the file to analyze.</param>
        /// <param name="parseCoverData">If set to <c>true</c> all covers data are parsed. (default: false)</param>
        /// <param name="parseReports">If set to <c>true</c> both specific and complete reports are immediately parsed. (default: false)</param>
        public MediaInfo(string path, bool parseCoverData = false, bool parseReports = false)
        {
            try
            {
#if DEBUG
                var sw = System.Diagnostics.Stopwatch.StartNew();
#endif

                OpenFile(path);
#if DEBUG
                sw.Stop();
                System.Diagnostics.Debug.WriteLine(string.Format("GetMediainfo handle openining time: {0} ms", sw.ElapsedMilliseconds));
                sw.Restart();
#endif     

                RetrieveInformation(parseReports, parseCoverData);

#if DEBUG
                sw.Stop();
                System.Diagnostics.Debug.WriteLine(string.Format("GetMediaInfo data parsing time: {0} ms", sw.ElapsedMilliseconds));
#endif     

            }
            catch (Exception ex)
            {
                throw new MediaInfoAnalysisException("An error occurred while trying to analyze the file.", ex);
            }
            finally
            {
                MediaInfoLib.CloseFile(this.Handle);
            }
        }

        /// <summary>
        /// Creates a new MediaInfo object instance filled with all parsed information from the specified stream.
        /// </summary>
        /// <param name="stream">The stream that contains the file content to analyze.</param>
        /// <param name="parseCoverData">If set to <c>true</c> all covers data are parsed. (default: false)</param>
        /// <param name="parseReports">If set to <c>true</c> both specific and complete reports are immediately parsed. (default: false)</param>
        public MediaInfo(Stream stream, bool parseCoverData = false, bool parseReports = false)
        {
            try
            {
#if DEBUG
                var sw = System.Diagnostics.Stopwatch.StartNew();
#endif

                OpenStream(stream);

#if DEBUG
                sw.Stop();
                System.Diagnostics.Debug.WriteLine(string.Format("GetMediainfo handle openining time: {0} ms", sw.ElapsedMilliseconds));
                sw.Restart();
#endif     

                RetrieveInformation(parseReports, parseCoverData);

                //check if mediainfo can get filename
                this._FileName = General.Count > 0 && !string.IsNullOrEmpty(General[0].FileNameExtension) ? General[0].FileNameExtension : "stream";

#if DEBUG
                sw.Stop();
                System.Diagnostics.Debug.WriteLine(string.Format("GetMediaInfo data parsing time: {0} ms", sw.ElapsedMilliseconds));
#endif     

            } catch (Exception ex)
            {
                throw new MediaInfoAnalysisException("An error occurred while trying to analyze input stream.", ex);
            }
            finally
            {
                MediaInfoLib.CloseFile(this.Handle);
            }
        }

        #region Helpers
        /// <summary>
        /// Opens a file.
        /// </summary>
        /// <param name="path">The path of the file</param>
        private void OpenFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified file could not be found.");
            }

            this._FileName = path;

            //creates a new handle
            this.Handle = MediaInfoLib.CreateHandle();

            //opens the handle
            if (!MediaInfoLib.OpenFile(this.Handle, path))
            {
                throw new MediainfoOpenException("Unable to open the file.");
            }
        }

        /// <summary>
        /// Opens a stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        private void OpenStream(Stream stream)
        {
            if (!stream.CanRead || !stream.CanSeek || stream.Length <= 0)
            {
                throw new ArgumentException("The stream cannot be read or it's empty");
            }

            this._FileName = string.Empty;

            //get data from stream
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                var rawDataHandle = GCHandle.Alloc(memoryStream.GetBuffer(), GCHandleType.Pinned);
                IntPtr pointer = rawDataHandle.AddrOfPinnedObject();

                //creates a new handle
                this.Handle = MediaInfoLib.CreateHandle();

                //opens the buffer
                if (!MediaInfoLib.InitializeBuffer(this.Handle, stream.Length, 0))
                {
                    throw new MediainfoOpenException("Unable to open the stream.");
                }
                MediaInfoLib.ReadBuffer(this.Handle, pointer, (UIntPtr)memoryStream.Length);
                if (!MediaInfoLib.FinalizeBuffer(this.Handle))
                {
                    throw new MediainfoFinalizeException("Unable to finalize the stream.");
                }
            }
        }

        /// <summary>
        /// Retrieves all informations from Mediainfo library.
        /// </summary>
        /// <param name="parseReports">Flag to determine whether to parse reports or not.</param>
        /// <param name="parseCoverData">Flag to determine whether to parse cover data or not.</param>
        private void RetrieveInformation(bool parseReports, bool parseCoverData)
        {
            this._ParseCoverData = parseCoverData;
            //forces to parse cover data
            EnableCoverDataParsing(parseCoverData);

            //retrieve all information
            GetStreamCount();
            ParseInformation();

            if (parseReports)
            {
                GetCompleteReport();
                GetStandardReport();
            }
        }
        #endregion

        #endregion

        #region Parsing

        /// <summary>
        /// Gets all infos from the input file.
        /// </summary>
        private void ParseInformation()
        {
            GetGeneralInfo();
            GetVideoInfo();
            GetAudioInfo();
            GetTextInfo();
            GetImageInfo();
            GetMenuInfo();
        }

        #region General information
        /// <summary>
        /// Gets all stream types count.
        /// </summary>
        private void GetStreamCount()
        {
            _AudioCount = MediaInfoLib.GetStreamCount(this.Handle, StreamKind.Audio, -1);
            _VideoCount = MediaInfoLib.GetStreamCount(this.Handle, StreamKind.Video, -1);
            _GeneralCount = MediaInfoLib.GetStreamCount(this.Handle, StreamKind.General, -1);
            _TextCount = MediaInfoLib.GetStreamCount(this.Handle, StreamKind.Text, -1);
            _ImageCount = MediaInfoLib.GetStreamCount(this.Handle, StreamKind.Image, -1);
            _MenuCount = MediaInfoLib.GetStreamCount(this.Handle, StreamKind.Menu, -1);
        }

        /// <summary>
        /// Gets the complete report.
        /// </summary>
        private void GetCompleteReport()
        {
            MediaInfoLib.SetOption(this.Handle, "Complete", "1");
            EnableCoverDataParsing(false);
            _InfoComplete = Marshal.PtrToStringUni(MediaInfoLib.Inform(this.Handle, 0));
            EnableCoverDataParsing(true);
        }

        /// <summary>
        /// Gets the standard report.
        /// </summary>
        private void GetStandardReport()
        {
            MediaInfoLib.SetOption(this.Handle, "Complete", "");
            EnableCoverDataParsing(false);
            _InfoStandard = Marshal.PtrToStringUni(MediaInfoLib.Inform(this.Handle, 0));
            EnableCoverDataParsing(true);
        }
        #endregion

        #region Track information
        /// <summary>
        /// Gets the informations of all general streams.
        /// </summary>
        private void GetGeneralInfo()
        {
            if (this._General == null)
            {
                this._General = new List<GeneralTrack>();

                for (int index = 0; index < _GeneralCount; index++)
                {
                    GeneralTrack tmp = new GeneralTrack
                    {
                        Count = ParseInt(GetParameter(StreamKind.General, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.General, index, "Status");
                        StreamCount = ParseInt(GetParameter(StreamKind.General, index, "StreamCount")),
                        StreamKind = GetParameter(StreamKind.General, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetParameter(StreamKind.General, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetParameter(StreamKind.General, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetParameter(StreamKind.General, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.General, index, "FirstPacketOrder");
                        Inform = GetParameter(StreamKind.General, index, "Inform"),
                        ID = ParseInt(GetParameter(StreamKind.General, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.General, index, "ID/String");
                        OriginalSourceMediumID = GetParameter(StreamKind.General, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.General, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetParameter(StreamKind.General, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.General, index, "UniqueID/String");
                        //string MenuID = getSpecificInfo(MediaInfoStreamKind.General, index, "MenuID");
                        //string MenuIDString = getSpecificInfo(MediaInfoStreamKind.General, index, "MenuID/String");

                        GeneralCount = _GeneralCount,
                        VideoCount = _VideoCount,
                        AudioCount = _AudioCount,
                        TextCount = _TextCount,
                        //string OtherCount = getSpecificInfo(MediaInfoStreamKind.General, index, "OtherCount");
                        ImageCount = _ImageCount,
                        MenuCount = _MenuCount,

                        VideoFormatList = ParseList(GetParameter(StreamKind.General, index, "Video_Format_List"), "/"),
                        //string VideoFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Video_Format_WithHint_List");
                        VideoLanguageList = ParseList(GetParameter(StreamKind.General, index, "Video_Language_List"), "/"),
                        AudioFormatList = ParseList(GetParameter(StreamKind.General, index, "Audio_Format_List"), "/"),
                        //string AudioFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Audio_Format_WithHint_List");
                        AudioLanguageList = ParseList(GetParameter(StreamKind.General, index, "Audio_Language_List"), "/"),
                        TextFormatList = ParseList(GetParameter(StreamKind.General, index, "Text_Format_List"), "/"),
                        //string TextFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Text_Format_WithHint_List");
                        TextLanguageList = ParseList(GetParameter(StreamKind.General, index, "Text_Language_List"), "/"),
                        //string OtherFormatList = getSpecificInfo(MediaInfoStreamKind.General, index, "Other_Format_List");
                        //string OtherFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Other_Format_WithHint_List");
                        //string OtherLanguageList = getSpecificInfo(MediaInfoStreamKind.General, index, "Other_Language_List");
                        ImageFormatList = ParseList(GetParameter(StreamKind.General, index, "Image_Format_List"), "/"),
                        //string ImageFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Image_Format_WithHint_List");
                        ImageLanguageList = ParseList(GetParameter(StreamKind.General, index, "Image_Language_List"), "/"),
                        MenuFormatList = ParseList(GetParameter(StreamKind.General, index, "Menu_Format_List"), "/"),
                        //string MenuFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Menu_Format_WithHint_List");
                        MenuLanguageList = ParseList(GetParameter(StreamKind.General, index, "Menu_Language_List"), "/"),

                        CompleteName = GetParameter(StreamKind.General, index, "CompleteName"),
                        FolderName = GetParameter(StreamKind.General, index, "FolderName"),
                        FileNameExtension = GetParameter(StreamKind.General, index, "FileNameExtension"),
                        FileName = GetParameter(StreamKind.General, index, "FileName"),
                        FileExtension = GetParameter(StreamKind.General, index, "FileExtension"),
                        //string CompleteNameLast = getSpecificInfo(MediaInfoStreamKind.General, index, "CompleteName_Last");
                        //string FolderNameLast = getSpecificInfo(MediaInfoStreamKind.General, index, "FolderName_Last");
                        //string FileNameExtensionLast = getSpecificInfo(MediaInfoStreamKind.General, index, "FileNameExtension_Last");
                        //string FileNameLast = getSpecificInfo(MediaInfoStreamKind.General, index, "FileName_Last");
                        //string FileExtensionLast = getSpecificInfo(MediaInfoStreamKind.General, index, "FileExtension_Last");

                        Format = GetParameter(StreamKind.General, index, "Format"),
                        FormatWithExtra = GetParameter(StreamKind.General, index, "Format/String"),
                        FormatInfo = GetParameter(StreamKind.General, index, "Format/Info"),
                        FormatUrl = GetParameter(StreamKind.General, index, "Format/Url"),
                        FormatExtensions = GetParameter(StreamKind.General, index, "Format/Extensions"),
                        FormatCommercial = GetParameter(StreamKind.General, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.General, index, "Format_Commercial_IfAny");
                        FormatVersion = GetParameter(StreamKind.General, index, "Format_Version"),
                        FormatProfile = GetParameter(StreamKind.General, index, "Format_Profile"),
                        FormatCompression = GetParameter(StreamKind.General, index, "Format_Compression"),
                        FormatSettings = GetParameter(StreamKind.General, index, "Format_Settings"),
                        FormatAdditionalFeatures = GetParameter(StreamKind.General, index, "Format_AdditionalFeatures"),
                        InternetMediaType = GetParameter(StreamKind.General, index, "InternetMediaType"),

                        CodecID = ParseInt(GetParameter(StreamKind.General, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.General, index, "CodecID/String");
                        CodecIDInfo = GetParameter(StreamKind.General, index, "CodecID/Info"),
                        CodecIDHint = GetParameter(StreamKind.General, index, "CodecID/Hint"),
                        CodecIDUrl = GetParameter(StreamKind.General, index, "CodecID/Url"),
                        CodecIDDescription = GetParameter(StreamKind.General, index, "CodecID_Description"),
                        CodecIDVersion = GetParameter(StreamKind.General, index, "CodecID_Version"),
                        CodecIDCompatible = GetParameter(StreamKind.General, index, "CodecID_Compatible"),
                        Interleaved = ParseBool(GetParameter(StreamKind.General, index, "Interleaved")),

                        FileSize = ParseULong(GetParameter(StreamKind.General, index, "FileSize")),
                        FileSizeString = GetParameter(StreamKind.General, index, "FileSize/String"),
                        //string FileSizeString1 = getSpecificInfo(MediaInfoStreamKind.General, index, "FileSize/String1");
                        FileSizeStringRounded = GetParameter(StreamKind.General, index, "FileSize/String2"),
                        //string FileSizeString3 = getSpecificInfo(MediaInfoStreamKind.General, index, "FileSize/String3");
                        FileSizeStringAccurate = GetParameter(StreamKind.General, index, "FileSize/String4"),

                        Duration = ParseInt(GetParameter(StreamKind.General, index, "Duration")),
                        DurationFormatted = GetParameter(StreamKind.General, index, "Duration/String"),
                        DurationFormattedComplete = GetParameter(StreamKind.General, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration/String2");
                        DurationTimecode = GetParameter(StreamKind.General, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration/String5");
                        //string DurationStart = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration_Start");
                        //string DurationEnd = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration_End");

                        OverallBitRateMode = GetParameter(StreamKind.General, index, "OverallBitRate_Mode"),
                        //string OverallBitRateModeString = getSpecificInfo(MediaInfoStreamKind.General, index, "OverallBitRate_Mode/String");
                        OverallBitRate = ParseULong(GetParameter(StreamKind.General, index, "OverallBitRate")),
                        OverallBitRateFormatted = GetParameter(StreamKind.General, index, "OverallBitRate/String"),
                        OverallBitRateMinimum = ParseULong(GetParameter(StreamKind.General, index, "OverallBitRate_Minimum")),
                        OverallBitRateMinimumFormatted = GetParameter(StreamKind.General, index, "OverallBitRate_Minimum/String"),
                        OverallBitRateNominal = ParseULong(GetParameter(StreamKind.General, index, "OverallBitRate_Nominal")),
                        OverallBitRateNominalFormatted = GetParameter(StreamKind.General, index, "OverallBitRate_Nominal/String"),
                        OverallBitRateMaximum = ParseULong(GetParameter(StreamKind.General, index, "OverallBitRate_Maximum")),
                        OverallBitRateMaximumFormatted = GetParameter(StreamKind.General, index, "OverallBitRate_Maximum/String"),

                        FrameRate = ParseDouble(GetParameter(StreamKind.General, index, "FrameRate")),
                        FrameRateString = GetParameter(StreamKind.General, index, "FrameRate/String"),
                        FrameRateNum = ParseInt(GetParameter(StreamKind.General, index, "FrameRate_Num")),
                        FrameRateDen = ParseInt(GetParameter(StreamKind.General, index, "FrameRate_Den")),
                        FrameCount = ParseInt(GetParameter(StreamKind.General, index, "FrameCount")),

                        Delay = ParseInt(GetParameter(StreamKind.General, index, "Delay")),
                        DelayFormatted = GetParameter(StreamKind.General, index, "Delay/String"),
                        DelayFormattedComplete = GetParameter(StreamKind.General, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.General, index, "Delay/String2");
                        DelayTimecode = GetParameter(StreamKind.General, index, "Delay/String3"),

                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.General, index, "Delay/String4"),
                        //string DelayString5 = GetSpecificInfo(MediaInfoStreamKind.General, index, "Delay/String5"),
                        DelaySettings = GetParameter(StreamKind.General, index, "Delay_Settings"),
                        DelayDropFrame = GetParameter(StreamKind.General, index, "Delay_DropFrame"),
                        DelaySource = GetParameter(StreamKind.General, index, "Delay_Source"),
                        DelaySourceFormatted = GetParameter(StreamKind.General, index, "Delay_Source/String"),

                        StreamSize = ParseULong(GetParameter(StreamKind.General, index, "StreamSize")),
                        StreamSizeFormatted = GetParameter(StreamKind.General, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize/String1"),
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize/String2"),
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize/String3"),
                        StreamSizePrecise = GetParameter(StreamKind.General, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize/String5"),
                        StreamSizeProportion = ParseDouble(GetParameter(StreamKind.General, index, "StreamSize_Proportion")),
                        //string StreamSizeDemuxed = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize_Demuxed"),
                        //string StreamSizeDemuxedString = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize_Demuxed/String"),
                        //string StreamSizeDemuxedString1 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize_Demuxed/String1"),
                        //string StreamSizeDemuxedString2 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize_Demuxed/String2"),
                        //string StreamSizeDemuxedString3 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize_Demuxed/String3"),
                        //string StreamSizeDemuxedString4 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize_Demuxed/String4"),
                        //string StreamSizeDemuxedString5 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize_Demuxed/String5"),

                        //string HeaderSize = getSpecificInfo(MediaInfoStreamKind.General, index, "HeaderSize"),
                        //string DataSize = getSpecificInfo(MediaInfoStreamKind.General, index, "DataSize"),
                        //string FooterSize = getSpecificInfo(MediaInfoStreamKind.General, index, "FooterSize"),
                        //string IsStreamable = getSpecificInfo(MediaInfoStreamKind.General, index, "IsStreamable"),
                        AlbumReplayGainGain = ParseDouble(GetParameter(StreamKind.General, index, "Album_ReplayGain_Gain")),
                        //string AlbumReplayGainGainString = getSpecificInfo(MediaInfoStreamKind.General, index, "Album_ReplayGain_Gain/String"),
                        AlbumReplayGainPeak = ParseDouble(GetParameter(StreamKind.General, index, "Album_ReplayGain_Peak")),
                        //string Encryption = getSpecificInfo(MediaInfoStreamKind.General, index, "Encryption"),
                        //string EncryptionFormat = getSpecificInfo(MediaInfoStreamKind.General, index, "Encryption_Format"),
                        //string EncryptionLength = getSpecificInfo(MediaInfoStreamKind.General, index, "Encryption_Length"),
                        //string EncryptionMethod = getSpecificInfo(MediaInfoStreamKind.General, index, "Encryption_Method"),
                        //string EncryptionMode = getSpecificInfo(MediaInfoStreamKind.General, index, "Encryption_Mode"),
                        //string EncryptionPadding = getSpecificInfo(MediaInfoStreamKind.General, index, "Encryption_Padding"),
                        //string EncryptionInitializationVector = getSpecificInfo(MediaInfoStreamKind.General, index, "Encryption_InitializationVector"),

                        //string UniversalAdID = getSpecificInfo(MediaInfoStreamKind.General, index, "UniversalAdID/String"),
                        //string UniversalAdIDRegistry = getSpecificInfo(MediaInfoStreamKind.General, index, "UniversalAdID_Registry"),
                        //string UniversalAdIDValue = getSpecificInfo(MediaInfoStreamKind.General, index, "UniversalAdID_Value"),

                        Title = GetParameter(StreamKind.General, index, "Title"),
                        TitleMore = GetParameter(StreamKind.General, index, "Title_More"),
                        TitleUrl = GetParameter(StreamKind.General, index, "Title/Url"),
                        Domain = GetParameter(StreamKind.General, index, "Domain"),
                        Collection = GetParameter(StreamKind.General, index, "Collection"),
                        Season = GetParameter(StreamKind.General, index, "Season"),
                        SeasonPosition = ParseInt(GetParameter(StreamKind.General, index, "Season_Position")),
                        SeasonPositionTotal = GetParameter(StreamKind.General, index, "Season_Position_Total"),
                        Movie = GetParameter(StreamKind.General, index, "Movie"),
                        MovieMore = GetParameter(StreamKind.General, index, "Movie_More"),
                        MovieCountry = GetParameter(StreamKind.General, index, "Movie/Country"),
                        MovieUrl = GetParameter(StreamKind.General, index, "Movie/Url"),
                        Album = GetParameter(StreamKind.General, index, "Album"),
                        //string AlbumMore = getSpecificInfo(MediaInfoStreamKind.General, index, "Album_More"),
                        //string AlbumSort = getSpecificInfo(MediaInfoStreamKind.General, index, "Album/Sort"),
                        //string AlbumPerformer = getSpecificInfo(MediaInfoStreamKind.General, index, "Album/Performer"),
                        //string AlbumPerformerSort = getSpecificInfo(MediaInfoStreamKind.General, index, "Album/Performer/Sort"),
                        AlbumPerformerUrl = GetParameter(StreamKind.General, index, "Album/Performer/Url"),
                        Comic = GetParameter(StreamKind.General, index, "Comic"),
                        //string ComicMore = getSpecificInfo(MediaInfoStreamKind.General, index, "Comic_More"),
                        //string ComicPositionTotal = getSpecificInfo(MediaInfoStreamKind.General, index, "Comic/Position_Total"),
                        Part = GetParameter(StreamKind.General, index, "Part"),
                        PartPosition = ParseInt(GetParameter(StreamKind.General, index, "Part/Position")),
                        PartPositionTotal = GetParameter(StreamKind.General, index, "Part/Position_Total"),
                        Track = GetParameter(StreamKind.General, index, "Track"),
                        //string TrackMore = getSpecificInfo(MediaInfoStreamKind.General, index, "Track_More"),
                        TrackUrl = GetParameter(StreamKind.General, index, "Track/Url"),
                        //string TrackSort = getSpecificInfo(MediaInfoStreamKind.General, index, "Track/Sort"),
                        TrackPosition = ParseInt(GetParameter(StreamKind.General, index, "Track/Position")),
                        TrackPositionTotal = GetParameter(StreamKind.General, index, "Track/Position_Total"),
                        PackageName = GetParameter(StreamKind.General, index, "PackageName"),
                        Grouping = GetParameter(StreamKind.General, index, "Grouping"),
                        Chapter = GetParameter(StreamKind.General, index, "Chapter"),
                        SubTrack = GetParameter(StreamKind.General, index, "SubTrack"),
                        OriginalAlbum = GetParameter(StreamKind.General, index, "Original/Album"),
                        OriginalMovie = GetParameter(StreamKind.General, index, "Original/Movie"),
                        OriginalPart = GetParameter(StreamKind.General, index, "Original/Part"),
                        OriginalTrack = GetParameter(StreamKind.General, index, "Original/Track"),
                        Compilation = GetParameter(StreamKind.General, index, "Compilation"),
                        CompilationString = GetParameter(StreamKind.General, index, "Compilation/String"),
                        Performer = GetParameter(StreamKind.General, index, "Performer"),
                        //string PerformerSort = getSpecificInfo(MediaInfoStreamKind.General, index, "Performer/Sort"),
                        PerformerUrl = GetParameter(StreamKind.General, index, "Performer/Url"),
                        OriginalPerformer = GetParameter(StreamKind.General, index, "Original/Performer"),
                        Accompaniment = GetParameter(StreamKind.General, index, "Accompaniment"),
                        Composer = GetParameter(StreamKind.General, index, "Composer"),
                        ComposerNationality = GetParameter(StreamKind.General, index, "Composer/Nationality"),
                        Arranger = GetParameter(StreamKind.General, index, "Arranger"),
                        Lyricist = GetParameter(StreamKind.General, index, "Lyricist"),
                        OriginalLyricist = GetParameter(StreamKind.General, index, "Original/Lyricist"),
                        Conductor = GetParameter(StreamKind.General, index, "Conductor"),
                        Director = GetParameter(StreamKind.General, index, "Director"),
                        CoDirector = GetParameter(StreamKind.General, index, "CoDirector"),
                        AssistantDirector = GetParameter(StreamKind.General, index, "AssistantDirector"),
                        DirectorOfPhotography = GetParameter(StreamKind.General, index, "DirectorOfPhotography"),
                        SoundEngineer = GetParameter(StreamKind.General, index, "SoundEngineer"),
                        ArtDirector = GetParameter(StreamKind.General, index, "ArtDirector"),
                        ProductionDesigner = GetParameter(StreamKind.General, index, "ProductionDesigner"),
                        Choreographer = GetParameter(StreamKind.General, index, "Choreographer"),
                        CostumeDesigner = GetParameter(StreamKind.General, index, "CostumeDesigner"),
                        Actor = GetParameter(StreamKind.General, index, "Actor"),
                        ActorCharacter = GetParameter(StreamKind.General, index, "Actor_Character"),
                        WrittenBy = GetParameter(StreamKind.General, index, "WrittenBy"),
                        ScreenplayBy = GetParameter(StreamKind.General, index, "ScreenplayBy"),
                        EditedBy = GetParameter(StreamKind.General, index, "EditedBy"),
                        CommissionedBy = GetParameter(StreamKind.General, index, "CommissionedBy"),
                        Producer = GetParameter(StreamKind.General, index, "Producer"),
                        CoProducer = GetParameter(StreamKind.General, index, "CoProducer"),
                        ExecutiveProducer = GetParameter(StreamKind.General, index, "ExecutiveProducer"),
                        MusicBy = GetParameter(StreamKind.General, index, "MusicBy"),
                        DistributedBy = GetParameter(StreamKind.General, index, "DistributedBy"),
                        //string OriginalSourceFormDistributor = getSpecificInfo(MediaInfoStreamKind.General, index, "OriginalSourceForm/Distributor"),
                        MasteredBy = GetParameter(StreamKind.General, index, "MasteredBy"),
                        EncodedBy = GetParameter(StreamKind.General, index, "EncodedBy"),
                        RemixedBy = GetParameter(StreamKind.General, index, "RemixedBy"),
                        ProductionStudio = GetParameter(StreamKind.General, index, "ProductionStudio"),
                        ThanksTo = GetParameter(StreamKind.General, index, "ThanksTo"),
                        Publisher = GetParameter(StreamKind.General, index, "Publisher"),
                        PublisherURL = GetParameter(StreamKind.General, index, "Publisher/URL"),
                        Label = GetParameter(StreamKind.General, index, "Label"),
                        Genre = GetParameter(StreamKind.General, index, "Genre"),
                        Mood = GetParameter(StreamKind.General, index, "Mood"),
                        ContentType = GetParameter(StreamKind.General, index, "ContentType"),
                        Subject = GetParameter(StreamKind.General, index, "Subject"),
                        Description = GetParameter(StreamKind.General, index, "Description"),
                        Keywords = GetParameter(StreamKind.General, index, "Keywords"),
                        Summary = GetParameter(StreamKind.General, index, "Summary"),
                        Synopsis = GetParameter(StreamKind.General, index, "Synopsis"),
                        Period = GetParameter(StreamKind.General, index, "Period"),
                        LawRating = GetParameter(StreamKind.General, index, "LawRating"),
                        LawRatingReason = GetParameter(StreamKind.General, index, "LawRating_Reason"),
                        ICRA = GetParameter(StreamKind.General, index, "ICRA"),
                        ReleasedDate = GetParameter(StreamKind.General, index, "Released_Date"),
                        OriginalReleasedDate = GetParameter(StreamKind.General, index, "Original/Released_Date"),
                        RecordedDate = GetParameter(StreamKind.General, index, "Recorded_Date"),
                        EncodedDate = GetParameter(StreamKind.General, index, "Encoded_Date"),
                        TaggedDate = GetParameter(StreamKind.General, index, "Tagged_Date"),
                        WrittenDate = GetParameter(StreamKind.General, index, "Written_Date"),
                        MasteredDate = GetParameter(StreamKind.General, index, "Mastered_Date"),
                        FileCreatedDate = GetParameter(StreamKind.General, index, "File_Created_Date"),
                        FileCreatedDateLocal = GetParameter(StreamKind.General, index, "File_Created_Date_Local"),
                        FileModifiedDate = GetParameter(StreamKind.General, index, "File_Modified_Date"),
                        FileModifiedDateLocal = GetParameter(StreamKind.General, index, "File_Modified_Date_Local"),
                        RecordedLocation = GetParameter(StreamKind.General, index, "Recorded_Location"),
                        WrittenLocation = GetParameter(StreamKind.General, index, "Written_Location"),
                        ArchivalLocation = GetParameter(StreamKind.General, index, "Archival_Location"),

                        EncodedApplication = GetParameter(StreamKind.General, index, "Encoded_Application"),
                        EncodedApplicationString = GetParameter(StreamKind.General, index, "Encoded_Application/Strin"),
                        EncodedApplicationCompany = GetParameter(StreamKind.General, index, "Encoded_Application_Compa"),
                        EncodedApplicationName = GetParameter(StreamKind.General, index, "Encoded_Application_Name"),
                        EncodedApplicationVersion = GetParameter(StreamKind.General, index, "Encoded_Application_Versi"),
                        EncodedApplicationUrl = GetParameter(StreamKind.General, index, "Encoded_Application_Url"),
                        EncodedLibrary = GetParameter(StreamKind.General, index, "Encoded_Library"),
                        EncodedLibraryString = GetParameter(StreamKind.General, index, "Encoded_Library/String"),
                        EncodedLibraryCompanyName = GetParameter(StreamKind.General, index, "Encoded_Library_CompanyNa"),
                        EncodedLibraryName = GetParameter(StreamKind.General, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetParameter(StreamKind.General, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetParameter(StreamKind.General, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetParameter(StreamKind.General, index, "Encoded_Library_Settings"),
                        EncodedOperatingSystem = GetParameter(StreamKind.General, index, "Encoded_OperatingSystem"),

                        Cropped = GetParameter(StreamKind.General, index, "Cropped"),
                        Dimensions = GetParameter(StreamKind.General, index, "Dimensions"),
                        DotsPerInch = GetParameter(StreamKind.General, index, "DotsPerInch"),
                        Lightness = GetParameter(StreamKind.General, index, "Lightness"),
                        OriginalSourceMedium = GetParameter(StreamKind.General, index, "OriginalSourceMedium"),
                        OriginalSourceForm = GetParameter(StreamKind.General, index, "OriginalSourceForm"),
                        OriginalSourceFormNumColour = GetParameter(StreamKind.General, index, "OriginalSourceForm/NumCol"),
                        OriginalSourceFormName = GetParameter(StreamKind.General, index, "OriginalSourceForm/Name"),
                        OriginalSourceFormCropped = GetParameter(StreamKind.General, index, "OriginalSourceForm/Croppe"),
                        OriginalSourceFormSharpness = GetParameter(StreamKind.General, index, "OriginalSourceForm/Sharpn"),
                        TaggedApplication = GetParameter(StreamKind.General, index, "Tagged_Application"),
                        BPM = ParseInt(GetParameter(StreamKind.General, index, "BPM")),
                        ISRC = GetParameter(StreamKind.General, index, "ISRC"),
                        ISBN = GetParameter(StreamKind.General, index, "ISBN"),
                        BarCode = GetParameter(StreamKind.General, index, "BarCode"),
                        LCCN = GetParameter(StreamKind.General, index, "LCCN"),
                        CatalogNumber = GetParameter(StreamKind.General, index, "CatalogNumber"),
                        LabelCode = GetParameter(StreamKind.General, index, "LabelCode"),
                        Owner = GetParameter(StreamKind.General, index, "Owner"),
                        Copyright = GetParameter(StreamKind.General, index, "Copyright"),
                        CopyrightUrl = GetParameter(StreamKind.General, index, "Copyright/Url"),
                        ProducerCopyright = GetParameter(StreamKind.General, index, "Producer_Copyright"),
                        TermsOfUse = GetParameter(StreamKind.General, index, "TermsOfUse"),

                        //string ServiceName = getSpecificInfo(MediaInfoStreamKind.General, index, "ServiceName"),
                        //string ServiceChannel = getSpecificInfo(MediaInfoStreamKind.General, index, "ServiceChannel"),
                        //string ServiceUrl = getSpecificInfo(MediaInfoStreamKind.General, index, "Service/Url"),
                        //string ServiceProvider = getSpecificInfo(MediaInfoStreamKind.General, index, "ServiceProvider"),
                        //string ServiceProviderUrl = getSpecificInfo(MediaInfoStreamKind.General, index, "ServiceProvider/Url"),
                        //string ServiceType = getSpecificInfo(MediaInfoStreamKind.General, index, "ServiceType"),
                        //string NetworkName = getSpecificInfo(MediaInfoStreamKind.General, index, "NetworkName"),
                        //string OriginalNetworkName = getSpecificInfo(MediaInfoStreamKind.General, index, "OriginalNetworkName"),
                        //string Country = getSpecificInfo(MediaInfoStreamKind.General, index, "Country"),
                        //string TimeZone = getSpecificInfo(MediaInfoStreamKind.General, index, "TimeZone"),

                        Cover = ParseBool(GetParameter(StreamKind.General, index, "Cover")),
                        CoverDescription = ParseList(GetParameter(StreamKind.General, index, "Cover_Description"), " / "),
                        CoverType = ParseList(GetParameter(StreamKind.General, index, "Cover_Type"), " / "),
                        //string CoverMime = getSpecificInfo(MediaInfoStreamKind.General, index, "Cover_Mime"),

                        Lyrics = GetParameter(StreamKind.General, index, "Lyrics"),
                        Comment = GetParameter(StreamKind.General, index, "Comment"),
                        Rating = ParseInt(GetParameter(StreamKind.General, index, "Rating")),
                        AddedDate = GetParameter(StreamKind.General, index, "Added_Date"),
                        PlayedFirstDate = GetParameter(StreamKind.General, index, "Played_First_Date"),
                        PlayedLastDate = GetParameter(StreamKind.General, index, "Played_Last_Date"),
                        PlayedCount = ParseInt(GetParameter(StreamKind.General, index, "Played_Count")),
                        //string EPGPositionsBegin = getSpecificInfo(MediaInfoStreamKind.General, index, "EPG_Positions_Begin"),
                        //string EPGPositionsEnd = getSpecificInfo(MediaInfoStreamKind.General, index, "EPG_Positions_End"),
                    };

                    //get cover data if available
                    if (tmp.Cover.GetValueOrDefault())
                    {
                        tmp.CoverData = ParseCoverData(GetParameter(StreamKind.General, index, "Cover_Data"));
                    }

                    //Computer overall bitrate if mediainfo return 0
                    if (tmp.OverallBitRate == 0)
                    {
                        tmp.OverallBitRate = ComputeOverallBitRate(tmp);

                        if (tmp.OverallBitRate > 0 && string.IsNullOrEmpty(tmp.OverallBitRateFormatted))
                        {
                            tmp.OverallBitRateFormatted = GetFormattedBitrate(tmp.OverallBitRate);
                        }
                    }

                    this._General.Add(tmp);
                }
            }
        }

        /// <summary>
        /// Gets the informations of all video streams
        /// </summary>
        private void GetVideoInfo()
        {
            if (this._Video == null)
            {
                this._Video = new List<VideoTrack>();

                for (int index = 0; index < _VideoCount; index++)
                {
                    VideoTrack tmp = new VideoTrack
                    {
                        Count = ParseInt(GetParameter(StreamKind.Video, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Video, index, "Status");
                        StreamCount = ParseInt(GetParameter(StreamKind.Video, index, "StreamCount")),
                        StreamKind = GetParameter(StreamKind.Video, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetParameter(StreamKind.Video, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetParameter(StreamKind.Video, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetParameter(StreamKind.Video, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Video, index, "FirstPacketOrder");
                        Inform = GetParameter(StreamKind.Video, index, "Inform"),
                        ID = ParseInt(GetParameter(StreamKind.Video, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ID/String");
                        OriginalSourceMediumID = GetParameter(StreamKind.Video, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDS = getSpecificInfo(MediaInfoStreamKind.Video, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetParameter(StreamKind.Video, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Video, index, "UniqueID/String");
                        //string MenuID = getSpecificInfo(MediaInfoStreamKind.Video, index, "MenuID");
                        //string MenuIDString = getSpecificInfo(MediaInfoStreamKind.Video, index, "MenuID/String");

                        Format = GetParameter(StreamKind.Video, index, "Format"),
                        FormatWithExtra = GetParameter(StreamKind.Video, index, "Format/String"),
                        FormatInfo = GetParameter(StreamKind.Video, index, "Format/Info"),
                        FormatUrl = GetParameter(StreamKind.Video, index, "Format/Url"),
                        FormatCommercial = GetParameter(StreamKind.Video, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Commercial_IfAny");
                        FormatLevel = GetParameter(StreamKind.Video, index, "Format_Level"),
                        FormatTier = GetParameter(StreamKind.Video, index, "Format_Tier"),
                        FormatVersion = GetParameter(StreamKind.Video, index, "Format_Version"),
                        FormatProfile = GetParameter(StreamKind.Video, index, "Format_Profile"),
                        FormatCompression = GetParameter(StreamKind.Video, index, "Format_Compression"),
                        FormatAdditionalFeatures = GetParameter(StreamKind.Video, index, "Format_AdditionalFeatures"),

                        MultiViewBaseProfile = GetParameter(StreamKind.Video, index, "MultiView_BaseProfile"),
                        MultiViewCount = ParseInt(GetParameter(StreamKind.Video, index, "MultiView_Count")),
                        MultiViewLayout = GetParameter(StreamKind.Video, index, "MultiView_Layout"),

                        HDRFormat = GetParameter(StreamKind.Video, index, "HDR_Format"), //HDR_Format
                        HDRFormatComplete = GetParameter(StreamKind.Video, index, "HDR_Format/String"), //HDR_Format/String
                        HDRFormatCommercial = GetParameter(StreamKind.Video, index, "HDR_Format_Commercial"), //HDR_Format_Commercial
                        HDRFormatVersion = GetParameter(StreamKind.Video, index, "HDR_Format_Version"), //HDR_Format_Version
                        HDRFormatLevel = GetParameter(StreamKind.Video, index, "HDR_Format_Level"), //HDR_Format_Level
                        HDRFormatSettings = GetParameter(StreamKind.Video, index, "HDR_Format_Settings"), //HDR_Format_Settings
                        HDRFormatCompatibility = GetParameter(StreamKind.Video, index, "HDR_Format_Compatibility"), //HDR_Format_Compatibility

                        FormatSettings = GetParameter(StreamKind.Video, index, "Format_Settings"),
                        FormatSettingsBVOP = GetParameter(StreamKind.Video, index, "Format_Settings_BVOP"),
                        //string FormatSettingsBVOPString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_BVOP/String");
                        //string FormatSettingsBVOPString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_BVOP/String");
                        FormatSettingsQPel = GetParameter(StreamKind.Video, index, "Format_Settings_QPel"),
                        //string FormatSettingsQPelString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_QPel/String");
                        FormatSettingsGMC = GetParameter(StreamKind.Video, index, "Format_Settings_GMC"),
                        //string FormatSettingsGMCString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_GMC/String");
                        FormatSettingsMatrix = GetParameter(StreamKind.Video, index, "Format_Settings_Matrix"),
                        //string FormatSettingsMatrixString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_Matrix/String");
                        //string FormatSettingsMatrixData = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_Matrix_Data");
                        FormatSettingsCABAC = GetParameter(StreamKind.Video, index, "Format_Settings_CABAC"),
                        //string FormatSettingsCABACString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_CABAC/String");
                        FormatSettingsRefFrames = GetParameter(StreamKind.Video, index, "Format_Settings_RefFrames"),
                        FormatSettingsPulldown = GetParameter(StreamKind.Video, index, "Format_Settings_Pulldown"),
                        FormatSettingsEndianness = GetParameter(StreamKind.Video, index, "Format_Settings_Endianness"),
                        FormatSettingsPacking = GetParameter(StreamKind.Video, index, "Format_Settings_Packing"),
                        FormatSettingsFrameMode = GetParameter(StreamKind.Video, index, "Format_Settings_FrameMode"),
                        FormatSettingsGOP = GetParameter(StreamKind.Video, index, "Format_Settings_GOP"),
                        FormatSettingsPictureString = GetParameter(StreamKind.Video, index, "Format_Settings_PictureString"),
                        FormatSettingsWrapping = GetParameter(StreamKind.Video, index, "Format_Settings_Wrapping"),
                        InternetMediaType = GetParameter(StreamKind.Video, index, "InternetMediaType"),

                        MuxingMode = GetParameter(StreamKind.Video, index, "MuxingMode"),
                        CodecID = ParseInt(GetParameter(StreamKind.Video, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Video, index, "CodecID/String");
                        CodecIDInfo = GetParameter(StreamKind.Video, index, "CodecID/Info"),
                        CodecIDHint = GetParameter(StreamKind.Video, index, "CodecID/Hint"),
                        CodecIDUrl = GetParameter(StreamKind.Video, index, "CodecID/Url"),
                        CodecIDDescription = GetParameter(StreamKind.Video, index, "CodecID_Description"),

                        Duration = ParseInt(GetParameter(StreamKind.Video, index, "Duration")),
                        DurationFormatted = GetParameter(StreamKind.Video, index, "Duration/String"),
                        DurationFormattedComplete = GetParameter(StreamKind.Video, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration/String2");
                        DurationTimecode = GetParameter(StreamKind.Video, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration/String5");
                        DurationFirstFrame = ParseInt(GetParameter(StreamKind.Video, index, "Duration_FirstFrame")),
                        DurationFirstFrameFormatted = GetParameter(StreamKind.Video, index, "Duration_FirstFrame/String"),
                        DurationFirstFrameFormattedComplete = GetParameter(StreamKind.Video, index, "Duration_FirstFrame/String1"),
                        //string DurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_FirstFrame/String2");
                        DurationFirstFrameTimecode = GetParameter(StreamKind.Video, index, "Duration_FirstFrame/String3"),
                        //string DurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_FirstFrame/String4");
                        //string DurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_FirstFrame/String5");
                        DurationLastFrame = ParseInt(GetParameter(StreamKind.Video, index, "Duration_LastFrame")),
                        DurationLastFrameFormatted = GetParameter(StreamKind.Video, index, "Duration_LastFrame/String"),
                        DurationLastFrameFormattedComplete = GetParameter(StreamKind.Video, index, "Duration_LastFrame/String1"),
                        //string DurationLastFrameString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_LastFrame/String2");
                        DurationLastFrameTimecode = GetParameter(StreamKind.Video, index, "Duration_LastFrame/String3"),
                        //string DurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_LastFrame/String4");
                        //string DurationLastFrameString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_LastFrame/String5");
                        SourceDuration = ParseInt(GetParameter(StreamKind.Video, index, "Source_Duration")),
                        SourceDurationFormatted = GetParameter(StreamKind.Video, index, "Source_Duration/String"),
                        SourceDurationFormattedComplete = GetParameter(StreamKind.Video, index, "Source_Duration/String1"),
                        //string SourceDurationString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration/String2");
                        SourceDurationTimecode = GetParameter(StreamKind.Video, index, "Source_Duration/String3"),
                        //string SourceDurationString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration/String4");
                        //string SourceDurationString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration/String5");
                        SourceDurationFirstFrame = ParseInt(GetParameter(StreamKind.Video, index, "Source_Duration_FirstFrame")),
                        SourceDurationFirstFrameFormatted = GetParameter(StreamKind.Video, index, "Source_Duration_FirstFrame/String"),
                        SourceDurationFirstFrameFormattedComplete = GetParameter(StreamKind.Video, index, "Source_Duration_FirstFrame/String1"),
                        //string SourceDurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_FirstFrame/String2");
                        SourceDurationFirstFrameTimecode = GetParameter(StreamKind.Video, index, "Source_Duration_FirstFrame/String3"),
                        //string SourceDurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_FirstFrame/String4");
                        //string SourceDurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_FirstFrame/String5");
                        SourceDurationLastFrame = ParseInt(GetParameter(StreamKind.Video, index, "Source_Duration_LastFrame")),
                        SourceDurationLastFrameFormatted = GetParameter(StreamKind.Video, index, "Source_Duration_LastFrame/String"),
                        SourceDurationLastFrameFormattedComplete = GetParameter(StreamKind.Video, index, "Source_Duration_LastFrame/String1"),
                        //string SourceDurationLastFramestring2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_LastFrame/String2");
                        SourceDurationLastFrameTimecode = GetParameter(StreamKind.Video, index, "Source_Duration_LastFrame/String3"),
                        //string SourceDurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_LastFrame/String4");
                        //string SourceDurationLastFramestring5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_LastFrame/String5");

                        BitRateMode = GetParameter(StreamKind.Video, index, "BitRate_Mode"),
                        //string BitRateModeString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Mode/String");
                        BitRate = ParseULong(GetParameter(StreamKind.Video, index, "BitRate")),
                        //string BitRateString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate/String");
                        BitRateMinimum = ParseULong(GetParameter(StreamKind.Video, index, "BitRate_Minimum")),
                        //string BitRateMinimumString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Minimum/String");
                        BitRateNominal = ParseULong(GetParameter(StreamKind.Video, index, "BitRate_Nominal")),
                        //string BitRateNominalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Nominal/String");
                        BitRateMaximum = ParseULong(GetParameter(StreamKind.Video, index, "BitRate_Maximum")),
                        //string BitRateMaximumString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Maximum/String");
                        BitRateEncoded = ParseULong(GetParameter(StreamKind.Video, index, "BitRate_Encoded")),
                        //string BitRateEncodedString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Encoded/String");

                        Width = ParseInt(GetParameter(StreamKind.Video, index, "Width")),
                        //string WidthString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Width/String");
                        WidthOffset = ParseInt(GetParameter(StreamKind.Video, index, "Width_Offset")),
                        //string WidthOffsetString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Width_Offset/String");
                        WidthOriginal = ParseInt(GetParameter(StreamKind.Video, index, "Width_Original")),
                        //string WidthOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Width_Original/String");
                        WidthCleanAperture = ParseInt(GetParameter(StreamKind.Video, index, "Width_CleanAperture")),
                        //string WidthCleanApertureString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Width_CleanAperture/String");
                        Height = ParseInt(GetParameter(StreamKind.Video, index, "Height")),
                        //string HeightString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Height/String");
                        HeightOffset = ParseInt(GetParameter(StreamKind.Video, index, "Height_Offset")),
                        //string HeightOffsetString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Height_Offset/String");
                        HeightOriginal = ParseInt(GetParameter(StreamKind.Video, index, "Height_Original")),
                        //string HeightOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Height_Original/String");
                        HeightCleanAperture = ParseInt(GetParameter(StreamKind.Video, index, "Height_CleanAperture")),
                        //string HeightCleanApertureStrig = getSpecificInfo(MediaInfoStreamKind.Video, index, "Height_CleanAperture/String");
                        StoredWidth = ParseInt(GetParameter(StreamKind.Video, index, "Stored_Width")),
                        StoredHeight = ParseInt(GetParameter(StreamKind.Video, index, "Stored_Height")),
                        SampledWidth = ParseInt(GetParameter(StreamKind.Video, index, "Sampled_Width")),
                        SampledHeight = ParseInt(GetParameter(StreamKind.Video, index, "Sampled_Height")),

                        PixelAspectRatio = ParseInt(GetParameter(StreamKind.Video, index, "PixelAspectRatio")),
                        PixelAspectRatioFormatted = GetParameter(StreamKind.Video, index, "PixelAspectRatio/String"),
                        PixelAspectRatioOriginal = ParseDouble(GetParameter(StreamKind.Video, index, "PixelAspectRatio_Original")),
                        PixelAspectRatioOriginalFormatted = GetParameter(StreamKind.Video, index, "PixelAspectRatio_Original/String"),
                        PixelAspectRatioCleanAperture = ParseDouble(GetParameter(StreamKind.Video, index, "PixelAspectRatio_CleanAperture")),
                        PixelAspectRatioCleanApertureFormatted = GetParameter(StreamKind.Video, index, "PixelAspectRatio_CleanAperture/String"),
                        DisplayAspectRatio = ParseDouble(GetParameter(StreamKind.Video, index, "DisplayAspectRatio")),
                        DisplayAspectRatioFormatted = GetParameter(StreamKind.Video, index, "DisplayAspectRatio/String"),
                        DisplayAspectRatioOriginal = ParseDouble(GetParameter(StreamKind.Video, index, "DisplayAspectRatio_Original")),
                        DisplayAspectRatioOriginalFormatted = GetParameter(StreamKind.Video, index, "DisplayAspectRatio_Original/String"),
                        DisplayAspectRatioCleanAperture = ParseDouble(GetParameter(StreamKind.Video, index, "DisplayAspectRatio_CleanAperture")),
                        DisplayAspectRatioCleanApertureFormatted = GetParameter(StreamKind.Video, index, "DisplayAspectRatio_CleanAperture/String"),
                        ActiveFormatDescription = GetParameter(StreamKind.Video, index, "ActiveFormatDescription"),
                        //string ActiveFormatDescriptionString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ActiveFormatDescription/String");
                        ActiveFormatDescriptionMuxingMode = GetParameter(StreamKind.Video, index, "ActiveFormatDescription_MuxingMode"),
                        Rotation = ParseInt(GetParameter(StreamKind.Video, index, "Rotation")),
                        //string RotationString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Rotation/String");

                        FrameRateMode = GetParameter(StreamKind.Video, index, "FrameRate_Mode"),
                        //string FrameRateModeString = getSpecificInfo(MediaInfoStreamKind.Video, index, "FrameRate_Mode/String");
                        FrameRateModeOriginal = GetParameter(StreamKind.Video, index, "FrameRate_Mode_Original"),
                        //string FrameRateModeOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "FrameRate_Mode_Original/String");
                        FrameRate = ParseDouble(GetParameter(StreamKind.Video, index, "FrameRate")),
                        FrameRateFormatted = GetParameter(StreamKind.Video, index, "FrameRate/String"),
                        FrameRateNum = ParseInt(GetParameter(StreamKind.Video, index, "FrameRate_Num")),
                        FrameRateDen = ParseInt(GetParameter(StreamKind.Video, index, "FrameRate_Den")),
                        FrameRateMinimum = ParseDouble(GetParameter(StreamKind.Video, index, "FrameRate_Minimum")),
                        FrameRateMinimumFormatted = GetParameter(StreamKind.Video, index, "FrameRate_Minimum/String"),
                        FrameRateNominal = ParseDouble(GetParameter(StreamKind.Video, index, "FrameRate_Nominal")),
                        FrameRateNominalFormatted = GetParameter(StreamKind.Video, index, "FrameRate_Nominal/String"),
                        FrameRateMaximum = ParseDouble(GetParameter(StreamKind.Video, index, "FrameRate_Maximum")),
                        FrameRateMaximumFormatted = GetParameter(StreamKind.Video, index, "FrameRate_Maximum/String"),
                        FrameRateOriginal = ParseDouble(GetParameter(StreamKind.Video, index, "FrameRate_Original")),
                        FrameRateOriginalFormatted = GetParameter(StreamKind.Video, index, "FrameRate_Original/String"),
                        FrameRateOriginalNum = ParseInt(GetParameter(StreamKind.Video, index, "FrameRate_Original_Num")),
                        FrameRateOriginalDen = ParseInt(GetParameter(StreamKind.Video, index, "FrameRate_Original_Den")),
                        FrameCount = ParseInt(GetParameter(StreamKind.Video, index, "FrameCount")),
                        SourceFrameCount = ParseInt(GetParameter(StreamKind.Video, index, "Source_FrameCount")),

                        Standard = GetParameter(StreamKind.Video, index, "Standard"),
                        ColorSpace = GetParameter(StreamKind.Video, index, "ColorSpace"),
                        ChromaSubsampling = GetParameter(StreamKind.Video, index, "ChromaSubsampling"),
                        //string ChromaSubsamplingString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ChromaSubsampling/String");
                        ChromaSubsamplingPosition = ParseInt(GetParameter(StreamKind.Video, index, "ChromaSubsampling_Position")),
                        BitDepth = ParseInt(GetParameter(StreamKind.Video, index, "BitDepth")),
                        BitDepthFormatted = GetParameter(StreamKind.Video, index, "BitDepth/String"),
                        ScanType = GetParameter(StreamKind.Video, index, "ScanType"),
                        //string ScanTypeString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanType/String");
                        ScanTypeOriginal = GetParameter(StreamKind.Video, index, "ScanType_Original"),
                        //string ScanTypeOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanType_Original/String");
                        ScanTypeStoreMethod = GetParameter(StreamKind.Video, index, "ScanType_StoreMethod"),
                        ScanTypeStoreMethodFieldsPerBlock = GetParameter(StreamKind.Video, index, "ScanType_StoreMethod_FieldsPerBlock"),
                        //string ScanTypeStoreMethodString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanType_StoreMethod/String");
                        ScanOrder = GetParameter(StreamKind.Video, index, "ScanOrder"),
                        //string ScanOrderString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanOrder/String");
                        ScanOrderStored = GetParameter(StreamKind.Video, index, "ScanOrder_Stored"),
                        //string ScanOrderStoredString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanOrder_Stored/String");
                        ScanOrderStoredDisplayedInverted = GetParameter(StreamKind.Video, index, "ScanOrder_StoredDisplayedInverted"),
                        ScanOrderOriginal = GetParameter(StreamKind.Video, index, "ScanOrder_Original"),
                        //string ScanOrderOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanOrder_Original/String");
                        CompressionMode = GetParameter(StreamKind.Video, index, "Compression_Mode"),
                        //string CompressionModeString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Compression_Mode/String");
                        CompressionRatio = ParseDouble(GetParameter(StreamKind.Video, index, "Compression_Ratio")),
                        BitsPixelFrame = ParseDouble(GetParameter(StreamKind.Video, index, "Bits-(Pixel*Frame)")),

                        Delay = ParseInt(GetParameter(StreamKind.Video, index, "Delay")),
                        DelayFormatted = GetParameter(StreamKind.Video, index, "Delay/String"),
                        DelayFormattedComplete = GetParameter(StreamKind.Video, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay/String2");
                        DelayTimecode = GetParameter(StreamKind.Video, index, "Delay/String3"),
                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay/String4");
                        //string DelayString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay/String5");
                        DelaySettings = GetParameter(StreamKind.Video, index, "Delay_Settings"),
                        DelayDropFrame = GetParameter(StreamKind.Video, index, "Delay_DropFrame"),
                        DelaySource = GetParameter(StreamKind.Video, index, "Delay_Source"),
                        //string DelaySourceString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay_Source/String");
                        DelayOriginal = ParseInt(GetParameter(StreamKind.Video, index, "Delay_Original")),
                        DelayOriginalFormatted = GetParameter(StreamKind.Video, index, "Delay_Original/String"),
                        DelayOriginalFormattedComplete = GetParameter(StreamKind.Video, index, "Delay_Original/String1"),
                        //string DelayOriginalString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay_Original/String2");
                        DelayOriginalTimecode = GetParameter(StreamKind.Video, index, "Delay_Original/String3"),
                        //string DelayOriginalString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay_Original/String4");
                        //string DelayOriginalString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay_Original/String5");
                        DelayOriginalSettings = GetParameter(StreamKind.Video, index, "Delay_Original_Settings"),
                        DelayOriginalDropFrame = GetParameter(StreamKind.Video, index, "Delay_Original_DropFrame"),
                        DelayOriginalSource = GetParameter(StreamKind.Video, index, "Delay_Original_Source"),

                        TimeStampFirstFrame = ParseInt(GetParameter(StreamKind.Video, index, "TimeStamp_FirstFrame")),
                        TimeStampFirstFrameFormatted = GetParameter(StreamKind.Video, index, "TimeStamp_FirstFrame/String"),
                        TimeStampFirstFrameFormattedComplete = GetParameter(StreamKind.Video, index, "TimeStamp_FirstFrame/String1"),
                        //string TimeStampFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "TimeStamp_FirstFrame/String2");
                        TimeStampFirstFrameTimecode = GetParameter(StreamKind.Video, index, "TimeStamp_FirstFrame/String3"),
                        //string TimeStampFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "TimeStamp_FirstFrame/String4");
                        //string TimeStampFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "TimeStamp_FirstFrame/String5");
                        TimeCodeFirstFrame = GetParameter(StreamKind.Video, index, "TimeCode_FirstFrame"),
                        TimeCodeSettings = GetParameter(StreamKind.Video, index, "TimeCode_Settings"),
                        TimeCodeSource = GetParameter(StreamKind.Video, index, "TimeCode_Source"),
                        GopOpenClosed = GetParameter(StreamKind.Video, index, "Gop_OpenClosed"),
                        //string GopOpenClosedString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Gop_OpenClosed/String");
                        GopOpenClosedFirstFrame = GetParameter(StreamKind.Video, index, "Gop_OpenClosed_FirstFrame"),
                        //string GopOpenClosedFirstFrameString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Gop_OpenClosed_FirstFrame/String");

                        StreamSize = ParseULong(GetParameter(StreamKind.Video, index, "StreamSize")),
                        StreamSizeFormatted = GetParameter(StreamKind.Video, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize/String1");
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize/String2");
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize/String3");
                        StreamSizePrecise = GetParameter(StreamKind.Video, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize/String5");
                        StreamSizeProportion = ParseDouble(GetParameter(StreamKind.Video, index, "StreamSize_Proportion")),
                        //string StreamSizeDemuxed = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed");
                        //string StreamSizeDemuxedString = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String");
                        //string StreamSizeDemuxedString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String1");
                        //string StreamSizeDemuxedString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String2");
                        //string StreamSizeDemuxedString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String3");
                        //string StreamSizeDemuxedString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String4");
                        //string StreamSizeDemuxedString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String5");
                        SourceStreamSize = ParseULong(GetParameter(StreamKind.Video, index, "Source_StreamSize")),
                        SourceStreamSizeFormatted = GetParameter(StreamKind.Video, index, "Source_StreamSize/String"),
                        //string SourceStreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize/String1");
                        //string SourceStreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize/String2");
                        //string SourceStreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize/String3");
                        SourceStreamSizePrecise = GetParameter(StreamKind.Video, index, "Source_StreamSize/String4"),
                        //string SourceStreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize/String5");
                        SourceStreamSizeProportion = GetParameter(StreamKind.Video, index, "Source_StreamSize_Proportion"),
                        StreamSizeEncoded = ParseULong(GetParameter(StreamKind.Video, index, "StreamSize_Encoded")),
                        StreamSizeEncodedFormatted = GetParameter(StreamKind.Video, index, "StreamSize_Encoded/String"),
                        //string StreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Encoded/String1");
                        //string StreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Encoded/String2");
                        //string StreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Encoded/String3");
                        StreamSizeEncodedPrecise = GetParameter(StreamKind.Video, index, "StreamSize_Encoded/String4"),
                        //string StreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Encoded/String5");
                        StreamSizeEncodedProportion = ParseDouble(GetParameter(StreamKind.Video, index, "StreamSize_Encoded_Proportion")),
                        SourceStreamSizeEncoded = ParseULong(GetParameter(StreamKind.Video, index, "Source_StreamSize_Encoded")),
                        SourceStreamSizeEncodedFormatted = GetParameter(StreamKind.Video, index, "Source_StreamSize_Encoded/String"),
                        //string SourceStreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize_Encoded/String1");
                        //string SourceStreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize_Encoded/String2");
                        //string SourceStreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize_Encoded/String3");
                        SourceStreamSizeEncodedPrecise = GetParameter(StreamKind.Video, index, "Source_StreamSize_Encoded/String4"),
                        //string SourceStreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize_Encoded/String5");
                        SourceStreamSizeEncodedProportion = ParseDouble(GetParameter(StreamKind.Video, index, "Source_StreamSize_Encoded_Proportion")),

                        Alignment = GetParameter(StreamKind.Video, index, "Alignment"),
                        //string AlignmentString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Alignment/String");
                        Title = GetParameter(StreamKind.Video, index, "Title"),
                        EncodedApplication = GetParameter(StreamKind.Video, index, "Encoded_Application"),
                        //string EncodedApplicationString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Encoded_Application/String");
                        //string EncodedApplicationCompany = getSpecificInfo(MediaInfoStreamKind.Video, index, "Encoded_Application_Company");
                        EncodedApplicationName = GetParameter(StreamKind.Video, index, "Encoded_Application_Name"),
                        //string EncodedApplicationVersion = getSpecificInfo(MediaInfoStreamKind.Video, index, "Encoded_Application_Version");
                        EncodedApplicationUrl = GetParameter(StreamKind.Video, index, "Encoded_Application_Url"),
                        EncodedLibrary = GetParameter(StreamKind.Video, index, "Encoded_Library"),
                        //string EncodedLibraryString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Encoded_Library/String");
                        EncodedLibraryCompanyName = GetParameter(StreamKind.Video, index, "Encoded_Library_CompanyName"),
                        EncodedLibraryName = GetParameter(StreamKind.Video, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetParameter(StreamKind.Video, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetParameter(StreamKind.Video, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetParameter(StreamKind.Video, index, "Encoded_Library_Settings"),
                        EncodedOperatingSystem = GetParameter(StreamKind.Video, index, "Encoded_OperatingSystem"),

                        Language = GetParameter(StreamKind.Video, index, "Language"),
                        LanguageFull = GetParameter(StreamKind.Video, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Language/String1");
                        Language2Letter = GetParameter(StreamKind.Video, index, "Language/String2"),
                        Language3Letter = GetParameter(StreamKind.Video, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Language/String4");
                        LanguageMore = GetParameter(StreamKind.Video, index, "Language_More"),

                        ServiceKind = GetParameter(StreamKind.Video, index, "ServiceKind"),
                        Disabled = ParseBool(GetParameter(StreamKind.Video, index, "Disabled")),
                        DisabledString = GetParameter(StreamKind.Video, index, "Disabled/String"),
                        Default = ParseBool(GetParameter(StreamKind.Video, index, "Default")),
                        DefaultString = GetParameter(StreamKind.Video, index, "Default/String"),
                        Forced = ParseBool(GetParameter(StreamKind.Video, index, "Forced")),
                        ForcedString = GetParameter(StreamKind.Video, index, "Forced/String"),

                        AlternateGroup = ParseInt(GetParameter(StreamKind.Video, index, "AlternateGroup")),
                        //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Video, index, "AlternateGroup/String");
                        EncodedDate = GetParameter(StreamKind.Video, index, "Encoded_Date"),
                        TaggedDate = GetParameter(StreamKind.Video, index, "Tagged_Date"),
                        Encryption = GetParameter(StreamKind.Video, index, "Encryption"),
                        BufferSize = GetParameter(StreamKind.Video, index, "BufferSize"),

                        ColourDescriptionPresent = GetParameter(StreamKind.Video, index, "colour_description_present"),
                        ColourDescriptionPresentSource = GetParameter(StreamKind.Video, index, "colour_description_present_Source"),
                        ColourDescriptionPresentOriginal = GetParameter(StreamKind.Video, index, "colour_description_present_Original"),
                        ColourDescriptionPresentOriginalSource = GetParameter(StreamKind.Video, index, "colour_description_present_Original_Source"),

                        ColourRange = GetParameter(StreamKind.Video, index, "colour_range"),
                        ColourRangeSource = GetParameter(StreamKind.Video, index, "colour_range_Source"),
                        ColourRangeOriginal = GetParameter(StreamKind.Video, index, "colour_range_Original"),
                        ColourRangeOriginalSource = GetParameter(StreamKind.Video, index, "colour_range_Original_Source"),

                        ColourPrimaries = GetParameter(StreamKind.Video, index, "colour_primaries"),
                        ColourPrimariesSource = GetParameter(StreamKind.Video, index, "colour_primaries_Source"),
                        ColourPrimariesOriginal = GetParameter(StreamKind.Video, index, "colour_primaries_Original"),
                        ColourPrimariesOriginalSource = GetParameter(StreamKind.Video, index, "colour_primaries_Original_Source"),

                        TransferCharacteristics = GetParameter(StreamKind.Video, index, "transfer_characteristics"),
                        TransferCharacteristicsSource = GetParameter(StreamKind.Video, index, "transfer_characteristics_Source"),
                        TransferCharacteristicsOriginal = GetParameter(StreamKind.Video, index, "transfer_characteristics_Original"),
                        TransferCharacteristicsOriginalSource = GetParameter(StreamKind.Video, index, "transfer_characteristics_Original_Source"),

                        MatrixCoefficients = GetParameter(StreamKind.Video, index, "matrix_coefficients"),
                        MatrixCoefficientsSource = GetParameter(StreamKind.Video, index, "matrix_coefficients_Source"),
                        MatrixCoefficientsOriginal = GetParameter(StreamKind.Video, index, "matrix_coefficients_Original"),
                        MatrixCoefficientsOriginalSource = GetParameter(StreamKind.Video, index, "matrix_coefficients_Original_Source"),

                        MasteringDisplayColorPrimaries = GetParameter(StreamKind.Video, index, "MasteringDisplay_ColorPrimaries"),
                        MasteringDisplayColorPrimariesSource = GetParameter(StreamKind.Video, index, "MasteringDisplay_ColorPrimaries_Source"),
                        MasteringDisplayColorPrimariesOriginal = GetParameter(StreamKind.Video, index, "MasteringDisplay_ColorPrimaries_Original"),
                        MasteringDisplayColorPrimariesOriginalSource = GetParameter(StreamKind.Video, index, "MasteringDisplay_ColorPrimaries_Original_Source"),

                        MasteringDisplayLuminance = GetParameter(StreamKind.Video, index, "MasteringDisplay_Luminance"),
                        MasteringDisplayLuminanceSource = GetParameter(StreamKind.Video, index, "MasteringDisplay_Luminance_Source"),
                        MasteringDisplayLuminanceOriginal = GetParameter(StreamKind.Video, index, "MasteringDisplay_Luminance_Original"),
                        MasteringDisplayLuminanceOriginalSource = GetParameter(StreamKind.Video, index, "MasteringDisplay_Luminance_Original_Source"),

                        MaximumContentLightLevel = GetParameter(StreamKind.Video, index, "MaxCLL"),
                        MaximumContentLightLevelSource = GetParameter(StreamKind.Video, index, "MaxCLL_Source"),
                        MaximumContentLightLevelOriginal = GetParameter(StreamKind.Video, index, "MaxCLL_Original"),
                        MaximumContentLightLevelOriginalSource = GetParameter(StreamKind.Video, index, "MaxCLL_Original_Source"),

                        MaximumFrameAverageLightLevel = GetParameter(StreamKind.Video, index, "MaxFALL"),
                        MaximumFrameAverageLightLevelSource = GetParameter(StreamKind.Video, index, "MaxFALL_Source"),
                        MaximumFrameAverageLightLevelOriginal = GetParameter(StreamKind.Video, index, "MaxFALL_Original"),
                        MaximumFrameAverageLightLevelOriginalSource = GetParameter(StreamKind.Video, index, "MaxFALL_Original_Source"),
                    };

                    //get duration in milliseconds if mediainfo return 0
                    if (!tmp.Duration.HasValue)
                    {
                        tmp.Duration = ComputeDuration(tmp);
                    }

                    //if formatted bitrate is not present
                    if (string.IsNullOrEmpty(tmp.BitRateFormatted))
                    {
                        if (tmp.BitRate.HasValue)
                        {
                            tmp.BitRateFormatted = GetFormattedBitrate(tmp.BitRate);
                        }
                        else if (tmp.BitRateNominal.HasValue)
                        {
                            tmp.BitRateFormatted = GetFormattedBitrate(tmp.BitRateNominal);
                        }
                    }

                    this._Video.Add(tmp);
                }
            }
        }

        /// <summary>
        /// Gets the informations of all audio streams
        /// </summary>
        private void GetAudioInfo()
        {
            if (this._Audio == null)
            {
                this._Audio = new List<AudioTrack>();

                for (int index = 0; index < _AudioCount; index++)
                {
                    AudioTrack tmp = new AudioTrack
                    {
                        Count = ParseInt(GetParameter(StreamKind.Audio, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Status");
                        StreamCount = ParseInt(GetParameter(StreamKind.Audio, index, "StreamCount")),
                        StreamKind = GetParameter(StreamKind.Audio, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetParameter(StreamKind.Audio, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetParameter(StreamKind.Audio, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetParameter(StreamKind.Audio, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Audio, index, "FirstPacketOrder");
                        Inform = GetParameter(StreamKind.Audio, index, "Inform"),
                        ID = ParseInt(GetParameter(StreamKind.Audio, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "ID/String");
                        OriginalSourceMediumID = GetParameter(StreamKind.Audio, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetParameter(StreamKind.Audio, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "UniqueID/String");

                        Format = GetParameter(StreamKind.Audio, index, "Format"),
                        FormatWithExtra = GetParameter(StreamKind.Audio, index, "Format/String"),
                        FormatInfo = GetParameter(StreamKind.Audio, index, "Format/Info"),
                        FormatUrl = GetParameter(StreamKind.Audio, index, "Format/Url"),
                        FormatCommercial = GetParameter(StreamKind.Audio, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Format_Commercial_IfAny");
                        FormatVersion = GetParameter(StreamKind.Audio, index, "Format_Version"),
                        FormatProfile = GetParameter(StreamKind.Audio, index, "Format_Profile"),
                        FormatCompression = GetParameter(StreamKind.Audio, index, "Format_Compression"),
                        FormatSettings = GetParameter(StreamKind.Audio, index, "Format_Settings"),
                        FormatSettingsSBR = GetParameter(StreamKind.Audio, index, "Format_Settings_SBR"),
                        //string FormatSettingsSBRString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Format_Settings_SBR/String");
                        FormatSettingsPS = GetParameter(StreamKind.Audio, index, "Format_Settings_PS"),
                        //string FormatSettingsPSString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Format_Settings_PS/String");
                        FormatSettingsMode = GetParameter(StreamKind.Audio, index, "Format_Settings_Mode"),
                        FormatSettingsModeExtension = GetParameter(StreamKind.Audio, index, "Format_Settings_ModeExtension"),
                        FormatSettingsEmphasis = GetParameter(StreamKind.Audio, index, "Format_Settings_Emphasis"),
                        FormatSettingsFloor = GetParameter(StreamKind.Audio, index, "Format_Settings_Floor"),
                        FormatSettingsFirm = GetParameter(StreamKind.Audio, index, "Format_Settings_Firm"),
                        FormatSettingsEndianness = GetParameter(StreamKind.Audio, index, "Format_Settings_Endianness"),
                        FormatSettingsSign = GetParameter(StreamKind.Audio, index, "Format_Settings_Sign"),
                        FormatSettingsLaw = GetParameter(StreamKind.Audio, index, "Format_Settings_Law"),
                        FormatSettingsITU = GetParameter(StreamKind.Audio, index, "Format_Settings_ITU"),
                        FormatSettingsWrapping = GetParameter(StreamKind.Audio, index, "Format_Settings_Wrapping"),
                        FormatAdditionalFeatures = GetParameter(StreamKind.Audio, index, "Format_AdditionalFeatures"),

                        MatrixFormat = GetParameter(StreamKind.Audio, index, "Matrix_Format"),
                        InternetMediaType = GetParameter(StreamKind.Audio, index, "InternetMediaType"),
                        MuxingMode = GetParameter(StreamKind.Audio, index, "MuxingMode"),
                        MuxingModeMoreInfo = GetParameter(StreamKind.Audio, index, "MuxingMode_MoreInfo"),

                        CodecID = ParseInt(GetParameter(StreamKind.Audio, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "CodecID/String");
                        CodecIDInfo = GetParameter(StreamKind.Audio, index, "CodecID/Info"),
                        CodecIDHint = GetParameter(StreamKind.Audio, index, "CodecID/Hint"),
                        CodecIDUrl = GetParameter(StreamKind.Audio, index, "CodecID/Url"),
                        CodecIDDescription = GetParameter(StreamKind.Audio, index, "CodecID_Description"),

                        DurationFormatted = GetParameter(StreamKind.Audio, index, "Duration/String"),
                        DurationFormattedComplete = GetParameter(StreamKind.Audio, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration/String2");
                        DurationTimecode = GetParameter(StreamKind.Audio, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration/String5");
                        DurationFirstFrame = ParseInt(GetParameter(StreamKind.Audio, index, "Duration_FirstFrame")),
                        DurationFirstFrameFormatted = GetParameter(StreamKind.Audio, index, "Duration_FirstFrame/String"),
                        DurationFirstFrameFormattedComplete = GetParameter(StreamKind.Audio, index, "Duration_FirstFrame/String1"),
                        //string DurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_FirstFrame/String2");
                        DurationFirstFrameTimecode = GetParameter(StreamKind.Audio, index, "Duration_FirstFrame/String3"),
                        //string DurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_FirstFrame/String4");
                        //string DurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_FirstFrame/String5");
                        DurationLastFrame = ParseInt(GetParameter(StreamKind.Audio, index, "Duration_LastFrame")),
                        DurationLastFrameFormatted = GetParameter(StreamKind.Audio, index, "Duration_LastFrame/String"),
                        DurationLastFrameFormattedComplete = GetParameter(StreamKind.Audio, index, "Duration_LastFrame/String1"),
                        //string DurationLastFrameString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_LastFrame/String2");
                        DurationLastFrameTimecode = GetParameter(StreamKind.Audio, index, "Duration_LastFrame/String3"),
                        //string DurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_LastFrame/String4");
                        //string DurationLastFrameString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_LastFrame/String5");
                        SourceDuration = ParseInt(GetParameter(StreamKind.Audio, index, "Source_Duration")),
                        SourceDurationFormatted = GetParameter(StreamKind.Audio, index, "Source_Duration/String"),
                        SourceDurationFormattedComplete = GetParameter(StreamKind.Audio, index, "Source_Duration/String1"),
                        //string SourceDurationString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration/String2");
                        SourceDurationTimecode = GetParameter(StreamKind.Audio, index, "Source_Duration/String3"),
                        //string SourceDurationString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration/String4");
                        //string SourceDurationString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration/String5");
                        SourceDurationFirstFrame = ParseInt(GetParameter(StreamKind.Audio, index, "Source_Duration_FirstFrame")),
                        SourceDurationFirstFrameFormatted = GetParameter(StreamKind.Audio, index, "Source_Duration_FirstFrame/String"),
                        SourceDurationFirstFrameFormattedComplete = GetParameter(StreamKind.Audio, index, "Source_Duration_FirstFrame/String1"),
                        //string SourceDurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_FirstFrame/String2");
                        SourceDurationFirstFrameTimecode = GetParameter(StreamKind.Audio, index, "Source_Duration_FirstFrame/String3"),
                        //string SourceDurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_FirstFrame/String4");
                        //string SourceDurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_FirstFrame/String5");
                        SourceDurationLastFrame = ParseInt(GetParameter(StreamKind.Audio, index, "Source_Duration_LastFrame")),
                        SourceDurationLastFrameFormatted = GetParameter(StreamKind.Audio, index, "Source_Duration_LastFrame/String"),
                        SourceDurationLastFrameFormattedComplete = GetParameter(StreamKind.Audio, index, "Source_Duration_LastFrame/String1"),
                        //string SourceDurationLastFramestring2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_LastFrame/String2");
                        SourceDurationLastFrameTimecode = GetParameter(StreamKind.Audio, index, "Source_Duration_LastFrame/String3"),
                        //string SourceDurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_LastFrame/String4");
                        //string SourceDurationLastFramestring5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_LastFrame/String5");

                        BitRateMode = GetParameter(StreamKind.Audio, index, "BitRate_Mode"),
                        //string BitRateModeString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "BitRate_Mode/String");
                        BitRate = ParseULong(GetParameter(StreamKind.Audio, index, "BitRate")),
                        BitRateFormatted = GetParameter(StreamKind.Audio, index, "BitRate/String"),
                        BitRateMinimum = ParseULong(GetParameter(StreamKind.Audio, index, "BitRate_Minimum")),
                        BitRateMinimumFormatted = GetParameter(StreamKind.Audio, index, "BitRate_Minimum/String"),
                        BitRateNominal = ParseULong(GetParameter(StreamKind.Audio, index, "BitRate_Nominal")),
                        BitRateNominalFormatted = GetParameter(StreamKind.Audio, index, "BitRate_Nominal/String"),
                        BitRateMaximum = ParseULong(GetParameter(StreamKind.Audio, index, "BitRate_Maximum")),
                        BitRateMaximumFormatted = GetParameter(StreamKind.Audio, index, "BitRate_Maximum/String"),
                        BitRateEncoded = ParseULong(GetParameter(StreamKind.Audio, index, "BitRate_Encoded")),
                        BitRateEncodedFormatted = GetParameter(StreamKind.Audio, index, "BitRate_Encoded/String"),

                        Channels = ParseInt(GetParameter(StreamKind.Audio, index, "Channel(s)")),
                        ChannelsFormatted = GetParameter(StreamKind.Audio, index, "Channel(s)/String"),
                        ChannelsOriginal = ParseInt(GetParameter(StreamKind.Audio, index, "Channel(s)_Original")),
                        ChannelsOriginalFormatted = GetParameter(StreamKind.Audio, index, "Channel(s)_Original/String"),
                        MatrixChannels = ParseInt(GetParameter(StreamKind.Audio, index, "Matrix_Channel(s)")),
                        MatrixChannelsString = GetParameter(StreamKind.Audio, index, "Matrix_Channel(s)/String"),
                        ChannelPositions = GetParameter(StreamKind.Audio, index, "ChannelPositions"),
                        ChannelPositionsOriginal = GetParameter(StreamKind.Audio, index, "ChannelPositions_Original"),
                        ChannelPositionsFormatted = GetParameter(StreamKind.Audio, index, "ChannelPositions/String2"),
                        ChannelPositionsOriginalFormatted = GetParameter(StreamKind.Audio, index, "ChannelPositions_Original/String2"),
                        MatrixChannelPositions = GetParameter(StreamKind.Audio, index, "Matrix_ChannelPositions"),
                        MatrixChannelPositionsFormatted = GetParameter(StreamKind.Audio, index, "Matrix_ChannelPositions/String"),
                        ChannelLayout = GetParameter(StreamKind.Audio, index, "ChannelLayout"),
                        ChannelLayoutOriginal = GetParameter(StreamKind.Audio, index, "ChannelLayout_Original"),
                        ChannelLayoutID = GetParameter(StreamKind.Audio, index, "ChannelLayoutID"),

                        SamplesPerFrame = ParseInt(GetParameter(StreamKind.Audio, index, "SamplesPerFrame")),
                        SamplingRate = ParseInt(GetParameter(StreamKind.Audio, index, "SamplingRate")),
                        SamplingRateFormatted = GetParameter(StreamKind.Audio, index, "SamplingRate/String"),
                        SamplingCount = ParseInt(GetParameter(StreamKind.Audio, index, "SamplingCount")),
                        SourceSamplingCount = ParseInt(GetParameter(StreamKind.Audio, index, "Source_SamplingCount")),
                        FrameRate = ParseDouble(GetParameter(StreamKind.Audio, index, "FrameRate")),
                        FrameRateFormatted = GetParameter(StreamKind.Audio, index, "FrameRate/String"),
                        FrameRateNum = ParseInt(GetParameter(StreamKind.Audio, index, "FrameRate_Num")),
                        FrameRateDen = ParseInt(GetParameter(StreamKind.Audio, index, "FrameRate_Den")),
                        FrameCount = ParseInt(GetParameter(StreamKind.Audio, index, "FrameCount")),
                        SourceFrameCount = ParseInt(GetParameter(StreamKind.Audio, index, "Source_FrameCount")),

                        BitDepth = ParseInt(GetParameter(StreamKind.Audio, index, "BitDepth")),
                        BitDepthFormatted = GetParameter(StreamKind.Audio, index, "BitDepth/String"),
                        BitDepthDetected = ParseInt(GetParameter(StreamKind.Audio, index, "BitDepth_Detected")),
                        BitDepthDetectedFormatted = GetParameter(StreamKind.Audio, index, "BitDepth_Detected/String"),
                        BitDepthStored = ParseInt(GetParameter(StreamKind.Audio, index, "BitDepth_Stored")),
                        BitDepthStoredFormatted = GetParameter(StreamKind.Audio, index, "BitDepth_Stored/String"),

                        CompressionMode = GetParameter(StreamKind.Audio, index, "Compression_Mode"),
                        //string CompressionModeString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Compression_Mode/String");
                        CompressionRatio = ParseDouble(GetParameter(StreamKind.Audio, index, "Compression_Ratio")),

                        Delay = ParseInt(GetParameter(StreamKind.Audio, index, "Delay")),
                        DelayFormatted = GetParameter(StreamKind.Audio, index, "Delay/String"),
                        DelayFormattedComplete = GetParameter(StreamKind.Audio, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay/String2");
                        DelayTimecode = GetParameter(StreamKind.Audio, index, "Delay/String3"),
                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay/String4");
                        //string DelayString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay/String5");
                        DelaySettings = GetParameter(StreamKind.Audio, index, "Delay_Settings"),
                        DelayDropFrame = GetParameter(StreamKind.Audio, index, "Delay_DropFrame"),
                        DelaySource = GetParameter(StreamKind.Audio, index, "Delay_Source"),
                        //string DelaySourceString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay_Source/String");
                        DelayOriginal = ParseInt(GetParameter(StreamKind.Audio, index, "Delay_Original")),
                        DelayOriginalFormatted = GetParameter(StreamKind.Audio, index, "Delay_Original/String"),
                        DelayOriginalFormattedComplete = GetParameter(StreamKind.Audio, index, "Delay_Original/String1"),
                        //string DelayOriginalString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay_Original/String2");
                        DelayOriginalTimecode = GetParameter(StreamKind.Audio, index, "Delay_Original/String3"),
                        //string DelayOriginalString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay_Original/String4");
                        //string DelayOriginalString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay_Original/String5");
                        DelayOriginalSettings = GetParameter(StreamKind.Audio, index, "Delay_Original_Settings"),
                        DelayOriginalDropFrame = GetParameter(StreamKind.Audio, index, "Delay_Original_DropFrame"),
                        DelayOriginalSource = GetParameter(StreamKind.Audio, index, "Delay_Original_Source"),
                        VideoDelay = ParseInt(GetParameter(StreamKind.Audio, index, "Video_Delay")),
                        VideoDelayFormatted = GetParameter(StreamKind.Audio, index, "Video_Delay/String"),
                        VideoDelayFormattedComplete = GetParameter(StreamKind.Audio, index, "Video_Delay/String1"),
                        //string VideoDelayString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Video_Delay/String2");
                        VideoDelayTimecode = GetParameter(StreamKind.Audio, index, "Video_Delay/String3"),
                        //string VideoDelayString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Video_Delay/String4");
                        //string VideoDelayString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Video_Delay/String5");

                        ReplayGainGain = GetParameter(StreamKind.Audio, index, "ReplayGain_Gain"),
                        //string ReplayGainGainString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "ReplayGain_Gain/String");
                        ReplayGainPeak = GetParameter(StreamKind.Audio, index, "ReplayGain_Peak"),

                        StreamSize = ParseULong(GetParameter(StreamKind.Audio, index, "StreamSize")),
                        StreamSizeFormatted = GetParameter(StreamKind.Audio, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize/String1");
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize/String2");
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize/String3");
                        StreamSizePrecise = GetParameter(StreamKind.Audio, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize/String5");
                        StreamSizeProportion = ParseDouble(GetParameter(StreamKind.Audio, index, "StreamSize_Proportion")),
                        //string StreamSizeDemuxed = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed");
                        //string StreamSizeDemuxedString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String");
                        //string StreamSizeDemuxedString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String1");
                        //string StreamSizeDemuxedString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String2");
                        //string StreamSizeDemuxedString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String3");
                        //string StreamSizeDemuxedString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String4");
                        //string StreamSizeDemuxedString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String5");
                        SourceStreamSize = ParseULong(GetParameter(StreamKind.Audio, index, "Source_StreamSize")),
                        SourceStreamSizeFormatted = GetParameter(StreamKind.Audio, index, "Source_StreamSize/String"),
                        //string SourceStreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize/String1");
                        //string SourceStreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize/String2");
                        //string SourceStreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize/String3");
                        SourceStreamSizePrecise = GetParameter(StreamKind.Audio, index, "Source_StreamSize/String4"),
                        //string SourceStreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize/String5");
                        SourceStreamSizeProportion = GetParameter(StreamKind.Audio, index, "Source_StreamSize_Proportion"),
                        StreamSizeEncoded = ParseULong(GetParameter(StreamKind.Audio, index, "StreamSize_Encoded")),
                        StreamSizeEncodedFormatted = GetParameter(StreamKind.Audio, index, "StreamSize_Encoded/String"),
                        //string StreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Encoded/String1");
                        //string StreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Encoded/String2");
                        //string StreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Encoded/String3");
                        StreamSizeEncodedPrecise = GetParameter(StreamKind.Audio, index, "StreamSize_Encoded/String4"),
                        //string StreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Encoded/String5");
                        StreamSizeEncodedProportion = ParseDouble(GetParameter(StreamKind.Audio, index, "StreamSize_Encoded_Proportion")),
                        SourceStreamSizeEncoded = ParseULong(GetParameter(StreamKind.Audio, index, "Source_StreamSize_Encoded")),
                        SourceStreamSizeEncodedFormatted = GetParameter(StreamKind.Audio, index, "Source_StreamSize_Encoded/String"),
                        //string SourceStreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize_Encoded/String1");
                        //string SourceStreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize_Encoded/String2");
                        //string SourceStreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize_Encoded/String3");
                        SourceStreamSizeEncodedPrecise = GetParameter(StreamKind.Audio, index, "Source_StreamSize_Encoded/String4"),
                        //string SourceStreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize_Encoded/String5");
                        SourceStreamSizeEncodedProportion = ParseDouble(GetParameter(StreamKind.Audio, index, "Source_StreamSize_Encoded_Proportion")),

                        Alignment = GetParameter(StreamKind.Audio, index, "Alignment"),
                        AlignmentFormatted = GetParameter(StreamKind.Audio, index, "Alignment/String"),
                        InterleaveVideoFrames = ParseInt(GetParameter(StreamKind.Audio, index, "Interleave_VideoFrames")),
                        InterleaveDuration = ParseInt(GetParameter(StreamKind.Audio, index, "Interleave_Duration")),
                        InterleaveDurationFormatted = GetParameter(StreamKind.Audio, index, "Interleave_Duration/String"),
                        InterleavePreload = ParseInt(GetParameter(StreamKind.Audio, index, "Interleave_Preload")),
                        InterleavePreloadFormatted = GetParameter(StreamKind.Audio, index, "Interleave_Preload/String"),
                        Title = GetParameter(StreamKind.Audio, index, "Title"),

                        EncodedApplication = GetParameter(StreamKind.Audio, index, "Encoded_Application"),
                        EncodedApplicationFormatted = GetParameter(StreamKind.Audio, index, "Encoded_Application/String"),
                        EncodedApplicationCompany = GetParameter(StreamKind.Audio, index, "Encoded_Application_Company"),
                        EncodedApplicationName = GetParameter(StreamKind.Audio, index, "Encoded_Application_Name"),
                        EncodedApplicationVersion = GetParameter(StreamKind.Audio, index, "Encoded_Application_Version"),
                        EncodedApplicationUrl = GetParameter(StreamKind.Audio, index, "Encoded_Application_Url"),
                        EncodedLibrary = GetParameter(StreamKind.Audio, index, "Encoded_Library"),
                        EncodedLibraryFormatted = GetParameter(StreamKind.Audio, index, "Encoded_Library/String"),
                        EncodedLibraryCompanyName = GetParameter(StreamKind.Audio, index, "Encoded_Library_CompanyName"),
                        EncodedLibraryName = GetParameter(StreamKind.Audio, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetParameter(StreamKind.Audio, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetParameter(StreamKind.Audio, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetParameter(StreamKind.Audio, index, "Encoded_Library_Settings"),
                        EncodedOperatingSystem = GetParameter(StreamKind.Audio, index, "Encoded_OperatingSystem"),

                        Language = GetParameter(StreamKind.Audio, index, "Language"),
                        LanguageFull = GetParameter(StreamKind.Audio, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Language/String1");
                        Language2Letter = GetParameter(StreamKind.Audio, index, "Language/String2"),
                        Language3Letter = GetParameter(StreamKind.Audio, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Language/String4");
                        LanguageMore = GetParameter(StreamKind.Audio, index, "Language_More"),

                        Disabled = ParseBool(GetParameter(StreamKind.Audio, index, "Disabled")),
                        DisabledString = GetParameter(StreamKind.Audio, index, "Disabled/String"),
                        Default = ParseBool(GetParameter(StreamKind.Audio, index, "Default")),
                        DefaultString = GetParameter(StreamKind.Audio, index, "Default/String"),
                        Forced = ParseBool(GetParameter(StreamKind.Audio, index, "Forced")),
                        ForcedString = GetParameter(StreamKind.Audio, index, "Forced/String"),

                        AlternateGroup = ParseInt(GetParameter(StreamKind.Audio, index, "AlternateGroup")),
                        //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "AlternateGroup/String");
                        EncodedDate = GetParameter(StreamKind.Audio, index, "Encoded_Date"),
                        TaggedDate = GetParameter(StreamKind.Audio, index, "Tagged_Date"),
                        Encryption = GetParameter(StreamKind.Audio, index, "Encryption")
                    };

                    //get channels count from ChannelsString if mediainfo return 0
                    if (!tmp.Channels.HasValue && !string.IsNullOrEmpty(tmp.ChannelsFormatted))
                    {
                        tmp.Channels = ParseChannelsCount(tmp.ChannelsFormatted);
                    }

                    //if formatted bitrate is not present 
                    if (string.IsNullOrEmpty(tmp.BitRateFormatted))
                    {
                        if (tmp.BitRate.HasValue)
                        {
                            tmp.BitRateFormatted = GetFormattedBitrate(tmp.BitRate);
                        }
                        else if (tmp.BitRateNominal.HasValue)
                        {
                            tmp.BitRateFormatted = GetFormattedBitrate(tmp.BitRateNominal);
                        }
                    }

                    this._Audio.Add(tmp);
                }
            }
        }

        /// <summary>
        /// Gets the informations of all text streams
        /// </summary>
        private void GetTextInfo()
        {
            if (this._Text == null)
            {
                this._Text = new List<TextTrack>();

                for (int index = 0; index < _TextCount; index++)
                {
                    TextTrack tmp = new TextTrack
                    {
                        Count = ParseInt(GetParameter(StreamKind.Text, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Text, index, "Status");
                        StreamCount = ParseInt(GetParameter(StreamKind.Text, index, "StreamCount")),
                        StreamKind = GetParameter(StreamKind.Text, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetParameter(StreamKind.Text, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetParameter(StreamKind.Text, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetParameter(StreamKind.Text, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Text, index, "FirstPacketOrder");
                        Inform = GetParameter(StreamKind.Text, index, "Inform"),
                        ID = ParseInt(GetParameter(StreamKind.Text, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Text, index, "ID/String");
                        OriginalSourceMediumID = GetParameter(StreamKind.Text, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.Text, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetParameter(StreamKind.Text, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Text, index, "UniqueID/String");

                        Format = GetParameter(StreamKind.Text, index, "Format"),
                        FormatWithExtra = GetParameter(StreamKind.Text, index, "Format/String"),
                        FormatInfo = GetParameter(StreamKind.Text, index, "Format/Info"),
                        FormatUrl = GetParameter(StreamKind.Text, index, "Format/Url"),
                        FormatCommercial = GetParameter(StreamKind.Text, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Text, index, "Format_Commercial_IfAny");
                        FormatVersion = GetParameter(StreamKind.Text, index, "Format_Version"),
                        FormatProfile = GetParameter(StreamKind.Text, index, "Format_Profile"),
                        FormatCompression = GetParameter(StreamKind.Text, index, "Format_Compression"),
                        FormatSettings = GetParameter(StreamKind.Text, index, "Format_Settings"),
                        FormatSettingsWrapping = GetParameter(StreamKind.Text, index, "Format_Settings_Wrapping"),
                        FormatAdditionalFeatures = GetParameter(StreamKind.Text, index, "Format_AdditionalFeatures"),
                        InternetMediaType = GetParameter(StreamKind.Text, index, "InternetMediaType"),
                        MuxingMode = GetParameter(StreamKind.Text, index, "MuxingMode"),
                        MuxingModeMoreInfo = GetParameter(StreamKind.Text, index, "MuxingMode_MoreInfo"),

                        CodecID = ParseInt(GetParameter(StreamKind.Text, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Text, index, "CodecID/String");
                        CodecIDInfo = GetParameter(StreamKind.Text, index, "CodecID/Info"),
                        CodecIDHint = GetParameter(StreamKind.Text, index, "CodecID/Hint"),
                        CodecIDUrl = GetParameter(StreamKind.Text, index, "CodecID/Url"),
                        CodecIDDescription = GetParameter(StreamKind.Text, index, "CodecID_Description"),

                        DurationFormatted = GetParameter(StreamKind.Text, index, "Duration/String"),
                        DurationFormattedComplete = GetParameter(StreamKind.Text, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration/String2");
                        DurationTimecode = GetParameter(StreamKind.Text, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration/String5");
                        DurationFirstFrame = ParseInt(GetParameter(StreamKind.Text, index, "Duration_FirstFrame")),
                        DurationFirstFrameFormatted = GetParameter(StreamKind.Text, index, "Duration_FirstFrame/String"),
                        DurationFirstFrameFormattedComplete = GetParameter(StreamKind.Text, index, "Duration_FirstFrame/String1"),
                        //string DurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_FirstFrame/String2");
                        DurationFirstFrameTimecode = GetParameter(StreamKind.Text, index, "Duration_FirstFrame/String3"),
                        //string DurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_FirstFrame/String4");
                        //string DurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_FirstFrame/String5");
                        DurationLastFrame = ParseInt(GetParameter(StreamKind.Text, index, "Duration_LastFrame")),
                        DurationLastFrameFormatted = GetParameter(StreamKind.Text, index, "Duration_LastFrame/String"),
                        DurationLastFrameFormattedComplete = GetParameter(StreamKind.Text, index, "Duration_LastFrame/String1"),
                        //string DurationLastFrameString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_LastFrame/String2");
                        DurationLastFrameTimecode = GetParameter(StreamKind.Text, index, "Duration_LastFrame/String3"),
                        //string DurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_LastFrame/String4");
                        //string DurationLastFrameString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_LastFrame/String5");
                        SourceDuration = ParseInt(GetParameter(StreamKind.Text, index, "Source_Duration")),
                        SourceDurationFormatted = GetParameter(StreamKind.Text, index, "Source_Duration/String"),
                        SourceDurationFormattedComplete = GetParameter(StreamKind.Text, index, "Source_Duration/String1"),
                        //string SourceDurationString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration/String2");
                        SourceDurationTimecode = GetParameter(StreamKind.Text, index, "Source_Duration/String3"),
                        //string SourceDurationString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration/String4");
                        //string SourceDurationString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration/String5");
                        SourceDurationFirstFrame = ParseInt(GetParameter(StreamKind.Text, index, "Source_Duration_FirstFrame")),
                        SourceDurationFirstFrameFormatted = GetParameter(StreamKind.Text, index, "Source_Duration_FirstFrame/String"),
                        SourceDurationFirstFrameFormattedComplete = GetParameter(StreamKind.Text, index, "Source_Duration_FirstFrame/String1"),
                        //string SourceDurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_FirstFrame/String2");
                        SourceDurationFirstFrameTimecode = GetParameter(StreamKind.Text, index, "Source_Duration_FirstFrame/String3"),
                        //string SourceDurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_FirstFrame/String4");
                        //string SourceDurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_FirstFrame/String5");
                        SourceDurationLastFrame = ParseInt(GetParameter(StreamKind.Text, index, "Source_Duration_LastFrame")),
                        SourceDurationLastFrameFormatted = GetParameter(StreamKind.Text, index, "Source_Duration_LastFrame/String"),
                        SourceDurationLastFrameFormattedComplete = GetParameter(StreamKind.Text, index, "Source_Duration_LastFrame/String1"),
                        //string SourceDurationLastFramestring2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_LastFrame/String2");
                        SourceDurationLastFrameTimecode = GetParameter(StreamKind.Text, index, "Source_Duration_LastFrame/String3"),
                        //string SourceDurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_LastFrame/String4");
                        //string SourceDurationLastFramestring5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_LastFrame/String5");

                        BitRateMode = GetParameter(StreamKind.Text, index, "BitRate_Mode"),
                        //string BitRateModeString = getSpecificInfo(MediaInfoStreamKind.Text, index, "BitRate_Mode/String");
                        BitRate = ParseULong(GetParameter(StreamKind.Text, index, "BitRate")),
                        BitRateFormatted = GetParameter(StreamKind.Text, index, "BitRate/String"),
                        BitRateMinimum = ParseULong(GetParameter(StreamKind.Text, index, "BitRate_Minimum")),
                        BitRateMinimumFormatted = GetParameter(StreamKind.Text, index, "BitRate_Minimum/String"),
                        BitRateNominal = ParseULong(GetParameter(StreamKind.Text, index, "BitRate_Nominal")),
                        BitRateNominalFormatted = GetParameter(StreamKind.Text, index, "BitRate_Nominal/String"),
                        BitRateMaximum = ParseULong(GetParameter(StreamKind.Text, index, "BitRate_Maximum")),
                        BitRateMaximumFormatted = GetParameter(StreamKind.Text, index, "BitRate_Maximum/String"),
                        BitRateEncoded = ParseULong(GetParameter(StreamKind.Text, index, "BitRate_Encoded")),
                        BitRateEncodedFormatted = GetParameter(StreamKind.Text, index, "BitRate_Encoded/String"),

                        Width = ParseInt(GetParameter(StreamKind.Text, index, "Width")),
                        //string WidthString = getSpecificInfo(MediaInfoStreamKind.Text, index, "Width/String");
                        Height = ParseInt(GetParameter(StreamKind.Text, index, "Height")),
                        //string HeightString = getSpecificInfo(MediaInfoStreamKind.Text, index, "Height/String");

                        FrameRateMode = GetParameter(StreamKind.Text, index, "FrameRate_Mode"),
                        //string FrameRateModeString = getSpecificInfo(MediaInfoStreamKind.Text, index, "FrameRate_Mode/String");
                        FrameRate = ParseDouble(GetParameter(StreamKind.Text, index, "FrameRate")),
                        FrameRateFormatted = GetParameter(StreamKind.Text, index, "FrameRate/String"),
                        FrameRateNum = ParseInt(GetParameter(StreamKind.Text, index, "FrameRate_Num")),
                        FrameRateDen = ParseInt(GetParameter(StreamKind.Text, index, "FrameRate_Den")),
                        FrameRateMinimum = ParseDouble(GetParameter(StreamKind.Text, index, "FrameRate_Minimum")),
                        FrameRateMinimumFormatted = GetParameter(StreamKind.Text, index, "FrameRate_Minimum/String"),
                        FrameRateNominal = ParseDouble(GetParameter(StreamKind.Text, index, "FrameRate_Nominal")),
                        FrameRateNominalFormatted = GetParameter(StreamKind.Text, index, "FrameRate_Nominal/String"),
                        FrameRateMaximum = ParseDouble(GetParameter(StreamKind.Text, index, "FrameRate_Maximum")),
                        FrameRateMaximumFormatted = GetParameter(StreamKind.Text, index, "FrameRate_Maximum/String"),
                        FrameRateOriginal = ParseDouble(GetParameter(StreamKind.Text, index, "FrameRate_Original")),
                        FrameRateOriginalFormatted = GetParameter(StreamKind.Text, index, "FrameRate_Original/String"),
                        FrameCount = ParseInt(GetParameter(StreamKind.Text, index, "FrameCount")),
                        SourceFrameCount = ParseInt(GetParameter(StreamKind.Text, index, "Source_FrameCount")),

                        ColorSpace = GetParameter(StreamKind.Text, index, "ColorSpace"),
                        ChromaSubsampling = GetParameter(StreamKind.Text, index, "ChromaSubsampling"),
                        BitDepth = ParseInt(GetParameter(StreamKind.Text, index, "BitDepth")),
                        BitDepthFormatted = GetParameter(StreamKind.Text, index, "BitDepth/String"),
                        CompressionMode = GetParameter(StreamKind.Text, index, "Compression_Mode"),
                        //string CompressionModeString = getSpecificInfo(MediaInfoStreamKind.Text, index, "Compression_Mode/String");
                        CompressionRatio = ParseDouble(GetParameter(StreamKind.Text, index, "Compression_Ratio")),

                        Delay = ParseInt(GetParameter(StreamKind.Text, index, "Delay")),
                        DelayFormatted = GetParameter(StreamKind.Text, index, "Delay/String"),
                        DelayFormattedComplete = GetParameter(StreamKind.Text, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay/String2");
                        DelayTimecode = GetParameter(StreamKind.Text, index, "Delay/String3"),
                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay/String4");
                        //string DelayString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay/String5");
                        DelaySettings = GetParameter(StreamKind.Text, index, "Delay_Settings"),
                        DelayDropFrame = GetParameter(StreamKind.Text, index, "Delay_DropFrame"),
                        DelaySource = GetParameter(StreamKind.Text, index, "Delay_Source"),
                        //string DelaySourceString = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay_Source/String");
                        DelayOriginal = ParseInt(GetParameter(StreamKind.Text, index, "Delay_Original")),
                        DelayOriginalFormatted = GetParameter(StreamKind.Text, index, "Delay_Original/String"),
                        DelayOriginalFormattedComplete = GetParameter(StreamKind.Text, index, "Delay_Original/String1"),
                        //string DelayOriginalString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay_Original/String2");
                        DelayOriginalTimecode = GetParameter(StreamKind.Text, index, "Delay_Original/String3"),
                        //string DelayOriginalString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay_Original/String4");
                        //string DelayOriginalString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay_Original/String5");
                        DelayOriginalSettings = GetParameter(StreamKind.Text, index, "Delay_Original_Settings"),
                        DelayOriginalDropFrame = GetParameter(StreamKind.Text, index, "Delay_Original_DropFrame"),
                        DelayOriginalSource = GetParameter(StreamKind.Text, index, "Delay_Original_Source"),
                        VideoDelay = ParseInt(GetParameter(StreamKind.Text, index, "Video_Delay")),
                        VideoDelayFormatted = GetParameter(StreamKind.Text, index, "Video_Delay/String"),
                        VideoDelayFormattedComplete = GetParameter(StreamKind.Text, index, "Video_Delay/String1"),
                        //string VideoDelayString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Video_Delay/String2");
                        VideoDelayTimecode = GetParameter(StreamKind.Text, index, "Video_Delay/String3"),
                        //string VideoDelayString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Video_Delay/String4");
                        //string VideoDelayString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Video_Delay/String5");

                        StreamSize = ParseULong(GetParameter(StreamKind.Text, index, "StreamSize")),
                        StreamSizeFormatted = GetParameter(StreamKind.Text, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize/String1");
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize/String2");
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize/String3");
                        StreamSizePrecise = GetParameter(StreamKind.Text, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize/String5");
                        StreamSizeProportion = ParseDouble(GetParameter(StreamKind.Text, index, "StreamSize_Proportion")),
                        //string StreamSizeDemuxed = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed");
                        //string StreamSizeDemuxedString = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String");
                        //string StreamSizeDemuxedString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String1");
                        //string StreamSizeDemuxedString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String2");
                        //string StreamSizeDemuxedString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String3");
                        //string StreamSizeDemuxedString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String4");
                        //string StreamSizeDemuxedString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String5");
                        SourceStreamSize = ParseULong(GetParameter(StreamKind.Text, index, "Source_StreamSize")),
                        SourceStreamSizeFormatted = GetParameter(StreamKind.Text, index, "Source_StreamSize/String"),
                        //string SourceStreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize/String1");
                        //string SourceStreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize/String2");
                        //string SourceStreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize/String3");
                        SourceStreamSizePrecise = GetParameter(StreamKind.Text, index, "Source_StreamSize/String4"),
                        //string SourceStreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize/String5");
                        SourceStreamSizeProportion = GetParameter(StreamKind.Text, index, "Source_StreamSize_Proportion"),
                        StreamSizeEncoded = ParseULong(GetParameter(StreamKind.Text, index, "StreamSize_Encoded")),
                        StreamSizeEncodedFormatted = GetParameter(StreamKind.Text, index, "StreamSize_Encoded/String"),
                        //string StreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Encoded/String1");
                        //string StreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Encoded/String2");
                        //string StreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Encoded/String3");
                        StreamSizeEncodedPrecise = GetParameter(StreamKind.Text, index, "StreamSize_Encoded/String4"),
                        //string StreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Encoded/String5");
                        StreamSizeEncodedProportion = ParseDouble(GetParameter(StreamKind.Text, index, "StreamSize_Encoded_Proportion")),
                        SourceStreamSizeEncoded = ParseULong(GetParameter(StreamKind.Text, index, "Source_StreamSize_Encoded")),
                        SourceStreamSizeEncodedFormatted = GetParameter(StreamKind.Text, index, "Source_StreamSize_Encoded/String"),
                        //string SourceStreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize_Encoded/String1");
                        //string SourceStreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize_Encoded/String2");
                        //string SourceStreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize_Encoded/String3");
                        SourceStreamSizeEncodedPrecise = GetParameter(StreamKind.Text, index, "Source_StreamSize_Encoded/String4"),
                        //string SourceStreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize_Encoded/String5");
                        SourceStreamSizeEncodedProportion = ParseDouble(GetParameter(StreamKind.Text, index, "Source_StreamSize_Encoded_Proportion")),

                        Title = GetParameter(StreamKind.Text, index, "Title"),

                        EncodedApplication = GetParameter(StreamKind.Text, index, "Encoded_Application"),
                        EncodedApplicationFormatted = GetParameter(StreamKind.Text, index, "Encoded_Application/String"),
                        EncodedApplicationCompany = GetParameter(StreamKind.Text, index, "Encoded_Application_Company"),
                        EncodedApplicationName = GetParameter(StreamKind.Text, index, "Encoded_Application_Name"),
                        EncodedApplicationVersion = GetParameter(StreamKind.Text, index, "Encoded_Application_Version"),
                        EncodedApplicationUrl = GetParameter(StreamKind.Text, index, "Encoded_Application_Url"),
                        EncodedLibrary = GetParameter(StreamKind.Text, index, "Encoded_Library"),
                        EncodedLibraryFormatted = GetParameter(StreamKind.Text, index, "Encoded_Library/String"),
                        EncodedLibraryCompanyName = GetParameter(StreamKind.Text, index, "Encoded_Library_CompanyName"),
                        EncodedLibraryName = GetParameter(StreamKind.Text, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetParameter(StreamKind.Text, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetParameter(StreamKind.Text, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetParameter(StreamKind.Text, index, "Encoded_Library_Settings"),
                        EncodedOperatingSystem = GetParameter(StreamKind.Text, index, "Encoded_OperatingSystem"),

                        Language = GetParameter(StreamKind.Text, index, "Language"),
                        LanguageFull = GetParameter(StreamKind.Text, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Language/String1");
                        Language2Letter = GetParameter(StreamKind.Text, index, "Language/String2"),
                        Language3Letter = GetParameter(StreamKind.Text, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Language/String4");
                        LanguageMore = GetParameter(StreamKind.Text, index, "Language_More"),

                        Disabled = ParseBool(GetParameter(StreamKind.Text, index, "Disabled")),
                        DisabledString = GetParameter(StreamKind.Text, index, "Disabled/String"),
                        Default = ParseBool(GetParameter(StreamKind.Text, index, "Default")),
                        DefaultString = GetParameter(StreamKind.Text, index, "Default/String"),
                        Forced = ParseBool(GetParameter(StreamKind.Text, index, "Forced")),
                        ForcedString = GetParameter(StreamKind.Text, index, "Forced/String"),

                        AlternateGroup = ParseInt(GetParameter(StreamKind.Text, index, "AlternateGroup")),
                        //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Text, index, "AlternateGroup/String");
                        Summary = GetParameter(StreamKind.Text, index, "Summary"),
                        EncodedDate = GetParameter(StreamKind.Text, index, "Encoded_Date"),
                        TaggedDate = GetParameter(StreamKind.Text, index, "Tagged_Date"),
                        Encryption = GetParameter(StreamKind.Text, index, "Encryption")
                    };

                    this._Text.Add(tmp);
                }
            }
        }

        /// <summary>
        /// Gets the informations of all menu streams
        /// </summary>
        private void GetImageInfo()
        {
            if (this._Image == null)
            {
                this._Image = new List<ImageTrack>();

                for (int index = 0; index < _ImageCount; index++)
                {
                    ImageTrack tmp = new ImageTrack
                    {
                        Count = ParseInt(GetParameter(StreamKind.Image, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Image, index, "Status");
                        StreamCount = ParseInt(GetParameter(StreamKind.Image, index, "StreamCount")),
                        StreamKind = GetParameter(StreamKind.Image, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetParameter(StreamKind.Image, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetParameter(StreamKind.Image, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetParameter(StreamKind.Image, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Image, index, "FirstPacketOrder");
                        Inform = GetParameter(StreamKind.Image, index, "Inform"),
                        ID = ParseInt(GetParameter(StreamKind.Image, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Image, index, "ID/String");
                        OriginalSourceMediumID = GetParameter(StreamKind.Image, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.Image, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetParameter(StreamKind.Image, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Image, index, "UniqueID/String");

                        Format = GetParameter(StreamKind.Image, index, "Format"),
                        FormatWithExtra = GetParameter(StreamKind.Image, index, "Format/String"),
                        FormatInfo = GetParameter(StreamKind.Image, index, "Format/Info"),
                        FormatUrl = GetParameter(StreamKind.Image, index, "Format/Url"),
                        FormatCommercial = GetParameter(StreamKind.Image, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Image, index, "Format_Commercial_IfAny");
                        FormatVersion = GetParameter(StreamKind.Image, index, "Format_Version"),
                        FormatProfile = GetParameter(StreamKind.Image, index, "Format_Profile"),
                        //FormatSettingsEndianness = getSpecificInfo(MediaInfoStreamKind.Image, index, "Format_Settings_Endianness");
                        //FormatSettingsPacking = getSpecificInfo(MediaInfoStreamKind.Image, index, "Format_Settings_Packing");
                        FormatCompression = GetParameter(StreamKind.Image, index, "Format_Compression"),
                        FormatSettings = GetParameter(StreamKind.Image, index, "Format_Settings"),
                        FormatSettingsWrapping = GetParameter(StreamKind.Image, index, "Format_Settings_Wrapping"),
                        FormatAdditionalFeatures = GetParameter(StreamKind.Image, index, "Format_AdditionalFeatures"),
                        InternetMediaType = GetParameter(StreamKind.Image, index, "InternetMediaType"),

                        CodecID = ParseInt(GetParameter(StreamKind.Image, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Image, index, "CodecID/String");
                        CodecIDInfo = GetParameter(StreamKind.Image, index, "CodecID/Info"),
                        CodecIDHint = GetParameter(StreamKind.Image, index, "CodecID/Hint"),
                        CodecIDUrl = GetParameter(StreamKind.Image, index, "CodecID/Url"),
                        CodecIDDescription = GetParameter(StreamKind.Image, index, "CodecID_Description"),

                        Width = ParseInt(GetParameter(StreamKind.Image, index, "Width")),
                        WidthFormatted = GetParameter(StreamKind.Image, index, "Width/String"),
                        WidthOffset = ParseInt(GetParameter(StreamKind.Image, index, "Width_Offset")),
                        WidthOffsetFormatted = GetParameter(StreamKind.Image, index, "Width_Offset/String"),
                        WidthOriginal = ParseInt(GetParameter(StreamKind.Image, index, "Width_Original")),
                        WidthOriginalFormatted = GetParameter(StreamKind.Image, index, "Width_Original/String"),
                        Height = ParseInt(GetParameter(StreamKind.Image, index, "Height")),
                        HeightFormatted = GetParameter(StreamKind.Image, index, "Height/String"),
                        HeightOffset = ParseInt(GetParameter(StreamKind.Image, index, "Height_Offset")),
                        HeightOffsetFormatted = GetParameter(StreamKind.Image, index, "Height_Offset/String"),
                        HeightOriginal = ParseInt(GetParameter(StreamKind.Image, index, "Height_Original")),
                        HeightOriginalFormatted = GetParameter(StreamKind.Image, index, "Height_Original/String"),

                        PixelAspectRatio = ParseDouble(GetParameter(StreamKind.Image, index, "PixelAspectRatio")),
                        PixelAspectRatioFormatted = GetParameter(StreamKind.Image, index, "PixelAspectRatio/String"),
                        PixelAspectRatioOriginal = ParseDouble(GetParameter(StreamKind.Image, index, "PixelAspectRatio_Original")),
                        PixelAspectRatioOriginalFormatted = GetParameter(StreamKind.Image, index, "PixelAspectRatio_Original/String"),
                        DisplayAspectRatio = ParseDouble(GetParameter(StreamKind.Image, index, "DisplayAspectRatio")),
                        DisplayAspectRatioFormatted = GetParameter(StreamKind.Image, index, "DisplayAspectRatio/String"),
                        DisplayAspectRatioOriginal = ParseDouble(GetParameter(StreamKind.Image, index, "DisplayAspectRatio_Original")),
                        DisplayAspectRatioOriginalFormatted = GetParameter(StreamKind.Image, index, "DisplayAspectRatio_Original/String"),

                        ColorSpace = GetParameter(StreamKind.Image, index, "ColorSpace"),
                        ChromaSubsampling = GetParameter(StreamKind.Image, index, "ChromaSubsampling"),
                        BitDepth = ParseInt(GetParameter(StreamKind.Image, index, "BitDepth")),
                        BitDepthFormatted = GetParameter(StreamKind.Image, index, "BitDepth/String"),
                        CompressionMode = GetParameter(StreamKind.Image, index, "Compression_Mode"),
                        CompressionRatio = ParseDouble(GetParameter(StreamKind.Image, index, "Compression_Ratio")),

                        StreamSize = ParseULong(GetParameter(StreamKind.Image, index, "StreamSize")),
                        StreamSizeFormatted = GetParameter(StreamKind.Image, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamSize/String1");
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamSize/String2");
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamSize/String3");
                        StreamSizePrecise = GetParameter(StreamKind.Image, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamSize/String5");
                        StreamSizeProportion = ParseDouble(GetParameter(StreamKind.Image, index, "StreamSize_Proportion")),

                        EncodedLibrary = GetParameter(StreamKind.Image, index, "Encoded_Library"),
                        EncodedLibraryFormatted = GetParameter(StreamKind.Image, index, "Encoded_Library/String"),
                        EncodedLibraryName = GetParameter(StreamKind.Image, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetParameter(StreamKind.Image, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetParameter(StreamKind.Image, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetParameter(StreamKind.Image, index, "Encoded_Library_Settings"),

                        Language = GetParameter(StreamKind.Image, index, "Language"),
                        LanguageFull = GetParameter(StreamKind.Image, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Image, index, "Language/String1");
                        Language2Letter = GetParameter(StreamKind.Image, index, "Language/String2"),
                        Language3Letter = GetParameter(StreamKind.Image, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Image, index, "Language/String4");
                        LanguageMore = GetParameter(StreamKind.Image, index, "Language_More"),

                        Disabled = ParseBool(GetParameter(StreamKind.Image, index, "Disabled")),
                        DisabledString = GetParameter(StreamKind.Image, index, "Disabled/String"),
                        Default = ParseBool(GetParameter(StreamKind.Image, index, "Default")),
                        DefaultString = GetParameter(StreamKind.Image, index, "Default/String"),
                        Forced = ParseBool(GetParameter(StreamKind.Image, index, "Forced")),
                        ForcedString = GetParameter(StreamKind.Image, index, "Forced/String"),

                        AlternateGroup = ParseInt(GetParameter(StreamKind.Image, index, "AlternateGroup")),
                        //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Image, index, "AlternateGroup/String");
                        Summary = GetParameter(StreamKind.Image, index, "Summary"),
                        EncodedDate = GetParameter(StreamKind.Image, index, "Encoded_Date"),
                        TaggedDate = GetParameter(StreamKind.Image, index, "Tagged_Date"),
                        Encryption = GetParameter(StreamKind.Image, index, "Encryption"),

                        ColourDescriptionPresent = ParseBool(GetParameter(StreamKind.Image, index, "colour_description_present")),
                        ColourPrimaries = GetParameter(StreamKind.Image, index, "colour_primaries"),
                        TransferCharacteristics = GetParameter(StreamKind.Image, index, "transfer_characteristics"),
                        MatrixCoefficients = GetParameter(StreamKind.Image, index, "matrix_coefficients"),
                        ColourDescriptionPresentOriginal = ParseBool(GetParameter(StreamKind.Image, index, "colour_description_present_Original")),
                        ColourPrimariesOriginal = GetParameter(StreamKind.Image, index, "colour_primaries_Original"),
                        TransferCharacteristicsOriginal = GetParameter(StreamKind.Image, index, "transfer_characteristics_Original"),
                        MatrixCoefficientsOriginal = GetParameter(StreamKind.Image, index, "matrix_coefficients_Original")
                    };

                    this._Image.Add(tmp);
                }
            }
        }

        /// <summary>
        /// Gets the informations of all menu streams
        /// </summary>
        private void GetMenuInfo()
        {
            if (this._Menu == null)
            {
                this._Menu = new List<MenuTrack>();

                for (int index = 0; index < _MenuCount; index++)
                {
                    MenuTrack tmp = new MenuTrack
                    {
                        Count = ParseInt(GetParameter(StreamKind.Menu, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Status");
                        StreamCount = ParseInt(GetParameter(StreamKind.Menu, index, "StreamCount")),
                        StreamKind = GetParameter(StreamKind.Menu, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetParameter(StreamKind.Menu, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetParameter(StreamKind.Menu, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetParameter(StreamKind.Menu, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Menu, index, "FirstPacketOrder");
                        Inform = GetParameter(StreamKind.Menu, index, "Inform"),
                        ID = ParseInt(GetParameter(StreamKind.Menu, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "ID/String");
                        OriginalSourceMediumID = GetParameter(StreamKind.Menu, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetParameter(StreamKind.Menu, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "UniqueID/String");

                        Format = GetParameter(StreamKind.Menu, index, "Format"),
                        FormatWithExtra = GetParameter(StreamKind.Menu, index, "Format/String"),
                        FormatInfo = GetParameter(StreamKind.Menu, index, "Format/Info"),
                        FormatUrl = GetParameter(StreamKind.Menu, index, "Format/Url"),
                        FormatCommercial = GetParameter(StreamKind.Menu, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Format_Commercial_IfAny");
                        FormatVersion = GetParameter(StreamKind.Menu, index, "Format_Version"),
                        FormatProfile = GetParameter(StreamKind.Menu, index, "Format_Profile"),
                        FormatCompression = GetParameter(StreamKind.Menu, index, "Format_Compression"),
                        FormatSettings = GetParameter(StreamKind.Menu, index, "Format_Settings"),
                        FormatAdditionalFeatures = GetParameter(StreamKind.Menu, index, "Format_AdditionalFeatures"),

                        CodecID = ParseInt(GetParameter(StreamKind.Menu, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "CodecID/String");
                        CodecIDInfo = GetParameter(StreamKind.Menu, index, "CodecID/Info"),
                        CodecIDHint = GetParameter(StreamKind.Menu, index, "CodecID/Hint"),
                        CodecIDUrl = GetParameter(StreamKind.Menu, index, "CodecID/Url"),
                        CodecIDDescription = GetParameter(StreamKind.Menu, index, "CodecID_Description"),

                        DurationFormatted = GetParameter(StreamKind.Menu, index, "Duration/String"),
                        DurationFormattedComplete = GetParameter(StreamKind.Menu, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Duration/String2");
                        DurationTimecode = GetParameter(StreamKind.Menu, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Duration/String5");
                        DurationStart = ParseInt(GetParameter(StreamKind.Menu, index, "Duration_Start")),
                        DurationEnd = ParseInt(GetParameter(StreamKind.Menu, index, "Duration_End")),

                        Delay = ParseInt(GetParameter(StreamKind.Menu, index, "Delay")),
                        DelayFormatted = GetParameter(StreamKind.Menu, index, "Delay/String"),
                        DelayFormattedComplete = GetParameter(StreamKind.Menu, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Delay/String2");
                        DelayTimecode = GetParameter(StreamKind.Menu, index, "Delay/String3"),
                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Delay/String4");
                        //string DelayString5 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Delay/String5");
                        DelaySettings = GetParameter(StreamKind.Menu, index, "Delay_Settings"),
                        DelayDropFrame = GetParameter(StreamKind.Menu, index, "Delay_DropFrame"),
                        DelaySource = GetParameter(StreamKind.Menu, index, "Delay_Source"),

                        ListStreamKind = GetParameter(StreamKind.Menu, index, "List_StreamKind"),
                        ListStreamPos = GetParameter(StreamKind.Menu, index, "List_StreamPos"),
                        List = GetParameter(StreamKind.Menu, index, "List"),
                        //string ListString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "List/String");

                        Title = GetParameter(StreamKind.Menu, index, "Title"),

                        Language = GetParameter(StreamKind.Menu, index, "Language"),
                        LanguageFull = GetParameter(StreamKind.Menu, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Language/String1");
                        Language2Letter = GetParameter(StreamKind.Menu, index, "Language/String2"),
                        Language3Letter = GetParameter(StreamKind.Menu, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Language/String4");
                        LanguageMore = GetParameter(StreamKind.Menu, index, "Language_More"),

                        ServiceName = GetParameter(StreamKind.Menu, index, "ServiceName"),
                        ServiceChannel = GetParameter(StreamKind.Menu, index, "ServiceChannel"),
                        ServiceUrl = GetParameter(StreamKind.Menu, index, "Service/Url"),
                        ServiceProvider = GetParameter(StreamKind.Menu, index, "ServiceProvider"),
                        ServiceProviderUrl = GetParameter(StreamKind.Menu, index, "ServiceProvider/Url"),
                        ServiceType = GetParameter(StreamKind.Menu, index, "ServiceType"),
                        NetworkName = GetParameter(StreamKind.Menu, index, "NetworkName"),
                        OriginalNetworkName = GetParameter(StreamKind.Menu, index, "Original/NetworkName"),
                        Countries = GetParameter(StreamKind.Menu, index, "Countries"),
                        TimeZones = GetParameter(StreamKind.Menu, index, "TimeZones"),
                        LawRating = GetParameter(StreamKind.Menu, index, "LawRating"),
                        LawRatingReason = GetParameter(StreamKind.Menu, index, "LawRating_Reason"),

                        Disabled = ParseBool(GetParameter(StreamKind.Menu, index, "Disabled")),
                        DisabledString = GetParameter(StreamKind.Menu, index, "Disabled/String"),
                        Default = ParseBool(GetParameter(StreamKind.Menu, index, "Default")),
                        DefaultString = GetParameter(StreamKind.Menu, index, "Default/String"),
                        Forced = ParseBool(GetParameter(StreamKind.Menu, index, "Forced")),
                        ForcedString = GetParameter(StreamKind.Menu, index, "Forced/String"),
                        AlternateGroup = ParseInt(GetParameter(StreamKind.Menu, index, "AlternateGroup"))
                    };
                    //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "AlternateGroup/String");

                    //get chapters count (diff between start position and end position)
                    int endpos = ParseInt(GetParameter(StreamKind.Menu, index, "Chapters_Pos_End")).Value;
                    int startpos = ParseInt(GetParameter(StreamKind.Menu, index, "Chapters_Pos_Begin")).Value;

                    tmp.ChaptersCount = endpos - startpos;

                    //get chapters list inside each menu
                    if (tmp.ChaptersCount > 0)
                    {
                        tmp.Chapters = new List<ChapterTrack>();

                        for (int innerindex = startpos; innerindex < endpos; innerindex++)
                        {
                            ChapterTrack chapt = new ChapterTrack
                            {
                                Index = innerindex - startpos
                            };

                            string tmpstr = GetSpecificParameter(StreamKind.Menu, index, innerindex, InfoKind.Text);

                            if (!string.IsNullOrEmpty(tmpstr))
                            {
                                if (tmpstr.Contains(":"))
                                {
                                    int separatorIndex = tmpstr.IndexOf(':');
                                    chapt.Language = separatorIndex == 0 ? "und" : tmpstr.Substring(0, separatorIndex);
                                    chapt.Title = tmpstr.Substring(separatorIndex + 1, tmpstr.Length - (separatorIndex + 1));
                                }
                                else
                                {
                                    chapt.Language = string.Empty;
                                    chapt.Title = tmpstr;
                                }
                            }
                            else
                            {
                                chapt.Language = string.Empty;
                                chapt.Title = string.Empty;
                            }

                            chapt.Timecode = GetSpecificParameter(StreamKind.Menu, index, innerindex, InfoKind.Name);
                            tmp.Chapters.Add(chapt);
                        }
                    }

                    this._Menu.Add(tmp);
                }
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Gets the value of a parameter of a track at a defined index with a defined kind.
        /// </summary>
        /// <param name="kind">The kind of stream</param>
        /// <param name="trackIndex">The track index</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <returns>
        /// A string containing the parameter's value or an empty string in case of problems.
        /// </returns>
        private unsafe string GetParameter(StreamKind kind, int trackIndex, string parameterName)
        {
            IntPtr p = MediaInfoLib.GetParameter(this.Handle, kind, Convert.ToUInt32(trackIndex), parameterName, InfoKind.Text, InfoKind.Name);
            return ParseOutput(p);
        }

        /// <summary>
        /// Gets the value of a property of a parameter of a track at a defined index with a defined kind.
        /// </summary>
        /// <param name="kind">The kind of stream</param>
        /// <param name="trackIndex">The track index</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <param name="informationKind">the information kind</param>
        /// <returns>A string containing the parameter's value or an empty string in case of problems.</returns>
        private unsafe string GetSpecificParameter(StreamKind kind, int trackIndex, int parameterName, InfoKind informationKind)
        {
            IntPtr p = MediaInfoLib.GetParameter(this.Handle, kind, Convert.ToUInt32(trackIndex), Convert.ToUInt32(parameterName), informationKind);
            return ParseOutput(p);
        }

        /// <summary>
        /// Parses the mediainfo library output. The reading is performed using a buffer of 8192 chars.
        /// </summary>
        /// <param name="pointer">The pointer to the memory area when the informations are stored.</param>
        /// <returns>The parsed string from the memory area referred to the input pointer.</returns>
        private unsafe string ParseOutput(IntPtr pointer)
        {
            var sb = new StringBuilder();

            int len = 8192;
            int index = 0;
            char* ptr = (char*)pointer.ToPointer();
            char[] buffer = new char[len];

            while (*ptr != '\0')
            {
                if (index == len)
                {
                    sb.Append(buffer);
                    index = 0;
                }
                buffer[index] = *ptr;
                ++ptr;
                index++;
            }

            buffer[index] = '\0';
            sb.Append(buffer, 0, index);

            return sb.ToString();
        }
        #endregion

        #endregion

        #region Public properties

        /// <summary>
        /// Return if infos are available
        /// </summary>
        /// <returns>true, if at least one general stream has been found.</returns>
        public bool InfoAvailable()
        {
            return _GeneralCount > 0 ? true : false;
        }

        #region Reports
        /// <summary>
        /// Complete information (will throw exception if accessed from a Mediainfo instance created from a stream without specifying 'parseReports' argument to true or if the file does not exists anymore).
        /// </summary>
        public string InfoComplete
        {
            get
            {
                if (string.IsNullOrEmpty(this._InfoComplete))
                {
                    if (!MediaInfoLib.OpenFile(this.Handle, _FileName))
                    {
                        throw new MediainfoOpenException("The file was not found");
                    }
                    GetCompleteReport();
                    MediaInfoLib.CloseFile(this.Handle);
                }
                return _InfoComplete;
            }
        }

        /// <summary>
        /// Standard information (will throw exception if accessed from a Mediainfo instance created from a stream without specifying 'parseReports' argument to true or if the file does not exists anymore).
        /// </summary>
        public string InfoStandard
        {
            get
            {
                if (string.IsNullOrEmpty(this._InfoStandard))
                {
                    if (!MediaInfoLib.OpenFile(this.Handle, _FileName))
                    {
                        throw new MediainfoOpenException("The file was not found");
                    }
                    GetStandardReport();
                    MediaInfoLib.CloseFile(this.Handle);
                }
                return _InfoStandard;
            }
        }

        /// <summary>
        /// Lists all available properties for each track.
        /// </summary>
        public string InfoCustom
        {
            get
            {
                if (string.IsNullOrEmpty(_InfoCustom))
                {
                    var sb = new StringBuilder();

                    sb.AppendLine("General");
                    ListEveryAvailablePropery<GeneralTrack>(General, sb);
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("Video");
                    ListEveryAvailablePropery<VideoTrack>(Video, sb);
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("Audio");
                    ListEveryAvailablePropery<AudioTrack>(Audio, sb);
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("Text");
                    ListEveryAvailablePropery<TextTrack>(Text, sb);
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("Image");
                    ListEveryAvailablePropery<ImageTrack>(Image, sb);
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("Menu");
                    ListEveryAvailablePropery<MenuTrack>(Menu, sb);

                    _InfoCustom = sb.ToString();
                }
                return _InfoCustom;
            }
        }
        #endregion

        #region Tracks
        /// <summary>
        /// List of all the General streams available in the file, type GeneralTrack[trackindex] to access a specific track
        /// </summary>
        /// <value>
        /// The list of all general streams.
        /// </value>
        public List<GeneralTrack> General
        {
            get
            {
                if (this._General == null)
                {
                    GetGeneralInfo();
                }
                return this._General;
            }
        }

        /// <summary>
        /// List of all the Video streams available in the file, type VideoTrack[trackindex] to access a specific track
        /// </summary>
        /// <value>
        /// The list of all video streams.
        /// </value>
        public List<VideoTrack> Video
        {
            get
            {
                if (this._Video == null)
                {
                    GetVideoInfo();
                }
                return this._Video;
            }
        }

        /// <summary>
        /// List of all the Audio streams available in the file, type AudioTrack[trackindex] to access a specific track
        /// </summary>
        /// <value>
        /// The list of all audio streams.
        /// </value>
        public List<AudioTrack> Audio
        {
            get
            {
                if (this._Audio == null)
                {
                    GetAudioInfo();
                }
                return this._Audio;
            }
        }

        /// <summary>
        /// List of all the Text streams available in the file, type TextTrack[trackindex] to access a specific track
        /// </summary>
        /// <value>
        /// The list of all text streams.
        /// </value>
        public List<TextTrack> Text
        {
            get
            {
                if (this._Text == null)
                {
                    GetTextInfo();
                }
                return this._Text;
            }
        }

        /// <summary>
        /// List of all the Image streams available in the file, type ImageTrack[trackindex] to access a specific track
        /// </summary>
        /// <value>
        /// The list of all image streams.
        /// </value>
        public List<ImageTrack> Image
        {
            get
            {
                if (this._Image == null)
                {
                    GetImageInfo();
                }
                return this._Image;
            }
        }

        /// <summary>
        /// List of all the Menu streams available in the file, type MenuTrack[trackindex] to access a specific track
        /// </summary>
        /// <value>
        /// The list of all menu streams.
        /// </value>
        public List<MenuTrack> Menu
        {
            get
            {
                if (this._Menu == null)
                {
                    GetMenuInfo();
                }
                return this._Menu;
            }
        }
        #endregion

        #region Library information
        /// <summary>
        /// Lists media info capacities.
        /// </summary>
        /// <returns>The string that represents the formatted list of Mediainfo library capacities.</returns>
        public static string Capacities()
        {
            return Marshal.PtrToStringUni(MediaInfoLib.SetOption((IntPtr)0, "Info_Capacities", ""));
        }

        /// <summary>
        /// Lists all info parameters of the MediaInfo library.
        /// </summary>
        /// <returns>The string that represents the formatted list of all available parameters.</returns>
        public static string ParameterList()
        {
            return Marshal.PtrToStringUni(MediaInfoLib.SetOption((IntPtr)0, "Info_Parameters", ""));
        }
        
        /// <summary>
        /// Lists all supported codecs.
        /// </summary>
        /// <returns>The string that represents the formatted list of all supported codecs</returns>
        public static string KnownCodecs()
        {
            return Marshal.PtrToStringUni(MediaInfoLib.SetOption((IntPtr)0, "Info_Codecs", ""));
        }

        #region Helpers
        /// <summary>
        /// Gets every property for every track in a tracklist and get its value.
        /// </summary>
        /// <typeparam name="T">Type of tracklist, for instance List'VideoTrack'</typeparam>
        /// <param name="tracks">The track list</param>
        /// <param name="builder">The StringBuilder where append all properties strings.</param>
        private void ListEveryAvailablePropery<T>(List<T> tracks, StringBuilder builder)
        {
            foreach (T track in tracks)
            {
                foreach (PropertyInfo p in track.GetType().GetProperties())
                {
                    object value = p.GetValue(track, null);

                    if (value == null)
                    {
                        builder.AppendLine(p.Name + " : Not available");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(value.ToString()))
                        {
                            builder.AppendLine(p.Name + " : Not available");
                        }
                        else
                        {
                            builder.AppendLine(p.Name + " : " + value.ToString());
                        }
                    }
                }
            }
        }
        #endregion
        
        #endregion

        #endregion

        #region Helpers
        /// <summary>
        /// Changes the mediainfo option that determines whether to parse the cover data or not (only if the parseCoverData flag is true).
        /// </summary>
        /// <param name="flag">True, if the cover data parsing needs to be enabled; otherwise false.</param>
        private void EnableCoverDataParsing(bool flag)
        {
            if (this._ParseCoverData)
            {
                MediaInfoLib.SetOption(this.Handle, "Cover_Data", flag ? "base64" : "");
            }        
        }

        /// <summary>
        /// Gets the formatted bitrate string.
        /// </summary>
        /// <param name="bitrate">The bitrate.</param>
        /// <returns>the formatted bitrate string.</returns>
        private string GetFormattedBitrate(ulong? bitrate)
        {
            if (!bitrate.HasValue) { return null; }

            double doublebytes;

            if (bitrate >= 1099511627776)
            {
                doublebytes = (double)bitrate / (double)1099511627776;
                return string.Format("{0:0.##} Tbps", doublebytes);
            }
            else if (1073741824 < bitrate && bitrate < 1099511627775)
            {
                doublebytes = (double)bitrate / (double)1073741824;
                return string.Format("{0:0.##} Gbps", doublebytes);
            }
            else if (1048576 < bitrate && bitrate < 1073741823)
            {
                doublebytes = (double)bitrate / (double)1048576;
                return string.Format("{0:0.##} Mbps", doublebytes);
            }
            else if (1024 < bitrate && bitrate < 1048575)
            {
                doublebytes = (double)bitrate / (double)1024;
                return string.Format("{0:0.##} Kbps", doublebytes);
            }
            else if (0 < bitrate && bitrate < 1023)
            {
                doublebytes = (double)bitrate;
                return string.Format("{0:0.##} Bps", doublebytes);
            }

            return null;
        }

        /// <summary>
        /// Computes the overral bitrate.
        /// </summary>
        /// <param name="general">The GeneralTrack object.</param>
        /// <returns>The computed overral bitrate, 0 if not all necessary infos are available.</returns>
        private ulong? ComputeOverallBitRate(GeneralTrack general)
        {
            if (general.Duration > 0 && general.FileSize > 0)
            {
                return (ulong)((general.FileSize * 8) / ((ulong)general.Duration / 1000));
            }
            return null;
        }

        /// <summary>
        /// Parses channels count from channels string.
        /// </summary>
        /// <param name="channelstring">The formatted channel string.</param>
        /// <returns>Numbers of channels parsed from the input string, 0 in case of failure.</returns>
        private int? ParseChannelsCount(string channelstring)
        {
            var mat = new System.Text.RegularExpressions.Regex("(\\d)\\schannels").Match(channelstring);

            if (mat.Success)
            {
                if (int.TryParse(mat.Groups[1].Value, out int channels))
                {
                    return channels;
                }
            }
            return null;
        }

        /// <summary>
        /// Computes the total total milliseconds of a video track.
        /// </summary>
        /// <param name="track">The VideoTrack object.</param>
        /// <returns>The computed total milliseconds, 0 in case of failure.</returns>
        private int? ComputeDuration(VideoTrack track)
        {
            if (string.IsNullOrEmpty(track.DurationTimecode))
            {
                if (track.FrameCount > 0 && track.FrameRate > 0)
                {
                    return (int?)((track.FrameCount / track.FrameRate) * 1000);
                }
            }

            if (!string.IsNullOrEmpty(track.DurationTimecode))
            {
                TimeSpan.TryParse(track.DurationTimecode, out TimeSpan tmp);

                return (int?)tmp.TotalMilliseconds;
            }

            return null;
        }

        /// <summary>
        /// Helper method to parse an integer from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed integer or 0 in case of failure.</returns>
        private int? ParseInt(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            if (int.TryParse(input, out int value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Helper method to parse an ULong from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed ULong or 0 in case of failure.</returns>
        private ulong? ParseULong(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            if (ulong.TryParse(input, out ulong value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Helper method to parse an double from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed double or 0 in case of failure.</returns>
        private double? ParseDouble(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            if (double.TryParse(input.Replace(".", ","), out double value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Helper method to parse an bool from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed bool or "false" in case of failure.</returns>
        private bool? ParseBool(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            if (input.ToLower().Contains("yes")) { return true; }

            if (input.ToLower().Contains("no")) { return false; }

            if (bool.TryParse(input, out bool value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Helper method to parse a list from a string. Splits the input string using the input separator and removes trailing and leading whitespaces, if fails return an empty list.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The list with all splitted strings.</returns>
        private List<string> ParseList(string input, string separator)
        {
            List<string> output = new List<string>();

            if (separator.Length > 0 && string.IsNullOrEmpty(input))
            {
                foreach (string str in input.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    output.Add(str.Trim());
                }
            }

            return output;
        }

        /// <summary>
        /// Helper method to parse a cover data from a string. If multiple covers are present splits them.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The list with all splitted cover data.</returns>
        private List<byte[]> ParseCoverData(string input)
        {
            List<byte[]> list = new List<byte[]>();

            if (string.IsNullOrEmpty(input)) { return list; }

            if (input.Contains(" / "))
            {
                foreach (string str in input.Split(new string[] { " / " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    list.Add(Convert.FromBase64String(str));
                }
            }
            else
            {
                list.Add(Convert.FromBase64String(input));
            }

            return list;
        }
        #endregion

        #region Disposable
        /// <summary>
        /// Flags to detect multiple dispose calls.
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// Dispose all managed and unmanaged resources of this instance.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed objects or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //dispose managed resources
                }

                DisposeUnmanagedResources();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Default deconstructor
        /// </summary>
        ~MediaInfo()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose all managed and unmanaged resources of this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
            MediaInfoLib.CloseFile(this.Handle);
            MediaInfoLib.DeleteHandle(this.Handle);

            this.Handle = IntPtr.Zero;

            this._Video.Clear();
            this._General.Clear();
            this._Audio.Clear();
            this._Text.Clear();
            this._Image.Clear();
            this._Menu.Clear();

            this._Video = null;
            this._General = null;
            this._Audio = null;
            this._Text = null;
            this._Image = null;
            this._Menu = null;
        }
        #endregion
    }
}
