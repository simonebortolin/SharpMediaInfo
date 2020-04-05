using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace GetMediaInfo
{
    /// <summary>
    /// When called with a proper file target, returns a MediaInfo object filled with list of media tracks containing
    /// every information MediaInfo.dll can collect.
    /// Tracks are accessibles as properties.
    /// </summary>
    public class MediaInfo : IDisposable
    {
        #region ImportedMediaInfoFunctions
        [DllImport("MediaInfo.dll")]
        internal static extern int MediaInfo_Close(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        internal static extern int MediaInfo_Count_Get(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] StreamKind StreamKind, int StreamNumber);
        [DllImport("MediaInfo.dll")]
        internal static extern void MediaInfo_Delete(IntPtr Handle);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_Get(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] StreamKind StreamKind, uint StreamNumber, 
            string Parameter, [MarshalAs(UnmanagedType.U4)] InfoKind KindOfInfo, [MarshalAs(UnmanagedType.U4)] InfoKind KindOfSearch);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_GetI(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] StreamKind StreamKind, uint StreamNumber, 
            uint Parameter, [MarshalAs(UnmanagedType.U4)] InfoKind KindOfInfo);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_Inform(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] uint Reserved);
        [DllImport("MediaInfo.dll")]
        internal static extern IntPtr MediaInfo_New();
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern int MediaInfo_Open(IntPtr Handle, string FileName);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_Option(IntPtr Handle, string OptionString, string Value);
        [DllImport("MediaInfo.dll")]
        internal static extern int MediaInfo_State_Get(IntPtr Handle);
        #endregion

        #region GlobalVariables
        /// <summary>
        /// The list of video streams
        /// </summary>
        private List<VideoTrack> _Video;
        /// <summary>
        /// The list of general streams
        /// </summary>
        private List<GeneralTrack> _General;
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
        /// The video streams count
        /// </summary>
        private int _VideoCount;
        /// <summary>
        /// The general streams count
        /// </summary>
        private int _GeneralCount;
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
        /// <summary>
        /// The path of the file that it's analyzed
        /// </summary>
        private string _FileName;
        /// <summary>
        /// The handle to call mediainfo functions
        /// </summary>
        private IntPtr Handle;
        /// <summary>
        /// The flag to determine if the cover data must be parsed or not.
        /// </summary>
        private bool parseCoverData;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor :
        /// When called with a proper file target, returns a MediaInfo object filled with list of media tracks containing
        /// every information MediaInfo.dll can collect.
        /// Tracks are accessibles as properties.
        /// </summary>
        /// <param name="path">The path of the files to be analyzed.</param>
        /// <param name="parseCoverData">If set to <c>true</c> all covers data are parsed. (default: false)</param>
        /// <param name="parseReports">If set to <c>true</c> both specific and complete reports are immediately parsed. (default: false)</param>
        public MediaInfo(string path, bool parseCoverData = false, bool parseReports = false)
        {
#if DEBUG
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            long wct_open = 0;
            long wct_parsing = 0;
#endif
            if (!CheckFileExistence(path)) 
                return;

            this._FileName = path;
            this.parseCoverData = parseCoverData;

#if DEBUG
            sw.Start();
#endif

            try
            {
                //creates a new handle
                this.Handle = MediaInfo.MediaInfo_New();
                //forces to parse cover data
                ChangeCoverDataParsingOption(true);
                //opens the handle
                MediaInfo.MediaInfo_Open(this.Handle, path);

#if DEBUG
                sw.Stop();
                wct_open = sw.ElapsedMilliseconds;
                sw.Restart();
#endif     

                GetStreamCount();
                GetAllInfos();

                if (parseReports) {
                    GetInfoComplete();
                    GetInfoStandard();
                }
#if DEBUG
                sw.Stop();
                wct_parsing = sw.ElapsedMilliseconds;

                System.Diagnostics.Debug.WriteLine(string.Format("WCET Open: {0} ms", wct_open));
                System.Diagnostics.Debug.WriteLine(string.Format("WCET Parsing: {0} ms", wct_parsing));
#endif     
            }
            finally //ensure MediaInfo_Close is called even if something goes wrong 
            {
                MediaInfo.MediaInfo_Close(this.Handle);
            }
        }
#endregion

        #region Disposable Pattern
        private bool disposed;

        #pragma warning disable 1591
        ~MediaInfo()
        {
            Dispose(false);
        }

        protected bool IsDisposed
        {
            get { return disposed; }
        }

        protected void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
        }

        /// <summary>
        /// Call this one to kill the wrapper, and close his handle to the MediaInfo.dll, you should never need it anyway 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose methods
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    DisposeManagedResources();
                }
                DisposeUnmanagedResources();
            }
            this.disposed = true;
        }

        protected virtual void DisposeManagedResources()
        {

        }

        protected virtual void DisposeUnmanagedResources()
        {
            MediaInfo.MediaInfo_Close(this.Handle);
            MediaInfo.MediaInfo_Delete(this.Handle);
        }
        #endregion

        #region InfoGetters
        /// <summary>
        /// Gets all infos from the input file.
        /// </summary>
        private void GetAllInfos()
        {
            GetGeneralInfo();
            GetVideoInfo();
            GetAudioInfo();
            GetTextInfo();
            GetImageInfo();
            GetMenuInfo();
        }

        /// <summary>
        /// Gets all stream types count.
        /// </summary>
        private void GetStreamCount()
        {
            _AudioCount = MediaInfo.MediaInfo_Count_Get(this.Handle, StreamKind.Audio, -1);
            _VideoCount = MediaInfo.MediaInfo_Count_Get(this.Handle, StreamKind.Video, -1);
            _GeneralCount = MediaInfo.MediaInfo_Count_Get(this.Handle, StreamKind.General, -1);
            _TextCount = MediaInfo.MediaInfo_Count_Get(this.Handle, StreamKind.Text, -1);
            _ImageCount = MediaInfo.MediaInfo_Count_Get(this.Handle, StreamKind.Image, -1);
            _MenuCount = MediaInfo.MediaInfo_Count_Get(this.Handle, StreamKind.Menu, -1);
        }

        /// <summary>
        /// Gets the complete report.
        /// </summary>
        private void GetInfoComplete()
        {

            MediaInfo.MediaInfo_Option(this.Handle, "Complete", "1");
            ChangeCoverDataParsingOption(false);
            _InfoComplete = Marshal.PtrToStringUni(MediaInfo.MediaInfo_Inform(this.Handle, 0));
            ChangeCoverDataParsingOption(true);
        }

        /// <summary>
        /// Gets the standard report.
        /// </summary>
        private void GetInfoStandard()
        {
            MediaInfo.MediaInfo_Option(this.Handle, "Complete", "");
            ChangeCoverDataParsingOption(false);
            _InfoStandard = Marshal.PtrToStringUni(MediaInfo.MediaInfo_Inform(this.Handle, 0));
            ChangeCoverDataParsingOption(true);
        }

        /// <summary>
        /// Gets the specific information from mediainfo.
        /// </summary>
        /// <param name="KindOfStream">The kind of stream.</param>
        /// <param name="trackindex">The track index.</param>
        /// <param name="NameOfParameter">The name of parameter.</param>
        /// <returns>
        /// A string containing the parameter's value or an empty string in case of problems.
        /// </returns>
        private unsafe string GetSpecificInfo(StreamKind KindOfStream, int trackindex, string NameOfParameter)
        {
            IntPtr p = MediaInfo.MediaInfo_Get(this.Handle, KindOfStream, Convert.ToUInt32(trackindex), NameOfParameter, InfoKind.Text, InfoKind.Name);
            return ParseOutput(p);
        }

        /// <summary>
        /// Gets the specific sub information from mediainfo.
        /// </summary>
        /// <param name="KindOfStream">The kind of stream.</param>
        /// <param name="trackindex">The trackindex.</param>
        /// <param name="NameOfParameter">The name of parameter.</param>
        /// <param name="InfoKind">Kind of the information.</param>
        /// <returns>A string containing the parameter's value or an empty string in case of problems.</returns>
        private unsafe string GetSpecificSubInfo(StreamKind KindOfStream, int trackindex, int NameOfParameter, InfoKind InfoKind)
        {
            IntPtr p = MediaInfo.MediaInfo_GetI(this.Handle, KindOfStream, Convert.ToUInt32(trackindex), Convert.ToUInt32(NameOfParameter), InfoKind);
            return ParseOutput(p);
        }

        /// <summary>
        /// Parses the mediainfo library output. The reading is performed using a buffer of 8192 chars.
        /// </summary>
        /// <param name="p">The pointer to the memory area when the informations are stored.</param>
        /// <returns>The parsed string from the memory area referred to the input pointer.</returns>
        private unsafe string ParseOutput(IntPtr p)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int len = 8192;
            int index = 0;
            char* ptr = (char*)p.ToPointer();
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

        #region Helpers
        /// <summary>
        /// More detailed standard information.
        /// </summary>
        /// <value>
        /// The complete report from mediainfo.
        /// </value>
        public string InfoComplete
        {
            get
            {
                if (String.IsNullOrEmpty(this._InfoComplete))
                {
                    MediaInfo.MediaInfo_Open(this.Handle, _FileName);
                    GetInfoComplete();
                    MediaInfo.MediaInfo_Close(this.Handle);
                }
                return _InfoComplete;
            }
        }

        /// <summary>
        /// Detailed standard information.
        /// </summary>
        /// <value>
        /// The standard report from mediainfo.
        /// </value>
        public string InfoStandard
        {
            get
            {
                if (String.IsNullOrEmpty(this._InfoStandard))
                {
                    MediaInfo.MediaInfo_Open(this.Handle, _FileName);
                    GetInfoStandard();
                    MediaInfo.MediaInfo_Close(this.Handle);
                }
                return _InfoStandard;
            }
        }

        /// <summary>
        /// Lists every available property in every track
        /// </summary>
        /// <returns>The complete report with the lists of all infos of all streams and tracks</returns>
        public string InfoCustom
        {
            get
            {
                if (String.IsNullOrEmpty(_InfoCustom))
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

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

        /// <summary>
        /// Uses reflection to get every property for every track in a tracklist and get its value.
        /// </summary>
        /// <typeparam name="T1">Type of tracklist, for instance List'VideoTrack'</typeparam>
        /// <param name="L">tracklist, for instance Video</param>
        /// <param name="sb">The StringBuilder where append all properties strings.</param>
        private void ListEveryAvailablePropery<T1>(List<T1> L, System.Text.StringBuilder sb)
        {   
            foreach (T1 track in L)
            {
                foreach (PropertyInfo p in track.GetType().GetProperties() )
                {
                    object value = p.GetValue(track, null);

                    if (value == null)
                    {
                        sb.AppendLine(p.Name + " : Not available");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(value.ToString()))
                        {
                            sb.AppendLine(p.Name + " : Not available");
                        }
                        else
                        {
                            sb.AppendLine(p.Name + " : " + value.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Lists media info dll capacities
        /// </summary>
        /// <returns></returns>
        public static string Capacities()
        {
            return Marshal.PtrToStringUni(MediaInfo_Option((IntPtr)0, "Info_Capacities", ""));
        }

        /// <summary>
        /// Lists media info parameter list for MediaInfo_Get
        /// </summary>
        /// <returns></returns>
        private static string ParameterList()
        {
            return Marshal.PtrToStringUni(MediaInfo_Option((IntPtr)0, "Info_Parameters", ""));
        }

        /// <summary>
        /// Lists all supported codecs
        /// </summary>
        /// <returns></returns>
        public static string KnownCodecs()
        {
            return Marshal.PtrToStringUni(MediaInfo_Option((IntPtr)0, "Info_Codecs", ""));
        }
        #endregion

        #region GetListStreams

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
                        Count = ParseInt(GetSpecificInfo(StreamKind.General, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.General, index, "Status");
                        StreamCount = ParseInt(GetSpecificInfo(StreamKind.General, index, "StreamCount")),
                        StreamKind = GetSpecificInfo(StreamKind.General, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetSpecificInfo(StreamKind.General, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetSpecificInfo(StreamKind.General, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetSpecificInfo(StreamKind.General, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.General, index, "FirstPacketOrder");
                        Inform = GetSpecificInfo(StreamKind.General, index, "Inform"),
                        ID = ParseInt(GetSpecificInfo(StreamKind.General, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.General, index, "ID/String");
                        OriginalSourceMediumID = GetSpecificInfo(StreamKind.General, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.General, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetSpecificInfo(StreamKind.General, index, "UniqueID"),
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

                        VideoFormatList = ParseList(GetSpecificInfo(StreamKind.General, index, "Video_Format_List"), "/"),
                        //string VideoFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Video_Format_WithHint_List");
                        VideoLanguageList = ParseList(GetSpecificInfo(StreamKind.General, index, "Video_Language_List"), "/"),
                        AudioFormatList = ParseList(GetSpecificInfo(StreamKind.General, index, "Audio_Format_List"), "/"),
                        //string AudioFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Audio_Format_WithHint_List");
                        AudioLanguageList = ParseList(GetSpecificInfo(StreamKind.General, index, "Audio_Language_List"), "/"),
                        TextFormatList = ParseList(GetSpecificInfo(StreamKind.General, index, "Text_Format_List"), "/"),
                        //string TextFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Text_Format_WithHint_List");
                        TextLanguageList = ParseList(GetSpecificInfo(StreamKind.General, index, "Text_Language_List"), "/"),
                        //string OtherFormatList = getSpecificInfo(MediaInfoStreamKind.General, index, "Other_Format_List");
                        //string OtherFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Other_Format_WithHint_List");
                        //string OtherLanguageList = getSpecificInfo(MediaInfoStreamKind.General, index, "Other_Language_List");
                        ImageFormatList = ParseList(GetSpecificInfo(StreamKind.General, index, "Image_Format_List"), "/"),
                        //string ImageFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Image_Format_WithHint_List");
                        ImageLanguageList = ParseList(GetSpecificInfo(StreamKind.General, index, "Image_Language_List"), "/"),
                        MenuFormatList = ParseList(GetSpecificInfo(StreamKind.General, index, "Menu_Format_List"), "/"),
                        //string MenuFormatWithHintList = getSpecificInfo(MediaInfoStreamKind.General, index, "Menu_Format_WithHint_List");
                        MenuLanguageList = ParseList(GetSpecificInfo(StreamKind.General, index, "Menu_Language_List"), "/"),

                        CompleteName = GetSpecificInfo(StreamKind.General, index, "CompleteName"),
                        FolderName = GetSpecificInfo(StreamKind.General, index, "FolderName"),
                        FileNameExtension = GetSpecificInfo(StreamKind.General, index, "FileNameExtension"),
                        FileName = GetSpecificInfo(StreamKind.General, index, "FileName"),
                        FileExtension = GetSpecificInfo(StreamKind.General, index, "FileExtension"),
                        //string CompleteNameLast = getSpecificInfo(MediaInfoStreamKind.General, index, "CompleteName_Last");
                        //string FolderNameLast = getSpecificInfo(MediaInfoStreamKind.General, index, "FolderName_Last");
                        //string FileNameExtensionLast = getSpecificInfo(MediaInfoStreamKind.General, index, "FileNameExtension_Last");
                        //string FileNameLast = getSpecificInfo(MediaInfoStreamKind.General, index, "FileName_Last");
                        //string FileExtensionLast = getSpecificInfo(MediaInfoStreamKind.General, index, "FileExtension_Last");

                        Format = GetSpecificInfo(StreamKind.General, index, "Format"),
                        FormatWithExtra = GetSpecificInfo(StreamKind.General, index, "Format/String"),
                        FormatInfo = GetSpecificInfo(StreamKind.General, index, "Format/Info"),
                        FormatUrl = GetSpecificInfo(StreamKind.General, index, "Format/Url"),
                        FormatExtensions = GetSpecificInfo(StreamKind.General, index, "Format/Extensions"),
                        FormatCommercial = GetSpecificInfo(StreamKind.General, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.General, index, "Format_Commercial_IfAny");
                        FormatVersion = GetSpecificInfo(StreamKind.General, index, "Format_Version"),
                        FormatProfile = GetSpecificInfo(StreamKind.General, index, "Format_Profile"),
                        FormatCompression = GetSpecificInfo(StreamKind.General, index, "Format_Compression"),
                        FormatSettings = GetSpecificInfo(StreamKind.General, index, "Format_Settings"),
                        FormatAdditionalFeatures = GetSpecificInfo(StreamKind.General, index, "Format_AdditionalFeatures"),
                        InternetMediaType = GetSpecificInfo(StreamKind.General, index, "InternetMediaType"),

                        CodecID = ParseInt(GetSpecificInfo(StreamKind.General, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.General, index, "CodecID/String");
                        CodecIDInfo = GetSpecificInfo(StreamKind.General, index, "CodecID/Info"),
                        CodecIDHint = GetSpecificInfo(StreamKind.General, index, "CodecID/Hint"),
                        CodecIDUrl = GetSpecificInfo(StreamKind.General, index, "CodecID/Url"),
                        CodecIDDescription = GetSpecificInfo(StreamKind.General, index, "CodecID_Description"),
                        CodecIDVersion = GetSpecificInfo(StreamKind.General, index, "CodecID_Version"),
                        CodecIDCompatible = GetSpecificInfo(StreamKind.General, index, "CodecID_Compatible"),
                        Interleaved = ParseBool(GetSpecificInfo(StreamKind.General, index, "Interleaved")),

                        FileSize = ParseULong(GetSpecificInfo(StreamKind.General, index, "FileSize")),
                        FileSizeString = GetSpecificInfo(StreamKind.General, index, "FileSize/String"),
                        //string FileSizeString1 = getSpecificInfo(MediaInfoStreamKind.General, index, "FileSize/String1");
                        FileSizeStringRounded = GetSpecificInfo(StreamKind.General, index, "FileSize/String2"),
                        //string FileSizeString3 = getSpecificInfo(MediaInfoStreamKind.General, index, "FileSize/String3");
                        FileSizeStringAccurate = GetSpecificInfo(StreamKind.General, index, "FileSize/String4"),

                        Duration = ParseInt(GetSpecificInfo(StreamKind.General, index, "Duration")),
                        DurationFormatted = GetSpecificInfo(StreamKind.General, index, "Duration/String"),
                        DurationFormattedComplete = GetSpecificInfo(StreamKind.General, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration/String2");
                        DurationTimecode = GetSpecificInfo(StreamKind.General, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration/String5");
                        //string DurationStart = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration_Start");
                        //string DurationEnd = getSpecificInfo(MediaInfoStreamKind.General, index, "Duration_End");

                        OverallBitRateMode = GetSpecificInfo(StreamKind.General, index, "OverallBitRate_Mode"),
                        //string OverallBitRateModeString = getSpecificInfo(MediaInfoStreamKind.General, index, "OverallBitRate_Mode/String");
                        OverallBitRate = ParseULong(GetSpecificInfo(StreamKind.General, index, "OverallBitRate")),
                        OverallBitRateFormatted = GetSpecificInfo(StreamKind.General, index, "OverallBitRate/String"),
                        OverallBitRateMinimum = ParseULong(GetSpecificInfo(StreamKind.General, index, "OverallBitRate_Minimum")),
                        OverallBitRateMinimumFormatted = GetSpecificInfo(StreamKind.General, index, "OverallBitRate_Minimum/String"),
                        OverallBitRateNominal = ParseULong(GetSpecificInfo(StreamKind.General, index, "OverallBitRate_Nominal")),
                        OverallBitRateNominalFormatted = GetSpecificInfo(StreamKind.General, index, "OverallBitRate_Nominal/String"),
                        OverallBitRateMaximum = ParseULong(GetSpecificInfo(StreamKind.General, index, "OverallBitRate_Maximum")),
                        OverallBitRateMaximumFormatted = GetSpecificInfo(StreamKind.General, index, "OverallBitRate_Maximum/String"),

                        FrameRate = ParseDouble(GetSpecificInfo(StreamKind.General, index, "FrameRate")),
                        FrameRateString = GetSpecificInfo(StreamKind.General, index, "FrameRate/String"),
                        FrameRateNum = ParseInt(GetSpecificInfo(StreamKind.General, index, "FrameRate_Num")),
                        FrameRateDen = ParseInt(GetSpecificInfo(StreamKind.General, index, "FrameRate_Den")),
                        FrameCount = ParseInt(GetSpecificInfo(StreamKind.General, index, "FrameCount")),

                        Delay = ParseInt(GetSpecificInfo(StreamKind.General, index, "Delay")),
                        DelayFormatted = GetSpecificInfo(StreamKind.General, index, "Delay/String"),
                        DelayFormattedComplete = GetSpecificInfo(StreamKind.General, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.General, index, "Delay/String2");
                        DelayTimecode = GetSpecificInfo(StreamKind.General, index, "Delay/String3"),

                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.General, index, "Delay/String4"),
                        //string DelayString5 = GetSpecificInfo(MediaInfoStreamKind.General, index, "Delay/String5"),
                        DelaySettings = GetSpecificInfo(StreamKind.General, index, "Delay_Settings"),
                        DelayDropFrame = GetSpecificInfo(StreamKind.General, index, "Delay_DropFrame"),
                        DelaySource = GetSpecificInfo(StreamKind.General, index, "Delay_Source"),
                        //string DelaySourceString = getSpecificInfo(MediaInfoStreamKind.General, index, "Delay_Source/String"),

                        StreamSize = ParseULong(GetSpecificInfo(StreamKind.General, index, "StreamSize")),
                        StreamSizeFormatted = GetSpecificInfo(StreamKind.General, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize/String1"),
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize/String2"),
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize/String3"),
                        StreamSizePrecise = GetSpecificInfo(StreamKind.General, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.General, index, "StreamSize/String5"),
                        StreamSizeProportion = ParseDouble(GetSpecificInfo(StreamKind.General, index, "StreamSize_Proportion")),
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
                        AlbumReplayGainGain = ParseDouble(GetSpecificInfo(StreamKind.General, index, "Album_ReplayGain_Gain")),
                        //string AlbumReplayGainGainString = getSpecificInfo(MediaInfoStreamKind.General, index, "Album_ReplayGain_Gain/String"),
                        AlbumReplayGainPeak = ParseDouble(GetSpecificInfo(StreamKind.General, index, "Album_ReplayGain_Peak")),
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

                        Title = GetSpecificInfo(StreamKind.General, index, "Title"),
                        TitleMore = GetSpecificInfo(StreamKind.General, index, "Title_More"),
                        TitleUrl = GetSpecificInfo(StreamKind.General, index, "Title/Url"),
                        Domain = GetSpecificInfo(StreamKind.General, index, "Domain"),
                        Collection = GetSpecificInfo(StreamKind.General, index, "Collection"),
                        Season = GetSpecificInfo(StreamKind.General, index, "Season"),
                        SeasonPosition = ParseInt(GetSpecificInfo(StreamKind.General, index, "Season_Position")),
                        SeasonPositionTotal = GetSpecificInfo(StreamKind.General, index, "Season_Position_Total"),
                        Movie = GetSpecificInfo(StreamKind.General, index, "Movie"),
                        MovieMore = GetSpecificInfo(StreamKind.General, index, "Movie_More"),
                        MovieCountry = GetSpecificInfo(StreamKind.General, index, "Movie/Country"),
                        MovieUrl = GetSpecificInfo(StreamKind.General, index, "Movie/Url"),
                        Album = GetSpecificInfo(StreamKind.General, index, "Album"),
                        //string AlbumMore = getSpecificInfo(MediaInfoStreamKind.General, index, "Album_More"),
                        //string AlbumSort = getSpecificInfo(MediaInfoStreamKind.General, index, "Album/Sort"),
                        //string AlbumPerformer = getSpecificInfo(MediaInfoStreamKind.General, index, "Album/Performer"),
                        //string AlbumPerformerSort = getSpecificInfo(MediaInfoStreamKind.General, index, "Album/Performer/Sort"),
                        AlbumPerformerUrl = GetSpecificInfo(StreamKind.General, index, "Album/Performer/Url"),
                        Comic = GetSpecificInfo(StreamKind.General, index, "Comic"),
                        //string ComicMore = getSpecificInfo(MediaInfoStreamKind.General, index, "Comic_More"),
                        //string ComicPositionTotal = getSpecificInfo(MediaInfoStreamKind.General, index, "Comic/Position_Total"),
                        Part = GetSpecificInfo(StreamKind.General, index, "Part"),
                        PartPosition = ParseInt(GetSpecificInfo(StreamKind.General, index, "Part/Position")),
                        PartPositionTotal = GetSpecificInfo(StreamKind.General, index, "Part/Position_Total"),
                        Track = GetSpecificInfo(StreamKind.General, index, "Track"),
                        //string TrackMore = getSpecificInfo(MediaInfoStreamKind.General, index, "Track_More"),
                        TrackUrl = GetSpecificInfo(StreamKind.General, index, "Track/Url"),
                        //string TrackSort = getSpecificInfo(MediaInfoStreamKind.General, index, "Track/Sort"),
                        TrackPosition = ParseInt(GetSpecificInfo(StreamKind.General, index, "Track/Position")),
                        TrackPositionTotal = GetSpecificInfo(StreamKind.General, index, "Track/Position_Total"),
                        PackageName = GetSpecificInfo(StreamKind.General, index, "PackageName"),
                        Grouping = GetSpecificInfo(StreamKind.General, index, "Grouping"),
                        Chapter = GetSpecificInfo(StreamKind.General, index, "Chapter"),
                        SubTrack = GetSpecificInfo(StreamKind.General, index, "SubTrack"),
                        OriginalAlbum = GetSpecificInfo(StreamKind.General, index, "Original/Album"),
                        OriginalMovie = GetSpecificInfo(StreamKind.General, index, "Original/Movie"),
                        OriginalPart = GetSpecificInfo(StreamKind.General, index, "Original/Part"),
                        OriginalTrack = GetSpecificInfo(StreamKind.General, index, "Original/Track"),
                        Compilation = GetSpecificInfo(StreamKind.General, index, "Compilation"),
                        CompilationString = GetSpecificInfo(StreamKind.General, index, "Compilation/String"),
                        Performer = GetSpecificInfo(StreamKind.General, index, "Performer"),
                        //string PerformerSort = getSpecificInfo(MediaInfoStreamKind.General, index, "Performer/Sort"),
                        PerformerUrl = GetSpecificInfo(StreamKind.General, index, "Performer/Url"),
                        OriginalPerformer = GetSpecificInfo(StreamKind.General, index, "Original/Performer"),
                        Accompaniment = GetSpecificInfo(StreamKind.General, index, "Accompaniment"),
                        Composer = GetSpecificInfo(StreamKind.General, index, "Composer"),
                        ComposerNationality = GetSpecificInfo(StreamKind.General, index, "Composer/Nationality"),
                        Arranger = GetSpecificInfo(StreamKind.General, index, "Arranger"),
                        Lyricist = GetSpecificInfo(StreamKind.General, index, "Lyricist"),
                        OriginalLyricist = GetSpecificInfo(StreamKind.General, index, "Original/Lyricist"),
                        Conductor = GetSpecificInfo(StreamKind.General, index, "Conductor"),
                        Director = GetSpecificInfo(StreamKind.General, index, "Director"),
                        CoDirector = GetSpecificInfo(StreamKind.General, index, "CoDirector"),
                        AssistantDirector = GetSpecificInfo(StreamKind.General, index, "AssistantDirector"),
                        DirectorOfPhotography = GetSpecificInfo(StreamKind.General, index, "DirectorOfPhotography"),
                        SoundEngineer = GetSpecificInfo(StreamKind.General, index, "SoundEngineer"),
                        ArtDirector = GetSpecificInfo(StreamKind.General, index, "ArtDirector"),
                        ProductionDesigner = GetSpecificInfo(StreamKind.General, index, "ProductionDesigner"),
                        Choregrapher = GetSpecificInfo(StreamKind.General, index, "Choregrapher"),
                        CostumeDesigner = GetSpecificInfo(StreamKind.General, index, "CostumeDesigner"),
                        Actor = GetSpecificInfo(StreamKind.General, index, "Actor"),
                        ActorCharacter = GetSpecificInfo(StreamKind.General, index, "Actor_Character"),
                        WrittenBy = GetSpecificInfo(StreamKind.General, index, "WrittenBy"),
                        ScreenplayBy = GetSpecificInfo(StreamKind.General, index, "ScreenplayBy"),
                        EditedBy = GetSpecificInfo(StreamKind.General, index, "EditedBy"),
                        CommissionedBy = GetSpecificInfo(StreamKind.General, index, "CommissionedBy"),
                        Producer = GetSpecificInfo(StreamKind.General, index, "Producer"),
                        CoProducer = GetSpecificInfo(StreamKind.General, index, "CoProducer"),
                        ExecutiveProducer = GetSpecificInfo(StreamKind.General, index, "ExecutiveProducer"),
                        MusicBy = GetSpecificInfo(StreamKind.General, index, "MusicBy"),
                        DistributedBy = GetSpecificInfo(StreamKind.General, index, "DistributedBy"),
                        //string OriginalSourceFormDistributor = getSpecificInfo(MediaInfoStreamKind.General, index, "OriginalSourceForm/Distributor"),
                        MasteredBy = GetSpecificInfo(StreamKind.General, index, "MasteredBy"),
                        EncodedBy = GetSpecificInfo(StreamKind.General, index, "EncodedBy"),
                        RemixedBy = GetSpecificInfo(StreamKind.General, index, "RemixedBy"),
                        ProductionStudio = GetSpecificInfo(StreamKind.General, index, "ProductionStudio"),
                        ThanksTo = GetSpecificInfo(StreamKind.General, index, "ThanksTo"),
                        Publisher = GetSpecificInfo(StreamKind.General, index, "Publisher"),
                        PublisherURL = GetSpecificInfo(StreamKind.General, index, "Publisher/URL"),
                        Label = GetSpecificInfo(StreamKind.General, index, "Label"),
                        Genre = GetSpecificInfo(StreamKind.General, index, "Genre"),
                        Mood = GetSpecificInfo(StreamKind.General, index, "Mood"),
                        ContentType = GetSpecificInfo(StreamKind.General, index, "ContentType"),
                        Subject = GetSpecificInfo(StreamKind.General, index, "Subject"),
                        Description = GetSpecificInfo(StreamKind.General, index, "Description"),
                        Keywords = GetSpecificInfo(StreamKind.General, index, "Keywords"),
                        Summary = GetSpecificInfo(StreamKind.General, index, "Summary"),
                        Synopsis = GetSpecificInfo(StreamKind.General, index, "Synopsis"),
                        Period = GetSpecificInfo(StreamKind.General, index, "Period"),
                        LawRating = GetSpecificInfo(StreamKind.General, index, "LawRating"),
                        LawRatingReason = GetSpecificInfo(StreamKind.General, index, "LawRating_Reason"),
                        ICRA = GetSpecificInfo(StreamKind.General, index, "ICRA"),
                        ReleasedDate = GetSpecificInfo(StreamKind.General, index, "Released_Date"),
                        OriginalReleasedDate = GetSpecificInfo(StreamKind.General, index, "Original/Released_Date"),
                        RecordedDate = GetSpecificInfo(StreamKind.General, index, "Recorded_Date"),
                        EncodedDate = GetSpecificInfo(StreamKind.General, index, "Encoded_Date"),
                        TaggedDate = GetSpecificInfo(StreamKind.General, index, "Tagged_Date"),
                        WrittenDate = GetSpecificInfo(StreamKind.General, index, "Written_Date"),
                        MasteredDate = GetSpecificInfo(StreamKind.General, index, "Mastered_Date"),
                        FileCreatedDate = GetSpecificInfo(StreamKind.General, index, "File_Created_Date"),
                        FileCreatedDateLocal = GetSpecificInfo(StreamKind.General, index, "File_Created_Date_Local"),
                        FileModifiedDate = GetSpecificInfo(StreamKind.General, index, "File_Modified_Date"),
                        FileModifiedDateLocal = GetSpecificInfo(StreamKind.General, index, "File_Modified_Date_Local"),
                        RecordedLocation = GetSpecificInfo(StreamKind.General, index, "Recorded_Location"),
                        WrittenLocation = GetSpecificInfo(StreamKind.General, index, "Written_Location"),
                        ArchivalLocation = GetSpecificInfo(StreamKind.General, index, "Archival_Location"),

                        EncodedApplication = GetSpecificInfo(StreamKind.General, index, "Encoded_Application"),
                        EncodedApplicationString = GetSpecificInfo(StreamKind.General, index, "Encoded_Application/Strin"),
                        EncodedApplicationCompany = GetSpecificInfo(StreamKind.General, index, "Encoded_Application_Compa"),
                        EncodedApplicationName = GetSpecificInfo(StreamKind.General, index, "Encoded_Application_Name"),
                        EncodedApplicationVersion = GetSpecificInfo(StreamKind.General, index, "Encoded_Application_Versi"),
                        EncodedApplicationUrl = GetSpecificInfo(StreamKind.General, index, "Encoded_Application_Url"),
                        EncodedLibrary = GetSpecificInfo(StreamKind.General, index, "Encoded_Library"),
                        EncodedLibraryString = GetSpecificInfo(StreamKind.General, index, "Encoded_Library/String"),
                        EncodedLibraryCompanyName = GetSpecificInfo(StreamKind.General, index, "Encoded_Library_CompanyNa"),
                        EncodedLibraryName = GetSpecificInfo(StreamKind.General, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetSpecificInfo(StreamKind.General, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetSpecificInfo(StreamKind.General, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetSpecificInfo(StreamKind.General, index, "Encoded_Library_Settings"),
                        EncodedOperatingSystem = GetSpecificInfo(StreamKind.General, index, "Encoded_OperatingSystem"),

                        Cropped = GetSpecificInfo(StreamKind.General, index, "Cropped"),
                        Dimensions = GetSpecificInfo(StreamKind.General, index, "Dimensions"),
                        DotsPerInch = GetSpecificInfo(StreamKind.General, index, "DotsPerInch"),
                        Lightness = GetSpecificInfo(StreamKind.General, index, "Lightness"),
                        OriginalSourceMedium = GetSpecificInfo(StreamKind.General, index, "OriginalSourceMedium"),
                        OriginalSourceForm = GetSpecificInfo(StreamKind.General, index, "OriginalSourceForm"),
                        OriginalSourceFormNumColour = GetSpecificInfo(StreamKind.General, index, "OriginalSourceForm/NumCol"),
                        OriginalSourceFormName = GetSpecificInfo(StreamKind.General, index, "OriginalSourceForm/Name"),
                        OriginalSourceFormCropped = GetSpecificInfo(StreamKind.General, index, "OriginalSourceForm/Croppe"),
                        OriginalSourceFormSharpness = GetSpecificInfo(StreamKind.General, index, "OriginalSourceForm/Sharpn"),
                        TaggedApplication = GetSpecificInfo(StreamKind.General, index, "Tagged_Application"),
                        BPM = ParseInt(GetSpecificInfo(StreamKind.General, index, "BPM")),
                        ISRC = GetSpecificInfo(StreamKind.General, index, "ISRC"),
                        ISBN = GetSpecificInfo(StreamKind.General, index, "ISBN"),
                        BarCode = GetSpecificInfo(StreamKind.General, index, "BarCode"),
                        LCCN = GetSpecificInfo(StreamKind.General, index, "LCCN"),
                        CatalogNumber = GetSpecificInfo(StreamKind.General, index, "CatalogNumber"),
                        LabelCode = GetSpecificInfo(StreamKind.General, index, "LabelCode"),
                        Owner = GetSpecificInfo(StreamKind.General, index, "Owner"),
                        Copyright = GetSpecificInfo(StreamKind.General, index, "Copyright"),
                        CopyrightUrl = GetSpecificInfo(StreamKind.General, index, "Copyright/Url"),
                        ProducerCopyright = GetSpecificInfo(StreamKind.General, index, "Producer_Copyright"),
                        TermsOfUse = GetSpecificInfo(StreamKind.General, index, "TermsOfUse"),

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

                        Cover = ParseBool(GetSpecificInfo(StreamKind.General, index, "Cover")),
                        CoverDescription = ParseList(GetSpecificInfo(StreamKind.General, index, "Cover_Description"), " / "),
                        CoverType = ParseList(GetSpecificInfo(StreamKind.General, index, "Cover_Type"), " / "),
                        //string CoverMime = getSpecificInfo(MediaInfoStreamKind.General, index, "Cover_Mime"),

                        Lyrics = GetSpecificInfo(StreamKind.General, index, "Lyrics"),
                        Comment = GetSpecificInfo(StreamKind.General, index, "Comment"),
                        Rating = ParseInt(GetSpecificInfo(StreamKind.General, index, "Rating")),
                        AddedDate = GetSpecificInfo(StreamKind.General, index, "Added_Date"),
                        PlayedFirstDate = GetSpecificInfo(StreamKind.General, index, "Played_First_Date"),
                        PlayedLastDate = GetSpecificInfo(StreamKind.General, index, "Played_Last_Date"),
                        PlayedCount = ParseInt(GetSpecificInfo(StreamKind.General, index, "Played_Count")),
                        //string EPGPositionsBegin = getSpecificInfo(MediaInfoStreamKind.General, index, "EPG_Positions_Begin"),
                        //string EPGPositionsEnd = getSpecificInfo(MediaInfoStreamKind.General, index, "EPG_Positions_End"),
                    };

                    //get cover data if available
                    if (tmp.Cover.GetValueOrDefault())
                    {              
                        tmp.CoverData = ParseCoverData(GetSpecificInfo(StreamKind.General, index, "Cover_Data"));
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
                        Count = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Video, index, "Status");
                        StreamCount = ParseInt(GetSpecificInfo(StreamKind.Video, index, "StreamCount")),
                        StreamKind = GetSpecificInfo(StreamKind.Video, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetSpecificInfo(StreamKind.Video, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetSpecificInfo(StreamKind.Video, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetSpecificInfo(StreamKind.Video, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Video, index, "FirstPacketOrder");
                        Inform = GetSpecificInfo(StreamKind.Video, index, "Inform"),
                        ID = ParseInt(GetSpecificInfo(StreamKind.Video, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ID/String");
                        OriginalSourceMediumID = GetSpecificInfo(StreamKind.Video, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDS = getSpecificInfo(MediaInfoStreamKind.Video, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetSpecificInfo(StreamKind.Video, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Video, index, "UniqueID/String");
                        //string MenuID = getSpecificInfo(MediaInfoStreamKind.Video, index, "MenuID");
                        //string MenuIDString = getSpecificInfo(MediaInfoStreamKind.Video, index, "MenuID/String");

                        Format = GetSpecificInfo(StreamKind.Video, index, "Format"),
                        FormatWithExtra = GetSpecificInfo(StreamKind.Video, index, "Format/String"),
                        FormatInfo = GetSpecificInfo(StreamKind.Video, index, "Format/Info"),
                        FormatUrl = GetSpecificInfo(StreamKind.Video, index, "Format/Url"),
                        FormatCommercial = GetSpecificInfo(StreamKind.Video, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Commercial_IfAny");
                        FormatLevel = GetSpecificInfo(StreamKind.Video, index, "Format_Level"),
                        FormatTier = GetSpecificInfo(StreamKind.Video, index, "Format_Tier"),
                        FormatVersion = GetSpecificInfo(StreamKind.Video, index, "Format_Version"),
                        FormatProfile = GetSpecificInfo(StreamKind.Video, index, "Format_Profile"),
                        FormatCompression = GetSpecificInfo(StreamKind.Video, index, "Format_Compression"),
                        FormatAdditionalFeatures = GetSpecificInfo(StreamKind.Video, index, "Format_AdditionalFeatures"),

                        MultiViewBaseProfile = GetSpecificInfo(StreamKind.Video, index, "MultiView_BaseProfile"),
                        MultiViewCount = ParseInt(GetSpecificInfo(StreamKind.Video, index, "MultiView_Count")),
                        MultiViewLayout = GetSpecificInfo(StreamKind.Video, index, "MultiView_Layout"),

                        DolbyVision = GetSpecificInfo(StreamKind.Video, index, "DolbyVision/String"),
                        DolbyVisionVersion = GetSpecificInfo(StreamKind.Video, index, "DolbyVision_Version"),
                        DolbyVisionProfile = GetSpecificInfo(StreamKind.Video, index, "DolbyVision_Profile"),
                        DolbyVisionLevel = GetSpecificInfo(StreamKind.Video, index, "DolbyVision_Level"),
                        DolbyVisionLayers = GetSpecificInfo(StreamKind.Video, index, "DolbyVision_Layers"),

                        FormatSettings = GetSpecificInfo(StreamKind.Video, index, "Format_Settings"),
                        FormatSettingsBVOP = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_BVOP"),
                        //string FormatSettingsBVOPStri = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_BVOP/String");
                        FormatSettingsQPel = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_QPel"),
                        //string FormatSettingsQPelString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_QPel/String");
                        FormatSettingsGMC = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_GMC"),
                        //string FormatSettingsGMCString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_GMC/String");
                        FormatSettingsMatrix = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_Matrix"),
                        //string FormatSettingsMatrixString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_Matrix/String");
                        //string FormatSettingsMatrixData = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_Matrix_Data");
                        FormatSettingsCABAC = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_CABAC"),
                        //string FormatSettingsCABACString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Format_Settings_CABAC/String");
                        FormatSettingsRefFrames = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_RefFrames"),
                        FormatSettingsPulldown = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_Pulldown"),
                        FormatSettingsEndianness = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_Endianness"),
                        FormatSettingsPacking = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_Packing"),
                        FormatSettingsFrameMode = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_FrameMode"),
                        FormatSettingsGOP = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_GOP"),
                        FormatSettingsPictureString = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_PictureString"),
                        FormatSettingsWrapping = GetSpecificInfo(StreamKind.Video, index, "Format_Settings_Wrapping"),
                        InternetMediaType = GetSpecificInfo(StreamKind.Video, index, "InternetMediaType"),

                        MuxingMode = GetSpecificInfo(StreamKind.Video, index, "MuxingMode"),
                        CodecID = ParseInt(GetSpecificInfo(StreamKind.Video, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Video, index, "CodecID/String");
                        CodecIDInfo = GetSpecificInfo(StreamKind.Video, index, "CodecID/Info"),
                        CodecIDHint = GetSpecificInfo(StreamKind.Video, index, "CodecID/Hint"),
                        CodecIDUrl = GetSpecificInfo(StreamKind.Video, index, "CodecID/Url"),
                        CodecIDDescription = GetSpecificInfo(StreamKind.Video, index, "CodecID_Description"),

                        Duration = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Duration")),
                        DurationFormatted = GetSpecificInfo(StreamKind.Video, index, "Duration/String"),
                        DurationFormattedComplete = GetSpecificInfo(StreamKind.Video, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration/String2");
                        DurationTimecode = GetSpecificInfo(StreamKind.Video, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration/String5");
                        DurationFirstFrame = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Duration_FirstFrame")),
                        DurationFirstFrameFormatted = GetSpecificInfo(StreamKind.Video, index, "Duration_FirstFrame/String"),
                        DurationFirstFrameFormattedComplete = GetSpecificInfo(StreamKind.Video, index, "Duration_FirstFrame/String1"),
                        //string DurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_FirstFrame/String2");
                        DurationFirstFrameTimecode = GetSpecificInfo(StreamKind.Video, index, "Duration_FirstFrame/String3"),
                        //string DurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_FirstFrame/String4");
                        //string DurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_FirstFrame/String5");
                        DurationLastFrame = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Duration_LastFrame")),
                        DurationLastFrameFormatted = GetSpecificInfo(StreamKind.Video, index, "Duration_LastFrame/String"),
                        DurationLastFrameFormattedComplete = GetSpecificInfo(StreamKind.Video, index, "Duration_LastFrame/String1"),
                        //string DurationLastFrameString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_LastFrame/String2");
                        DurationLastFrameTimecode = GetSpecificInfo(StreamKind.Video, index, "Duration_LastFrame/String3"),
                        //string DurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_LastFrame/String4");
                        //string DurationLastFrameString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Duration_LastFrame/String5");
                        SourceDuration = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Source_Duration")),
                        SourceDurationFormatted = GetSpecificInfo(StreamKind.Video, index, "Source_Duration/String"),
                        SourceDurationFormattedComplete = GetSpecificInfo(StreamKind.Video, index, "Source_Duration/String1"),
                        //string SourceDurationString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration/String2");
                        SourceDurationTimecode = GetSpecificInfo(StreamKind.Video, index, "Source_Duration/String3"),
                        //string SourceDurationString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration/String4");
                        //string SourceDurationString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration/String5");
                        SourceDurationFirstFrame = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Source_Duration_FirstFrame")),
                        SourceDurationFirstFrameFormatted = GetSpecificInfo(StreamKind.Video, index, "Source_Duration_FirstFrame/String"),
                        SourceDurationFirstFrameFormattedComplete = GetSpecificInfo(StreamKind.Video, index, "Source_Duration_FirstFrame/String1"),
                        //string SourceDurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_FirstFrame/String2");
                        SourceDurationFirstFrameTimecode = GetSpecificInfo(StreamKind.Video, index, "Source_Duration_FirstFrame/String3"),
                        //string SourceDurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_FirstFrame/String4");
                        //string SourceDurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_FirstFrame/String5");
                        SourceDurationLastFrame = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Source_Duration_LastFrame")),
                        SourceDurationLastFrameFormatted = GetSpecificInfo(StreamKind.Video, index, "Source_Duration_LastFrame/String"),
                        SourceDurationLastFrameFormattedComplete = GetSpecificInfo(StreamKind.Video, index, "Source_Duration_LastFrame/String1"),
                        //string SourceDurationLastFramestring2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_LastFrame/String2");
                        SourceDurationLastFrameTimecode = GetSpecificInfo(StreamKind.Video, index, "Source_Duration_LastFrame/String3"),
                        //string SourceDurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_LastFrame/String4");
                        //string SourceDurationLastFramestring5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_Duration_LastFrame/String5");

                        BitRateMode = GetSpecificInfo(StreamKind.Video, index, "BitRate_Mode"),
                        //string BitRateModeString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Mode/String");
                        BitRate = ParseULong(GetSpecificInfo(StreamKind.Video, index, "BitRate")),
                        //string BitRateString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate/String");
                        BitRateMinimum = ParseULong(GetSpecificInfo(StreamKind.Video, index, "BitRate_Minimum")),
                        //string BitRateMinimumString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Minimum/String");
                        BitRateNominal = ParseULong(GetSpecificInfo(StreamKind.Video, index, "BitRate_Nominal")),
                        //string BitRateNominalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Nominal/String");
                        BitRateMaximum = ParseULong(GetSpecificInfo(StreamKind.Video, index, "BitRate_Maximum")),
                        //string BitRateMaximumString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Maximum/String");
                        BitRateEncoded = ParseULong(GetSpecificInfo(StreamKind.Video, index, "BitRate_Encoded")),
                        //string BitRateEncodedString = getSpecificInfo(MediaInfoStreamKind.Video, index, "BitRate_Encoded/String");

                        Width = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Width")),
                        //string WidthString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Width/String");
                        WidthOffset = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Width_Offset")),
                        //string WidthOffsetString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Width_Offset/String");
                        WidthOriginal = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Width_Original")),
                        //string WidthOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Width_Original/String");
                        WidthCleanAperture = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Width_CleanAperture")),
                        //string WidthCleanApertureString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Width_CleanAperture/String");
                        Height = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Height")),
                        //string HeightString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Height/String");
                        HeightOffset = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Height_Offset")),
                        //string HeightOffsetString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Height_Offset/String");
                        HeightOriginal = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Height_Original")),
                        //string HeightOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Height_Original/String");
                        HeightCleanAperture = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Height_CleanAperture")),
                        //string HeightCleanApertureStrig = getSpecificInfo(MediaInfoStreamKind.Video, index, "Height_CleanAperture/String");
                        StoredWidth = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Stored_Width")),
                        StoredHeight = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Stored_Height")),
                        SampledWidth = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Sampled_Width")),
                        SampledHeight = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Sampled_Height")),

                        PixelAspectRatio = ParseInt(GetSpecificInfo(StreamKind.Video, index, "PixelAspectRatio")),
                        PixelAspectRatioFormatted = GetSpecificInfo(StreamKind.Video, index, "PixelAspectRatio/String"),
                        PixelAspectRatioOriginal = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "PixelAspectRatio_Original")),
                        PixelAspectRatioOriginalFormatted = GetSpecificInfo(StreamKind.Video, index, "PixelAspectRatio_Original/String"),
                        PixelAspectRatioCleanAperture = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "PixelAspectRatio_CleanAperture")),
                        PixelAspectRatioCleanApertureFormatted = GetSpecificInfo(StreamKind.Video, index, "PixelAspectRatio_CleanAperture/String"),
                        DisplayAspectRatio = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "DisplayAspectRatio")),
                        DisplayAspectRatioFormatted = GetSpecificInfo(StreamKind.Video, index, "DisplayAspectRatio/String"),
                        DisplayAspectRatioOriginal = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "DisplayAspectRatio_Original")),
                        DisplayAspectRatioOriginalFormatted = GetSpecificInfo(StreamKind.Video, index, "DisplayAspectRatio_Original/String"),
                        DisplayAspectRatioCleanAperture = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "DisplayAspectRatio_CleanAperture")),
                        DisplayAspectRatioCleanApertureFormatted = GetSpecificInfo(StreamKind.Video, index, "DisplayAspectRatio_CleanAperture/String"),
                        ActiveFormatDescription = GetSpecificInfo(StreamKind.Video, index, "ActiveFormatDescription"),
                        //string ActiveFormatDescriptionString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ActiveFormatDescription/String");
                        ActiveFormatDescriptionMuxingMode = GetSpecificInfo(StreamKind.Video, index, "ActiveFormatDescription_MuxingMode"),
                        Rotation = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Rotation")),
                        //string RotationString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Rotation/String");

                        FrameRateMode = GetSpecificInfo(StreamKind.Video, index, "FrameRate_Mode"),
                        //string FrameRateModeString = getSpecificInfo(MediaInfoStreamKind.Video, index, "FrameRate_Mode/String");
                        FrameRateModeOriginal = GetSpecificInfo(StreamKind.Video, index, "FrameRate_Mode_Original"),
                        //string FrameRateModeOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "FrameRate_Mode_Original/String");
                        FrameRate = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "FrameRate")),
                        FrameRateFormatted = GetSpecificInfo(StreamKind.Video, index, "FrameRate/String"),
                        FrameRateNum = ParseInt(GetSpecificInfo(StreamKind.Video, index, "FrameRate_Num")),
                        FrameRateDen = ParseInt(GetSpecificInfo(StreamKind.Video, index, "FrameRate_Den")),
                        FrameRateMinimum = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "FrameRate_Minimum")),
                        FrameRateMinimumFormatted = GetSpecificInfo(StreamKind.Video, index, "FrameRate_Minimum/String"),
                        FrameRateNominal = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "FrameRate_Nominal")),
                        FrameRateNominalFormatted = GetSpecificInfo(StreamKind.Video, index, "FrameRate_Nominal/String"),
                        FrameRateMaximum = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "FrameRate_Maximum")),
                        FrameRateMaximumFormatted = GetSpecificInfo(StreamKind.Video, index, "FrameRate_Maximum/String"),
                        FrameRateOriginal = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "FrameRate_Original")),
                        FrameRateOriginalFormatted = GetSpecificInfo(StreamKind.Video, index, "FrameRate_Original/String"),
                        FrameRateOriginalNum = ParseInt(GetSpecificInfo(StreamKind.Video, index, "FrameRate_Original_Num")),
                        FrameRateOriginalDen = ParseInt(GetSpecificInfo(StreamKind.Video, index, "FrameRate_Original_Den")),
                        FrameCount = ParseInt(GetSpecificInfo(StreamKind.Video, index, "FrameCount")),
                        SourceFrameCount = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Source_FrameCount")),

                        Standard = GetSpecificInfo(StreamKind.Video, index, "Standard"),
                        ColorSpace = GetSpecificInfo(StreamKind.Video, index, "ColorSpace"),
                        ChromaSubsampling = GetSpecificInfo(StreamKind.Video, index, "ChromaSubsampling"),
                        //string ChromaSubsamplingString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ChromaSubsampling/String");
                        ChromaSubsamplingPosition = ParseInt(GetSpecificInfo(StreamKind.Video, index, "ChromaSubsampling_Position")),
                        BitDepth = ParseInt(GetSpecificInfo(StreamKind.Video, index, "BitDepth")),
                        BitDepthFormatted = GetSpecificInfo(StreamKind.Video, index, "BitDepth/String"),
                        ScanType = GetSpecificInfo(StreamKind.Video, index, "ScanType"),
                        //string ScanTypeString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanType/String");
                        ScanTypeOriginal = GetSpecificInfo(StreamKind.Video, index, "ScanType_Original"),
                        //string ScanTypeOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanType_Original/String");
                        ScanTypeStoreMethod = GetSpecificInfo(StreamKind.Video, index, "ScanType_StoreMethod"),
                        ScanTypeStoreMethodFieldsPerBlock = GetSpecificInfo(StreamKind.Video, index, "ScanType_StoreMethod_FieldsPerBlock"),
                        //string ScanTypeStoreMethodString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanType_StoreMethod/String");
                        ScanOrder = GetSpecificInfo(StreamKind.Video, index, "ScanOrder"),
                        //string ScanOrderString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanOrder/String");
                        ScanOrderStored = GetSpecificInfo(StreamKind.Video, index, "ScanOrder_Stored"),
                        //string ScanOrderStoredString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanOrder_Stored/String");
                        ScanOrderStoredDisplayedInverted = GetSpecificInfo(StreamKind.Video, index, "ScanOrder_StoredDisplayedInverted"),
                        ScanOrderOriginal = GetSpecificInfo(StreamKind.Video, index, "ScanOrder_Original"),
                        //string ScanOrderOriginalString = getSpecificInfo(MediaInfoStreamKind.Video, index, "ScanOrder_Original/String");
                        CompressionMode = GetSpecificInfo(StreamKind.Video, index, "Compression_Mode"),
                        //string CompressionModeString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Compression_Mode/String");
                        CompressionRatio = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "Compression_Ratio")),
                        BitsPixelFrame = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "Bits-(Pixel*Frame)")),

                        Delay = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Delay")),
                        DelayFormatted = GetSpecificInfo(StreamKind.Video, index, "Delay/String"),
                        DelayFormattedComplete = GetSpecificInfo(StreamKind.Video, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay/String2");
                        DelayTimecode = GetSpecificInfo(StreamKind.Video, index, "Delay/String3"),
                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay/String4");
                        //string DelayString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay/String5");
                        DelaySettings = GetSpecificInfo(StreamKind.Video, index, "Delay_Settings"),
                        DelayDropFrame = GetSpecificInfo(StreamKind.Video, index, "Delay_DropFrame"),
                        DelaySource = GetSpecificInfo(StreamKind.Video, index, "Delay_Source"),
                        //string DelaySourceString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay_Source/String");
                        DelayOriginal = ParseInt(GetSpecificInfo(StreamKind.Video, index, "Delay_Original")),
                        DelayOriginalFormatted = GetSpecificInfo(StreamKind.Video, index, "Delay_Original/String"),
                        DelayOriginalFormattedComplete = GetSpecificInfo(StreamKind.Video, index, "Delay_Original/String1"),
                        //string DelayOriginalString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay_Original/String2");
                        DelayOriginalTimecode = GetSpecificInfo(StreamKind.Video, index, "Delay_Original/String3"),
                        //string DelayOriginalString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay_Original/String4");
                        //string DelayOriginalString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Delay_Original/String5");
                        DelayOriginalSettings = GetSpecificInfo(StreamKind.Video, index, "Delay_Original_Settings"),
                        DelayOriginalDropFrame = GetSpecificInfo(StreamKind.Video, index, "Delay_Original_DropFrame"),
                        DelayOriginalSource = GetSpecificInfo(StreamKind.Video, index, "Delay_Original_Source"),

                        TimeStampFirstFrame = ParseInt(GetSpecificInfo(StreamKind.Video, index, "TimeStamp_FirstFrame")),
                        TimeStampFirstFrameFormatted = GetSpecificInfo(StreamKind.Video, index, "TimeStamp_FirstFrame/String"),
                        TimeStampFirstFrameFormattedComplete = GetSpecificInfo(StreamKind.Video, index, "TimeStamp_FirstFrame/String1"),
                        //string TimeStampFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "TimeStamp_FirstFrame/String2");
                        TimeStampFirstFrameTimecode = GetSpecificInfo(StreamKind.Video, index, "TimeStamp_FirstFrame/String3"),
                        //string TimeStampFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "TimeStamp_FirstFrame/String4");
                        //string TimeStampFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "TimeStamp_FirstFrame/String5");
                        TimeCodeFirstFrame = GetSpecificInfo(StreamKind.Video, index, "TimeCode_FirstFrame"),
                        TimeCodeSettings = GetSpecificInfo(StreamKind.Video, index, "TimeCode_Settings"),
                        TimeCodeSource = GetSpecificInfo(StreamKind.Video, index, "TimeCode_Source"),
                        GopOpenClosed = GetSpecificInfo(StreamKind.Video, index, "Gop_OpenClosed"),
                        //string GopOpenClosedString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Gop_OpenClosed/String");
                        GopOpenClosedFirstFrame = GetSpecificInfo(StreamKind.Video, index, "Gop_OpenClosed_FirstFrame"),
                        //string GopOpenClosedFirstFrameString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Gop_OpenClosed_FirstFrame/String");

                        StreamSize = ParseULong(GetSpecificInfo(StreamKind.Video, index, "StreamSize")),
                        StreamSizeFormatted = GetSpecificInfo(StreamKind.Video, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize/String1");
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize/String2");
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize/String3");
                        StreamSizePrecise = GetSpecificInfo(StreamKind.Video, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize/String5");
                        StreamSizeProportion = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "StreamSize_Proportion")),
                        //string StreamSizeDemuxed = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed");
                        //string StreamSizeDemuxedString = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String");
                        //string StreamSizeDemuxedString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String1");
                        //string StreamSizeDemuxedString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String2");
                        //string StreamSizeDemuxedString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String3");
                        //string StreamSizeDemuxedString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String4");
                        //string StreamSizeDemuxedString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Demuxed/String5");
                        SourceStreamSize = ParseULong(GetSpecificInfo(StreamKind.Video, index, "Source_StreamSize")),
                        SourceStreamSizeFormatted = GetSpecificInfo(StreamKind.Video, index, "Source_StreamSize/String"),
                        //string SourceStreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize/String1");
                        //string SourceStreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize/String2");
                        //string SourceStreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize/String3");
                        SourceStreamSizePrecise = GetSpecificInfo(StreamKind.Video, index, "Source_StreamSize/String4"),
                        //string SourceStreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize/String5");
                        SourceStreamSizeProportion = GetSpecificInfo(StreamKind.Video, index, "Source_StreamSize_Proportion"),
                        StreamSizeEncoded = ParseULong(GetSpecificInfo(StreamKind.Video, index, "StreamSize_Encoded")),
                        StreamSizeEncodedFormatted = GetSpecificInfo(StreamKind.Video, index, "StreamSize_Encoded/String"),
                        //string StreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Encoded/String1");
                        //string StreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Encoded/String2");
                        //string StreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Encoded/String3");
                        StreamSizeEncodedPrecise = GetSpecificInfo(StreamKind.Video, index, "StreamSize_Encoded/String4"),
                        //string StreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "StreamSize_Encoded/String5");
                        StreamSizeEncodedProportion = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "StreamSize_Encoded_Proportion")),
                        SourceStreamSizeEncoded = ParseULong(GetSpecificInfo(StreamKind.Video, index, "Source_StreamSize_Encoded")),
                        SourceStreamSizeEncodedFormatted = GetSpecificInfo(StreamKind.Video, index, "Source_StreamSize_Encoded/String"),
                        //string SourceStreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize_Encoded/String1");
                        //string SourceStreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize_Encoded/String2");
                        //string SourceStreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize_Encoded/String3");
                        SourceStreamSizeEncodedPrecise = GetSpecificInfo(StreamKind.Video, index, "Source_StreamSize_Encoded/String4"),
                        //string SourceStreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Source_StreamSize_Encoded/String5");
                        SourceStreamSizeEncodedProportion = ParseDouble(GetSpecificInfo(StreamKind.Video, index, "Source_StreamSize_Encoded_Proportion")),

                        Alignment = GetSpecificInfo(StreamKind.Video, index, "Alignment"),
                        //string AlignmentString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Alignment/String");
                        Title = GetSpecificInfo(StreamKind.Video, index, "Title"),
                        EncodedApplication = GetSpecificInfo(StreamKind.Video, index, "Encoded_Application"),
                        //string EncodedApplicationString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Encoded_Application/String");
                        //string EncodedApplicationCompany = getSpecificInfo(MediaInfoStreamKind.Video, index, "Encoded_Application_Company");
                        EncodedApplicationName = GetSpecificInfo(StreamKind.Video, index, "Encoded_Application_Name"),
                        //string EncodedApplicationVersion = getSpecificInfo(MediaInfoStreamKind.Video, index, "Encoded_Application_Version");
                        EncodedApplicationUrl = GetSpecificInfo(StreamKind.Video, index, "Encoded_Application_Url"),
                        EncodedLibrary = GetSpecificInfo(StreamKind.Video, index, "Encoded_Library"),
                        //string EncodedLibraryString = getSpecificInfo(MediaInfoStreamKind.Video, index, "Encoded_Library/String");
                        EncodedLibraryCompanyName = GetSpecificInfo(StreamKind.Video, index, "Encoded_Library_CompanyName"),
                        EncodedLibraryName = GetSpecificInfo(StreamKind.Video, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetSpecificInfo(StreamKind.Video, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetSpecificInfo(StreamKind.Video, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetSpecificInfo(StreamKind.Video, index, "Encoded_Library_Settings"),
                        EncodedOperatingSystem = GetSpecificInfo(StreamKind.Video, index, "Encoded_OperatingSystem"),

                        Language = GetSpecificInfo(StreamKind.Video, index, "Language"),
                        LanguageFull = GetSpecificInfo(StreamKind.Video, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Language/String1");
                        Language2Letter = GetSpecificInfo(StreamKind.Video, index, "Language/String2"),
                        Language3Letter = GetSpecificInfo(StreamKind.Video, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Video, index, "Language/String4");
                        LanguageMore = GetSpecificInfo(StreamKind.Video, index, "Language_More"),

                        ServiceKind = GetSpecificInfo(StreamKind.Video, index, "ServiceKind"),
                        Disabled = ParseBool(GetSpecificInfo(StreamKind.Video, index, "Disabled")),
                        DisabledString = GetSpecificInfo(StreamKind.Video, index, "Disabled/String"),
                        Default = ParseBool(GetSpecificInfo(StreamKind.Video, index, "Default")),
                        DefaultString = GetSpecificInfo(StreamKind.Video, index, "Default/String"),
                        Forced = ParseBool(GetSpecificInfo(StreamKind.Video, index, "Forced")),
                        ForcedString = GetSpecificInfo(StreamKind.Video, index, "Forced/String"),

                        AlternateGroup = ParseInt(GetSpecificInfo(StreamKind.Video, index, "AlternateGroup")),
                        //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Video, index, "AlternateGroup/String");
                        EncodedDate = GetSpecificInfo(StreamKind.Video, index, "Encoded_Date"),
                        TaggedDate = GetSpecificInfo(StreamKind.Video, index, "Tagged_Date"),
                        Encryption = GetSpecificInfo(StreamKind.Video, index, "Encryption"),
                        BufferSize = GetSpecificInfo(StreamKind.Video, index, "BufferSize"),
                        ColourRange = GetSpecificInfo(StreamKind.Video, index, "colour_range"),
                        ColourDescriptionPresent = GetSpecificInfo(StreamKind.Video, index, "colour_description_present"),
                        ColourPrimaries = GetSpecificInfo(StreamKind.Video, index, "colour_primaries"),
                        TransferCharacteristics = GetSpecificInfo(StreamKind.Video, index, "transfer_characteristics"),
                        MatrixCoefficients = GetSpecificInfo(StreamKind.Video, index, "matrix_coefficients"),
                        ColourDescriptionPresentOriginal = GetSpecificInfo(StreamKind.Video, index, "colour_description_present_Original"),
                        ColourPrimariesOriginal = GetSpecificInfo(StreamKind.Video, index, "colour_primaries_Original"),
                        TransferCharacteristicsOriginal = GetSpecificInfo(StreamKind.Video, index, "transfer_characteristics_Original"),
                        MatrixCoefficientsOriginal = GetSpecificInfo(StreamKind.Video, index, "matrix_coefficients_Original"),

                        MasteringDisplayColorPrimaries = GetSpecificInfo(StreamKind.Video, index, "MasteringDisplay_ColorPrimaries"),
                        MasteringDisplayLuminance = GetSpecificInfo(StreamKind.Video, index, "MasteringDisplay_Luminance"),
                        MaximumContentLightLevel = GetSpecificInfo(StreamKind.Video, index, "MaxCLL"),
                        MaximumFrameAverageLightLevel = GetSpecificInfo(StreamKind.Video, index, "MaxFALL")
                    };

                    //get duration in milliseconds if mediainfo return 0
                    if (!tmp.Duration.HasValue)
                    {
                        tmp.Duration = CalculateDuration(tmp);
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
                        Count = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Status");
                        StreamCount = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "StreamCount")),
                        StreamKind = GetSpecificInfo(StreamKind.Audio, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Audio, index, "FirstPacketOrder");
                        Inform = GetSpecificInfo(StreamKind.Audio, index, "Inform"),
                        ID = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "ID/String");
                        OriginalSourceMediumID = GetSpecificInfo(StreamKind.Audio, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetSpecificInfo(StreamKind.Audio, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "UniqueID/String");

                        Format = GetSpecificInfo(StreamKind.Audio, index, "Format"),
                        FormatWithExtra = GetSpecificInfo(StreamKind.Audio, index, "Format/String"),
                        FormatInfo = GetSpecificInfo(StreamKind.Audio, index, "Format/Info"),
                        FormatUrl = GetSpecificInfo(StreamKind.Audio, index, "Format/Url"),
                        FormatCommercial = GetSpecificInfo(StreamKind.Audio, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Format_Commercial_IfAny");
                        FormatVersion = GetSpecificInfo(StreamKind.Audio, index, "Format_Version"),
                        FormatProfile = GetSpecificInfo(StreamKind.Audio, index, "Format_Profile"),
                        FormatCompression = GetSpecificInfo(StreamKind.Audio, index, "Format_Compression"),
                        FormatSettings = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings"),
                        FormatSettingsSBR = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_SBR"),
                        //string FormatSettingsSBRString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Format_Settings_SBR/String");
                        FormatSettingsPS = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_PS"),
                        //string FormatSettingsPSString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Format_Settings_PS/String");
                        FormatSettingsMode = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_Mode"),
                        FormatSettingsModeExtension = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_ModeExtension"),
                        FormatSettingsEmphasis = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_Emphasis"),
                        FormatSettingsFloor = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_Floor"),
                        FormatSettingsFirm = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_Firm"),
                        FormatSettingsEndianness = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_Endianness"),
                        FormatSettingsSign = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_Sign"),
                        FormatSettingsLaw = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_Law"),
                        FormatSettingsITU = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_ITU"),
                        FormatSettingsWrapping = GetSpecificInfo(StreamKind.Audio, index, "Format_Settings_Wrapping"),
                        FormatAdditionalFeatures = GetSpecificInfo(StreamKind.Audio, index, "Format_AdditionalFeatures"),

                        MatrixFormat = GetSpecificInfo(StreamKind.Audio, index, "Matrix_Format"),
                        InternetMediaType = GetSpecificInfo(StreamKind.Audio, index, "InternetMediaType"),
                        MuxingMode = GetSpecificInfo(StreamKind.Audio, index, "MuxingMode"),
                        MuxingModeMoreInfo = GetSpecificInfo(StreamKind.Audio, index, "MuxingMode_MoreInfo"),

                        CodecID = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "CodecID/String");
                        CodecIDInfo = GetSpecificInfo(StreamKind.Audio, index, "CodecID/Info"),
                        CodecIDHint = GetSpecificInfo(StreamKind.Audio, index, "CodecID/Hint"),
                        CodecIDUrl = GetSpecificInfo(StreamKind.Audio, index, "CodecID/Url"),
                        CodecIDDescription = GetSpecificInfo(StreamKind.Audio, index, "CodecID_Description"),

                        DurationFormatted = GetSpecificInfo(StreamKind.Audio, index, "Duration/String"),
                        DurationFormattedComplete = GetSpecificInfo(StreamKind.Audio, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration/String2");
                        DurationTimecode = GetSpecificInfo(StreamKind.Audio, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration/String5");
                        DurationFirstFrame = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Duration_FirstFrame")),
                        DurationFirstFrameFormatted = GetSpecificInfo(StreamKind.Audio, index, "Duration_FirstFrame/String"),
                        DurationFirstFrameFormattedComplete = GetSpecificInfo(StreamKind.Audio, index, "Duration_FirstFrame/String1"),
                        //string DurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_FirstFrame/String2");
                        DurationFirstFrameTimecode = GetSpecificInfo(StreamKind.Audio, index, "Duration_FirstFrame/String3"),
                        //string DurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_FirstFrame/String4");
                        //string DurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_FirstFrame/String5");
                        DurationLastFrame = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Duration_LastFrame")),
                        DurationLastFrameFormatted = GetSpecificInfo(StreamKind.Audio, index, "Duration_LastFrame/String"),
                        DurationLastFrameFormattedComplete = GetSpecificInfo(StreamKind.Audio, index, "Duration_LastFrame/String1"),
                        //string DurationLastFrameString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_LastFrame/String2");
                        DurationLastFrameTimecode = GetSpecificInfo(StreamKind.Audio, index, "Duration_LastFrame/String3"),
                        //string DurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_LastFrame/String4");
                        //string DurationLastFrameString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Duration_LastFrame/String5");
                        SourceDuration = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Source_Duration")),
                        SourceDurationFormatted = GetSpecificInfo(StreamKind.Audio, index, "Source_Duration/String"),
                        SourceDurationFormattedComplete = GetSpecificInfo(StreamKind.Audio, index, "Source_Duration/String1"),
                        //string SourceDurationString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration/String2");
                        SourceDurationTimecode = GetSpecificInfo(StreamKind.Audio, index, "Source_Duration/String3"),
                        //string SourceDurationString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration/String4");
                        //string SourceDurationString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration/String5");
                        SourceDurationFirstFrame = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Source_Duration_FirstFrame")),
                        SourceDurationFirstFrameFormatted = GetSpecificInfo(StreamKind.Audio, index, "Source_Duration_FirstFrame/String"),
                        SourceDurationFirstFrameFormattedComplete = GetSpecificInfo(StreamKind.Audio, index, "Source_Duration_FirstFrame/String1"),
                        //string SourceDurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_FirstFrame/String2");
                        SourceDurationFirstFrameTimecode = GetSpecificInfo(StreamKind.Audio, index, "Source_Duration_FirstFrame/String3"),
                        //string SourceDurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_FirstFrame/String4");
                        //string SourceDurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_FirstFrame/String5");
                        SourceDurationLastFrame = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Source_Duration_LastFrame")),
                        SourceDurationLastFrameFormatted = GetSpecificInfo(StreamKind.Audio, index, "Source_Duration_LastFrame/String"),
                        SourceDurationLastFrameFormattedComplete = GetSpecificInfo(StreamKind.Audio, index, "Source_Duration_LastFrame/String1"),
                        //string SourceDurationLastFramestring2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_LastFrame/String2");
                        SourceDurationLastFrameTimecode = GetSpecificInfo(StreamKind.Audio, index, "Source_Duration_LastFrame/String3"),
                        //string SourceDurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_LastFrame/String4");
                        //string SourceDurationLastFramestring5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_Duration_LastFrame/String5");

                        BitRateMode = GetSpecificInfo(StreamKind.Audio, index, "BitRate_Mode"),
                        //string BitRateModeString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "BitRate_Mode/String");
                        BitRate = ParseULong(GetSpecificInfo(StreamKind.Audio, index, "BitRate")),
                        BitRateFormatted = GetSpecificInfo(StreamKind.Audio, index, "BitRate/String"),
                        BitRateMinimum = ParseULong(GetSpecificInfo(StreamKind.Audio, index, "BitRate_Minimum")),
                        BitRateMinimumFormatted = GetSpecificInfo(StreamKind.Audio, index, "BitRate_Minimum/String"),
                        BitRateNominal = ParseULong(GetSpecificInfo(StreamKind.Audio, index, "BitRate_Nominal")),
                        BitRateNominalFormatted = GetSpecificInfo(StreamKind.Audio, index, "BitRate_Nominal/String"),
                        BitRateMaximum = ParseULong(GetSpecificInfo(StreamKind.Audio, index, "BitRate_Maximum")),
                        BitRateMaximumFormatted = GetSpecificInfo(StreamKind.Audio, index, "BitRate_Maximum/String"),
                        BitRateEncoded = ParseULong(GetSpecificInfo(StreamKind.Audio, index, "BitRate_Encoded")),
                        BitRateEncodedFormatted = GetSpecificInfo(StreamKind.Audio, index, "BitRate_Encoded/String"),

                        Channels = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Channel(s)")),
                        ChannelsFormatted = GetSpecificInfo(StreamKind.Audio, index, "Channel(s)/String"),
                        ChannelsOriginal = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Channel(s)_Original")),
                        ChannelsOriginalFormatted = GetSpecificInfo(StreamKind.Audio, index, "Channel(s)_Original/String"),
                        MatrixChannels = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Matrix_Channel(s)")),
                        MatrixChannelsString = GetSpecificInfo(StreamKind.Audio, index, "Matrix_Channel(s)/String"),
                        ChannelPositions = GetSpecificInfo(StreamKind.Audio, index, "ChannelPositions"),
                        ChannelPositionsFormatted = GetSpecificInfo(StreamKind.Audio, index, "ChannelPositions/String2"),
                        MatrixChannelPositions = GetSpecificInfo(StreamKind.Audio, index, "Matrix_ChannelPositions"),
                        MatrixChannelPositionsFormatted = GetSpecificInfo(StreamKind.Audio, index, "Matrix_ChannelPositions/String"),
                        ChannelLayout = GetSpecificInfo(StreamKind.Audio, index, "ChannelLayout"),
                        ChannelLayoutID = GetSpecificInfo(StreamKind.Audio, index, "ChannelLayoutID"),

                        SamplesPerFrame = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "SamplesPerFrame")),
                        SamplingRate = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "SamplingRate")),
                        SamplingRateFormatted = GetSpecificInfo(StreamKind.Audio, index, "SamplingRate/String"),
                        SamplingCount = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "SamplingCount")),
                        SourceSamplingCount = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Source_SamplingCount")),
                        FrameRate = ParseDouble(GetSpecificInfo(StreamKind.Audio, index, "FrameRate")),
                        FrameRateFormatted = GetSpecificInfo(StreamKind.Audio, index, "FrameRate/String"),
                        FrameRateNum = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "FrameRate_Num")),
                        FrameRateDen = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "FrameRate_Den")),
                        FrameCount = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "FrameCount")),
                        SourceFrameCount = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Source_FrameCount")),

                        BitDepth = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "BitDepth")),
                        BitDepthFormatted = GetSpecificInfo(StreamKind.Audio, index, "BitDepth/String"),
                        BitDepthDetected = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "BitDepth_Detected")),
                        BitDepthDetectedFormatted = GetSpecificInfo(StreamKind.Audio, index, "BitDepth_Detected/String"),
                        BitDepthStored = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "BitDepth_Stored")),
                        BitDepthStoredFormatted = GetSpecificInfo(StreamKind.Audio, index, "BitDepth_Stored/String"),

                        CompressionMode = GetSpecificInfo(StreamKind.Audio, index, "Compression_Mode"),
                        //string CompressionModeString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Compression_Mode/String");
                        CompressionRatio = ParseDouble(GetSpecificInfo(StreamKind.Audio, index, "Compression_Ratio")),

                        Delay = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Delay")),
                        DelayFormatted = GetSpecificInfo(StreamKind.Audio, index, "Delay/String"),
                        DelayFormattedComplete = GetSpecificInfo(StreamKind.Audio, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay/String2");
                        DelayTimecode = GetSpecificInfo(StreamKind.Audio, index, "Delay/String3"),
                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay/String4");
                        //string DelayString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay/String5");
                        DelaySettings = GetSpecificInfo(StreamKind.Audio, index, "Delay_Settings"),
                        DelayDropFrame = GetSpecificInfo(StreamKind.Audio, index, "Delay_DropFrame"),
                        DelaySource = GetSpecificInfo(StreamKind.Audio, index, "Delay_Source"),
                        //string DelaySourceString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay_Source/String");
                        DelayOriginal = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Delay_Original")),
                        DelayOriginalFormatted = GetSpecificInfo(StreamKind.Audio, index, "Delay_Original/String"),
                        DelayOriginalFormattedComplete = GetSpecificInfo(StreamKind.Audio, index, "Delay_Original/String1"),
                        //string DelayOriginalString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay_Original/String2");
                        DelayOriginalTimecode = GetSpecificInfo(StreamKind.Audio, index, "Delay_Original/String3"),
                        //string DelayOriginalString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay_Original/String4");
                        //string DelayOriginalString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Delay_Original/String5");
                        DelayOriginalSettings = GetSpecificInfo(StreamKind.Audio, index, "Delay_Original_Settings"),
                        DelayOriginalDropFrame = GetSpecificInfo(StreamKind.Audio, index, "Delay_Original_DropFrame"),
                        DelayOriginalSource = GetSpecificInfo(StreamKind.Audio, index, "Delay_Original_Source"),
                        VideoDelay = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Video_Delay")),
                        VideoDelayFormatted = GetSpecificInfo(StreamKind.Audio, index, "Video_Delay/String"),
                        VideoDelayFormattedComplete = GetSpecificInfo(StreamKind.Audio, index, "Video_Delay/String1"),
                        //string VideoDelayString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Video_Delay/String2");
                        VideoDelayTimecode = GetSpecificInfo(StreamKind.Audio, index, "Video_Delay/String3"),
                        //string VideoDelayString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Video_Delay/String4");
                        //string VideoDelayString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Video_Delay/String5");

                        ReplayGainGain = GetSpecificInfo(StreamKind.Audio, index, "ReplayGain_Gain"),
                        //string ReplayGainGainString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "ReplayGain_Gain/String");
                        ReplayGainPeak = GetSpecificInfo(StreamKind.Audio, index, "ReplayGain_Peak"),

                        StreamSize = ParseULong(GetSpecificInfo(StreamKind.Audio, index, "StreamSize")),
                        StreamSizeFormatted = GetSpecificInfo(StreamKind.Audio, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize/String1");
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize/String2");
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize/String3");
                        StreamSizePrecise = GetSpecificInfo(StreamKind.Audio, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize/String5");
                        StreamSizeProportion = ParseDouble(GetSpecificInfo(StreamKind.Audio, index, "StreamSize_Proportion")),
                        //string StreamSizeDemuxed = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed");
                        //string StreamSizeDemuxedString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String");
                        //string StreamSizeDemuxedString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String1");
                        //string StreamSizeDemuxedString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String2");
                        //string StreamSizeDemuxedString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String3");
                        //string StreamSizeDemuxedString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String4");
                        //string StreamSizeDemuxedString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Demuxed/String5");
                        SourceStreamSize = ParseULong(GetSpecificInfo(StreamKind.Audio, index, "Source_StreamSize")),
                        SourceStreamSizeFormatted = GetSpecificInfo(StreamKind.Audio, index, "Source_StreamSize/String"),
                        //string SourceStreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize/String1");
                        //string SourceStreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize/String2");
                        //string SourceStreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize/String3");
                        SourceStreamSizePrecise = GetSpecificInfo(StreamKind.Audio, index, "Source_StreamSize/String4"),
                        //string SourceStreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize/String5");
                        SourceStreamSizeProportion = GetSpecificInfo(StreamKind.Audio, index, "Source_StreamSize_Proportion"),
                        StreamSizeEncoded = ParseULong(GetSpecificInfo(StreamKind.Audio, index, "StreamSize_Encoded")),
                        StreamSizeEncodedFormatted = GetSpecificInfo(StreamKind.Audio, index, "StreamSize_Encoded/String"),
                        //string StreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Encoded/String1");
                        //string StreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Encoded/String2");
                        //string StreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Encoded/String3");
                        StreamSizeEncodedPrecise = GetSpecificInfo(StreamKind.Audio, index, "StreamSize_Encoded/String4"),
                        //string StreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "StreamSize_Encoded/String5");
                        StreamSizeEncodedProportion = ParseDouble(GetSpecificInfo(StreamKind.Audio, index, "StreamSize_Encoded_Proportion")),
                        SourceStreamSizeEncoded = ParseULong(GetSpecificInfo(StreamKind.Audio, index, "Source_StreamSize_Encoded")),
                        SourceStreamSizeEncodedFormatted = GetSpecificInfo(StreamKind.Audio, index, "Source_StreamSize_Encoded/String"),
                        //string SourceStreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize_Encoded/String1");
                        //string SourceStreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize_Encoded/String2");
                        //string SourceStreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize_Encoded/String3");
                        SourceStreamSizeEncodedPrecise = GetSpecificInfo(StreamKind.Audio, index, "Source_StreamSize_Encoded/String4"),
                        //string SourceStreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Source_StreamSize_Encoded/String5");
                        SourceStreamSizeEncodedProportion = ParseDouble(GetSpecificInfo(StreamKind.Audio, index, "Source_StreamSize_Encoded_Proportion")),

                        Alignment = GetSpecificInfo(StreamKind.Audio, index, "Alignment"),
                        AlignmentFormatted = GetSpecificInfo(StreamKind.Audio, index, "Alignment/String"),
                        InterleaveVideoFrames = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Interleave_VideoFrames")),
                        InterleaveDuration = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Interleave_Duration")),
                        InterleaveDurationFormatted = GetSpecificInfo(StreamKind.Audio, index, "Interleave_Duration/String"),
                        InterleavePreload = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "Interleave_Preload")),
                        InterleavePreloadFormatted = GetSpecificInfo(StreamKind.Audio, index, "Interleave_Preload/String"),
                        Title = GetSpecificInfo(StreamKind.Audio, index, "Title"),

                        EncodedApplication = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Application"),
                        EncodedApplicationFormatted = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Application/String"),
                        EncodedApplicationCompany = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Application_Company"),
                        EncodedApplicationName = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Application_Name"),
                        EncodedApplicationVersion = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Application_Version"),
                        EncodedApplicationUrl = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Application_Url"),
                        EncodedLibrary = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Library"),
                        EncodedLibraryFormatted = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Library/String"),
                        EncodedLibraryCompanyName = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Library_CompanyName"),
                        EncodedLibraryName = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Library_Settings"),
                        EncodedOperatingSystem = GetSpecificInfo(StreamKind.Audio, index, "Encoded_OperatingSystem"),

                        Language = GetSpecificInfo(StreamKind.Audio, index, "Language"),
                        LanguageFull = GetSpecificInfo(StreamKind.Audio, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Language/String1");
                        Language2Letter = GetSpecificInfo(StreamKind.Audio, index, "Language/String2"),
                        Language3Letter = GetSpecificInfo(StreamKind.Audio, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Audio, index, "Language/String4");
                        LanguageMore = GetSpecificInfo(StreamKind.Audio, index, "Language_More"),

                        Disabled = ParseBool(GetSpecificInfo(StreamKind.Audio, index, "Disabled")),
                        DisabledString = GetSpecificInfo(StreamKind.Audio, index, "Disabled/String"),
                        Default = ParseBool(GetSpecificInfo(StreamKind.Audio, index, "Default")),
                        DefaultString = GetSpecificInfo(StreamKind.Audio, index, "Default/String"),
                        Forced = ParseBool(GetSpecificInfo(StreamKind.Audio, index, "Forced")),
                        ForcedString = GetSpecificInfo(StreamKind.Audio, index, "Forced/String"),

                        AlternateGroup = ParseInt(GetSpecificInfo(StreamKind.Audio, index, "AlternateGroup")),
                        //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Audio, index, "AlternateGroup/String");
                        EncodedDate = GetSpecificInfo(StreamKind.Audio, index, "Encoded_Date"),
                        TaggedDate = GetSpecificInfo(StreamKind.Audio, index, "Tagged_Date"),
                        Encryption = GetSpecificInfo(StreamKind.Audio, index, "Encryption")
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
                        Count = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Text, index, "Status");
                        StreamCount = ParseInt(GetSpecificInfo(StreamKind.Text, index, "StreamCount")),
                        StreamKind = GetSpecificInfo(StreamKind.Text, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetSpecificInfo(StreamKind.Text, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetSpecificInfo(StreamKind.Text, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetSpecificInfo(StreamKind.Text, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Text, index, "FirstPacketOrder");
                        Inform = GetSpecificInfo(StreamKind.Text, index, "Inform"),
                        ID = ParseInt(GetSpecificInfo(StreamKind.Text, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Text, index, "ID/String");
                        OriginalSourceMediumID = GetSpecificInfo(StreamKind.Text, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.Text, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetSpecificInfo(StreamKind.Text, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Text, index, "UniqueID/String");

                        Format = GetSpecificInfo(StreamKind.Text, index, "Format"),
                        FormatWithExtra = GetSpecificInfo(StreamKind.Text, index, "Format/String"),
                        FormatInfo = GetSpecificInfo(StreamKind.Text, index, "Format/Info"),
                        FormatUrl = GetSpecificInfo(StreamKind.Text, index, "Format/Url"),
                        FormatCommercial = GetSpecificInfo(StreamKind.Text, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Text, index, "Format_Commercial_IfAny");
                        FormatVersion = GetSpecificInfo(StreamKind.Text, index, "Format_Version"),
                        FormatProfile = GetSpecificInfo(StreamKind.Text, index, "Format_Profile"),
                        FormatCompression = GetSpecificInfo(StreamKind.Text, index, "Format_Compression"),
                        FormatSettings = GetSpecificInfo(StreamKind.Text, index, "Format_Settings"),
                        FormatSettingsWrapping = GetSpecificInfo(StreamKind.Text, index, "Format_Settings_Wrapping"),
                        FormatAdditionalFeatures = GetSpecificInfo(StreamKind.Text, index, "Format_AdditionalFeatures"),
                        InternetMediaType = GetSpecificInfo(StreamKind.Text, index, "InternetMediaType"),
                        MuxingMode = GetSpecificInfo(StreamKind.Text, index, "MuxingMode"),
                        MuxingModeMoreInfo = GetSpecificInfo(StreamKind.Text, index, "MuxingMode_MoreInfo"),

                        CodecID = ParseInt(GetSpecificInfo(StreamKind.Text, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Text, index, "CodecID/String");
                        CodecIDInfo = GetSpecificInfo(StreamKind.Text, index, "CodecID/Info"),
                        CodecIDHint = GetSpecificInfo(StreamKind.Text, index, "CodecID/Hint"),
                        CodecIDUrl = GetSpecificInfo(StreamKind.Text, index, "CodecID/Url"),
                        CodecIDDescription = GetSpecificInfo(StreamKind.Text, index, "CodecID_Description"),

                        DurationFormatted = GetSpecificInfo(StreamKind.Text, index, "Duration/String"),
                        DurationFormattedComplete = GetSpecificInfo(StreamKind.Text, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration/String2");
                        DurationTimecode = GetSpecificInfo(StreamKind.Text, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration/String5");
                        DurationFirstFrame = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Duration_FirstFrame")),
                        DurationFirstFrameFormatted = GetSpecificInfo(StreamKind.Text, index, "Duration_FirstFrame/String"),
                        DurationFirstFrameFormattedComplete = GetSpecificInfo(StreamKind.Text, index, "Duration_FirstFrame/String1"),
                        //string DurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_FirstFrame/String2");
                        DurationFirstFrameTimecode = GetSpecificInfo(StreamKind.Text, index, "Duration_FirstFrame/String3"),
                        //string DurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_FirstFrame/String4");
                        //string DurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_FirstFrame/String5");
                        DurationLastFrame = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Duration_LastFrame")),
                        DurationLastFrameFormatted = GetSpecificInfo(StreamKind.Text, index, "Duration_LastFrame/String"),
                        DurationLastFrameFormattedComplete = GetSpecificInfo(StreamKind.Text, index, "Duration_LastFrame/String1"),
                        //string DurationLastFrameString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_LastFrame/String2");
                        DurationLastFrameTimecode = GetSpecificInfo(StreamKind.Text, index, "Duration_LastFrame/String3"),
                        //string DurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_LastFrame/String4");
                        //string DurationLastFrameString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Duration_LastFrame/String5");
                        SourceDuration = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Source_Duration")),
                        SourceDurationFormatted = GetSpecificInfo(StreamKind.Text, index, "Source_Duration/String"),
                        SourceDurationFormattedComplete = GetSpecificInfo(StreamKind.Text, index, "Source_Duration/String1"),
                        //string SourceDurationString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration/String2");
                        SourceDurationTimecode = GetSpecificInfo(StreamKind.Text, index, "Source_Duration/String3"),
                        //string SourceDurationString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration/String4");
                        //string SourceDurationString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration/String5");
                        SourceDurationFirstFrame = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Source_Duration_FirstFrame")),
                        SourceDurationFirstFrameFormatted = GetSpecificInfo(StreamKind.Text, index, "Source_Duration_FirstFrame/String"),
                        SourceDurationFirstFrameFormattedComplete = GetSpecificInfo(StreamKind.Text, index, "Source_Duration_FirstFrame/String1"),
                        //string SourceDurationFirstFrameString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_FirstFrame/String2");
                        SourceDurationFirstFrameTimecode = GetSpecificInfo(StreamKind.Text, index, "Source_Duration_FirstFrame/String3"),
                        //string SourceDurationFirstFrameString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_FirstFrame/String4");
                        //string SourceDurationFirstFrameString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_FirstFrame/String5");
                        SourceDurationLastFrame = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Source_Duration_LastFrame")),
                        SourceDurationLastFrameFormatted = GetSpecificInfo(StreamKind.Text, index, "Source_Duration_LastFrame/String"),
                        SourceDurationLastFrameFormattedComplete = GetSpecificInfo(StreamKind.Text, index, "Source_Duration_LastFrame/String1"),
                        //string SourceDurationLastFramestring2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_LastFrame/String2");
                        SourceDurationLastFrameTimecode = GetSpecificInfo(StreamKind.Text, index, "Source_Duration_LastFrame/String3"),
                        //string SourceDurationLastFrameString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_LastFrame/String4");
                        //string SourceDurationLastFramestring5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_Duration_LastFrame/String5");

                        BitRateMode = GetSpecificInfo(StreamKind.Text, index, "BitRate_Mode"),
                        //string BitRateModeString = getSpecificInfo(MediaInfoStreamKind.Text, index, "BitRate_Mode/String");
                        BitRate = ParseULong(GetSpecificInfo(StreamKind.Text, index, "BitRate")),
                        BitRateFormatted = GetSpecificInfo(StreamKind.Text, index, "BitRate/String"),
                        BitRateMinimum = ParseULong(GetSpecificInfo(StreamKind.Text, index, "BitRate_Minimum")),
                        BitRateMinimumFormatted = GetSpecificInfo(StreamKind.Text, index, "BitRate_Minimum/String"),
                        BitRateNominal = ParseULong(GetSpecificInfo(StreamKind.Text, index, "BitRate_Nominal")),
                        BitRateNominalFormatted = GetSpecificInfo(StreamKind.Text, index, "BitRate_Nominal/String"),
                        BitRateMaximum = ParseULong(GetSpecificInfo(StreamKind.Text, index, "BitRate_Maximum")),
                        BitRateMaximumFormatted = GetSpecificInfo(StreamKind.Text, index, "BitRate_Maximum/String"),
                        BitRateEncoded = ParseULong(GetSpecificInfo(StreamKind.Text, index, "BitRate_Encoded")),
                        BitRateEncodedFormatted = GetSpecificInfo(StreamKind.Text, index, "BitRate_Encoded/String"),

                        Width = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Width")),
                        //string WidthString = getSpecificInfo(MediaInfoStreamKind.Text, index, "Width/String");
                        Height = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Height")),
                        //string HeightString = getSpecificInfo(MediaInfoStreamKind.Text, index, "Height/String");

                        FrameRateMode = GetSpecificInfo(StreamKind.Text, index, "FrameRate_Mode"),
                        //string FrameRateModeString = getSpecificInfo(MediaInfoStreamKind.Text, index, "FrameRate_Mode/String");
                        FrameRate = ParseDouble(GetSpecificInfo(StreamKind.Text, index, "FrameRate")),
                        FrameRateFormatted = GetSpecificInfo(StreamKind.Text, index, "FrameRate/String"),
                        FrameRateNum = ParseInt(GetSpecificInfo(StreamKind.Text, index, "FrameRate_Num")),
                        FrameRateDen = ParseInt(GetSpecificInfo(StreamKind.Text, index, "FrameRate_Den")),
                        FrameRateMinimum = ParseDouble(GetSpecificInfo(StreamKind.Text, index, "FrameRate_Minimum")),
                        FrameRateMinimumFormatted = GetSpecificInfo(StreamKind.Text, index, "FrameRate_Minimum/String"),
                        FrameRateNominal = ParseDouble(GetSpecificInfo(StreamKind.Text, index, "FrameRate_Nominal")),
                        FrameRateNominalFormatted = GetSpecificInfo(StreamKind.Text, index, "FrameRate_Nominal/String"),
                        FrameRateMaximum = ParseDouble(GetSpecificInfo(StreamKind.Text, index, "FrameRate_Maximum")),
                        FrameRateMaximumFormatted = GetSpecificInfo(StreamKind.Text, index, "FrameRate_Maximum/String"),
                        FrameRateOriginal = ParseDouble(GetSpecificInfo(StreamKind.Text, index, "FrameRate_Original")),
                        FrameRateOriginalFormatted = GetSpecificInfo(StreamKind.Text, index, "FrameRate_Original/String"),
                        FrameCount = ParseInt(GetSpecificInfo(StreamKind.Text, index, "FrameCount")),
                        SourceFrameCount = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Source_FrameCount")),

                        ColorSpace = GetSpecificInfo(StreamKind.Text, index, "ColorSpace"),
                        ChromaSubsampling = GetSpecificInfo(StreamKind.Text, index, "ChromaSubsampling"),
                        BitDepth = ParseInt(GetSpecificInfo(StreamKind.Text, index, "BitDepth")),
                        BitDepthFormatted = GetSpecificInfo(StreamKind.Text, index, "BitDepth/String"),
                        CompressionMode = GetSpecificInfo(StreamKind.Text, index, "Compression_Mode"),
                        //string CompressionModeString = getSpecificInfo(MediaInfoStreamKind.Text, index, "Compression_Mode/String");
                        CompressionRatio = ParseDouble(GetSpecificInfo(StreamKind.Text, index, "Compression_Ratio")),

                        Delay = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Delay")),
                        DelayFormatted = GetSpecificInfo(StreamKind.Text, index, "Delay/String"),
                        DelayFormattedComplete = GetSpecificInfo(StreamKind.Text, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay/String2");
                        DelayTimecode = GetSpecificInfo(StreamKind.Text, index, "Delay/String3"),
                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay/String4");
                        //string DelayString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay/String5");
                        DelaySettings = GetSpecificInfo(StreamKind.Text, index, "Delay_Settings"),
                        DelayDropFrame = GetSpecificInfo(StreamKind.Text, index, "Delay_DropFrame"),
                        DelaySource = GetSpecificInfo(StreamKind.Text, index, "Delay_Source"),
                        //string DelaySourceString = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay_Source/String");
                        DelayOriginal = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Delay_Original")),
                        DelayOriginalFormatted = GetSpecificInfo(StreamKind.Text, index, "Delay_Original/String"),
                        DelayOriginalFormattedComplete = GetSpecificInfo(StreamKind.Text, index, "Delay_Original/String1"),
                        //string DelayOriginalString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay_Original/String2");
                        DelayOriginalTimecode = GetSpecificInfo(StreamKind.Text, index, "Delay_Original/String3"),
                        //string DelayOriginalString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay_Original/String4");
                        //string DelayOriginalString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Delay_Original/String5");
                        DelayOriginalSettings = GetSpecificInfo(StreamKind.Text, index, "Delay_Original_Settings"),
                        DelayOriginalDropFrame = GetSpecificInfo(StreamKind.Text, index, "Delay_Original_DropFrame"),
                        DelayOriginalSource = GetSpecificInfo(StreamKind.Text, index, "Delay_Original_Source"),
                        VideoDelay = ParseInt(GetSpecificInfo(StreamKind.Text, index, "Video_Delay")),
                        VideoDelayFormatted = GetSpecificInfo(StreamKind.Text, index, "Video_Delay/String"),
                        VideoDelayFormattedComplete = GetSpecificInfo(StreamKind.Text, index, "Video_Delay/String1"),
                        //string VideoDelayString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Video_Delay/String2");
                        VideoDelayTimecode = GetSpecificInfo(StreamKind.Text, index, "Video_Delay/String3"),
                        //string VideoDelayString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Video_Delay/String4");
                        //string VideoDelayString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Video_Delay/String5");

                        StreamSize = ParseULong(GetSpecificInfo(StreamKind.Text, index, "StreamSize")),
                        StreamSizeFormatted = GetSpecificInfo(StreamKind.Text, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize/String1");
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize/String2");
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize/String3");
                        StreamSizePrecise = GetSpecificInfo(StreamKind.Text, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize/String5");
                        StreamSizeProportion = ParseDouble(GetSpecificInfo(StreamKind.Text, index, "StreamSize_Proportion")),
                        //string StreamSizeDemuxed = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed");
                        //string StreamSizeDemuxedString = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String");
                        //string StreamSizeDemuxedString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String1");
                        //string StreamSizeDemuxedString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String2");
                        //string StreamSizeDemuxedString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String3");
                        //string StreamSizeDemuxedString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String4");
                        //string StreamSizeDemuxedString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Demuxed/String5");
                        SourceStreamSize = ParseULong(GetSpecificInfo(StreamKind.Text, index, "Source_StreamSize")),
                        SourceStreamSizeFormatted = GetSpecificInfo(StreamKind.Text, index, "Source_StreamSize/String"),
                        //string SourceStreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize/String1");
                        //string SourceStreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize/String2");
                        //string SourceStreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize/String3");
                        SourceStreamSizePrecise = GetSpecificInfo(StreamKind.Text, index, "Source_StreamSize/String4"),
                        //string SourceStreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize/String5");
                        SourceStreamSizeProportion = GetSpecificInfo(StreamKind.Text, index, "Source_StreamSize_Proportion"),
                        StreamSizeEncoded = ParseULong(GetSpecificInfo(StreamKind.Text, index, "StreamSize_Encoded")),
                        StreamSizeEncodedFormatted = GetSpecificInfo(StreamKind.Text, index, "StreamSize_Encoded/String"),
                        //string StreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Encoded/String1");
                        //string StreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Encoded/String2");
                        //string StreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Encoded/String3");
                        StreamSizeEncodedPrecise = GetSpecificInfo(StreamKind.Text, index, "StreamSize_Encoded/String4"),
                        //string StreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "StreamSize_Encoded/String5");
                        StreamSizeEncodedProportion = ParseDouble(GetSpecificInfo(StreamKind.Text, index, "StreamSize_Encoded_Proportion")),
                        SourceStreamSizeEncoded = ParseULong(GetSpecificInfo(StreamKind.Text, index, "Source_StreamSize_Encoded")),
                        SourceStreamSizeEncodedFormatted = GetSpecificInfo(StreamKind.Text, index, "Source_StreamSize_Encoded/String"),
                        //string SourceStreamSizeEncodedString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize_Encoded/String1");
                        //string SourceStreamSizeEncodedString2 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize_Encoded/String2");
                        //string SourceStreamSizeEncodedString3 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize_Encoded/String3");
                        SourceStreamSizeEncodedPrecise = GetSpecificInfo(StreamKind.Text, index, "Source_StreamSize_Encoded/String4"),
                        //string SourceStreamSizeEncodedString5 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Source_StreamSize_Encoded/String5");
                        SourceStreamSizeEncodedProportion = ParseDouble(GetSpecificInfo(StreamKind.Text, index, "Source_StreamSize_Encoded_Proportion")),

                        Title = GetSpecificInfo(StreamKind.Text, index, "Title"),

                        EncodedApplication = GetSpecificInfo(StreamKind.Text, index, "Encoded_Application"),
                        EncodedApplicationFormatted = GetSpecificInfo(StreamKind.Text, index, "Encoded_Application/String"),
                        EncodedApplicationCompany = GetSpecificInfo(StreamKind.Text, index, "Encoded_Application_Company"),
                        EncodedApplicationName = GetSpecificInfo(StreamKind.Text, index, "Encoded_Application_Name"),
                        EncodedApplicationVersion = GetSpecificInfo(StreamKind.Text, index, "Encoded_Application_Version"),
                        EncodedApplicationUrl = GetSpecificInfo(StreamKind.Text, index, "Encoded_Application_Url"),
                        EncodedLibrary = GetSpecificInfo(StreamKind.Text, index, "Encoded_Library"),
                        EncodedLibraryFormatted = GetSpecificInfo(StreamKind.Text, index, "Encoded_Library/String"),
                        EncodedLibraryCompanyName = GetSpecificInfo(StreamKind.Text, index, "Encoded_Library_CompanyName"),
                        EncodedLibraryName = GetSpecificInfo(StreamKind.Text, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetSpecificInfo(StreamKind.Text, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetSpecificInfo(StreamKind.Text, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetSpecificInfo(StreamKind.Text, index, "Encoded_Library_Settings"),
                        EncodedOperatingSystem = GetSpecificInfo(StreamKind.Text, index, "Encoded_OperatingSystem"),

                        Language = GetSpecificInfo(StreamKind.Text, index, "Language"),
                        LanguageFull = GetSpecificInfo(StreamKind.Text, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Language/String1");
                        Language2Letter = GetSpecificInfo(StreamKind.Text, index, "Language/String2"),
                        Language3Letter = GetSpecificInfo(StreamKind.Text, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Text, index, "Language/String4");
                        LanguageMore = GetSpecificInfo(StreamKind.Text, index, "Language_More"),

                        Disabled = ParseBool(GetSpecificInfo(StreamKind.Text, index, "Disabled")),
                        DisabledString = GetSpecificInfo(StreamKind.Text, index, "Disabled/String"),
                        Default = ParseBool(GetSpecificInfo(StreamKind.Text, index, "Default")),
                        DefaultString = GetSpecificInfo(StreamKind.Text, index, "Default/String"),
                        Forced = ParseBool(GetSpecificInfo(StreamKind.Text, index, "Forced")),
                        ForcedString = GetSpecificInfo(StreamKind.Text, index, "Forced/String"),

                        AlternateGroup = ParseInt(GetSpecificInfo(StreamKind.Text, index, "AlternateGroup")),
                        //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Text, index, "AlternateGroup/String");
                        Summary = GetSpecificInfo(StreamKind.Text, index, "Summary"),
                        EncodedDate = GetSpecificInfo(StreamKind.Text, index, "Encoded_Date"),
                        TaggedDate = GetSpecificInfo(StreamKind.Text, index, "Tagged_Date"),
                        Encryption = GetSpecificInfo(StreamKind.Text, index, "Encryption")
                    };

                    this._Text.Add(tmp);
                }
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
                        Count = ParseInt(GetSpecificInfo(StreamKind.Image, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Image, index, "Status");
                        StreamCount = ParseInt(GetSpecificInfo(StreamKind.Image, index, "StreamCount")),
                        StreamKind = GetSpecificInfo(StreamKind.Image, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetSpecificInfo(StreamKind.Image, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetSpecificInfo(StreamKind.Image, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetSpecificInfo(StreamKind.Image, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Image, index, "FirstPacketOrder");
                        Inform = GetSpecificInfo(StreamKind.Image, index, "Inform"),
                        ID = ParseInt(GetSpecificInfo(StreamKind.Image, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Image, index, "ID/String");
                        OriginalSourceMediumID = GetSpecificInfo(StreamKind.Image, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.Image, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetSpecificInfo(StreamKind.Image, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Image, index, "UniqueID/String");

                        Format = GetSpecificInfo(StreamKind.Image, index, "Format"),
                        FormatWithExtra = GetSpecificInfo(StreamKind.Image, index, "Format/String"),
                        FormatInfo = GetSpecificInfo(StreamKind.Image, index, "Format/Info"),
                        FormatUrl = GetSpecificInfo(StreamKind.Image, index, "Format/Url"),
                        FormatCommercial = GetSpecificInfo(StreamKind.Image, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Image, index, "Format_Commercial_IfAny");
                        FormatVersion = GetSpecificInfo(StreamKind.Image, index, "Format_Version"),
                        FormatProfile = GetSpecificInfo(StreamKind.Image, index, "Format_Profile"),
                        //FormatSettingsEndianness = getSpecificInfo(MediaInfoStreamKind.Image, index, "Format_Settings_Endianness");
                        //FormatSettingsPacking = getSpecificInfo(MediaInfoStreamKind.Image, index, "Format_Settings_Packing");
                        FormatCompression = GetSpecificInfo(StreamKind.Image, index, "Format_Compression"),
                        FormatSettings = GetSpecificInfo(StreamKind.Image, index, "Format_Settings"),
                        FormatSettingsWrapping = GetSpecificInfo(StreamKind.Image, index, "Format_Settings_Wrapping"),
                        FormatAdditionalFeatures = GetSpecificInfo(StreamKind.Image, index, "Format_AdditionalFeatures"),
                        InternetMediaType = GetSpecificInfo(StreamKind.Image, index, "InternetMediaType"),

                        CodecID = ParseInt(GetSpecificInfo(StreamKind.Image, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Image, index, "CodecID/String");
                        CodecIDInfo = GetSpecificInfo(StreamKind.Image, index, "CodecID/Info"),
                        CodecIDHint = GetSpecificInfo(StreamKind.Image, index, "CodecID/Hint"),
                        CodecIDUrl = GetSpecificInfo(StreamKind.Image, index, "CodecID/Url"),
                        CodecIDDescription = GetSpecificInfo(StreamKind.Image, index, "CodecID_Description"),

                        Width = ParseInt(GetSpecificInfo(StreamKind.Image, index, "Width")),
                        WidthFormatted = GetSpecificInfo(StreamKind.Image, index, "Width/String"),
                        WidthOffset = ParseInt(GetSpecificInfo(StreamKind.Image, index, "Width_Offset")),
                        WidthOffsetFormatted = GetSpecificInfo(StreamKind.Image, index, "Width_Offset/String"),
                        WidthOriginal = ParseInt(GetSpecificInfo(StreamKind.Image, index, "Width_Original")),
                        WidthOriginalFormatted = GetSpecificInfo(StreamKind.Image, index, "Width_Original/String"),
                        Height = ParseInt(GetSpecificInfo(StreamKind.Image, index, "Height")),
                        HeightFormatted = GetSpecificInfo(StreamKind.Image, index, "Height/String"),
                        HeightOffset = ParseInt(GetSpecificInfo(StreamKind.Image, index, "Height_Offset")),
                        HeightOffsetFormatted = GetSpecificInfo(StreamKind.Image, index, "Height_Offset/String"),
                        HeightOriginal = ParseInt(GetSpecificInfo(StreamKind.Image, index, "Height_Original")),
                        HeightOriginalFormatted = GetSpecificInfo(StreamKind.Image, index, "Height_Original/String"),

                        PixelAspectRatio = ParseDouble(GetSpecificInfo(StreamKind.Image, index, "PixelAspectRatio")),
                        PixelAspectRatioFormatted = GetSpecificInfo(StreamKind.Image, index, "PixelAspectRatio/String"),
                        PixelAspectRatioOriginal = ParseDouble(GetSpecificInfo(StreamKind.Image, index, "PixelAspectRatio_Original")),
                        PixelAspectRatioOriginalFormatted = GetSpecificInfo(StreamKind.Image, index, "PixelAspectRatio_Original/String"),
                        DisplayAspectRatio = ParseDouble(GetSpecificInfo(StreamKind.Image, index, "DisplayAspectRatio")),
                        DisplayAspectRatioFormatted = GetSpecificInfo(StreamKind.Image, index, "DisplayAspectRatio/String"),
                        DisplayAspectRatioOriginal = ParseDouble(GetSpecificInfo(StreamKind.Image, index, "DisplayAspectRatio_Original")),
                        DisplayAspectRatioOriginalFormatted = GetSpecificInfo(StreamKind.Image, index, "DisplayAspectRatio_Original/String"),

                        ColorSpace = GetSpecificInfo(StreamKind.Image, index, "ColorSpace"),
                        ChromaSubsampling = GetSpecificInfo(StreamKind.Image, index, "ChromaSubsampling"),
                        BitDepth = ParseInt(GetSpecificInfo(StreamKind.Image, index, "BitDepth")),
                        BitDepthFormatted = GetSpecificInfo(StreamKind.Image, index, "BitDepth/String"),
                        CompressionMode = GetSpecificInfo(StreamKind.Image, index, "Compression_Mode"),
                        CompressionRatio = ParseDouble(GetSpecificInfo(StreamKind.Image, index, "Compression_Ratio")),

                        StreamSize = ParseULong(GetSpecificInfo(StreamKind.Image, index, "StreamSize")),
                        StreamSizeFormatted = GetSpecificInfo(StreamKind.Image, index, "StreamSize/String"),
                        //string StreamSizeString1 = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamSize/String1");
                        //string StreamSizeString2 = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamSize/String2");
                        //string StreamSizeString3 = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamSize/String3");
                        StreamSizePrecise = GetSpecificInfo(StreamKind.Image, index, "StreamSize/String4"),
                        //string StreamSizeString5 = getSpecificInfo(MediaInfoStreamKind.Image, index, "StreamSize/String5");
                        StreamSizeProportion = ParseDouble(GetSpecificInfo(StreamKind.Image, index, "StreamSize_Proportion")),

                        EncodedLibrary = GetSpecificInfo(StreamKind.Image, index, "Encoded_Library"),
                        EncodedLibraryFormatted = GetSpecificInfo(StreamKind.Image, index, "Encoded_Library/String"),
                        EncodedLibraryName = GetSpecificInfo(StreamKind.Image, index, "Encoded_Library_Name"),
                        EncodedLibraryVersion = GetSpecificInfo(StreamKind.Image, index, "Encoded_Library_Version"),
                        EncodedLibraryDate = GetSpecificInfo(StreamKind.Image, index, "Encoded_Library_Date"),
                        EncodedLibrarySettings = GetSpecificInfo(StreamKind.Image, index, "Encoded_Library_Settings"),

                        Language = GetSpecificInfo(StreamKind.Image, index, "Language"),
                        LanguageFull = GetSpecificInfo(StreamKind.Image, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Image, index, "Language/String1");
                        Language2Letter = GetSpecificInfo(StreamKind.Image, index, "Language/String2"),
                        Language3Letter = GetSpecificInfo(StreamKind.Image, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Image, index, "Language/String4");
                        LanguageMore = GetSpecificInfo(StreamKind.Image, index, "Language_More"),

                        Disabled = ParseBool(GetSpecificInfo(StreamKind.Image, index, "Disabled")),
                        DisabledString = GetSpecificInfo(StreamKind.Image, index, "Disabled/String"),
                        Default = ParseBool(GetSpecificInfo(StreamKind.Image, index, "Default")),
                        DefaultString = GetSpecificInfo(StreamKind.Image, index, "Default/String"),
                        Forced = ParseBool(GetSpecificInfo(StreamKind.Image, index, "Forced")),
                        ForcedString = GetSpecificInfo(StreamKind.Image, index, "Forced/String"),

                        AlternateGroup = ParseInt(GetSpecificInfo(StreamKind.Image, index, "AlternateGroup")),
                        //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Image, index, "AlternateGroup/String");
                        Summary = GetSpecificInfo(StreamKind.Image, index, "Summary"),
                        EncodedDate = GetSpecificInfo(StreamKind.Image, index, "Encoded_Date"),
                        TaggedDate = GetSpecificInfo(StreamKind.Image, index, "Tagged_Date"),
                        Encryption = GetSpecificInfo(StreamKind.Image, index, "Encryption"),

                        ColourDescriptionPresent = ParseBool(GetSpecificInfo(StreamKind.Image, index, "colour_description_present")),
                        ColourPrimaries = GetSpecificInfo(StreamKind.Image, index, "colour_primaries"),
                        TransferCharacteristics = GetSpecificInfo(StreamKind.Image, index, "transfer_characteristics"),
                        MatrixCoefficients = GetSpecificInfo(StreamKind.Image, index, "matrix_coefficients"),
                        ColourDescriptionPresentOriginal = ParseBool(GetSpecificInfo(StreamKind.Image, index, "colour_description_present_Original")),
                        ColourPrimariesOriginal = GetSpecificInfo(StreamKind.Image, index, "colour_primaries_Original"),
                        TransferCharacteristicsOriginal = GetSpecificInfo(StreamKind.Image, index, "transfer_characteristics_Original"),
                        MatrixCoefficientsOriginal = GetSpecificInfo(StreamKind.Image, index, "matrix_coefficients_Original")
                    };

                    this._Image.Add(tmp);
                }
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
                        Count = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "Count")),
                        //string Status = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Status");
                        StreamCount = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "StreamCount")),
                        StreamKind = GetSpecificInfo(StreamKind.Menu, index, "StreamKind"),
                        //string StreamKindString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "StreamKind/String");
                        StreamKindID = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "StreamKindID")),
                        StreamKindPos = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "StreamKindPos")),
                        StreamOrder = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "StreamOrder")),
                        //string FirstPacketOrder = getSpecificInfo(MediaInfoStreamKind.Menu, index, "FirstPacketOrder");
                        Inform = GetSpecificInfo(StreamKind.Menu, index, "Inform"),
                        ID = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "ID")),
                        //string IDString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "ID/String");
                        OriginalSourceMediumID = GetSpecificInfo(StreamKind.Menu, index, "OriginalSourceMedium_ID"),
                        //string OriginalSourceMediumIDString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "OriginalSourceMedium_ID/String");
                        UniqueID = GetSpecificInfo(StreamKind.Menu, index, "UniqueID"),
                        //string UniqueIDString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "UniqueID/String");

                        Format = GetSpecificInfo(StreamKind.Menu, index, "Format"),
                        FormatWithExtra = GetSpecificInfo(StreamKind.Menu, index, "Format/String"),
                        FormatInfo = GetSpecificInfo(StreamKind.Menu, index, "Format/Info"),
                        FormatUrl = GetSpecificInfo(StreamKind.Menu, index, "Format/Url"),
                        FormatCommercial = GetSpecificInfo(StreamKind.Menu, index, "Format_Commercial"),
                        //string FormatCommercialIfAny = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Format_Commercial_IfAny");
                        FormatVersion = GetSpecificInfo(StreamKind.Menu, index, "Format_Version"),
                        FormatProfile = GetSpecificInfo(StreamKind.Menu, index, "Format_Profile"),
                        FormatCompression = GetSpecificInfo(StreamKind.Menu, index, "Format_Compression"),
                        FormatSettings = GetSpecificInfo(StreamKind.Menu, index, "Format_Settings"),
                        FormatAdditionalFeatures = GetSpecificInfo(StreamKind.Menu, index, "Format_AdditionalFeatures"),

                        CodecID = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "CodecID")),
                        //string CodecIDString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "CodecID/String");
                        CodecIDInfo = GetSpecificInfo(StreamKind.Menu, index, "CodecID/Info"),
                        CodecIDHint = GetSpecificInfo(StreamKind.Menu, index, "CodecID/Hint"),
                        CodecIDUrl = GetSpecificInfo(StreamKind.Menu, index, "CodecID/Url"),
                        CodecIDDescription = GetSpecificInfo(StreamKind.Menu, index, "CodecID_Description"),

                        DurationFormatted = GetSpecificInfo(StreamKind.Menu, index, "Duration/String"),
                        DurationFormattedComplete = GetSpecificInfo(StreamKind.Menu, index, "Duration/String1"),
                        //string DurationString2 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Duration/String2");
                        DurationTimecode = GetSpecificInfo(StreamKind.Menu, index, "Duration/String3"),
                        //string DurationString4 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Duration/String4");
                        //string DurationString5 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Duration/String5");
                        DurationStart = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "Duration_Start")),
                        DurationEnd = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "Duration_End")),

                        Delay = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "Delay")),
                        DelayFormatted = GetSpecificInfo(StreamKind.Menu, index, "Delay/String"),
                        DelayFormattedComplete = GetSpecificInfo(StreamKind.Menu, index, "Delay/String1"),
                        //string DelayString2 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Delay/String2");
                        DelayTimecode = GetSpecificInfo(StreamKind.Menu, index, "Delay/String3"),
                        //string DelayString4 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Delay/String4");
                        //string DelayString5 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Delay/String5");
                        DelaySettings = GetSpecificInfo(StreamKind.Menu, index, "Delay_Settings"),
                        DelayDropFrame = GetSpecificInfo(StreamKind.Menu, index, "Delay_DropFrame"),
                        DelaySource = GetSpecificInfo(StreamKind.Menu, index, "Delay_Source"),

                        ListStreamKind = GetSpecificInfo(StreamKind.Menu, index, "List_StreamKind"),
                        ListStreamPos = GetSpecificInfo(StreamKind.Menu, index, "List_StreamPos"),
                        List = GetSpecificInfo(StreamKind.Menu, index, "List"),
                        //string ListString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "List/String");

                        Title = GetSpecificInfo(StreamKind.Menu, index, "Title"),

                        Language = GetSpecificInfo(StreamKind.Menu, index, "Language"),
                        LanguageFull = GetSpecificInfo(StreamKind.Menu, index, "Language/String"),
                        //string LanguageString1 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Language/String1");
                        Language2Letter = GetSpecificInfo(StreamKind.Menu, index, "Language/String2"),
                        Language3Letter = GetSpecificInfo(StreamKind.Menu, index, "Language/String3"),
                        //string LanguageString4 = getSpecificInfo(MediaInfoStreamKind.Menu, index, "Language/String4");
                        LanguageMore = GetSpecificInfo(StreamKind.Menu, index, "Language_More"),

                        ServiceName = GetSpecificInfo(StreamKind.Menu, index, "ServiceName"),
                        ServiceChannel = GetSpecificInfo(StreamKind.Menu, index, "ServiceChannel"),
                        ServiceUrl = GetSpecificInfo(StreamKind.Menu, index, "Service/Url"),
                        ServiceProvider = GetSpecificInfo(StreamKind.Menu, index, "ServiceProvider"),
                        ServiceProviderUrl = GetSpecificInfo(StreamKind.Menu, index, "ServiceProvider/Url"),
                        ServiceType = GetSpecificInfo(StreamKind.Menu, index, "ServiceType"),
                        NetworkName = GetSpecificInfo(StreamKind.Menu, index, "NetworkName"),
                        OriginalNetworkName = GetSpecificInfo(StreamKind.Menu, index, "Original/NetworkName"),
                        Countries = GetSpecificInfo(StreamKind.Menu, index, "Countries"),
                        TimeZones = GetSpecificInfo(StreamKind.Menu, index, "TimeZones"),
                        LawRating = GetSpecificInfo(StreamKind.Menu, index, "LawRating"),
                        LawRatingReason = GetSpecificInfo(StreamKind.Menu, index, "LawRating_Reason"),

                        Disabled = ParseBool(GetSpecificInfo(StreamKind.Menu, index, "Disabled")),
                        DisabledString = GetSpecificInfo(StreamKind.Menu, index, "Disabled/String"),
                        Default = ParseBool(GetSpecificInfo(StreamKind.Menu, index, "Default")),
                        DefaultString = GetSpecificInfo(StreamKind.Menu, index, "Default/String"),
                        Forced = ParseBool(GetSpecificInfo(StreamKind.Menu, index, "Forced")),
                        ForcedString = GetSpecificInfo(StreamKind.Menu, index, "Forced/String"),
                        AlternateGroup = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "AlternateGroup"))
                    };
                    //string AlternateGroupString = getSpecificInfo(MediaInfoStreamKind.Menu, index, "AlternateGroup/String");

                    //get chapters count (diff between start position and end position)
                    int endpos = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "Chapters_Pos_End")).Value;
                    int startpos = ParseInt(GetSpecificInfo(StreamKind.Menu, index, "Chapters_Pos_Begin")).Value;

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

                            string tmpstr = GetSpecificSubInfo(StreamKind.Menu, index, innerindex, InfoKind.Text);

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
                            } else {
                                chapt.Language = string.Empty;
                                chapt.Title = string.Empty;
                            }

                            chapt.Timecode = GetSpecificSubInfo(StreamKind.Menu, index, innerindex, InfoKind.Name);
                            tmp.Chapters.Add(chapt);
                        }
                    }

                    this._Menu.Add(tmp);
                }
            }
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Changes the mediainfo option that determines whether to parse the cover data or not (only if the parseCoverData flag is true).
        /// </summary>
        /// <param name="flag">True, if the cover data parsing needs to be enabled; otherwise false.</param>
        private void ChangeCoverDataParsingOption(bool flag)
        {
            if (parseCoverData)
            {
                MediaInfo_Option(this.Handle, "Cover_Data", flag ? "base64" : "");
            }
        }

        /// <summary>
        /// Simply checks file presence else throws a FileNotFoundException
        /// </summary>
        /// <param name="filepath">The path of the file.</param>
        /// <returns>true, if the file exists.</returns>
        /// <exception cref="FileNotFoundException">File not found: " + filepath</exception>
        protected bool CheckFileExistence(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("File not found: " + filepath, filepath);
            else
                return true;
        }

        /// <summary>
        /// Return if infos are available
        /// </summary>
        /// <returns>true, if at least one general stream has been found.</returns>
        public bool InfoAvailable()
        {
            return _GeneralCount > 0 ? true : false;
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
                return String.Format("{0:0.##} Tbps", doublebytes);
            }
            else if (1073741824 < bitrate && bitrate < 1099511627775)
            {
                doublebytes = (double)bitrate / (double)1073741824;
                return String.Format("{0:0.##} Gbps", doublebytes);
            }
            else if (1048576 < bitrate && bitrate < 1073741823)
            {
                doublebytes = (double)bitrate / (double)1048576;
                return String.Format("{0:0.##} Mbps", doublebytes);
            }
            else if (1024 < bitrate && bitrate < 1048575)
            {
                doublebytes = (double)bitrate / (double)1024;
                return String.Format("{0:0.##} Kbps", doublebytes);
            }
            else if (0 < bitrate && bitrate < 1023)
            {
                doublebytes = (double)bitrate;
                return String.Format("{0:0.##} Bps", doublebytes);
            }

            return null;
        }

        /// <summary>
        /// Computes the overral bitrate.
        /// </summary>
        /// <param name="general">The filled GeneralTrack class.</param>
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
        /// Helper method to parse channels count from channels string, if fails return default value
        /// </summary>
        /// <param name="channelstring">The formatted channel string.</param>
        /// <returns>Numbers of channels parsed from the input string, 0 in case of failure.</returns>
        private int? ParseChannelsCount(string channelstring)
        {
            System.Text.RegularExpressions.Match mat = new System.Text.RegularExpressions.Regex("(\\d)\\schannels").Match(channelstring);

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
        /// Helper method to calculate total milliseconds
        /// </summary>
        /// <param name="tmpitem">The filled VideoTrack class.</param>
        /// <returns>The computed total milliseconds, 0 in case of failure.</returns>
        private int? CalculateDuration(VideoTrack tmpitem)
        {
            if (String.IsNullOrEmpty(tmpitem.DurationTimecode))
            {
                if (tmpitem.FrameCount > 0 && tmpitem.FrameRate > 0)
                {
                    return (int?)((tmpitem.FrameCount / tmpitem.FrameRate) * 1000);
                }
            }

            if (!String.IsNullOrEmpty(tmpitem.DurationTimecode))
            {
                TimeSpan.TryParse(tmpitem.DurationTimecode, out TimeSpan tmp);

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
                foreach (string str in input.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
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
    }
}
