using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace GetMediaInfos
{
    /// <summary>
    /// Stream Kind Enumeration
    /// </summary>
    public enum MediaInfoStreamKind
    {
        General,
        Video,
        Audio,
        Text,
        Chapters,
        Image,
        Menu,
        Max
    }

    /// <summary>
    /// Info Kind Enumeration
    /// </summary>
    public enum MediaInfoInfoKind
    {
        Name,
        Text,
        Measure,
        Options,
        NameText,
        MeasureText,
        Info,
        HowTo
    }

    /// <summary>
    /// Info Options Enumeration
    /// </summary>
    public enum InfoOptions
    {
        ShowInInform,
        Support,
        ShowInSupported,
        TypeOfValue
    }
     
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
        internal static extern int MediaInfo_Count_Get(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] MediaInfoStreamKind StreamKind, int StreamNumber);
        [DllImport("MediaInfo.dll")]
        internal static extern void MediaInfo_Delete(IntPtr Handle);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_Get(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] MediaInfoStreamKind StreamKind, uint StreamNumber, string Parameter, [MarshalAs(UnmanagedType.U4)] MediaInfoInfoKind KindOfInfo, [MarshalAs(UnmanagedType.U4)] MediaInfoInfoKind KindOfSearch);
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MediaInfo_GetI(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] MediaInfoStreamKind StreamKind, uint StreamNumber, uint Parameter, [MarshalAs(UnmanagedType.U4)] MediaInfoInfoKind KindOfInfo);
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
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor :
        /// When called with a proper file target, returns a MediaInfo object filled with list of media tracks containing
        /// every information MediaInfo.dll can collect.
        /// Tracks are accessibles as properties.
        /// </summary>
        /// <param name="path">The path of the files to be analyzed.</param>
        public MediaInfo(string path)
        {
            if (!CheckFileExistence(path)) 
                return;
            _FileName = path;
            
            this.Handle = MediaInfo.MediaInfo_New();
            MediaInfo.MediaInfo_Open(this.Handle, path);
            try
            {
                getStreamCount();
                getAllInfos();
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
        private void getAllInfos()
        {
            getGeneralInfo();
            getVideoInfo();
            getAudioInfo();
            getTextInfo();
            getMenuInfo();
        }

        /// <summary>
        /// Gets all stream types count.
        /// </summary>
        private void getStreamCount()
        {
            _AudioCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Audio, -1);
            _VideoCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Video, -1);
            _GeneralCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.General, -1);
            _TextCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Text, -1);
            _MenuCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Menu, -1);
        }

        /// <summary>
        /// Gets the specific information from mediainfo.
        /// </summary>
        /// <param name="KindOfStream">The kind of stream.</param>
        /// <param name="trackindex">The track index.</param>
        /// <param name="NameOfParameter">The name of parameter.</param>
        /// <returns>A string containing the parameter's value or an empty string in case of problems.</returns>
        private unsafe string GetSpecificInfo(MediaInfoStreamKind KindOfStream, int trackindex, string NameOfParameter)
        {
            IntPtr p = MediaInfo.MediaInfo_Get(this.Handle, KindOfStream, Convert.ToUInt32(trackindex), NameOfParameter, MediaInfoInfoKind.Text, MediaInfoInfoKind.Name);
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
        private unsafe string GetSpecificSubInfo(MediaInfoStreamKind KindOfStream, int trackindex, int NameOfParameter, MediaInfoInfoKind InfoKind)
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
                    IntPtr tmp = MediaInfo.MediaInfo_Option(this.Handle, "Complete", "1");
                    string ctrl = Marshal.PtrToStringUni(tmp);
                    _InfoComplete = Marshal.PtrToStringUni(MediaInfo.MediaInfo_Inform(this.Handle, 0));
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
                    MediaInfo.MediaInfo_Option(this.Handle, "Complete", "");
                    _InfoStandard = Marshal.PtrToStringUni(MediaInfo.MediaInfo_Inform(this.Handle, 0));
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
        /// <param name="L">tracklist, for instance Video</param>
        /// <param name="sb">The StringBuilder where append all properties strings.</param>
        private void ListEveryAvailablePropery<T1>(List<T1> L, System.Text.StringBuilder sb)
        {   
            foreach (T1 track in L)
            {
                foreach (PropertyInfo p in track.GetType().GetProperties() )
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
                   getGeneralInfo();
                }
                return this._General;
            }
        }

        /// <summary>
        /// Gets the informations of all general streams.
        /// </summary>
        private void getGeneralInfo()
        {
            if (this._General == null)
            {
                this._General = new List<GeneralTrack>();
                int num1 = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.General, -1);
                if (num1 > 0)
                {
                    int num3 = num1 - 1;
                    for (int num2 = 0; num2 <= num3; num2++)
                    {
                        GeneralTrack _tracktemp_ = new GeneralTrack();           
                        _tracktemp_.Count= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.General,num2,"Count"));
                        _tracktemp_.StreamCount= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.General,num2,"StreamCount"));
                        _tracktemp_.StreamKind= GetSpecificInfo(MediaInfoStreamKind.General,num2,"StreamKind");
                        _tracktemp_.StreamKindID= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.General,num2,"StreamKindID"));
                        _tracktemp_.StreamOrder = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.General, num2, "StreamOrder"));
                        _tracktemp_.Inform= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Inform");
                        _tracktemp_.ID= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.General,num2,"ID"));
                        _tracktemp_.UniqueID= GetSpecificInfo(MediaInfoStreamKind.General,num2,"UniqueID");
                        _tracktemp_.GeneralCount = _GeneralCount;
                        _tracktemp_.VideoCount = _VideoCount;
                        _tracktemp_.AudioCount = _AudioCount;
                        _tracktemp_.TextCount = _TextCount;
                        _tracktemp_.MenuCount = _MenuCount;
                        _tracktemp_.ImageCount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Image, -1);
                        _tracktemp_.CompleteName= GetSpecificInfo(MediaInfoStreamKind.General,num2,"CompleteName");
                        _tracktemp_.FolderName= GetSpecificInfo(MediaInfoStreamKind.General,num2,"FolderName");
                        _tracktemp_.FileName= GetSpecificInfo(MediaInfoStreamKind.General,num2,"FileName");
                        _tracktemp_.FileExtension= GetSpecificInfo(MediaInfoStreamKind.General,num2,"FileExtension");
                        _tracktemp_.FileSize= getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.General,num2,"FileSize"));
                        _tracktemp_.FileSizeRounded = GetSpecificInfo(MediaInfoStreamKind.General,num2,"FileSize/String");
                        _tracktemp_.FileSizePrecise = GetSpecificInfo(MediaInfoStreamKind.General,num2,"FileSize/String4");
                        _tracktemp_.Format= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Format");
                        _tracktemp_.FormatString= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Format/String");
                        _tracktemp_.FormatInfo= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Format/Info");
                        _tracktemp_.FormatUrl= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Format/Url");
                        _tracktemp_.FormatExtensions= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Format/Extensions");
                        _tracktemp_.OveralBitRate= getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.General,num2,"OveralBitRate"));
                        _tracktemp_.OveralBitRateString= GetSpecificInfo(MediaInfoStreamKind.General,num2,"OveralBitRate/String");
                        _tracktemp_.PlayTimeMillisec= getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.General,num2,"PlayTime"));
                        _tracktemp_.PlayTimeFormatted= GetSpecificInfo(MediaInfoStreamKind.General,num2,"PlayTime/String3");
                        _tracktemp_.Title= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Title");
                        _tracktemp_.TitleMore= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Title/More");
                        _tracktemp_.Domain= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Domain");
                        _tracktemp_.Collection= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Collection");
                        _tracktemp_.CollectionTotalParts= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Collection/Total_Parts");
                        _tracktemp_.Season= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Season");
                        _tracktemp_.Movie= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Movie");
                        _tracktemp_.MovieMore= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Movie/More");
                        _tracktemp_.Album= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Album");
                        _tracktemp_.AlbumTotalParts= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Album/Total_Parts");
                        _tracktemp_.AlbumSort= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Album/Sort");
                        _tracktemp_.Comic= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Comic");
                        _tracktemp_.ComicTotalParts= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Comic/Total_Parts");
                        _tracktemp_.Part= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Part");
                        _tracktemp_.PartTotalParts= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Part/Total_Parts");
                        _tracktemp_.PartPosition= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Part/Position");
                        _tracktemp_.Track= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Track");
                        _tracktemp_.TrackPosition= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Track/Position");
                        _tracktemp_.TrackMore= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Track/More");
                        _tracktemp_.TrackSort= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Track/Sort");
                        _tracktemp_.Chapter= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Chapter");
                        _tracktemp_.SubTrack= GetSpecificInfo(MediaInfoStreamKind.General,num2,"SubTrack");
                        _tracktemp_.OriginalAlbum= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Original/Album");
                        _tracktemp_.OriginalMovie= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Original/Movie");
                        _tracktemp_.OriginalPart= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Original/Part");
                        _tracktemp_.OriginalTrack= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Original/Track");
                        _tracktemp_.Author= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Author");
                        _tracktemp_.Artist= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Artist");
                        _tracktemp_.PerformerSort= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Performer/Sort");
                        _tracktemp_.OriginalPerformer= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Original/Performer");
                        _tracktemp_.Accompaniment= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Accompaniment");
                        _tracktemp_.MusicianInstrument= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Musician_Instrument");
                        _tracktemp_.Composer= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Composer");
                        _tracktemp_.ComposerNationality= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Composer/Nationality");
                        _tracktemp_.Arranger= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Arranger");
                        _tracktemp_.Lyricist= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Lyricist");
                        _tracktemp_.OriginalLyricist= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Original/Lyricist");
                        _tracktemp_.Conductor= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Conductor");
                        _tracktemp_.Actor= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Actor");
                        _tracktemp_.ActorCharacter= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Actor_Character");
                        _tracktemp_.WrittenBy= GetSpecificInfo(MediaInfoStreamKind.General,num2,"WrittenBy");
                        _tracktemp_.ScreenplayBy= GetSpecificInfo(MediaInfoStreamKind.General,num2,"ScreenplayBy");
                        _tracktemp_.Director= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Director");
                        _tracktemp_.AssistantDirector= GetSpecificInfo(MediaInfoStreamKind.General,num2,"AssistantDirector");
                        _tracktemp_.DirectorOfPhotography= GetSpecificInfo(MediaInfoStreamKind.General,num2,"DirectorOfPhotography");
                        _tracktemp_.ArtDirector= GetSpecificInfo(MediaInfoStreamKind.General,num2,"ArtDirector");
                        _tracktemp_.EditedBy= GetSpecificInfo(MediaInfoStreamKind.General,num2,"EditedBy");
                        _tracktemp_.Producer= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Producer");
                        _tracktemp_.CoProducer= GetSpecificInfo(MediaInfoStreamKind.General,num2,"CoProducer");
                        _tracktemp_.ExecutiveProducer= GetSpecificInfo(MediaInfoStreamKind.General,num2,"ExecutiveProducer");
                        _tracktemp_.ProductionDesigner= GetSpecificInfo(MediaInfoStreamKind.General,num2,"ProductionDesigner");
                        _tracktemp_.CostumeDesigner= GetSpecificInfo(MediaInfoStreamKind.General,num2,"CostumeDesigner");
                        _tracktemp_.Choregrapher= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Choregrapher");
                        _tracktemp_.SoundEngineer= GetSpecificInfo(MediaInfoStreamKind.General,num2,"SoundEngineer");
                        _tracktemp_.MasteredBy= GetSpecificInfo(MediaInfoStreamKind.General,num2,"MasteredBy");
                        _tracktemp_.RemixedBy= GetSpecificInfo(MediaInfoStreamKind.General,num2,"RemixedBy");
                        _tracktemp_.ProductionStudio= GetSpecificInfo(MediaInfoStreamKind.General,num2,"ProductionStudio");
                        _tracktemp_.Publisher= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Publisher");
                        _tracktemp_.PublisherURL= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Publisher/URL");
                        _tracktemp_.DistributedBy= GetSpecificInfo(MediaInfoStreamKind.General,num2,"DistributedBy");
                        _tracktemp_.EncodedBy= GetSpecificInfo(MediaInfoStreamKind.General,num2,"EncodedBy");
                        _tracktemp_.ThanksTo= GetSpecificInfo(MediaInfoStreamKind.General,num2,"ThanksTo");
                        _tracktemp_.Technician= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Technician");
                        _tracktemp_.CommissionedBy= GetSpecificInfo(MediaInfoStreamKind.General,num2,"CommissionedBy");
                        _tracktemp_.EncodedOriginalDistributedBy= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Encoded_Original/DistributedBy");
                        _tracktemp_.RadioStation= GetSpecificInfo(MediaInfoStreamKind.General,num2,"RadioStation");
                        _tracktemp_.RadioStationOwner= GetSpecificInfo(MediaInfoStreamKind.General,num2,"RadioStation/Owner");
                        _tracktemp_.RadioStationURL= GetSpecificInfo(MediaInfoStreamKind.General,num2,"RadioStation/URL");
                        _tracktemp_.ContentType= GetSpecificInfo(MediaInfoStreamKind.General,num2,"ContentType");
                        _tracktemp_.Subject= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Subject");
                        _tracktemp_.Synopsys= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Synopsys");
                        _tracktemp_.Summary= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Summary");
                        _tracktemp_.Description= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Description");
                        _tracktemp_.Keywords= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Keywords");
                        _tracktemp_.Period= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Period");
                        _tracktemp_.LawRating= GetSpecificInfo(MediaInfoStreamKind.General,num2,"LawRating");
                        _tracktemp_.IRCA= GetSpecificInfo(MediaInfoStreamKind.General,num2,"IRCA");
                        _tracktemp_.Language= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Language");
                        _tracktemp_.Medium= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Medium");
                        _tracktemp_.Product= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Product");
                        _tracktemp_.Country= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Country");
                        _tracktemp_.WrittenDate= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Written_Date");
                        _tracktemp_.RecordedDate= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Recorded_Date");
                        _tracktemp_.ReleasedDate= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Released_Date");
                        _tracktemp_.MasteredDate= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Mastered_Date");
                        _tracktemp_.EncodedDate= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Encoded_Date");
                        _tracktemp_.TaggedDate= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Tagged_Date");
                        _tracktemp_.OriginalReleasedDate= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Original/Released_Date");
                        _tracktemp_.OriginalRecordedDate= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Original/Recorded_Date");
                        _tracktemp_.WrittenLocation= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Written_Location");
                        _tracktemp_.RecordedLocation= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Recorded_Location");
                        _tracktemp_.ArchivalLocation= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Archival_Location");
                        _tracktemp_.Genre= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Genre");
                        _tracktemp_.Mood= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Mood");
                        _tracktemp_.Comment= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Comment");
                        _tracktemp_.Rating= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Rating ");
                        _tracktemp_.EncodedApplication= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Encoded_Application");
                        _tracktemp_.EncodedLibrary= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Encoded_Library");
                        _tracktemp_.EncodedLibrarySettings= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Encoded_Library_Settings");
                        _tracktemp_.EncodedOriginal= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Encoded_Original");
                        _tracktemp_.EncodedOriginalUrl= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Encoded_Original/Url");
                        _tracktemp_.Copyright= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Copyright");
                        _tracktemp_.ProducerCopyright= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Producer_Copyright");
                        _tracktemp_.TermsOfUse= GetSpecificInfo(MediaInfoStreamKind.General,num2,"TermsOfUse");
                        _tracktemp_.CopyrightUrl= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Copyright/Url");
                        _tracktemp_.ISRC= GetSpecificInfo(MediaInfoStreamKind.General,num2,"ISRC");
                        _tracktemp_.MSDI= GetSpecificInfo(MediaInfoStreamKind.General,num2,"MSDI");
                        _tracktemp_.ISBN= GetSpecificInfo(MediaInfoStreamKind.General,num2,"ISBN");
                        _tracktemp_.BarCode= GetSpecificInfo(MediaInfoStreamKind.General,num2,"BarCode");
                        _tracktemp_.LCCN= GetSpecificInfo(MediaInfoStreamKind.General,num2,"LCCN");
                        _tracktemp_.CatalogNumber= GetSpecificInfo(MediaInfoStreamKind.General,num2,"CatalogNumber");
                        _tracktemp_.LabelCode= GetSpecificInfo(MediaInfoStreamKind.General,num2,"LabelCode");
                        _tracktemp_.Cover = GetSpecificInfo(MediaInfoStreamKind.General, num2, "Cover") == "Yes" ? true : false;
                        _tracktemp_.CoverType = GetSpecificInfo(MediaInfoStreamKind.General, num2, "Cover_MIME");
                        _tracktemp_.CoverData= Convert.FromBase64String(GetSpecificInfo(MediaInfoStreamKind.General, num2, "Cover_Data"));
                        _tracktemp_.BPM= GetSpecificInfo(MediaInfoStreamKind.General,num2,"BPM");
                        _tracktemp_.VideoCodecList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Video_Codec_List");
                        _tracktemp_.VideoLanguageList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Video_Language_List");
                        _tracktemp_.AudioCodecList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Audio_Codec_List");
                        _tracktemp_.AudioLanguageList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Audio_Language_List");
                        _tracktemp_.TextCodecList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Text_Codec_List");
                        _tracktemp_.TextLanguageList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Text_Language_List");
                        _tracktemp_.ChaptersCodecList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Chapters_Codec_List");
                        _tracktemp_.ChaptersLanguageList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Chapters_Language_List");
                        _tracktemp_.ImageCodecList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Image_Codec_List");
                        _tracktemp_.ImageLanguageList= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Image_Language_List");
                        _tracktemp_.Other= GetSpecificInfo(MediaInfoStreamKind.General,num2,"Other");

                        if (_tracktemp_.OveralBitRate == 0)
                        {
                            _tracktemp_.OveralBitRate = computeOverralBitRate(_tracktemp_);

                            if (_tracktemp_.OveralBitRate > 0 && String.IsNullOrEmpty(_tracktemp_.OveralBitRateString))
                            {
                                _tracktemp_.OveralBitRateString = getFormattedBitrate(_tracktemp_.OveralBitRate);
                            }
                        }

                        this._General.Add(_tracktemp_);
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
                   getVideoInfo();
                }
                return this._Video;
            }
        }

        /// <summary>
        /// Gets the informations of all video streams
        /// </summary>
        private void getVideoInfo()
        {
            if (this._Video == null)
            {
                this._Video = new List<VideoTrack>();
                int num1 = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Video, -1);
                if (num1 > 0)
                {
                    int num3 = num1 - 1;
                    for (int num2 = 0; num2 <= num3; num2++)
                    {
                        VideoTrack _tracktemp_ = new VideoTrack();                            
                        _tracktemp_.Count = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Count"));
                        _tracktemp_.StreamCount = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"StreamCount"));
                        _tracktemp_.StreamKind = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"StreamKind");
                        _tracktemp_.StreamKindID = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"StreamKindID"));
                        _tracktemp_.StreamOrder = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "StreamOrder"));
                        _tracktemp_.Inform = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Inform");
                        _tracktemp_.ID = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"ID");
                        _tracktemp_.UniqueID = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"UniqueID");
                        _tracktemp_.Title = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Title");
                        _tracktemp_.Codec = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Codec");
                        _tracktemp_.CodecString = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Codec/String");
                        _tracktemp_.CodecInfo = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Codec/Info");
                        _tracktemp_.CodecUrl = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Codec/Url");
                        _tracktemp_.CodecID = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "CodecID");
                        _tracktemp_.CodecIDInfo = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "CodecID/Info");
                        _tracktemp_.BitRate = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"BitRate"));
                        _tracktemp_.BitRateString = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"BitRate/String");
                        _tracktemp_.NominalBitRate = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "BitRate_Minimum"));
                        _tracktemp_.MinimumBitRateString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "BitRate_Minimum/String");
                        _tracktemp_.MinimumBitRate = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "BitRate_Nominal"));
                        _tracktemp_.NominalBitRateString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "BitRate_Nominal/String");
                        _tracktemp_.MaximumBitRate = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "BitRate_Maximum"));
                        _tracktemp_.MaximumBitRateString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "BitRate_Maximum/String");
                        _tracktemp_.BitRateMode = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "BitRate_Mode");
                        _tracktemp_.EncodedLibrary = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Encoded_Library");
                        _tracktemp_.EncodedLibrarySettings = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Encoded_Library_Settings");
                        _tracktemp_.Width = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Width"));
                        _tracktemp_.Height = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Height"));
                        _tracktemp_.AspectRatio = getDoubleOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"AspectRatio"));
                        _tracktemp_.AspectRatioString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "AspectRatio/String");
                        _tracktemp_.PixelAspectRatio = getDoubleOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "PixelAspectRatio"));
                        _tracktemp_.PixelAspectRatioString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "PixelAspectRatio/String");
                        _tracktemp_.FrameRate = getDoubleOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"FrameRate"));
                        _tracktemp_.FrameRateString = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"FrameRate/String");
                        _tracktemp_.FrameRateOriginal = getDoubleOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "FrameRate_Original"));
                        _tracktemp_.FrameRateOriginalString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "FrameRate_Original/String");
                        _tracktemp_.FrameRateMode = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "FrameRate_Mode");
                        _tracktemp_.FrameRateModeString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "FrameRate_Mode/String");
                        _tracktemp_.FrameCount = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"FrameCount"));
                        _tracktemp_.BitDepth = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"BitDepth"));
                        _tracktemp_.BitsPixelFrame = getDoubleOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Bits/(Pixel*Frame)"));
                        _tracktemp_.Delay = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Delay"));
                        _tracktemp_.DurationMillisec = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Duration"));
                        _tracktemp_.DurationString = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Duration/String3");
                        _tracktemp_.Language = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Language");
                        _tracktemp_.LanguageString = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Language/String");
                        _tracktemp_.LanguageMore = GetSpecificInfo(MediaInfoStreamKind.Video,num2,"Language_More");
                        _tracktemp_.Format = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format");
                        _tracktemp_.FormatInfo = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format/Info");
                        _tracktemp_.FormatProfile = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Profile");
                        _tracktemp_.MultiViewCount = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "MultiView_Count"));
                        _tracktemp_.MultiViewLayout = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "MultiView_Layout");
                        _tracktemp_.FormatSettings = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings");
                        _tracktemp_.FormatSettingsBVOP = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_BVOP");
                        _tracktemp_.FormatSettingsBVOPString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_BVOP/String");
                        _tracktemp_.FormatSettingsCABAC = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_CABAC");
                        _tracktemp_.FormatSettingsCABACString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_CABAC/String");
                        _tracktemp_.FormatSettingsGMC = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_GMC");
                        _tracktemp_.FormatSettingsGMCString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_GMAC/String");
                        _tracktemp_.FormatSettingsMatrix = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_Matrix");
                        _tracktemp_.FormatSettingsMatrixData = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_Matrix_Data");
                        _tracktemp_.FormatSettingsMatrixString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_Matrix/String");
                        _tracktemp_.FormatSettingsPulldown = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_Pulldown");
                        _tracktemp_.FormatSettingsQPel = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_QPel");
                        _tracktemp_.FormatSettingsQPelString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_QPel/String");
                        _tracktemp_.FormatSettingsRefFrames = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_RefFrames"));
                        _tracktemp_.FormatSettingsRefFramesString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Settings_RefFrames/String");
                        _tracktemp_.ScanType = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "ScanType");
                        _tracktemp_.ScanTypeString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "ScanType/String");
                        _tracktemp_.FormatUrl = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format/Url");
                        _tracktemp_.FormatVersion = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Format_Version");
                        _tracktemp_.Default = getBoolOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Default"));
                        _tracktemp_.DefaultString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Default/String");
                        _tracktemp_.Forced = getBoolOrDefault(GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Forced"));
                        _tracktemp_.ForcedString = GetSpecificInfo(MediaInfoStreamKind.Video, num2, "Forced/String");

                        if (_tracktemp_.DurationMillisec == 0)
                        {
                            _tracktemp_.DurationMillisec = calculateTotalMillis(_tracktemp_);
                        }

                         this._Video.Add(_tracktemp_);
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
                   getAudioInfo();
                }
                return this._Audio;
            }
        }

        /// <summary>
        /// Gets the informations of all audio streams
        /// </summary>
        private void getAudioInfo()
        {
            if (this._Audio == null)
            {
                this._Audio = new List<AudioTrack>();
                int num1 = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Audio, -1);
                if (num1 > 0)
                {
                    int num3 = num1 - 1;
                    for (int num2 = 0; num2 <= num3; num2++)
                    {
                        AudioTrack _tracktemp_ = new AudioTrack();                                              
                        _tracktemp_.Count= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"Count"));
                        _tracktemp_.StreamCount= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"StreamCount"));
                        _tracktemp_.StreamKind= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"StreamKind");
                        _tracktemp_.StreamKindString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "StreamKind/String");
                        _tracktemp_.StreamKindID= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"StreamKindID"));
                        _tracktemp_.StreamKindPos = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "StreamKindPos"));
                        _tracktemp_.StreamOrder = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "StreamOrder"));
                        _tracktemp_.Inform= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"Inform");
                        _tracktemp_.ID= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"ID");
                        _tracktemp_.IDString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "ID/String");
                        _tracktemp_.UniqueID= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"UniqueID");
                        _tracktemp_.MenuID = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "MenuID");
                        _tracktemp_.MenuIDString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "MenuID/String");
                        _tracktemp_.Format = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format");
                        _tracktemp_.FormatInfo = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format/Info");
                        _tracktemp_.FormatUrl = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format/Url");
                        _tracktemp_.FormatVersion = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Version");
                        _tracktemp_.FormatProfile = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Profile");
                        _tracktemp_.FormatSettings = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings");
                        _tracktemp_.FormatSettingsSBR = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_SBR");
                        _tracktemp_.FormatSettingsSBRString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_SBR/String");
                        _tracktemp_.FormatSettingsPS = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_PS");
                        _tracktemp_.FormatSettingsPSString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_PS/String");
                        _tracktemp_.FormatSettingsFloor = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_Floor");
                        _tracktemp_.FormatSettingsFirm = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_Firm");
                        _tracktemp_.FormatSettingsEndianness = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_Endianness");
                        _tracktemp_.FormatSettingsSign = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_Sign");
                        _tracktemp_.FormatSettingsLaw = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_Law");
                        _tracktemp_.FormatSettingsITU = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Format_Settings_ITU");
                        _tracktemp_.MuxingMode = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "MuxingMode");
                        _tracktemp_.CodecID = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "CodecID"));
                        _tracktemp_.CodecIDInfo = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "CodecID/Info");
                        _tracktemp_.CodecIDUrl = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "CodecID/Url");
                        _tracktemp_.CodecIDHint = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "CodecID/Hint");
                        _tracktemp_.CodecIDDescription = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "CodecID_Description");
                        _tracktemp_.DurationMillisec = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Duration"));
                        _tracktemp_.DurationStringFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Duration/String3");
                        _tracktemp_.BitRateMode = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate_Mode");
                        _tracktemp_.BitRateModeString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate_Mode/String");
                        _tracktemp_.BitRate = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate"));
                        _tracktemp_.BitRateString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate/String");
                        _tracktemp_.BitRateMinimum = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate_Minimum"));
                        _tracktemp_.BitRateMinimumString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate_Minimum/String");
                        _tracktemp_.BitRateNominal = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate_Nominal"));
                        _tracktemp_.BitRateNominalString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate_Nominal/String");
                        _tracktemp_.BitRateMaximum = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate_Maximum"));
                        _tracktemp_.BitRateMaximumString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitRate_Maximum/String");
                        _tracktemp_.Channels = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Channel(s)"));
                        _tracktemp_.ChannelsString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Channel(s)/String");
                        _tracktemp_.ChannelMode = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "ChannelMode");
                        _tracktemp_.ChannelPositions = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "ChannelPositions");
                        _tracktemp_.ChannelPositionsString2 = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "ChannelPositions/String2");
                        _tracktemp_.SamplingRate = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "SamplingRate"));
                        _tracktemp_.SamplingRateString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "SamplingRate/String");
                        _tracktemp_.SamplingCount = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "SamplingCount"));
                        _tracktemp_.BitDepth = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitDepth"));
                        _tracktemp_.BitDepthString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "BitDepth/String");
                        _tracktemp_.CompressionRatio = getDoubleOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "CompressionRatio"));
                        _tracktemp_.Delay = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Delay"));
                        _tracktemp_.DelayString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Delay/String");
                        _tracktemp_.DelayStringFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Delay/String3");
                        _tracktemp_.VideoDelay = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Video_Delay"));
                        _tracktemp_.VideoDelayString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Video_Delay/String");
                        _tracktemp_.VideoDelayStringFormatted = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Video_Delay/String3");
                        _tracktemp_.ReplayGainGain = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "ReplayGain_Gain"));
                        _tracktemp_.ReplayGainGainString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "ReplayGain_Gain/String");
                        _tracktemp_.ReplayGainPeak = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "ReplayGain_Peak");
                        _tracktemp_.StreamSize = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "StreamSize"));
                        _tracktemp_.StreamSizeString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "StreamSize/String");
                        _tracktemp_.StreamSizeStringRounded = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "StreamSize/String1");
                        _tracktemp_.StreamSizeStringPrecise = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "StreamSize/String4");
                        _tracktemp_.StreamSizeProportion = getDoubleOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "StreamSize_Proportion"));
                        _tracktemp_.Alignment = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Alignment");
                        _tracktemp_.AlignmentString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Alignment/String");
                        _tracktemp_.InterleaveVideoFrames = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Interleave_VideoFrames");
                        _tracktemp_.InterleaveDuration = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Interleave_Duration");
                        _tracktemp_.InterleaveDurationString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Interleave_Duration/String");
                        _tracktemp_.InterleavePreload = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Interleave_Preload");
                        _tracktemp_.InterleavePreloadString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Interleave_Preload/String");
                        _tracktemp_.Title= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"Title");
                        _tracktemp_.EncodedLibrary= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"Encoded_Library");
                        _tracktemp_.EncodedLibraryString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Encoded_Library/String");
                        _tracktemp_.EncodedLibraryName = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Encoded_Library/Name");
                        _tracktemp_.EncodedLibraryVersion = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Encoded_Library/Version");
                        _tracktemp_.EncodedLibraryDate = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Encoded_Library/Date");
                        _tracktemp_.EncodedLibrarySettings= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"Encoded_Library_Settings");
                        _tracktemp_.Language= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"Language");
                        _tracktemp_.LanguageString= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"Language/String");
                        _tracktemp_.LanguageMore= GetSpecificInfo(MediaInfoStreamKind.Audio,num2,"Language_More");
                        _tracktemp_.EncodedDate = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Encoded_Date");
                        _tracktemp_.TaggedDate = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Tagged_Date");
                        _tracktemp_.Encryption = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Encryption");
                        _tracktemp_.Default = getBoolOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Default"));
                        _tracktemp_.DefaultString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Default/String");
                        _tracktemp_.Forced = getBoolOrDefault(GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Forced"));
                        _tracktemp_.ForcedString = GetSpecificInfo(MediaInfoStreamKind.Audio, num2, "Forced/String");   
                        
                        if (_tracktemp_.Channels == 0 && !String.IsNullOrEmpty(_tracktemp_.ChannelsString))
                        {
                            _tracktemp_.Channels = parseChannelsCount(_tracktemp_.ChannelsString);
                        }
                                                        
                        this._Audio.Add(_tracktemp_);
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
                   getTextInfo();
                }
                return this._Text;
            }
        }

        /// <summary>
        /// Gets the informations of all text streams
        /// </summary>
        private void getTextInfo()
        {
            if (this._Text == null)
            {
                this._Text = new List<TextTrack>();
                int num1 = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Text, -1);
                if (num1 > 0)
                {
                    int num3 = num1 - 1;
                    for (int num2 = 0; num2 <= num3; num2++)
                    {
                        TextTrack _tracktemp_ = new TextTrack();                              
                        _tracktemp_.Count= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Count"));
                        _tracktemp_.StreamCount= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Text,num2,"StreamCount"));
                        _tracktemp_.StreamKind= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"StreamKind");
                        _tracktemp_.StreamKindID= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Text,num2,"StreamKindID"));
                        _tracktemp_.StreamOrder = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Text, num2, "StreamOrder"));
                        _tracktemp_.Inform= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Inform");
                        _tracktemp_.ID= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"ID");
                        _tracktemp_.UniqueID= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"UniqueID");
                        _tracktemp_.Title= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Title");
                        _tracktemp_.Codec= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Codec");
                        _tracktemp_.CodecString= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Codec/String");
                        _tracktemp_.CodecUrl= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Codec/Url");
                        _tracktemp_.Delay= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Delay"));
                        _tracktemp_.Video0Delay= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Video0_Delay"));
                        _tracktemp_.PlayTimeMillisec = getULongOrDefault(GetSpecificInfo(MediaInfoStreamKind.Text,num2,"PlayTime"));
                        _tracktemp_.PlayTimeStringFormatted = GetSpecificInfo(MediaInfoStreamKind.Text,num2,"PlayTime/String3");
                        _tracktemp_.Language= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Language");
                        _tracktemp_.LanguageString= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Language/String");
                        _tracktemp_.LanguageMore= GetSpecificInfo(MediaInfoStreamKind.Text,num2,"Language_More");
                        _tracktemp_.Default = getBoolOrDefault(GetSpecificInfo(MediaInfoStreamKind.Text, num2, "Default"));
                        _tracktemp_.DefaultString = GetSpecificInfo(MediaInfoStreamKind.Text, num2, "Default/String");
                        _tracktemp_.Forced = getBoolOrDefault(GetSpecificInfo(MediaInfoStreamKind.Text, num2, "Forced"));
                        _tracktemp_.ForcedString = GetSpecificInfo(MediaInfoStreamKind.Text, num2, "Forced/String");
                         this._Text.Add(_tracktemp_);
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
                   getMenuInfo();
                }
                return this._Menu;
            }
        }

        /// <summary>
        /// Gets the informations of all menu streams
        /// </summary>
        private void getMenuInfo()
        {
            if (this._Menu == null)
            {
                this._Menu = new List<MenuTrack>();
                int menuscount = MediaInfo.MediaInfo_Count_Get(this.Handle, MediaInfoStreamKind.Menu, -1);

                if (menuscount > 0)
                {
                    int max = menuscount - 1;
                    for (int curr = 0; curr <= max; curr++)
                    {
                        MenuTrack _tracktemp_ = new MenuTrack();
                        int endpos = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Chapters_Pos_End"));
                        int startpos = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "Chapters_Pos_Begin"));

                        _tracktemp_.ChaptersCount = endpos - startpos;
                        _tracktemp_.StreamCount= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Menu, curr,"StreamCount"));
                        _tracktemp_.StreamKind= GetSpecificInfo(MediaInfoStreamKind.Menu, curr,"StreamKind");
                        _tracktemp_.StreamKindID= getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Menu, curr,"StreamKindID"));
                        _tracktemp_.StreamOrder = getIntOrDefault(GetSpecificInfo(MediaInfoStreamKind.Menu, curr, "StreamOrder"));
                        _tracktemp_.RawData= GetSpecificInfo(MediaInfoStreamKind.Menu, curr,"Inform");
                        _tracktemp_.Default = getBoolOrDefault(GetSpecificInfo(MediaInfoStreamKind.Chapters, curr, "Default"));
                        _tracktemp_.Forced = getBoolOrDefault(GetSpecificInfo(MediaInfoStreamKind.Chapters, curr, "Forced"));

                        //get chapters list
                        for (int index = startpos; index < endpos; index++)
                        {
                            ChapterTrack _chapter_ = new ChapterTrack();
                            string tmpstr = GetSpecificSubInfo(MediaInfoStreamKind.Menu, curr, index, MediaInfoInfoKind.Text);
                            _chapter_.Language = tmpstr.Substring(0, 2);
                            _chapter_.Title = tmpstr.Substring(3, tmpstr.Length - 3);
                            _chapter_.Index = index - startpos;
                            _chapter_.Timecode = GetSpecificSubInfo(MediaInfoStreamKind.Menu, curr, index, MediaInfoInfoKind.Name);                  
                            _tracktemp_.Chapters.Add(_chapter_);
                        }

                        this._Menu.Add(_tracktemp_);
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
        /// Gets the formatted bitrate string.
        /// </summary>
        /// <param name="bitrate">The bitrate.</param>
        /// <returns>the formatted bitrate string.</returns>
        private string getFormattedBitrate(ulong bitrate)
        {
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

            return "";
        }

        /// <summary>
        /// Computes the overral bitrate.
        /// </summary>
        /// <param name="general">The filled GeneralTrack class.</param>
        /// <returns>The computed overral bitrate, 0 if not all necessary infos are available.</returns>
        private ulong computeOverralBitRate(GeneralTrack general)
        {
            if (general.PlayTimeMillisec > 0 && general.FileSize > 0)
            {
                return (ulong)((general.FileSize * 8) / (general.PlayTimeMillisec / 1000));
            }
            return 0;
        }

        /// <summary>
        /// Helper method to parse channels count from channels string, if fails return default value
        /// </summary>
        /// <param name="channelstring">The formatted channel string.</param>
        /// <returns>Numbers of channels parsed from the input string, 0 in case of failure.</returns>
        private int parseChannelsCount(string channelstring)
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
        private ulong calculateTotalMillis(VideoTrack tmpitem)
        {
            if (String.IsNullOrEmpty(tmpitem.DurationString))
            {
                if (tmpitem.FrameCount > 0 && tmpitem.FrameRate > 0)
                {
                    return (ulong)((tmpitem.FrameCount / tmpitem.FrameRate) * 1000);
                }
            }

            if (!String.IsNullOrEmpty(tmpitem.DurationString))
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
        /// <returns>The parsed integer or 0 in case of failure.</returns>
        private int getIntOrDefault(string input)
        {
            int value;
            return int.TryParse(input, out value) ? value : 0;
        }

        /// <summary>
        /// Helper method to parse an ULong from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed ULong or 0 in case of failure.</returns>
        private ulong getULongOrDefault(string input)
        {
            ulong value;
            return ulong.TryParse(input, out value) ? value : 0UL;
        }

        /// <summary>
        /// Helper method to parse an double from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed double or 0 in case of failure.</returns>
        private double getDoubleOrDefault(string input)
        {
            double value;
            return double.TryParse(input.Replace(".", ","), out value) ? value : 0D;
        }

        /// <summary>
        /// Helper method to parse an bool from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed bool or "false" in case of failure.</returns>
        private bool getBoolOrDefault(string input)
        {
            bool value;
            return bool.TryParse(input, out value) ? value : false;
        }
        #endregion
    }
}
