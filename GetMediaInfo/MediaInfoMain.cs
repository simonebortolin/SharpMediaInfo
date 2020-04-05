using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GetMediaInfo;

namespace GetMediaInfoDDLV2
{
    /// <summary>
    /// Stream Kind Enumeration
    /// </summary>
    public enum MediaInfoStreamKind
    {
        /// <summary>
        /// General
        /// </summary>
        General,
        /// <summary>
        /// Video
        /// </summary>
        Video,
        /// <summary>
        /// Audio
        /// </summary>
        Audio,
        /// <summary>
        /// Text
        /// </summary>
        Text,
        /// <summary>
        /// Chapters
        /// </summary>
        Chapters,
        /// <summary>
        /// Image
        /// </summary>
        Image,
        /// <summary>
        /// Menu
        /// </summary>
        Menu,
    }

    /// <summary>
    /// Info Kind Enumeration
    /// </summary>
    public enum MediaInfoInfoKind
    {
        /// <summary>
        /// 
        /// </summary>
        Name,
        /// <summary>
        /// 
        /// </summary>
        Text,
        /// <summary>
        /// 
        /// </summary>
        Measure,
        /// <summary>
        /// 
        /// </summary>
        Options,
        /// <summary>
        /// 
        /// </summary>
        NameText,
        /// <summary>
        /// 
        /// </summary>
        MeasureText,
        /// <summary>
        /// 
        /// </summary>
        Info,
        /// <summary>
        /// 
        /// </summary>
        HowTo
    }

    /// <summary>
    /// Info Options Enumeration
    /// </summary>
    public enum InfoOptions
    {
        /// <summary>
        /// 
        /// </summary>
        ShowInInform,
        /// <summary>
        /// 
        /// </summary>
        Support,
        /// <summary>
        /// 
        /// </summary>
        ShowInSupported,
        /// <summary>
        /// 
        /// </summary>
        TypeOfValue
    }

    /// <inheritdoc />
    /// <summary>
    /// When called with a proper file target, returns a MediaInfo object filled with list of media tracks containing
    /// every information MediaInfo.dll can collect.
    /// Tracks are accessibles as properties.
    /// </summary>
    public class MediaInfo : IDisposable
    {
        #region ImportedMediaInfoFunctions
        [DllImport("MediaInfo.dll")]
        internal static extern int MediaInfo_Close(IntPtr handle);
        [DllImport("MediaInfo.dll")]
        internal static extern int MediaInfo_Count_Get(IntPtr handle, [MarshalAs(UnmanagedType.U4)] MediaInfoStreamKind streamKind, int streamNumber);
        [DllImport("MediaInfo.dll")]
        internal static extern void MediaInfo_Delete(IntPtr handle);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_Get(IntPtr handle, [MarshalAs(UnmanagedType.U4)] MediaInfoStreamKind streamKind, uint streamNumber, string parameter, [MarshalAs(UnmanagedType.U4)] MediaInfoInfoKind kindOfInfo, [MarshalAs(UnmanagedType.U4)] MediaInfoInfoKind kindOfSearch);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_GetI(IntPtr handle, [MarshalAs(UnmanagedType.U4)] MediaInfoStreamKind streamKind, uint streamNumber, uint parameter, [MarshalAs(UnmanagedType.U4)] MediaInfoInfoKind kindOfInfo);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_Inform(IntPtr handle, [MarshalAs(UnmanagedType.U4)] uint reserved);
        [DllImport("MediaInfo.dll")]
        internal static extern IntPtr MediaInfo_New();
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern int MediaInfo_Open(IntPtr handle, string fileName);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_Option(IntPtr handle, string optionString, string value);
        [DllImport("MediaInfo.dll")]
        internal static extern int MediaInfo_State_Get(IntPtr handle);
        #endregion

        #region GlobalVariables
        /// <summary>
        /// The list of video streams
        /// </summary>
        internal List<VideoTrack> _Video;
        /// <summary>
        /// The list of image streams
        /// </summary>
        internal List<ImageTrack> _Image;
        /// <summary>
        /// The list of general streams
        /// </summary>
        internal List<GeneralTrack> _General;
        /// <summary>
        /// The list of audio streams
        /// </summary>
        internal List<AudioTrack> _Audio;
        /// <summary>
        /// The list of text streams
        /// </summary>
        internal List<TextTrack> _Text;
        /// <summary>
        /// The list of menu streams
        /// </summary>
        internal List<MenuTrack> _Menu;
        /// <summary>
        /// The video streams count
        /// </summary>
        internal int _VideoCount;
        /// <summary>
        /// The general streams count
        /// </summary>
        internal int _GeneralCount;
        /// <summary>
        /// The audio streams count
        /// </summary>
        internal int _AudioCount;
        /// <summary>
        /// The text streams count
        /// </summary>
        internal int _TextCount;
        /// <summary>
        /// The menu streams count
        /// </summary>
        internal int _MenuCount;
        /// <summary>
        /// The image streams count
        /// </summary>
        internal int _ImageCount;
        /// <summary>report
        /// The complete information from Mediainfo
        /// </summary>
        internal string _InfoComplete;
        /// <summary>
        /// The standard information from Mediainfo
        /// </summary>
        internal string _InfoStandard;
        /// <summary>
        /// The custom information from Mediainfo
        /// </summary>
        internal string _InfoCustom;
        /// <summary>
        /// The path of the file that it's analyzed
        /// </summary>
        internal string _FileName;
        /// <summary>
        /// The handle to call mediainfo functions
        /// </summary>
        internal IntPtr Handle;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor :
        /// When called with a proper file target, returns a MediaInfo object filled with list of media tracks containing
        /// every information MediaInfo.dll can collect.
        /// Tracks are accessibles as properties.
        /// </summary>
        /// <param name="path">The path of the files to be analyzed.</param>
        /// <param name="report"></param>
        /// <param name="cover"></param>
        public MediaInfo(string path, bool report = false, bool cover = false)
        {
            if (!CheckFileExistence(path))
                return;
            _FileName = path;

            this.Handle = MediaInfo.MediaInfo_New();
            if (cover) GetCover();
            MediaInfo.MediaInfo_Open(this.Handle, path);
            try
            {

                GetStreamCount();
                GetAllInfos(cover);
                if (report) GetReport();
            }
            finally //ensure MediaInfo_Close is called even if something goes wrong 
            {
                MediaInfo.MediaInfo_Close(this.Handle);
            }
        }
        #endregion

        #region Disposable Pattern
        internal bool Disposed;

#pragma warning disable 1591
        ~MediaInfo()
        {
            Dispose(false);
        }

        protected bool IsDisposed
        {
            get { return Disposed; }
        }

        protected void CheckDisposed()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
        }

        /// <summary>Call this one to kill the wrapper, and close his handle to the MediaInfo.dll, you should never need it anyway </summary>
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
            if (!this.Disposed)
            {
                if (disposing)
                {
                    DisposeManagedResources();
                }
                DisposeUnmanagedResources();
            }
            this.Disposed = true;
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
        internal void GetAllInfos(bool cover)
        {
            GetGeneralInfo(cover);
            GetVideoInfo();
            GetAudioInfo();
            GetTextInfo();
            GetMenuInfo();
            GetImageInfo();
        }

        /// <summary>
        /// Gets all stream types count.
        /// </summary>
        internal void GetStreamCount()
        {
            _AudioCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Audio, -1);
            _VideoCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Video, -1);
            _GeneralCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.General, -1);
            _TextCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Text, -1);
            _MenuCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Menu, -1);
            _ImageCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Image, -1);

        }

        /// <summary>
        /// Gets the specific information from mediainfo.
        /// </summary>
        /// <param name="kindOfStream">The kind of stream.</param>
        /// <param name="trackindex">The track index.</param>
        /// <param name="nameOfParameter">The name of parameter.</param>
        /// <returns>A string containing the parameter's value or an empty string in case of problems.</returns>
        internal unsafe string GetSpecificInfo(MediaInfoStreamKind kindOfStream, int trackindex, string nameOfParameter)
        {
            IntPtr p = MediaInfo.MediaInfo_Get(this.Handle, kindOfStream, Convert.ToUInt32(trackindex), nameOfParameter, MediaInfoInfoKind.Text, MediaInfoInfoKind.Name);
            return ParseOutput(p);
        }

        /// <summary>
        /// Gets the specific sub information from mediainfo.
        /// </summary>
        /// <param name="kindOfStream">The kind of stream.</param>
        /// <param name="trackindex">The trackindex.</param>
        /// <param name="nameOfParameter">The name of parameter.</param>
        /// <param name="infoKind">Kind of the information.</param>
        /// <returns>A string containing the parameter's value or an empty string in case of problems.</returns>
        internal unsafe string GetSpecificSubInfo(MediaInfoStreamKind kindOfStream, int trackindex, int nameOfParameter, MediaInfoInfoKind infoKind)
        {
            IntPtr p = MediaInfo.MediaInfo_GetI(this.Handle, kindOfStream, Convert.ToUInt32(trackindex), Convert.ToUInt32(nameOfParameter), infoKind);
            return ParseOutput(p);
        }

        /// <summary>
        /// Parses the mediainfo library output. The reading is performed using a buffer of 8192 chars.
        /// </summary>
        /// <param name="p">The pointer to the memory area when the informations are stored.</param>
        /// <returns>The parsed string from the memory area referred to the input pointer.</returns>
        internal unsafe string ParseOutput(IntPtr p)
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

        internal void GetReport()
        {
            IntPtr tmp = MediaInfo.MediaInfo_Option(this.Handle, "Complete", "1");
            string ctrl = Marshal.PtrToStringUni(tmp);
            _InfoComplete = Marshal.PtrToStringUni(MediaInfo.MediaInfo_Inform(this.Handle, 0));
            MediaInfo.MediaInfo_Option(this.Handle, "Complete", "");
            _InfoStandard = Marshal.PtrToStringUni(MediaInfo.MediaInfo_Inform(this.Handle, 0));
        }

        internal void GetCover()
        {
            MediaInfo.MediaInfo_Option(this.Handle, "Cover_Data", "base64");
        }

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
                if (string.IsNullOrEmpty(this._InfoComplete))
                {
                    MediaInfo.MediaInfo_Open(this.Handle, _FileName);
                    IntPtr tmp = MediaInfo.MediaInfo_Option(this.Handle, "Complete", "1");
                    string ctrl = Marshal.PtrToStringUni(tmp);
                    _InfoComplete = Marshal.PtrToStringUni(MediaInfo.MediaInfo_Inform(this.Handle, 0));
                    MediaInfo.MediaInfo_Close(this.Handle);
                }
                return _InfoComplete;
            }
        }

        public string InfoCompleteNoSpace => InfoComplete.Replace("  ", "").Replace(" : ", ": ");

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
                if (string.IsNullOrEmpty(this._InfoStandard))
                {
                    MediaInfo.MediaInfo_Open(this.Handle, _FileName);
                    MediaInfo.MediaInfo_Option(this.Handle, "Complete", "");
                    _InfoStandard = Marshal.PtrToStringUni(MediaInfo.MediaInfo_Inform(this.Handle, 0));
                    MediaInfo.MediaInfo_Close(this.Handle);
                }
                return _InfoStandard;
            }
        }

        public string InfoStandardNoSpace => InfoStandard.Replace("  ", "").Replace(" : ", ": ");

        /// <summary>
        /// Lists every available property in every track
        /// </summary>
        /// <returns>The complete report with the lists of all infos of all streams and tracks</returns>
        public string InfoCustom
        {
            get
            {
                if (string.IsNullOrEmpty(_InfoCustom))
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("General" + Environment.NewLine);
                    ListEveryAvailablePropery<GeneralTrack>(General, sb);
                    sb.Append(Environment.NewLine);
                    sb.Append("Video" + Environment.NewLine);
                    ListEveryAvailablePropery<VideoTrack>(Video, sb);
                    sb.Append(Environment.NewLine);
                    sb.Append("Audio" + Environment.NewLine);
                    ListEveryAvailablePropery<AudioTrack>(Audio, sb);
                    sb.Append(Environment.NewLine);
                    sb.Append("Text" + Environment.NewLine);
                    ListEveryAvailablePropery<TextTrack>(Text, sb);
                    sb.Append(Environment.NewLine);
                    sb.Append("Menu" + Environment.NewLine);
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
        /// <param name="l">tracklist, for instance Video</param>
        /// <param name="sb">The StringBuilder where append all properties strings.</param>
        internal void ListEveryAvailablePropery<T1>(List<T1> l, System.Text.StringBuilder sb)
        {
            foreach (T1 track in l)
            {
                foreach (PropertyInfo p in track.GetType().GetProperties())
                {
                    sb.Append((p.GetValue(track, null).ToString() == "") ? p.Name + " : Not available" + Environment.NewLine : p.Name + " : " + p.GetValue(track, null) + Environment.NewLine);
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
        internal static string ParameterList()
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
                    GetGeneralInfo(false);
                }
                return this._General;
            }
        }

        /// <summary>
        /// Gets the informations of all general streams.
        /// </summary>
        internal void GetGeneralInfo(bool cover)
        {
            if (this._General == null)
            {
                this._General = new List<GeneralTrack>();
                int count = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.General, -1);
                if (count > 0)
                {
                    int max = count - 1;
                    for (int curr = 0; curr <= max; curr++)
                    {
                        GeneralTrack tracktemp = new GeneralTrack();
                        tracktemp.Count = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Count"));
                        tracktemp.Status = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Status");
                        tracktemp.StreamCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamCount"));
                        tracktemp.StreamKind = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamKind");
                        tracktemp.StreamKindString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamKind/String");
                        tracktemp.StreamKindID = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamKindID"));
                        tracktemp.StreamKindPos = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamKindPos"));
                        tracktemp.StreamOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamOrder"));
                        tracktemp.FirstPacketOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "FirstPacketOrder"));
                        tracktemp.Inform = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Inform");
                        tracktemp.ID = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ID");
                        tracktemp.IDString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ID/String");
                        tracktemp.OriginalSourceMediumID = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OriginalSourceMedium_ID");
                        tracktemp.OriginalSourceMediumIDString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OriginalSourceMedium_ID/String");
                        tracktemp.UniqueID = GetSpecificInfo(MediaInfoStreamKind.General, curr, "UniqueID");
                        tracktemp.UniqueIDString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "UniqueID/String");
                        tracktemp.MenuID = GetSpecificInfo(MediaInfoStreamKind.General, curr, "MenuID");
                        tracktemp.MenuIDString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "MenuID/String");
                        tracktemp.GeneralCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "GeneralCount"));
                        tracktemp.VideoCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "VideoCount"));
                        tracktemp.AudioCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "AudioCount"));
                        tracktemp.TextCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "TextCount"));
                        tracktemp.OtherCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "OtherCount"));
                        tracktemp.ImageCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "ImageCount"));
                        tracktemp.MenuCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "MenuCount"));
                        tracktemp.VideoFormatList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Video_Format_List"));
                        tracktemp.VideoFormatWithHintList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Video_Format_WithHint_List"));
                        tracktemp.VideoLanguageList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Video_Language_List"));
                        tracktemp.AudioFormatList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Audio_Format_List"));
                        tracktemp.AudioFormatWithHintList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Audio_Format_WithHint_List"));
                        tracktemp.AudioLanguageList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Audio_Language_List"));
                        tracktemp.TextFormatList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Text_Format_List"));
                        tracktemp.TextFormatWithHintList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Text_Format_WithHint_List"));
                        tracktemp.TextLanguageList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Text_Language_List"));
                        tracktemp.OtherFormatList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Other_Format_List"));
                        tracktemp.OtherFormatWithHintLis = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Other_Format_WithHint_Lis"));
                        tracktemp.OtherLanguageList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Other_Language_List"));
                        tracktemp.ImageFormatList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Image_Format_List"));
                        tracktemp.ImageFormatWithHintLis = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Image_Format_WithHint_Lis"));
                        tracktemp.ImageLanguageList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Image_Language_List"));
                        tracktemp.MenuFormatList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Menu_Format_List"));
                        tracktemp.MenuFormatWithHintList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Menu_Format_WithHint_List"));
                        tracktemp.MenuLanguageList = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Menu_Language_List"));
                        tracktemp.CompleteName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CompleteName");
                        tracktemp.FolderName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FolderName");
                        tracktemp.FileNameExtension = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileNameExtension");
                        tracktemp.FileName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileName");
                        tracktemp.FileExtension = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileExtension");
                        tracktemp.CompleteNameLast = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CompleteName_Last");
                        tracktemp.FolderNameLast = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FolderName_Last");
                        tracktemp.FileNameExtensionLast = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileNameExtension_Last");
                        tracktemp.FileNameLast = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileName_Last");
                        tracktemp.FileExtensionLast = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileExtension_Last");
                        tracktemp.Format = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format");
                        tracktemp.FormatInfo = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format/Info");
                        tracktemp.FormatUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format/Url");
                        tracktemp.FormatExtensions = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format/Extensions");
                        tracktemp.FormatCommercial = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format_Commercial");
                        tracktemp.FormatCommercialIfAny = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format_Commercial_IfAny");
                        tracktemp.FormatVersion = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format_Version");
                        tracktemp.FormatProfile = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format_Profile");
                        tracktemp.FormatLevel = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format_Level");
                        tracktemp.FormatCompression = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format_Compression");
                        tracktemp.FormatSettings = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Format_Settings");
                        tracktemp.InternetMediaType = GetSpecificInfo(MediaInfoStreamKind.General, curr, "InternetMediaType");
                        tracktemp.CodecID = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CodecID");
                        tracktemp.CodecIDString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CodecID/String");
                        tracktemp.CodecIDInfo = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CodecID/Info");
                        tracktemp.CodecIDHint = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CodecID/Hint");
                        tracktemp.CodecIDUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CodecID/Url");
                        tracktemp.CodecIDDescription = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CodecID_Description");
                        tracktemp.CodecIDVersion = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CodecID_Version");
                        tracktemp.CodecIDCompatible = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CodecID_Compatible");
                        tracktemp.Interleaved = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Interleaved");
                        tracktemp.FileSize = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileSize"));
                        tracktemp.FileSizeString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileSize/String");
                        tracktemp.FileSizeString1 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileSize/String1");
                        tracktemp.FileSizeString2 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileSize/String2");
                        tracktemp.FileSizeString3 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileSize/String3");
                        tracktemp.FileSizeFormatted = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FileSize/String4");
                        tracktemp.Duration = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Duration"));
                        tracktemp.DurationString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Duration/String");
                        tracktemp.DurationString1 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Duration/String1");
                        tracktemp.DurationString2 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Duration/String2");
                        tracktemp.DurationFormatted = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Duration/String3");
                        tracktemp.DurationString4 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Duration/String4");
                        tracktemp.DurationString5 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Duration/String5");
                        tracktemp.DurationStart = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Duration_Start");
                        tracktemp.DurationEnd = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Duration_End");
                        tracktemp.OverallBitRateMode = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate_Mode");
                        tracktemp.OverallBitRateModeString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate_Mode/String");
                        tracktemp.OverallBitRate = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate"));
                        tracktemp.OverallBitRateString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate/String");
                        tracktemp.OverallBitRateMinimum = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate_Minimum"));
                        tracktemp.OverallBitRateMinimumString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate_Minimum/String");
                        tracktemp.OverallBitRateNominal = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate_Nominal"));
                        tracktemp.OverallBitRateNominalString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate_Nominal/String");
                        tracktemp.OverallBitRateMaximum = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate_Maximum"));
                        tracktemp.OverallBitRateMaximumString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OverallBitRate_Maximum/String");
                        tracktemp.FrameRate = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "FrameRate"));
                        tracktemp.FrameRateString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FrameRate/String");
                        tracktemp.FrameRateNum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "FrameRate_Num"));
                        tracktemp.FrameRateDen = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "FrameRate_Den"));
                        tracktemp.FrameCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "FrameCount"));
                        tracktemp.Delay = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay"));
                        tracktemp.DelayString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay/String");
                        tracktemp.DelayString1 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay/String1");
                        tracktemp.DelayString2 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay/String2");
                        tracktemp.DelayFormatted = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay/String3");
                        tracktemp.DelayString4 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay/String4");
                        tracktemp.DelayString5 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay/String5");
                        tracktemp.DelaySettings = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay_Settings");
                        tracktemp.DelayDropFrame = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay_DropFrame");
                        tracktemp.DelaySource = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay_Source");
                        tracktemp.DelaySourceString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Delay_Source/String");
                        tracktemp.StreamSize = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize"));
                        tracktemp.StreamSizeString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize/String");
                        tracktemp.StreamSizeString1 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize/String1");
                        tracktemp.StreamSizeString2 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize/String2");
                        tracktemp.StreamSizeString3 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize/String3");
                        tracktemp.StreamSizeString4 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize/String4");
                        tracktemp.StreamSizeProportion = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize_Proportion"));
                        tracktemp.StreamSizeDemuxed = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize_Demuxed"));
                        tracktemp.StreamSizeDemuxedString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize_Demuxed/String");
                        tracktemp.StreamSizeDemuxedString1 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize_Demuxed/String1");
                        tracktemp.StreamSizeDemuxedString2 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize_Demuxed/String2");
                        tracktemp.StreamSizeDemuxedString3 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize_Demuxed/String3");
                        tracktemp.StreamSizeDemuxedString4 = GetSpecificInfo(MediaInfoStreamKind.General, curr, "StreamSize_Demuxed/String4");
                        tracktemp.HeaderSize = GetSpecificInfo(MediaInfoStreamKind.General, curr, "HeaderSize");
                        tracktemp.DataSize = GetSpecificInfo(MediaInfoStreamKind.General, curr, "DataSize");
                        tracktemp.FooterSize = GetSpecificInfo(MediaInfoStreamKind.General, curr, "FooterSize");
                        tracktemp.IsStreamable = GetSpecificInfo(MediaInfoStreamKind.General, curr, "IsStreamable");
                        tracktemp.AlbumReplayGainGain = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Album_ReplayGain_Gain");
                        tracktemp.AlbumReplayGainGainString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Album_ReplayGain_Gain/String");
                        tracktemp.AlbumReplayGainPeak = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Album_ReplayGain_Peak");
                        tracktemp.Encryption = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encryption");
                        tracktemp.EncryptionFormat = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encryption_Format");
                        tracktemp.EncryptionLength = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encryption_Length");
                        tracktemp.EncryptionMethod = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encryption_Method");
                        tracktemp.EncryptionMode = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encryption_Mode");
                        tracktemp.EncryptionPadding = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encryption_Padding");
                        tracktemp.EncryptionInitializationVector = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encryption_InitializationVector");
                        tracktemp.UniversalAdIDString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "UniversalAdID/String");
                        tracktemp.UniversalAdIDRegistry = GetSpecificInfo(MediaInfoStreamKind.General, curr, "UniversalAdID_Registry");
                        tracktemp.UniversalAdIDValue = GetSpecificInfo(MediaInfoStreamKind.General, curr, "UniversalAdID_Value");
                        tracktemp.Title = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Title");
                        tracktemp.TitleMore = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Title_More");
                        tracktemp.TitleUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Title/Url");
                        tracktemp.Domain = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Domain");
                        tracktemp.Collection = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Collection");
                        tracktemp.Season = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Season");
                        tracktemp.SeasonPosition = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Season_Position"));
                        tracktemp.SeasonPositionTotal = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Season_Position_Total");
                        tracktemp.Movie = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Movie");
                        tracktemp.MovieMore = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Movie_More");
                        tracktemp.MovieCountry = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Movie/Country"));
                        tracktemp.MovieUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Movie/Url");
                        tracktemp.Album = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Album");
                        tracktemp.Thejoshuatree = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Thejoshuatree");
                        tracktemp.AlbumMore = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Album_More");
                        tracktemp.AlbumSort = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Album/Sort");
                        tracktemp.AlbumPerformer = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Album/Performer");
                        tracktemp.AlbumPerformerSort = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Album/Performer/Sort");
                        tracktemp.AlbumPerformerUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Album/Performer/Url");
                        tracktemp.Comic = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Comic");
                        tracktemp.ComicMore = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Comic_More");
                        tracktemp.ComicPositionTotal = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Comic/Position_Total");
                        tracktemp.Part = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Part");
                        tracktemp.PartPosition = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Part/Position"));
                        tracktemp.PartPositionTotal = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Part/Position_Total");
                        tracktemp.Track = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Track");
                        tracktemp.TrackMore = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Track_More");
                        tracktemp.TrackUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Track/Url");
                        tracktemp.TrackSort = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Track/Sort");
                        tracktemp.TrackPosition = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Track/Position"));
                        tracktemp.TrackPositionTotal = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Track/Position_Total");
                        tracktemp.PackageName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "PackageName");
                        tracktemp.Grouping = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Grouping");
                        tracktemp.Chapter = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Chapter");
                        tracktemp.SubTrack = GetSpecificInfo(MediaInfoStreamKind.General, curr, "SubTrack");
                        tracktemp.OriginalAlbum = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Original/Album");
                        tracktemp.OriginalMovie = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Original/Movie");
                        tracktemp.OriginalPart = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Original/Part");
                        tracktemp.OriginalTrack = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Original/Track");
                        tracktemp.Compilation = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Compilation");
                        tracktemp.CompilationString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Compilation/String");
                        tracktemp.Performer = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Performer");
                        tracktemp.PerformerSort = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Performer/Sort");
                        tracktemp.PerformerUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Performer/Url");
                        tracktemp.OriginalPerformer = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Original/Performer");
                        tracktemp.Accompaniment = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Accompaniment");
                        tracktemp.Composer = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Composer");
                        tracktemp.ComposerNationality = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Composer/Nationality");
                        tracktemp.ComposerSort = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Composer/Sort");
                        tracktemp.Arranger = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Arranger");
                        tracktemp.Lyricist = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Lyricist");
                        tracktemp.OriginalLyricist = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Original/Lyricist");
                        tracktemp.Conductor = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Conductor");
                        tracktemp.Director = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Director");
                        tracktemp.CoDirector = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CoDirector");
                        tracktemp.AssistantDirector = GetSpecificInfo(MediaInfoStreamKind.General, curr, "AssistantDirector");
                        tracktemp.DirectorOfPhotography = GetSpecificInfo(MediaInfoStreamKind.General, curr, "DirectorOfPhotography");
                        tracktemp.SoundEngineer = GetSpecificInfo(MediaInfoStreamKind.General, curr, "SoundEngineer");
                        tracktemp.ArtDirector = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ArtDirector");
                        tracktemp.ProductionDesigner = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ProductionDesigner");
                        tracktemp.Choregrapher = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Choregrapher");
                        tracktemp.CostumeDesigner = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CostumeDesigner");
                        tracktemp.Actor = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Actor");
                        tracktemp.ActorCharacter = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Actor_Character");
                        tracktemp.WrittenBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "WrittenBy");
                        tracktemp.ScreenplayBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ScreenplayBy");
                        tracktemp.EditedBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "EditedBy");
                        tracktemp.CommissionedBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CommissionedBy");
                        tracktemp.Producer = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Producer");
                        tracktemp.CoProducer = GetSpecificInfo(MediaInfoStreamKind.General, curr, "CoProducer");
                        tracktemp.ExecutiveProducer = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ExecutiveProducer");
                        tracktemp.MusicBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "MusicBy");
                        tracktemp.DistributedBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "DistributedBy");
                        tracktemp.MasteredBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "MasteredBy");
                        tracktemp.EncodedBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "EncodedBy");
                        tracktemp.RemixedBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "RemixedBy");
                        tracktemp.ProductionStudio = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ProductionStudio");
                        tracktemp.ThanksTo = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ThanksTo");
                        tracktemp.Publisher = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Publisher");
                        tracktemp.PublisherURL = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Publisher/URL");
                        tracktemp.Label = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Label");
                        tracktemp.Genre = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Genre");
                        tracktemp.PodcastCategory = GetSpecificInfo(MediaInfoStreamKind.General, curr, "PodcastCategory");
                        tracktemp.Mood = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Mood");
                        tracktemp.ContentType = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ContentType");
                        tracktemp.Subject = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Subject");
                        tracktemp.Description = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Description");
                        tracktemp.Keywords = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Keywords");
                        tracktemp.Summary = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Summary");
                        tracktemp.Synopsis = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Synopsis");
                        tracktemp.Period = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Period");
                        tracktemp.LawRating = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "LawRating"));
                        tracktemp.LawRatingReason = GetSpecificInfo(MediaInfoStreamKind.General, curr, "LawRating_Reason");
                        tracktemp.ICRA = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ICRA");
                        tracktemp.ReleasedDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Released_Date");
                        tracktemp.OriginalReleasedDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Original/Released_Date");
                        tracktemp.RecordedDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Recorded_Date");
                        tracktemp.EncodedDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Date");
                        tracktemp.TaggedDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Tagged_Date");
                        tracktemp.WrittenDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Written_Date");
                        tracktemp.MasteredDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Mastered_Date");
                        tracktemp.FileCreatedDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "File_Created_Date");
                        tracktemp.FileCreatedDateLocal = GetSpecificInfo(MediaInfoStreamKind.General, curr, "File_Created_Date_Local");
                        tracktemp.FileModifiedDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "File_Modified_Date");
                        tracktemp.FileModifiedDateLocal = GetSpecificInfo(MediaInfoStreamKind.General, curr, "File_Modified_Date_Local");
                        tracktemp.RecordedLocation = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Recorded_Location");
                        tracktemp.WrittenLocation = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Written_Location"));
                        tracktemp.ArchivalLocation = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Archival_Location");
                        tracktemp.EncodedApplication = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Application");
                        tracktemp.EncodedApplicationString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Application/String");
                        tracktemp.EncodedApplicationCompany = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Application_Company");
                        tracktemp.EncodedApplicationName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Application_Name");
                        tracktemp.EncodedApplicationVersion = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Application_Version");
                        tracktemp.EncodedApplicationUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Application_Url");
                        tracktemp.EncodedLibrary = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Library");
                        tracktemp.EncodedLibraryString = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Library/String");
                        tracktemp.EncodedLibraryCompanyName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Library_CompanyName");
                        tracktemp.EncodedLibraryName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Library_Name");
                        tracktemp.EncodedLibraryVersion = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Library_Version");
                        tracktemp.EncodedLibraryDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Library_Date");
                        tracktemp.EncodedLibrarySettings = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Library_Settings");
                        tracktemp.EncodedOperatingSystem = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_OperatingSystem");
                        tracktemp.Cropped = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Cropped");
                        tracktemp.Dimensions = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Dimensions");
                        tracktemp.DotsPerInch = GetSpecificInfo(MediaInfoStreamKind.General, curr, "DotsPerInch");
                        tracktemp.Lightness = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Lightness");
                        tracktemp.OriginalSourceMedium = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OriginalSourceMedium");
                        tracktemp.OriginalSourceForm = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OriginalSourceForm");
                        tracktemp.OriginalSourceFormName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OriginalSourceForm/Name");
                        tracktemp.TaggedApplication = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Tagged_Application");
                        tracktemp.BPM = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "BPM"));
                        tracktemp.ISRC = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ISRC");
                        tracktemp.ISBN = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "ISBN"));
                        tracktemp.BarCode = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "BarCode"));
                        tracktemp.LCCN = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "LCCN"));
                        tracktemp.CatalogNumber = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "CatalogNumber"));
                        tracktemp.LabelCode = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "LabelCode"));
                        tracktemp.Owner = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Owner");
                        tracktemp.Copyright = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Copyright");
                        tracktemp.CopyrightUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Copyright/Url");
                        tracktemp.ProducerCopyright = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Producer_Copyright");
                        tracktemp.TermsOfUse = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "TermsOfUse"));
                        tracktemp.ServiceName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ServiceName");
                        tracktemp.ServiceChannel = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ServiceChannel");
                        tracktemp.ServiceUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Service/Url");
                        tracktemp.ServiceProvider = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ServiceProvider");
                        tracktemp.ServiceProviderUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ServiceProvider/Url");
                        tracktemp.ServiceType = GetSpecificInfo(MediaInfoStreamKind.General, curr, "ServiceType");
                        tracktemp.NetworkName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "NetworkName");
                        tracktemp.OriginalNetworkName = GetSpecificInfo(MediaInfoStreamKind.General, curr, "OriginalNetworkName");
                        tracktemp.Country = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Country");
                        tracktemp.TimeZone = GetSpecificInfo(MediaInfoStreamKind.General, curr, "TimeZone");
                        tracktemp.Cover = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Cover"),
                            GetBoolOrNull);
                        tracktemp.CoverDescription = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Cover_Description"));
                        tracktemp.CoverType = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Cover_Type"));
                        tracktemp.CoverMime = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Cover_Mime"));
                        if(cover) tracktemp.CoverData = GetListOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Cover_Data"),
                                Convert.FromBase64String);
                        tracktemp.CanGetCover = cover;
                        tracktemp.Lyrics = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Lyrics");
                        tracktemp.Comment = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Comment");
                        tracktemp.Rating = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Rating"));
                        tracktemp.AddedDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Added_Date");
                        tracktemp.PlayedFirstDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Played_First_Date");
                        tracktemp.PlayedLastDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Played_Last_Date");
                        tracktemp.PlayedCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "Played_Count"));
                        tracktemp.EPGPositionsBegin = GetSpecificInfo(MediaInfoStreamKind.General, curr, "EPG_Positions_Begin");
                        tracktemp.EPGPositionsEnd = GetSpecificInfo(MediaInfoStreamKind.General, curr, "EPG_Positions_End");
                        tracktemp.Author = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Author");
                        tracktemp.Artist = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Artist");
                        tracktemp.MusicianInstrument = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Musician_Instrument");
                        tracktemp.Technician = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Technician");
                        tracktemp.EncodedOriginalDistributedBy = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Original/DistributedBy");
                        tracktemp.RadioStation = GetSpecificInfo(MediaInfoStreamKind.General, curr, "RadioStation");
                        tracktemp.RadioStationOwner = GetSpecificInfo(MediaInfoStreamKind.General, curr, "RadioStation/Owner");
                        tracktemp.RadioStationURL = GetSpecificInfo(MediaInfoStreamKind.General, curr, "RadioStation/URL");
                        tracktemp.Synopsys = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Synopsys");
                        tracktemp.IRCA = GetSpecificInfo(MediaInfoStreamKind.General, curr, "IRCA");
                        tracktemp.Language = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Language");
                        tracktemp.Medium = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Medium");
                        tracktemp.Product = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Product");
                        tracktemp.OriginalRecordedDate = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Original/Recorded_Date");
                        tracktemp.EncodedOriginal = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Original");
                        tracktemp.EncodedOriginalUrl = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Encoded_Original/Url");
                        tracktemp.MSDI = GetSpecificInfo(MediaInfoStreamKind.General, curr, "MSDI");

                        // Other
                        tracktemp.VideoCodecList = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Video_Codec_List");
                        tracktemp.AudioCodecList = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Audio_Codec_List");
                        tracktemp.TextCodecList = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Text_Codec_List");
                        tracktemp.ChaptersCodecList = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Chapters_Codec_List");
                        tracktemp.ChaptersLanguageList = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Chapters_Language_List");
                        tracktemp.ImageCodecList = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Image_Codec_List");
                        tracktemp.Other = GetSpecificInfo(MediaInfoStreamKind.General, curr, "Other");
                        tracktemp.PlayTimeMillisec = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.General, curr, "PlayTime"));
                        tracktemp.PlayTimeFormatted = GetSpecificInfo(MediaInfoStreamKind.General, curr, "PlayTime/String3");



                        if (tracktemp.OverallBitRate == 0)
                        {
                            tracktemp.OverallBitRate = ComputeOverrallBitRate(tracktemp);

                            if (tracktemp.OverallBitRate > 0 && string.IsNullOrEmpty(tracktemp.OverallBitRateString))
                            {
                                tracktemp.OverallBitRateString = GetFormattedBitRate(tracktemp.OverallBitRate);
                            }
                        }

                        this._General.Add(tracktemp);
                    }
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
        internal void GetVideoInfo()
        {
            if (this._Video == null)
            {
                this._Video = new List<VideoTrack>();
                int count = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Video, -1);
                if (count > 0)
                {
                    int max = count - 1;
                    for (int curr = 0; curr <= max; curr++)
                    {
                        VideoTrack tracktemp = new VideoTrack();
                        tracktemp.Count = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Count"));
                        tracktemp.Status = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Status");
                        tracktemp.StreamCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamCount"));
                        tracktemp.StreamKind = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamKind");
                        tracktemp.StreamKindString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamKind/String");
                        tracktemp.StreamKindID = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamKindID"));
                        tracktemp.StreamKindPos = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamKindPos"));
                        tracktemp.StreamOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamOrder"));
                        tracktemp.FirstPacketOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FirstPacketOrder"));
                        tracktemp.Inform = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Inform");
                        tracktemp.ID = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ID");
                        tracktemp.IDString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ID/String");
                        tracktemp.OriginalSourceMediumID = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "OriginalSourceMedium_ID");
                        tracktemp.OriginalSourceMediumIDString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "OriginalSourceMedium_ID/String");
                        tracktemp.UniqueID = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "UniqueID");
                        tracktemp.UniqueIDString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "UniqueID/String");
                        tracktemp.MenuID = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "MenuID");
                        tracktemp.MenuIDString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "MenuID/String");
                        tracktemp.Format = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format");
                        tracktemp.FormatInfo = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format/Info");
                        tracktemp.FormatUrl = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format/Url");
                        tracktemp.FormatCommercial = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Commercial");
                        tracktemp.FormatCommercialIfAny = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Commercial_IfAny");
                        tracktemp.FormatVersion = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Version");
                        tracktemp.FormatProfile = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Profile");
                        tracktemp.FormatLevel = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Level");
                        tracktemp.FormatTier = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Tier");
                        tracktemp.FormatCompression = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Compression");
                        tracktemp.MultiViewBaseProfile = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "MultiView_BaseProfile");
                        tracktemp.MultiViewCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "MultiView_Count"));
                        tracktemp.MultiViewLayout = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "MultiView_Layout");
                        tracktemp.DolbyVisionString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "DolbyVision/String");
                        tracktemp.DolbyVisionVersion = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "DolbyVision_Version");
                        tracktemp.DolbyVisionProfile = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "DolbyVision_Profile");
                        tracktemp.DolbyVisionLayers = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "DolbyVision_Layers");
                        tracktemp.FormatSettings = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings");
                        tracktemp.FormatSettingsBVOP = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_BVOP");
                        tracktemp.FormatSettingsBVOPString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_BVOP/String");
                        tracktemp.FormatSettingsQPel = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_QPel");
                        tracktemp.FormatSettingsQPelString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_QPel/String");
                        tracktemp.FormatSettingsGMC = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_GMC");
                        tracktemp.FormatSettingsGMCString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_GMC/String");
                        tracktemp.FormatSettingsMatrix = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_Matrix");
                        tracktemp.FormatSettingsMatrixString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_Matrix/String");
                        tracktemp.FormatSettingsCABAC = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_CABAC");
                        tracktemp.FormatSettingsCABACString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_CABAC/String");
                        tracktemp.FormatSettingsRefFrames = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_RefFrames");
                        tracktemp.FormatSettingsPulldown = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_Pulldown");
                        tracktemp.FormatSettingsEndianness = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_Endianness");
                        tracktemp.FormatSettingsPacking = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_Packing");
                        tracktemp.FormatSettingsFrameMode = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_FrameMode");
                        tracktemp.FormatSettingsGOP = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_GOP");
                        tracktemp.FormatSettingsWrapping = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_Wrapping");
                        tracktemp.InternetMediaType = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "InternetMediaType");
                        tracktemp.MuxingMode = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "MuxingMode");
                        tracktemp.CodecID = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "CodecID");
                        tracktemp.CodecIDString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "CodecID/String");
                        tracktemp.CodecIDInfo = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "CodecID/Info");
                        tracktemp.CodecIDHint = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "CodecID/Hint");
                        tracktemp.CodecIDUrl = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "CodecID/Url");
                        tracktemp.CodecIDDescription = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "CodecID_Description");
                        tracktemp.Duration = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration"));
                        tracktemp.DurationString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration/String");
                        tracktemp.DurationString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration/String1");
                        tracktemp.DurationString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration/String2");
                        tracktemp.DurationFormatted = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration/String3");
                        tracktemp.DurationString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration/String4");
                        tracktemp.DurationString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration/String5");
                        tracktemp.DurationFirstFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_FirstFrame"));
                        tracktemp.DurationFirstFrameString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_FirstFrame/String");
                        tracktemp.DurationFirstFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_FirstFrame/String1");
                        tracktemp.DurationFirstFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_FirstFrame/String2");
                        tracktemp.DurationFirstFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_FirstFrame/String3");
                        tracktemp.DurationFirstFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_FirstFrame/String4");
                        tracktemp.DurationFirstFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_FirstFrame/String5");
                        tracktemp.DurationLastFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_LastFrame"));
                        tracktemp.DurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_LastFrame/String");
                        tracktemp.DurationLastFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_LastFrame/String1");
                        tracktemp.DurationLastFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_LastFrame/String2");
                        tracktemp.DurationLastFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_LastFrame/String3");
                        tracktemp.DurationLastFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_LastFrame/String4");
                        tracktemp.DurationLastFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Duration_LastFrame/String5");
                        tracktemp.SourceDuration = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration"));
                        tracktemp.SourceDurationString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration/String");
                        tracktemp.SourceDurationString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration/String1");
                        tracktemp.SourceDurationString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration/String2");
                        tracktemp.SourceDurationFormatted = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration/String3");
                        tracktemp.SourceDurationString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration/String4");
                        tracktemp.SourceDurationString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration/String5");
                        tracktemp.SourceDurationFirstFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_FirstFrame"));
                        tracktemp.SourceDurationFirstFrameString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_FirstFrame/String");
                        tracktemp.SourceDurationFirstFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_FirstFrame/String1");
                        tracktemp.SourceDurationFirstFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_FirstFrame/String2");
                        tracktemp.SourceDurationFirstFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_FirstFrame/String3");
                        tracktemp.SourceDurationFirstFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_FirstFrame/String4");
                        tracktemp.SourceDurationFirstFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_FirstFrame/String5");
                        tracktemp.SourceDurationLastFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_LastFrame"));
                        tracktemp.SourceDurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_LastFrame/String");
                        tracktemp.SourceDurationLastFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_LastFrame/String1");
                        tracktemp.SourceDurationLastFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_LastFrame/String2");
                        tracktemp.SourceDurationLastFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_LastFrame/String3");
                        tracktemp.SourceDurationLastFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_LastFrame/String4");
                        tracktemp.SourceDurationLastFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_Duration_LastFrame/String5");
                        tracktemp.BitRateMode = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Mode");
                        tracktemp.BitRateModeString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Mode/String");
                        tracktemp.BitRate = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate"));
                        tracktemp.BitRateString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate/String");
                        tracktemp.BitRateMinimum = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Minimum"));
                        tracktemp.BitRateMinimumString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Minimum/String");
                        tracktemp.BitRateNominal = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Nominal"));
                        tracktemp.BitRateNominalString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Nominal/String");
                        tracktemp.BitRateMaximum = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Maximum"));
                        tracktemp.BitRateMaximumString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Maximum/String");
                        tracktemp.BitRateEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Encoded"));
                        tracktemp.BitRateEncodedString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Encoded/String");
                        tracktemp.Width = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Width");
                        tracktemp.WidthString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Width/String");
                        tracktemp.WidthOffset = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Width_Offset");
                        tracktemp.WidthOffsetString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Width_Offset/String");
                        tracktemp.WidthOriginal = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Width_Original");
                        tracktemp.WidthOriginalString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Width_Original/String");
                        tracktemp.WidthCleanAperture = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Width_CleanAperture");
                        tracktemp.WidthCleanApertureString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Width_CleanAperture/String");
                        tracktemp.Height = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Height");
                        tracktemp.HeightString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Height/String");
                        tracktemp.HeightOffset = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Height_Offset");
                        tracktemp.HeightOffsetString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Height_Offset/String");
                        tracktemp.HeightOriginal = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Height_Original");
                        tracktemp.HeightOriginalString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Height_Original/String");
                        tracktemp.HeightCleanAperture = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Height_CleanAperture");
                        tracktemp.HeightCleanApertureString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Height_CleanAperture/String");
                        tracktemp.StoredWidth = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Stored_Width");
                        tracktemp.StoredHeight = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Stored_Height");
                        tracktemp.SampledWidth = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Sampled_Width");
                        tracktemp.SampledHeight = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Sampled_Height");
                        tracktemp.PixelAspectRatio = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "PixelAspectRatio"));
                        tracktemp.PixelAspectRatioString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "PixelAspectRatio/String");
                        tracktemp.PixelAspectRatioOriginal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "PixelAspectRatio_Original"));
                        tracktemp.PixelAspectRatioCleanAspectRatio = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "PixelAspectRatio_CleanAspectRatio"));
                        tracktemp.DisplayAspectRatio = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "DisplayAspectRatio"));
                        tracktemp.DisplayAspectRatioString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "DisplayAspectRatio/String");
                        tracktemp.DisplayAspectRatioOriginal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "DisplayAspectRatio_Original"));
                        tracktemp.ActiveFormatDescription = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ActiveFormatDescription");
                        tracktemp.ActiveFormatDescriptionString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ActiveFormatDescription/String");
                        tracktemp.Rotation = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Rotation");
                        tracktemp.RotationString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Rotation/String");
                        tracktemp.FrameRateMode = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Mode"); 
                        tracktemp.FrameRateModeString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Mode/String");
                        tracktemp.FrameRateModeOriginal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Mode_Original"));
                        tracktemp.FrameRateModeOriginalString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Mode_Original/String");
                        tracktemp.FrameRate = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate"));
                        tracktemp.FrameRateString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate/String");
                        tracktemp.FrameRateNum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Num"));
                        tracktemp.FrameRateDen = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Den"));
                        tracktemp.FrameRateMinimum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Minimum"));
                        tracktemp.FrameRateMinimumString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Minimum/String");
                        tracktemp.FrameRateNominal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Nominal"));
                        tracktemp.FrameRateNominalString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Nominal/String");
                        tracktemp.FrameRateMaximum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Maximum"));
                        tracktemp.FrameRateMaximumString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Maximum/String");
                        tracktemp.FrameRateOriginal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Original"));
                        tracktemp.FrameRateOriginalString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Original/String");
                        tracktemp.FrameRateOriginalNum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Original_Num"));
                        tracktemp.FrameRateOriginalDen = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameRate_Original_Den"));
                        tracktemp.FrameCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "FrameCount"));
                        tracktemp.SourceFrameCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_FrameCount"));
                        tracktemp.Standard = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Standard");
                        tracktemp.ColorSpace = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ColorSpace");
                        tracktemp.ChromaSubsampling = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ChromaSubsampling");
                        tracktemp.ChromaSubsamplingString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ChromaSubsampling/String");
                        tracktemp.ChromaSubsamplingPosition = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ChromaSubsampling_Position");
                        tracktemp.BitDepth = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitDepth");
                        tracktemp.BitDepthString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitDepth/String");
                        tracktemp.ScanType = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanType");
                        tracktemp.ScanTypeString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanType/String");
                        tracktemp.ScanTypeOriginal = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanType_Original");
                        tracktemp.ScanTypeOriginalString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanType_Original/String");
                        tracktemp.ScanTypeStoreMethod = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanType_StoreMethod");
                        tracktemp.ScanTypeStoreMethodFieldsPerBlock = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanType_StoreMethod_FieldsPerBlock");
                        tracktemp.ScanTypeStoreMethodString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanType_StoreMethod/String");
                        tracktemp.ScanOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanOrder"));
                        tracktemp.ScanOrderString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanOrder/String");
                        tracktemp.ScanOrderStored = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanOrder_Stored"));
                        tracktemp.ScanOrderStoredString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanOrder_Stored/String");
                        tracktemp.ScanOrderStoredDisplayedInverted = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanOrder_StoredDisplayedInverted"));
                        tracktemp.ScanOrderOriginal = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanOrder_Original"));
                        tracktemp.ScanOrderOriginalString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ScanOrder_Original/String");
                        tracktemp.CompressionMode = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Compression_Mode");
                        tracktemp.CompressionModeString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Compression_Mode/String");
                        tracktemp.CompressionRatio = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Compression_Ratio");
                        tracktemp.BitsPixelFrame = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Bits-(Pixel*Frame)"));
                        tracktemp.Delay = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay"));
                        tracktemp.DelayString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay/String");
                        tracktemp.DelayString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay/String1");
                        tracktemp.DelayString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay/String2");
                        tracktemp.DelayFormatted = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay/String3");
                        tracktemp.DelayString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay/String4");
                        tracktemp.DelayString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay/String5");
                        tracktemp.DelaySettings = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Settings");
                        tracktemp.DelayDropFrame = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_DropFrame");
                        tracktemp.DelaySource = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Source");
                        tracktemp.DelaySourceString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Source/String");
                        tracktemp.DelayOriginal = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original"));
                        tracktemp.DelayOriginalString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original/String");
                        tracktemp.DelayOriginalString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original/String1");
                        tracktemp.DelayOriginalString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original/String2");
                        tracktemp.DelayOriginalFormatted = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original/String3");
                        tracktemp.DelayOriginalString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original/String4");
                        tracktemp.DelayOriginalString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original/String5");
                        tracktemp.DelayOriginalSettings = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original_Settings");
                        tracktemp.DelayOriginalDropFrame = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original_DropFrame");
                        tracktemp.DelayOriginalSource = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Delay_Original_Source");
                        tracktemp.TimeStampFirstFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeStamp_FirstFrame"));
                        tracktemp.TimeStampFirstFrameString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeStamp_FirstFrame/String");
                        tracktemp.TimeStampFirstFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeStamp_FirstFrame/String1");
                        tracktemp.TimeStampFirstFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeStamp_FirstFrame/String2");
                        tracktemp.TimeStampFirstFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeStamp_FirstFrame/String3");
                        tracktemp.TimeStampFirstFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeStamp_FirstFrame/String4");
                        tracktemp.TimeStampFirstFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeStamp_FirstFrame/String5");
                        tracktemp.TimeCodeFirstFrame = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeCode_FirstFrame");
                        tracktemp.TimeCodeSettings = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeCode_Settings");
                        tracktemp.TimeCodeSource = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "TimeCode_Source"));
                        tracktemp.GopOpenClosed = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Gop_OpenClosed");
                        tracktemp.GopOpenClosedString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Gop_OpenClosed/String");
                        tracktemp.GopOpenClosedFirstFrame = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Gop_OpenClosed_FirstFrame");
                        tracktemp.StreamSize = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize"));
                        tracktemp.StreamSizeString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize/String");
                        tracktemp.StreamSizeString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize/String1");
                        tracktemp.StreamSizeString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize/String2");
                        tracktemp.StreamSizeString3 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize/String3");
                        tracktemp.StreamSizeString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize/String4");
                        tracktemp.StreamSizeProportion = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Proportion"));
                        tracktemp.StreamSizeDemuxed = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Demuxed"));
                        tracktemp.StreamSizeDemuxedString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Demuxed/String");
                        tracktemp.StreamSizeDemuxedString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Demuxed/String1");
                        tracktemp.StreamSizeDemuxedString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Demuxed/String2");
                        tracktemp.StreamSizeDemuxedString3 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Demuxed/String3");
                        tracktemp.StreamSizeDemuxedString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Demuxed/String4");
                        tracktemp.SourceStreamSize = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize"));
                        tracktemp.SourceStreamSizeString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize/String");
                        tracktemp.SourceStreamSizeString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize/String1");
                        tracktemp.SourceStreamSizeString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize/String2");
                        tracktemp.SourceStreamSizeString3 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize/String3");
                        tracktemp.SourceStreamSizeString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize/String4");
                        tracktemp.SourceStreamSizeProport = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize_Proport"));
                        tracktemp.StreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Encoded"));
                        tracktemp.StreamSizeEncodedString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Encoded/String");
                        tracktemp.StreamSizeEncodedString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Encoded/String1");
                        tracktemp.StreamSizeEncodedString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Encoded/String2");
                        tracktemp.StreamSizeEncodedString3 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Encoded/String3");
                        tracktemp.StreamSizeEncodedString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Encoded/String4");
                        tracktemp.StreamSizeEncodedString5 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Encoded/String5");
                        tracktemp.StreamSizeEncodedPropor = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "StreamSize_Encoded_Propor"));
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize_Encoded"));
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize_Encoded"));
                        tracktemp.SourceStreamSizeEncodedString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize_Encoded/String1");
                        tracktemp.SourceStreamSizeEncodedString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize_Encoded/String2");
                        tracktemp.SourceStreamSizeEncodedString3 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize_Encoded/String3");
                        tracktemp.SourceStreamSizeEncodedString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize_Encoded/String4");
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize_Encoded"));
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Source_StreamSize_Encoded"));
                        tracktemp.Alignment = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Alignment");
                        tracktemp.AlignmentString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Alignment/String");
                        tracktemp.Title = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Title");
                        tracktemp.EncodedApplication = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Application");
                        tracktemp.EncodedApplicationString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Application/String");
                        tracktemp.EncodedApplicationCompany = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Application_Company");
                        tracktemp.EncodedApplicationName = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Application_Name");
                        tracktemp.EncodedApplicationVersion = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Application_Version");
                        tracktemp.EncodedApplicationUrl = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Application_Url");
                        tracktemp.EncodedLibrary = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Library");
                        tracktemp.EncodedLibraryString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Library/String");
                        tracktemp.EncodedLibraryCompanyName = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Library_CompanyName");
                        tracktemp.EncodedLibraryName = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Library_Name");
                        tracktemp.EncodedLibraryVersion = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Library_Version");
                        tracktemp.EncodedLibraryDate = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Library_Date");
                        tracktemp.EncodedLibrarySettings = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Library_Settings");
                        tracktemp.EncodedOperatingSystem = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_OperatingSystem");
                        tracktemp.Language = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Language"); 
                        tracktemp.LanguageString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Language/String");
                        tracktemp.LanguageString1 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Language/String1");
                        tracktemp.LanguageString2 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Language/String2");
                        tracktemp.LanguageString3 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Language/String3");
                        tracktemp.LanguageString4 = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Language/String4");
                        tracktemp.LanguageMore = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Language_More");
                        tracktemp.ServiceKind = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ServiceKind");
                        tracktemp.ServiceKindString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "ServiceKind/String");
                        tracktemp.Disabled = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Disabled");
                        tracktemp.DisabledString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Disabled/String");
                        tracktemp.Default = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Default"));
                        tracktemp.DefaultString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Default/String");
                        tracktemp.Forced = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Forced"));
                        tracktemp.ForcedString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Forced/String");
                        tracktemp.AlternateGroup = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "AlternateGroup"));
                        tracktemp.AlternateGroupString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "AlternateGroup/String");
                        tracktemp.EncodedDate = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encoded_Date");
                        tracktemp.TaggedDate = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Tagged_Date");
                        tracktemp.Encryption = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Encryption");
                        tracktemp.BufferSize = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BufferSize");
                        tracktemp.Colourrange = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "colour_range");
                        tracktemp.Colourprimaries = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "colour_primaries");
                        tracktemp.Transfercharacteristics = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "transfer_characteristics");
                        tracktemp.Matrixcoefficients = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "matrix_coefficients");

                        // Other
                        tracktemp.Codec = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Codec");
                        tracktemp.CodecString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Codec/String");
                        tracktemp.CodecInfo = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Codec/Info");
                        tracktemp.CodecUrl = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Codec/Url");
                        tracktemp.NominalBitRate = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Minimum"));
                        tracktemp.MinimumBitRateString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Minimum/String");
                        tracktemp.MinimumBitRate = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Nominal"));
                        tracktemp.NominalBitRateString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Nominal/String");
                        tracktemp.MaximumBitRate = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Maximum"));
                        tracktemp.MaximumBitRateString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "BitRate_Maximum/String");
                        tracktemp.AspectRatio = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Video, curr, "AspectRatio"));
                        tracktemp.AspectRatioString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "AspectRatio/String");
                        tracktemp.FormatSettingsMatrixData = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_Matrix_Data");
                        tracktemp.FormatSettingsRefFramesString = GetSpecificInfo(MediaInfoStreamKind.Video, curr, "Format_Settings_RefFrames/String");



                        if (tracktemp.DurationMillisec == 0)
                        {
                            tracktemp.DurationMillisec = CalculateTotalMillis(tracktemp);
                        }

                        this._Video.Add(tracktemp);
                    }
                }
            }
        }

        /// <summary>
        /// List of all the Video streams available in the file, type VideoTrack[trackindex] to access a specific track
        /// </summary>
        /// <value>
        /// The list of all video streams.
        /// </value>
        public List<ImageTrack> Image
        {
            get
            {
                if (this._Video == null)
                {
                    GetImageInfo();
                }
                return this._Image;
            }
        }

        /// <summary>
        /// Gets the informations of all video streams
        /// </summary>
        internal void GetImageInfo()
        {
            if (this._Image == null)
            {
                this._Image = new List<ImageTrack>();
                int count = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Image, -1);
                if (count > 0)
                {
                    int max = count - 1;
                    for (int curr = 0; curr <= max; curr++)
                    {
                        ImageTrack tracktemp = new ImageTrack();
                        tracktemp.Count = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Count"));
                        tracktemp.Status = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Status");
                        tracktemp.StreamCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamCount"));
                        tracktemp.StreamKind = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamKind");
                        tracktemp.StreamKindString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamKind/String");
                        tracktemp.StreamKindID = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamKindID"));
                        tracktemp.StreamKindPos = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamKindPos"));
                        tracktemp.StreamOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamOrder"));
                        tracktemp.FirstPacketOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "FirstPacketOrder"));
                        tracktemp.Inform = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Inform");
                        tracktemp.ID = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "ID");
                        tracktemp.IDString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "ID/String");
                        tracktemp.OriginalSourceMediumID = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "OriginalSourceMedium_ID");
                        tracktemp.OriginalSourceMediumIDString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "OriginalSourceMedium_ID/String");
                        tracktemp.UniqueID = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "UniqueID");
                        tracktemp.UniqueIDString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "UniqueID/String");
                        tracktemp.MenuID = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "MenuID");
                        tracktemp.MenuIDString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "MenuID/String");
                        tracktemp.Title = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Title");
                        tracktemp.Format = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format");
                        tracktemp.FormatInfo = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format/Info");
                        tracktemp.FormatUrl = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format/Url");
                        tracktemp.FormatCommercial = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format_Commercial");
                        tracktemp.FormatCommercialIfAny = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format_Commercial_IfAny");
                        tracktemp.FormatVersion = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format_Version");
                        tracktemp.FormatProfile = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format_Profile");
                        tracktemp.FormatSettingsEndianness = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format_Settings_Endianness");
                        tracktemp.FormatSettingsPacking = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format_Settings_Packing");
                        tracktemp.FormatCompression = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format_Compression");
                        tracktemp.FormatSettings = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format_Settings");
                        tracktemp.FormatSettingsWrapping = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Format_Settings_Wrapping");
                        tracktemp.InternetMediaType = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "InternetMediaType");
                        tracktemp.CodecID = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "CodecID");
                        tracktemp.CodecIDString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "CodecID/String");
                        tracktemp.CodecIDInfo = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "CodecID/Info");
                        tracktemp.CodecIDHint = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "CodecID/Hint");
                        tracktemp.CodecIDUrl = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "CodecID/Url");
                        tracktemp.CodecIDDescription = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "CodecID_Description");
                        tracktemp.Width = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Width");
                        tracktemp.WidthString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Width/String");
                        tracktemp.WidthOffset = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Width_Offset");
                        tracktemp.WidthOffsetString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Width_Offset/String");
                        tracktemp.WidthOriginal = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Width_Original");
                        tracktemp.WidthOriginalString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Width_Original/String");
                        tracktemp.Height = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Height");
                        tracktemp.HeightString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Height/String");
                        tracktemp.HeightOffset = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Height_Offset");
                        tracktemp.HeightOffsetString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Height_Offset/String");
                        tracktemp.HeightOriginal = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Height_Original");
                        tracktemp.HeightOriginalString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Height_Original/String");
                        tracktemp.PixelAspectRatio = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "PixelAspectRatio"));
                        tracktemp.PixelAspectRatioString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "PixelAspectRatio/String");
                        tracktemp.PixelAspectRatioOriginal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "PixelAspectRatio_Original"));
                        tracktemp.DisplayAspectRatio = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "DisplayAspectRatio"));
                        tracktemp.DisplayAspectRatioString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "DisplayAspectRatio/String");
                        tracktemp.DisplayAspectRatioOriginal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "DisplayAspectRatio_Original"));
                        tracktemp.ColorSpace = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "ColorSpace");
                        tracktemp.ChromaSubsampling = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "ChromaSubsampling");
                        tracktemp.BitDepth = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "BitDepth");
                        tracktemp.BitDepthString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "BitDepth/String");
                        tracktemp.CompressionMode = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Compression_Mode");
                        tracktemp.CompressionModeString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Compression_Mode/String");
                        tracktemp.CompressionRatio = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Compression_Ratio");
                        tracktemp.StreamSize = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize"));
                        tracktemp.StreamSizeString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize/String");
                        tracktemp.StreamSizeString1 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize/String1");
                        tracktemp.StreamSizeString2 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize/String2");
                        tracktemp.StreamSizeString3 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize/String3");
                        tracktemp.StreamSizeString4 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize/String4");
                        tracktemp.StreamSizeProportion = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize_Proportion"));
                        tracktemp.StreamSizeDemuxed = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize_Demuxed"));
                        tracktemp.StreamSizeDemuxedString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize_Demuxed/String");
                        tracktemp.StreamSizeDemuxedString1 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize_Demuxed/String1");
                        tracktemp.StreamSizeDemuxedString2 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize_Demuxed/String2");
                        tracktemp.StreamSizeDemuxedString3 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize_Demuxed/String3");
                        tracktemp.StreamSizeDemuxedString4 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "StreamSize_Demuxed/String4");
                        tracktemp.EncodedLibrary = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Encoded_Library");
                        tracktemp.EncodedLibraryString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Encoded_Library/String");
                        tracktemp.EncodedLibraryName = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Encoded_Library_Name");
                        tracktemp.EncodedLibraryVersion = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Encoded_Library_Version");
                        tracktemp.EncodedLibraryDate = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Encoded_Library_Date");
                        tracktemp.EncodedLibrarySettings = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Encoded_Library_Settings");
                        tracktemp.Language = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Language"));
                        tracktemp.LanguageString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Language/String");
                        tracktemp.LanguageString1 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Language/String1");
                        tracktemp.LanguageString2 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Language/String2");
                        tracktemp.LanguageString3 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Language/String3");
                        tracktemp.LanguageString4 = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Language/String4");
                        tracktemp.LanguageMore = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Language_More");
                        tracktemp.ServiceKind = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "ServiceKind");
                        tracktemp.ServiceKindString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "ServiceKind/String");
                        tracktemp.Disabled = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Disabled");
                        tracktemp.DisabledString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Disabled/String");
                        tracktemp.Default = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Default"));
                        tracktemp.DefaultString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Default/String");
                        tracktemp.Forced = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Forced"));
                        tracktemp.ForcedString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Forced/String");
                        tracktemp.AlternateGroup = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Image, curr, "AlternateGroup"));
                        tracktemp.AlternateGroupString = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "AlternateGroup/String");
                        tracktemp.Summary = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Summary");
                        tracktemp.EncodedDate = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Encoded_Date");
                        tracktemp.TaggedDate = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Tagged_Date");
                        tracktemp.Encryption = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "Encryption");
                        tracktemp.Colourprimaries = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "colour_primaries");
                        tracktemp.Transfercharacteristics = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "transfer_characteristics");
                        tracktemp.Matrixcoefficients = GetSpecificInfo(MediaInfoStreamKind.Image, curr, "matrix_coefficients");

                        this._Image.Add(tracktemp);
                    }
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
        internal void GetAudioInfo()
        {
            if (this._Audio == null)
            {
                this._Audio = new List<AudioTrack>();
                int count = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Audio, -1);
                if (count > 0)
                {
                    int max = count - 1;
                    for (int curr = 0; curr <= max; curr++)
                    {
                        AudioTrack tracktemp = new AudioTrack();
                        tracktemp.Count = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Count"));
                        tracktemp.Status = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Status");
                        tracktemp.StreamCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamCount"));
                        tracktemp.StreamKind = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamKind");
                        tracktemp.StreamKindString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamKind/String");
                        tracktemp.StreamKindID = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamKindID"));
                        tracktemp.StreamKindPos = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamKindPos"));
                        tracktemp.StreamOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamOrder"));
                        tracktemp.FirstPacketOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "FirstPacketOrder"));
                        tracktemp.Inform = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Inform");
                        tracktemp.ID = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ID");
                        tracktemp.IDString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ID/String");
                        tracktemp.OriginalSourceMediumID = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "OriginalSourceMedium_ID");
                        tracktemp.OriginalSourceMediumIDString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "OriginalSourceMedium_ID/String");
                        tracktemp.UniqueID = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "UniqueID");
                        tracktemp.UniqueIDString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "UniqueID/String");
                        tracktemp.MenuID = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "MenuID");
                        tracktemp.MenuIDString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "MenuID/String");
                        tracktemp.Format = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format");
                        tracktemp.FormatInfo = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format/Info");
                        tracktemp.FormatUrl = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format/Url");
                        tracktemp.FormatCommercial = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Commercial");
                        tracktemp.FormatCommercialIfAny = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Commercial_IfAny");
                        tracktemp.FormatVersion = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Version");
                        tracktemp.FormatProfile = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Profile");
                        tracktemp.FormatLevel = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Level");
                        tracktemp.FormatCompression = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Compression");
                        tracktemp.FormatSettings = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings");
                        tracktemp.FormatSettingsSBR = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_SBR");
                        tracktemp.FormatSettingsSBRString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_SBR/String");
                        tracktemp.FormatSettingsPS = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_PS");
                        tracktemp.FormatSettingsPSString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_PS/String");
                        tracktemp.FormatSettingsMode = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_Mode");
                        tracktemp.FormatSettingsModeExtension = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_ModeExtension");
                        tracktemp.FormatSettingsEmphasis = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_Emphasis");
                        tracktemp.FormatSettingsFloor = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_Floor");
                        tracktemp.FormatSettingsFirm = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_Firm");
                        tracktemp.FormatSettingsEndianness = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_Endianness");
                        tracktemp.FormatSettingsSign = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_Sign");
                        tracktemp.FormatSettingsLaw = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_Law");
                        tracktemp.FormatSettingsITU = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_ITU");
                        tracktemp.FormatSettingsWrapping = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Format_Settings_Wrapping");
                        tracktemp.MatrixFormat = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Matrix_Format");
                        tracktemp.InternetMediaType = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "InternetMediaType");
                        tracktemp.MuxingMode = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "MuxingMode");
                        tracktemp.MuxingModeMoreInfo = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "MuxingMode_MoreInfo");
                        tracktemp.CodecID = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "CodecID");
                        tracktemp.CodecIDString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "CodecID/String");
                        tracktemp.CodecIDInfo = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "CodecID/Info");
                        tracktemp.CodecIDHint = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "CodecID/Hint");
                        tracktemp.CodecIDUrl = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "CodecID/Url");
                        tracktemp.CodecIDDescription = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "CodecID_Description");
                        tracktemp.Duration = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration"));
                        tracktemp.DurationString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration/String");
                        tracktemp.DurationString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration/String1");
                        tracktemp.DurationString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration/String2");
                        tracktemp.DurationFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration/String3");
                        tracktemp.DurationString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration/String4");
                        tracktemp.DurationString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration/String5");
                        tracktemp.DurationFirstFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_FirstFrame"));
                        tracktemp.DurationFirstFrameString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_FirstFrame/String");
                        tracktemp.DurationFirstFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_FirstFrame/String1");
                        tracktemp.DurationFirstFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_FirstFrame/String2");
                        tracktemp.DurationFirstFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_FirstFrame/String3");
                        tracktemp.DurationFirstFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_FirstFrame/String4");
                        tracktemp.DurationFirstFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_FirstFrame/String5");
                        tracktemp.DurationLastFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_LastFrame"));
                        tracktemp.DurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_LastFrame/String");
                        tracktemp.DurationLastFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_LastFrame/String1");
                        tracktemp.DurationLastFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_LastFrame/String2");
                        tracktemp.DurationLastFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_LastFrame/String3");
                        tracktemp.DurationLastFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_LastFrame/String4");
                        tracktemp.DurationLastFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Duration_LastFrame/String5");
                        tracktemp.SourceDuration = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration"));
                        tracktemp.SourceDurationString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration/String");
                        tracktemp.SourceDurationString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration/String1");
                        tracktemp.SourceDurationString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration/String2");
                        tracktemp.SourceDurationFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration/String3");
                        tracktemp.SourceDurationString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration/String4");
                        tracktemp.SourceDurationString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration/String5");
                        tracktemp.SourceDurationFirstFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_FirstFrame"));
                        tracktemp.SourceDurationFirstFrameString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_FirstFrame/String");
                        tracktemp.SourceDurationFirstFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_FirstFrame/String1");
                        tracktemp.SourceDurationFirstFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_FirstFrame/String2");
                        tracktemp.SourceDurationFirstFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_FirstFrame/String3");
                        tracktemp.SourceDurationFirstFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_FirstFrame/String4");
                        tracktemp.SourceDurationFirstFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_FirstFrame/String5");
                        tracktemp.SourceDurationLastFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_LastFrame"));
                        tracktemp.SourceDurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_LastFrame/String");
                        tracktemp.SourceDurationLastFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_LastFrame/String1");
                        tracktemp.SourceDurationLastFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_LastFrame/String2");
                        tracktemp.SourceDurationLastFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_LastFrame/String3");
                        tracktemp.SourceDurationLastFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_LastFrame/String4");
                        tracktemp.SourceDurationLastFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_Duration_LastFrame/String5");
                        tracktemp.BitRateMode = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Mode");
                        tracktemp.BitRateModeString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Mode/String");
                        tracktemp.BitRate = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate"));
                        tracktemp.BitRateString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate/String");
                        tracktemp.BitRateMinimum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Minimum"));
                        tracktemp.BitRateMinimumString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Minimum/String");
                        tracktemp.BitRateNominal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Nominal"));
                        tracktemp.BitRateNominalString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Nominal/String");
                        tracktemp.BitRateMaximum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Maximum"));
                        tracktemp.BitRateMaximumString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Maximum/String");
                        tracktemp.BitRateEncoded = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Encoded"));
                        tracktemp.BitRateEncodedString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitRate_Encoded/String");
                        tracktemp.Channels = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Channel(s)"));
                        tracktemp.ChannelsString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Channel(s)/String");
                        tracktemp.ChannelsOriginal = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Channel(s)_Original"));
                        tracktemp.ChannelsOriginalString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Channel(s)_Original/String");
                        tracktemp.MatrixChannels = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Matrix_Channel(s)"));
                        tracktemp.MatrixChannelsString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Matrix_Channel(s)/String");
                        tracktemp.ChannelPositions = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ChannelPositions");
                        tracktemp.ChannelPositionsString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ChannelPositions/String2");
                        tracktemp.MatrixChannelPositions = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Matrix_ChannelPositions");
                        tracktemp.MatrixChannelPositionsString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Matrix_ChannelPositions/String");
                        tracktemp.ChannelLayout = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ChannelLayout");
                        tracktemp.ChannelLayoutID = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ChannelLayoutID");
                        tracktemp.SamplesPerFrame = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "SamplesPerFrame");
                        tracktemp.SamplingRate = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "SamplingRate");
                        tracktemp.SamplingRateString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "SamplingRate/String");
                        tracktemp.SamplingCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "SamplingCount"));
                        tracktemp.SourceSamplingCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_SamplingCount"));
                        tracktemp.FrameRate = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "FrameRate"));
                        tracktemp.FrameRateString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "FrameRate/String");
                        tracktemp.FrameRateNum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "FrameRate_Num"));
                        tracktemp.FrameRateDen = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "FrameRate_Den"));
                        tracktemp.FrameCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "FrameCount"));
                        tracktemp.SourceFrameCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_FrameCount"));
                        tracktemp.BitDepth = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitDepth");
                        tracktemp.BitDepthString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitDepth/String");
                        tracktemp.BitDepthDetected = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitDepth_Detected");
                        tracktemp.BitDepthDetectedString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitDepth_Detected/String");
                        tracktemp.BitDepthStored = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitDepth_Stored");
                        tracktemp.BitDepthStoredString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "BitDepth_Stored/String");
                        tracktemp.CompressionMode = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Compression_Mode");
                        tracktemp.CompressionModeString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Compression_Mode/String");
                        tracktemp.CompressionRatio = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Compression_Ratio");
                        tracktemp.Delay = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay"));
                        tracktemp.DelayString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay/String");
                        tracktemp.DelayString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay/String1");
                        tracktemp.DelayString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay/String2");
                        tracktemp.DelayFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay/String3");
                        tracktemp.DelayString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay/String4");
                        tracktemp.DelayString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay/String5");
                        tracktemp.DelaySettings = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Settings");
                        tracktemp.DelayDropFrame = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_DropFrame");
                        tracktemp.DelaySource = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Source");
                        tracktemp.DelaySourceString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Source/String");
                        tracktemp.DelayOriginal = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original"));
                        tracktemp.DelayOriginalString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original/String");
                        tracktemp.DelayOriginalString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original/String1");
                        tracktemp.DelayOriginalString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original/String2");
                        tracktemp.DelayOriginalFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original/String3");
                        tracktemp.DelayOriginalString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original/String4");
                        tracktemp.DelayOriginalString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original/String5");
                        tracktemp.DelayOriginalSettings = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original_Settings");
                        tracktemp.DelayOriginalDropFrame = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original_DropFrame");
                        tracktemp.DelayOriginalSource = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Delay_Original_Source");
                        tracktemp.VideoDelay = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Video_Delay");
                        tracktemp.VideoDelayString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Video_Delay/String");
                        tracktemp.VideoDelayString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Video_Delay/String1");
                        tracktemp.VideoDelayString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Video_Delay/String2");
                        tracktemp.VideoDelayFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Video_Delay/String3");
                        tracktemp.VideoDelayString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Video_Delay/String4");
                        tracktemp.VideoDelayString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Video_Delay/String5");
                        tracktemp.ReplayGainGain = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ReplayGain_Gain");
                        tracktemp.ReplayGainGainString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ReplayGain_Gain/String");
                        tracktemp.ReplayGainPeak = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ReplayGain_Peak");
                        tracktemp.StreamSize = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize"));
                        tracktemp.StreamSizeString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize/String");
                        tracktemp.StreamSizeString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize/String1");
                        tracktemp.StreamSizeString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize/String2");
                        tracktemp.StreamSizeString3 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize/String3");
                        tracktemp.StreamSizeString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize/String4");
                        tracktemp.StreamSizeProportion = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Proportion")); 
                        tracktemp.StreamSizeDemuxed = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Demuxed"));
                        tracktemp.StreamSizeDemuxedString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Demuxed/String");
                        tracktemp.StreamSizeDemuxedString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Demuxed/String1");
                        tracktemp.StreamSizeDemuxedString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Demuxed/String2");
                        tracktemp.StreamSizeDemuxedString3 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Demuxed/String3");
                        tracktemp.StreamSizeDemuxedString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Demuxed/String4");
                        tracktemp.SourceStreamSize = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize"));
                        tracktemp.SourceStreamSizeString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize/String");
                        tracktemp.SourceStreamSizeString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize/String1");
                        tracktemp.SourceStreamSizeString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize/String2");
                        tracktemp.SourceStreamSizeString3 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize/String3");
                        tracktemp.SourceStreamSizeString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize/String4");
                        tracktemp.SourceStreamSizeProport = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize_Proport"));
                        tracktemp.StreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Encoded"));
                        tracktemp.StreamSizeEncodedString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Encoded/String1");
                        tracktemp.StreamSizeEncodedString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Encoded/String2");
                        tracktemp.StreamSizeEncodedString3 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Encoded/String3");
                        tracktemp.StreamSizeEncodedString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Encoded/String4");
                        tracktemp.StreamSizeEncodedString5 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "StreamSize_Encoded/String5");
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize_Encoded"));
                        tracktemp.SourceStreamSizeEncodedString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize_Encoded/String1");
                        tracktemp.SourceStreamSizeEncodedString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize_Encoded/String2");
                        tracktemp.SourceStreamSizeEncodedString3 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize_Encoded/String3");
                        tracktemp.SourceStreamSizeEncodedString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize_Encoded/String4");
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Source_StreamSize_Encoded"));
                        tracktemp.Alignment = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Alignment");
                        tracktemp.AlignmentString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Alignment/String");
                        tracktemp.InterleaveVideoFrames = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Interleave_VideoFrames");
                        tracktemp.InterleaveDuration = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Interleave_Duration"));
                        tracktemp.InterleaveDurationString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Interleave_Duration/String");
                        tracktemp.InterleavePreload = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Interleave_Preload");
                        tracktemp.InterleavePreloadString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Interleave_Preload/String");
                        tracktemp.Title = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Title");
                        tracktemp.EncodedApplication = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Application");
                        tracktemp.EncodedApplicationString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Application/String");
                        tracktemp.EncodedApplicationCompany = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Application_Company");
                        tracktemp.EncodedApplicationName = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Application_Name");
                        tracktemp.EncodedApplicationVersion = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Application_Version");
                        tracktemp.EncodedApplicationUrl = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Application_Url");
                        tracktemp.EncodedLibrary = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Library");
                        tracktemp.EncodedLibraryString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Library/String");
                        tracktemp.EncodedLibraryCompanyName = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Library_CompanyName");
                        tracktemp.EncodedLibraryName = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Library_Name");
                        tracktemp.EncodedLibraryVersion = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Library_Version");
                        tracktemp.EncodedLibraryDate = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Library_Date");
                        tracktemp.EncodedLibrarySettings = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Library_Settings");
                        tracktemp.EncodedOperatingSystem = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_OperatingSystem");
                        tracktemp.Language = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Language"));
                        tracktemp.LanguageString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Language/String");
                        tracktemp.LanguageString1 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Language/String1");
                        tracktemp.LanguageString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Language/String2");
                        tracktemp.LanguageString3 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Language/String3");
                        tracktemp.LanguageString4 = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Language/String4");
                        tracktemp.LanguageMore = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Language_More");
                        tracktemp.ServiceKind = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ServiceKind");
                        tracktemp.ServiceKindString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ServiceKind/String");
                        tracktemp.Disabled = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Disabled");
                        tracktemp.DisabledString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Disabled/String");
                        tracktemp.Default = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Default"));
                        tracktemp.DefaultString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Default/String");
                        tracktemp.Forced = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Forced"));
                        tracktemp.ForcedString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Forced/String");
                        tracktemp.AlternateGroup = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "AlternateGroup"));
                        tracktemp.AlternateGroupString = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "AlternateGroup/String");
                        tracktemp.EncodedDate = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encoded_Date");
                        tracktemp.TaggedDate = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Tagged_Date");
                        tracktemp.Encryption = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "Encryption");

                        // Other
                        tracktemp.ChannelMode = GetSpecificInfo(MediaInfoStreamKind.Audio, curr, "ChannelMode");

                        if (tracktemp.Channels == 0 && !string.IsNullOrEmpty(tracktemp.ChannelsString))
                        {
                            tracktemp.Channels = ParseChannelsCount(tracktemp.ChannelsString);
                        }

                        this._Audio.Add(tracktemp);
                    }
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
        internal void GetTextInfo()
        {
            if (this._Text == null)
            {
                this._Text = new List<TextTrack>();
                int count = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Text, -1);
                if (count > 0)
                {
                    int max = count - 1;
                    for (int curr = 0; curr <= max; curr++)
                    {
                        TextTrack tracktemp = new TextTrack();
                        tracktemp.Text = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Text");
                        tracktemp.Count = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Count"));
                        tracktemp.Status = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Status");
                        tracktemp.StreamCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamCount"));
                        tracktemp.StreamKind = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamKind");
                        tracktemp.StreamKindString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamKind/String");
                        tracktemp.StreamKindID = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamKindID"));
                        tracktemp.StreamKindPos = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamKindPos"));
                        tracktemp.StreamOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamOrder"));
                        tracktemp.FirstPacketOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FirstPacketOrder"));
                        tracktemp.Inform = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Inform");
                        tracktemp.ID = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "ID");
                        tracktemp.IDString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "ID/String");
                        tracktemp.OriginalSourceMediumID = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "OriginalSourceMedium_ID");
                        tracktemp.OriginalSourceMediumIDString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "OriginalSourceMedium_ID/String");
                        tracktemp.UniqueID = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "UniqueID");
                        tracktemp.UniqueIDString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "UniqueID/String");
                        tracktemp.MenuID = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "MenuID");
                        tracktemp.MenuIDString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "MenuID/String");
                        tracktemp.Format = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format");
                        tracktemp.FormatInfo = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format/Info");
                        tracktemp.FormatUrl = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format/Url");
                        tracktemp.FormatCommercial = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format_Commercial");
                        tracktemp.FormatCommercialIfAny = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format_Commercial_IfAny");
                        tracktemp.FormatVersion = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format_Version");
                        tracktemp.FormatProfile = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format_Profile");
                        tracktemp.FormatCompression = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format_Compression");
                        tracktemp.FormatSettings = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format_Settings");
                        tracktemp.FormatSettingsWrapping = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Format_Settings_Wrapping");
                        tracktemp.InternetMediaType = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "InternetMediaType");
                        tracktemp.MuxingMode = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "MuxingMode");
                        tracktemp.MuxingModeMoreInfo = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "MuxingMode_MoreInfo");
                        tracktemp.CodecID = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "CodecID");
                        tracktemp.CodecIDString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "CodecID/String");
                        tracktemp.CodecIDInfo = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "CodecID/Info");
                        tracktemp.CodecIDHint = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "CodecID/Hint");
                        tracktemp.CodecIDUrl = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "CodecID/Url");
                        tracktemp.CodecIDDescription = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "CodecID_Description");
                        tracktemp.Duration = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration"));
                        tracktemp.DurationString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration/String");
                        tracktemp.DurationString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration/String1");
                        tracktemp.DurationString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration/String2");
                        tracktemp.DurationFormatted = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration/String3");
                        tracktemp.DurationString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration/String4");
                        tracktemp.DurationString5 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration/String5");
                        tracktemp.DurationFirstFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_FirstFrame"));
                        tracktemp.DurationFirstFrameString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_FirstFrame/String");
                        tracktemp.DurationFirstFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_FirstFrame/String1");
                        tracktemp.DurationFirstFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_FirstFrame/String2");
                        tracktemp.DurationFirstFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_FirstFrame/String3");
                        tracktemp.DurationFirstFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_FirstFrame/String4");
                        tracktemp.DurationFirstFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_FirstFrame/String5");
                        tracktemp.DurationLastFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_LastFrame"));
                        tracktemp.DurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_LastFrame/String");
                        tracktemp.DurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_LastFrame/String");
                        tracktemp.DurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_LastFrame/String");
                        tracktemp.DurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_LastFrame/String");
                        tracktemp.DurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_LastFrame/String");
                        tracktemp.DurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Duration_LastFrame/String");
                        tracktemp.SourceDuration = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration"));
                        tracktemp.SourceDurationString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration/String");
                        tracktemp.SourceDurationString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration/String1");
                        tracktemp.SourceDurationString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration/String2");
                        tracktemp.SourceDurationFormatted = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration/String3");
                        tracktemp.SourceDurationString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration/String4");
                        tracktemp.SourceDurationString5 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration/String5");
                        tracktemp.SourceDurationFirstFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_FirstFrame"));
                        tracktemp.SourceDurationFirstFrameString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_FirstFrame/String");
                        tracktemp.SourceDurationFirstFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_FirstFrame/String1");
                        tracktemp.SourceDurationFirstFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_FirstFrame/String2");
                        tracktemp.SourceDurationFirstFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_FirstFrame/String3");
                        tracktemp.SourceDurationFirstFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_FirstFrame/String4");
                        tracktemp.SourceDurationFirstFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_FirstFrame/String5");
                        tracktemp.SourceDurationLastFrame = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_LastFrame"));
                        tracktemp.SourceDurationLastFrameString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_LastFrame/String");
                        tracktemp.SourceDurationLastFrameString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_LastFrame/String1");
                        tracktemp.SourceDurationLastFrameString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_LastFrame/String2");
                        tracktemp.SourceDurationLastFrameFormatted = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_LastFrame/String3");
                        tracktemp.SourceDurationLastFrameString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_LastFrame/String4");
                        tracktemp.SourceDurationLastFrameString5 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_Duration_LastFrame/String5");
                        tracktemp.BitRateMode = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Mode");
                        tracktemp.BitRateModeString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Mode/String");
                        tracktemp.BitRate = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate"));
                        tracktemp.BitRateString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate/String");
                        tracktemp.BitRateMinimum = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Minimum"));
                        tracktemp.BitRateMinimumString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Minimum/String");
                        tracktemp.BitRateNominal = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Nominal"));
                        tracktemp.BitRateNominalString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Nominal/String");
                        tracktemp.BitRateMaximum = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Maximum"));
                        tracktemp.BitRateMaximumString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Maximum/String");
                        tracktemp.BitRateEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Encoded"));
                        tracktemp.BitRateEncodedString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitRate_Encoded/String"); 
                        tracktemp.Width = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Width");
                        tracktemp.WidthString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Width/String");
                        tracktemp.Height = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Height");
                        tracktemp.HeightString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Height/String");
                        tracktemp.FrameRateMode = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Mode"));
                        tracktemp.FrameRateModeString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Mode/String");
                        tracktemp.FrameRate = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate"));
                        tracktemp.FrameRateString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate/String");
                        tracktemp.FrameRateNum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Num"));
                        tracktemp.FrameRateDen = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Den"));
                        tracktemp.FrameRateMinimum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Minimum"));
                        tracktemp.FrameRateMinimumString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Minimum/String");
                        tracktemp.FrameRateNominal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Nominal"));
                        tracktemp.FrameRateNominalString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Nominal/String");
                        tracktemp.FrameRateMaximum = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Maximum"));
                        tracktemp.FrameRateMaximumString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Maximum/String");
                        tracktemp.FrameRateOriginal = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Original"));
                        tracktemp.FrameRateOriginalString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameRate_Original/String");
                        tracktemp.FrameCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "FrameCount"));
                        tracktemp.ElementCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "ElementCount"));
                        tracktemp.SourceFrameCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_FrameCount"));
                        tracktemp.ColorSpace = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "ColorSpace");
                        tracktemp.ChromaSubsampling = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "ChromaSubsampling");
                        tracktemp.BitDepth = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitDepth");
                        tracktemp.BitDepthString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "BitDepth/String");
                        tracktemp.CompressionMode = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Compression_Mode");
                        tracktemp.CompressionModeString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Compression_Mode/String");
                        tracktemp.CompressionRatio = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Compression_Ratio");
                        tracktemp.Delay = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay"));
                        tracktemp.DelayString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay/String");
                        tracktemp.DelayString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay/String1");
                        tracktemp.DelayString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay/String2");
                        tracktemp.DelayFormatted = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay/String3");
                        tracktemp.DelayString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay/String4");
                        tracktemp.DelayString5 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay/String5");
                        tracktemp.DelaySettings = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Settings");
                        tracktemp.DelayDropFrame = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_DropFrame");
                        tracktemp.DelaySource = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Source");
                        tracktemp.DelaySourceString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Source/String");
                        tracktemp.DelayOriginal = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original"));
                        tracktemp.DelayOriginalString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original/String");
                        tracktemp.DelayOriginalString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original/String1");
                        tracktemp.DelayOriginalString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original/String2");
                        tracktemp.DelayOriginalFormatted = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original/String3");
                        tracktemp.DelayOriginalString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original/String4");
                        tracktemp.DelayOriginalString5 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original/String5");
                        tracktemp.DelayOriginalSettings = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original_Settings");
                        tracktemp.DelayOriginalDropFrame = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original_DropFrame");
                        tracktemp.DelayOriginalSource = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Delay_Original_Source");
                        tracktemp.VideoDelay = GetDoubleOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Video_Delay"));
                        tracktemp.VideoDelayString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Video_Delay/String");
                        tracktemp.VideoDelayString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Video_Delay/String1");
                        tracktemp.VideoDelayString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Video_Delay/String2");
                        tracktemp.VideoDelayFormatted = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Video_Delay/String3");
                        tracktemp.VideoDelayString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Video_Delay/String4");
                        tracktemp.VideoDelayString5 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Video_Delay/String5");
                        tracktemp.StreamSize = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize"));
                        tracktemp.StreamSizeString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize/String");
                        tracktemp.StreamSizeString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize/String1");
                        tracktemp.StreamSizeString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize/String2");
                        tracktemp.StreamSizeString3 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize/String3");
                        tracktemp.StreamSizeString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize/String4");
                        tracktemp.StreamSizeProportion = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Proportion"));
                        tracktemp.StreamSizeDemuxed = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Demuxed"));
                        tracktemp.StreamSizeDemuxedString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Demuxed/String");
                        tracktemp.StreamSizeDemuxedString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Demuxed/String1");
                        tracktemp.StreamSizeDemuxedString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Demuxed/String2");
                        tracktemp.StreamSizeDemuxedString3 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Demuxed/String3");
                        tracktemp.StreamSizeDemuxedString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Demuxed/String4");
                        tracktemp.SourceStreamSize = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize"));
                        tracktemp.SourceStreamSizeString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize/String");
                        tracktemp.SourceStreamSizeString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize/String1");
                        tracktemp.SourceStreamSizeString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize/String2");
                        tracktemp.SourceStreamSizeString3 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize/String3");
                        tracktemp.SourceStreamSizeString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize/String4");
                        tracktemp.SourceStreamSizeProport = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize_Proport"));
                        tracktemp.StreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Encoded"));
                        tracktemp.StreamSizeEncodedString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Encoded/String");
                        tracktemp.StreamSizeEncodedString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Encoded/String1");
                        tracktemp.StreamSizeEncodedString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Encoded/String2");
                        tracktemp.StreamSizeEncodedString3 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Encoded/String3");
                        tracktemp.StreamSizeEncodedString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Encoded/String4");
                        tracktemp.StreamSizeEncodedString5 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Encoded/String5");
                        tracktemp.StreamSizeEncodedPropor = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "StreamSize_Encoded_Propor"));
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize_Encoded"));
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize_Encoded"));
                        tracktemp.SourceStreamSizeEncodedString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize_Encoded/String1");
                        tracktemp.SourceStreamSizeEncodedString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize_Encoded/String2");
                        tracktemp.SourceStreamSizeEncodedString3 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize_Encoded/String3");
                        tracktemp.SourceStreamSizeEncodedString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize_Encoded/String4");
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize_Encoded"));
                        tracktemp.SourceStreamSizeEncoded = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Source_StreamSize_Encoded"));
                        tracktemp.Title = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Title");
                        tracktemp.EncodedApplication = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Application");
                        tracktemp.EncodedApplicationString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Application/String");
                        tracktemp.EncodedApplicationCompa = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Application_Compa");
                        tracktemp.EncodedApplicationName = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Application_Name");
                        tracktemp.EncodedApplicationVersi = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Application_Versi");
                        tracktemp.EncodedApplicationUrl = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Application_Url");
                        tracktemp.EncodedLibrary = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Library");
                        tracktemp.EncodedLibraryString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Library/String");
                        tracktemp.EncodedLibraryCompanyNa = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Library_CompanyNa");
                        tracktemp.EncodedLibraryName = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Library_Name");
                        tracktemp.EncodedLibraryVersion = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Library_Version");
                        tracktemp.EncodedLibraryDate = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Library_Date");
                        tracktemp.EncodedLibrarySettings = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Library_Settings");
                        tracktemp.EncodedOperatingSystem = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_OperatingSystem");
                        tracktemp.Language = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Language"));
                        tracktemp.LanguageString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Language/String");
                        tracktemp.LanguageString1 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Language/String1");
                        tracktemp.LanguageString2 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Language/String2");
                        tracktemp.LanguageString3 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Language/String3");
                        tracktemp.LanguageString4 = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Language/String4");
                        tracktemp.LanguageMore = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Language_More");
                        tracktemp.ServiceKind = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "ServiceKind");
                        tracktemp.ServiceKindString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "ServiceKind/String");
                        tracktemp.Disabled = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Disabled");
                        tracktemp.DisabledString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Disabled/String");
                        tracktemp.Default = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Default"));
                        tracktemp.DefaultString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Default/String");
                        tracktemp.Forced = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Forced"));
                        tracktemp.ForcedString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Forced/String");
                        tracktemp.AlternateGroup = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "AlternateGroup"));
                        tracktemp.AlternateGroupString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "AlternateGroup/String");
                        tracktemp.Summary = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Summary");
                        tracktemp.EncodedDate = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encoded_Date");
                        tracktemp.TaggedDate = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Tagged_Date");
                        tracktemp.Encryption = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Encryption");

                        // Other
                        tracktemp.Codec = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Codec");
                        tracktemp.CodecString = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Codec/String");
                        tracktemp.CodecUrl = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Codec/Url");
                        tracktemp.Video0Delay = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "Video0_Delay"));
                        tracktemp.PlayTimeMillisec = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Text, curr, "PlayTime"));
                        tracktemp.PlayTimeStringFormatted = GetSpecificInfo(MediaInfoStreamKind.Text, curr, "PlayTime/String3");


                        this._Text.Add(tracktemp);
                    }
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
        internal void GetMenuInfo()
        {
            if (this._Menu == null)
            {
                this._Menu = new List<MenuTrack>();
                int count = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Menu, -1);

                if (count > 0)
                {
                    int max = count - 1;
                    for (int curr = 0; curr <= max; curr++)
                    {
                        MenuTrack tracktemp = new MenuTrack();
                        int endpos = GetIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Chapters_Pos_End"));
                        int startpos = GetIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Chapters_Pos_Begin"));
                        tracktemp.Count = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Count"));
                        tracktemp.Status = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Status");
                        tracktemp.StreamCount = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "StreamCount"));
                        tracktemp.StreamKind = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "StreamKind");
                        tracktemp.StreamKindString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "StreamKind/String");
                        tracktemp.StreamKindID = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "StreamKindID"));
                        tracktemp.StreamKindPos = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "StreamKindPos"));
                        tracktemp.StreamOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "StreamOrder"));
                        tracktemp.FirstPacketOrder = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "FirstPacketOrder"));
                        tracktemp.Inform = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Inform");
                        tracktemp.ID = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "ID");
                        tracktemp.IDString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "ID/String");
                        tracktemp.OriginalSourceMediumID = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "OriginalSourceMedium_ID");
                        tracktemp.OriginalSourceMediumIDString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "OriginalSourceMedium_ID/String");
                        tracktemp.UniqueID = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "UniqueID");
                        tracktemp.UniqueIDString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "UniqueID/String");
                        tracktemp.MenuID = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "MenuID");
                        tracktemp.MenuIDString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "MenuID/String");
                        tracktemp.Format = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Format");
                        tracktemp.FormatInfo = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Format/Info");
                        tracktemp.FormatUrl = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Format/Url");
                        tracktemp.FormatCommercial = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Format_Commercial");
                        tracktemp.FormatCommercialIfAny = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Format_Commercial_IfAny");
                        tracktemp.FormatVersion = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Format_Version");
                        tracktemp.FormatProfile = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Format_Profile");
                        tracktemp.FormatCompression = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Format_Compression");
                        tracktemp.FormatSettings = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Format_Settings");
                        tracktemp.CodecID = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "CodecID");
                        tracktemp.CodecIDString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "CodecID/String");
                        tracktemp.CodecIDInfo = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "CodecID/Info");
                        tracktemp.CodecIDHint = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "CodecID/Hint");
                        tracktemp.CodecIDUrl = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "CodecID/Url");
                        tracktemp.CodecIDDescription = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "CodecID_Description");
                        tracktemp.Duration = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Duration"));
                        tracktemp.DurationString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Duration/String");
                        tracktemp.DurationString1 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Duration/String1");
                        tracktemp.DurationString2 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Duration/String2");
                        tracktemp.DurationFormatted = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Duration/String3");
                        tracktemp.DurationString4 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Duration/String4");
                        tracktemp.DurationString5 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Duration/String5");
                        tracktemp.DurationStart = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Duration_Start");
                        tracktemp.DurationEnd = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Duration_End");
                        tracktemp.Delay = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay"));
                        tracktemp.DelayString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay/String");
                        tracktemp.DelayString1 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay/String1");
                        tracktemp.DelayString2 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay/String2");
                        tracktemp.DelayFormatted = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay/String3");
                        tracktemp.DelayString4 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay/String4");
                        tracktemp.DelayString5 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay/String5");
                        tracktemp.DelaySettings = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay_Settings");
                        tracktemp.DelayDropFrame = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay_DropFrame");
                        tracktemp.DelaySource = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Delay_Source");
                        tracktemp.ListStreamKind = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "List_StreamKind"));
                        tracktemp.ListStreamPos = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "List_StreamPos"));
                        tracktemp.List = GetUlongOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "List"));
                        tracktemp.ListString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "List/String");
                        tracktemp.Title = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Title");
                        tracktemp.Language = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Language"));
                        tracktemp.LanguageString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Language/String");
                        tracktemp.LanguageString1 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Language/String1");
                        tracktemp.LanguageString2 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Language/String2");
                        tracktemp.LanguageString3 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Language/String3");
                        tracktemp.LanguageString4 = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Language/String4");
                        tracktemp.LanguageMore = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Language_More");
                        tracktemp.ServiceKind = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "ServiceKind");
                        tracktemp.ServiceKindString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "ServiceKind/String");
                        tracktemp.ServiceName = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "ServiceName");
                        tracktemp.ServiceChannel = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "ServiceChannel");
                        tracktemp.ServiceUrl = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Service/Url");
                        tracktemp.ServiceProvider = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "ServiceProvider");
                        tracktemp.ServiceProviderUrl = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "ServiceProvider/Url");
                        tracktemp.ServiceType = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "ServiceType");
                        tracktemp.NetworkName = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "NetworkName");
                        tracktemp.OriginalNetworkName = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Original/NetworkName");
                        tracktemp.Countries = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Countries");
                        tracktemp.TimeZones = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "TimeZones");
                        tracktemp.LawRating = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "LawRating"));
                        tracktemp.LawRatingReason = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "LawRating_Reason");
                        tracktemp.Disabled = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Disabled");
                        tracktemp.DisabledString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Disabled/String");
                        tracktemp.Default = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Default"));
                        tracktemp.DefaultString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Default/String");
                        tracktemp.Forced = GetBoolOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Forced"));
                        tracktemp.ForcedString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Forced/String");
                        tracktemp.AlternateGroup = GetIntOrNull(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "AlternateGroup"));
                        tracktemp.AlternateGroupString = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "AlternateGroup/String");
                        tracktemp.ChaptersPosBegin = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Chapters_Pos_Begin");
                        tracktemp.ChaptersPosEnd = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Chapters_Pos_End");

                        // Other

                        tracktemp.ChaptersCount = endpos - startpos;
                        tracktemp.RawData = GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Inform");

                        //get chapters list
                        for (int index = startpos; index < endpos; index++)
                        {
                            ChapterTrack chapter = new ChapterTrack();
                            string tmpstr = GetSpecificSubInfo(MediaInfoStreamKind.Menu, curr, index, MediaInfoInfoKind.Text);
                            if (tmpstr.StartsWith(":"))
                            {
                                chapter.Title = tmpstr.Substring(1);
                                chapter.Language = "";
                            }
                            else
                            {
                                chapter.Language = tmpstr.Substring(0, 2);
                                chapter.Title = tmpstr.Substring(3, tmpstr.Length - 3);
                            }

                            chapter.Index = index - startpos;
                            chapter.Timecode = GetSpecificSubInfo(MediaInfoStreamKind.Menu, curr, index, MediaInfoInfoKind.Name);
                            tracktemp.Chapters.Add(chapter);
                        }

                        this._Menu.Add(tracktemp);
                    }
                }
            }
        }
        #endregion

        #region HelperMethods
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
        /// Return if video infos are available
        /// </summary>
        /// <returns>true, if at least one video stream has been found.</returns>
        public bool InfoAvailable()
        {
            return _VideoCount > 0 ? true : false;
        }

        /// <summary>
        /// Gets the formatted BitRate string.
        /// </summary>
        /// <param name="BitRate">The BitRate.</param>
        /// <returns>the formatted BitRate string.</returns>
        internal string GetFormattedBitRate(double? BitRate)
        {
            if (!BitRate.HasValue) return null;
            double doublebytes;

            if (BitRate >= 1099511627776)
            {
                doublebytes = (double)BitRate / (double)1099511627776;
                return string.Format("{0:0.##} Tbps", doublebytes);
            }
            else if (1073741824 < BitRate && BitRate < 1099511627775)
            {
                doublebytes = (double)BitRate / (double)1073741824;
                return string.Format("{0:0.##} Gbps", doublebytes);
            }
            else if (1048576 < BitRate && BitRate < 1073741823)
            {
                doublebytes = (double)BitRate / (double)1048576;
                return string.Format("{0:0.##} Mbps", doublebytes);
            }
            else if (1024 < BitRate && BitRate < 1048575)
            {
                doublebytes = (double)BitRate / (double)1024;
                return string.Format("{0:0.##} Kbps", doublebytes);
            }
            else if (0 < BitRate && BitRate < 1023)
            {
                doublebytes = (double)BitRate;
                return string.Format("{0:0.##} Bps", doublebytes);
            }

            return "";
        }

        /// <summary>
        /// Computes the overral BitRate.
        /// </summary>
        /// <param name="general">The filled GeneralTrack class.</param>
        /// <returns>The computed overral BitRate, 0 if not all necessary infos are available.</returns>
        internal ulong ComputeOverrallBitRate(GeneralTrack general)
        {
            if (general.Duration > 0 && general.FileSize > 0)
            {
                return (ulong)((general.FileSize * 8) / (general.Duration / 1000));
            }
            return 0;
        }

        /// <summary>
        /// Helper method to parse channels count from channels string, if fails return default value
        /// </summary>
        /// <param name="channelstring">The formatted channel string.</param>
        /// <returns>Numbers of channels parsed from the input string, 0 in case of failure.</returns>
        internal int ParseChannelsCount(string channelstring)
        {
            System.Text.RegularExpressions.Match mat = new System.Text.RegularExpressions.Regex("(\\d)\\schannels").Match(channelstring);

            if (mat.Success)
            {
                int channels = 0;
                if (int.TryParse(mat.Groups[1].Value, out channels))
                {
                    return channels;
                }
            }
            return 0;
        }

        /// <summary>
        /// Helper method to calculate total milliseconds
        /// </summary>
        /// <param name="tmpitem">The filled VideoTrack class.</param>
        /// <returns>The computed total milliseconds, 0 in case of failure.</returns>
        internal ulong CalculateTotalMillis(VideoTrack tmpitem)
        {
            if (string.IsNullOrEmpty(tmpitem.DurationString))
            {
                if (tmpitem.FrameCount > 0 && tmpitem.FrameRate > 0)
                {
                    return (ulong)((tmpitem.FrameCount / tmpitem.FrameRate) * 1000);
                }
            }

            if (!string.IsNullOrEmpty(tmpitem.DurationString))
            {
                TimeSpan tmp;
                TimeSpan.TryParse(tmpitem.DurationString, out tmp);

                return (ulong)tmp.TotalMilliseconds;
            }

            return 0;
        }

        /// <summary>
        /// Helper method to parse an integer from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed integer or null in case of failure.</returns>
        internal int? GetIntOrNull(string input)
        {
            return int.TryParse(input, out int value) ? (int?)value : null;
        }

        /// <summary>
        /// Helper method to parse an integer from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed integer or 0 in case of failure.</returns>
        internal int GetIntOrDefault(string input)
        {

            return int.TryParse(input, out int value) ? value : 0;
        }

        /// <summary>
        /// Helper method to parse an ULong from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed ULong or null in case of failure.</returns>
        internal ulong? GetUlongOrNull(string input)
        {

            return ulong.TryParse(input, out ulong value) ? (ulong?)value : null;
        }

        /// <summary>
        /// Helper method to parse an double from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed double or null in case of failure.</returns>
        internal double? GetDoubleOrNull(string input)
        {
            return double.TryParse(input,NumberStyles.Any, CultureInfo.InvariantCulture, out double value) ? (double?)value : null;
        }

        /// <summary>
        /// Helper method to parse an bool from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed bool or null in case of failure.</returns>
        internal bool? GetBoolOrNull(string input)
        {
            input = input.ToLower();
            string[] trues = { "yes", "true", "y", "t" };
            string[] falses = { "no", "false", "n", "f" };
            if (trues.Contains(input)) return true;
            if (falses.Contains(input)) return false;

            return null;
        }

        /// <summary>
        /// Helper method to parse an List from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed integer or 0 in case of failure.</returns>
        internal List<string> GetListOrNull(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            return input.Split(new[] { " / " }, StringSplitOptions.None).ToList();
        }

        internal List<T> GetListOrNull<T>(string input, Func<string, T> selector)
        {
            var list = GetListOrNull(input);
            return list?.Select(selector).ToList();
        }
        #endregion
    }

}