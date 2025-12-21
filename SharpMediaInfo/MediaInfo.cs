using SharpMediaInfo.Exception;
using SharpMediaInfo.Info;
using SharpMediaInfo.Native;
using SharpMediaInfo.Native.Enum;
using SharpMediaInfo.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpMediaInfo
{
    public sealed partial class MediaInfo : IMediaInfo, IDisposable
    {
        #region Information
        private IMediaInfoNative? _MediaInfoNative;

        #endregion

            #region Global variables
            /// <summary>
            /// The path of the file that it's analyzed.
            /// </summary>
        private string? _FileName;
        /// <summary>
        /// The flag to determine if the cover data must be parsed or not.
        /// </summary>
        private bool _ParseCoverData;

        #region Information
        /// <summary>
        /// The list of general streams
        /// </summary>
        private List<GeneralTrack>? _General;
        /// <summary>
        /// The list of video streams
        /// </summary>
        private List<VideoTrack>? _Video;
        /// <summary>
        /// The list of audio streams
        /// </summary>
        private List<AudioTrack>? _Audio;
        /// <summary>
        /// The list of text streams
        /// </summary>
        private List<TextTrack>? _Text;
        /// <summary>
        /// The list of image streams
        /// </summary>
        private List<ImageTrack>? _Image;
        /// <summary>
        /// The list of menu streams
        /// </summary>
        private List<MenuTrack>? _Menu;
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
        private string? _InfoComplete;
        /// <summary>
        /// The standard information from Mediainfo
        /// </summary>
        private string? _InfoStandard;
        /// <summary>
        /// The custom information from Mediainfo
        /// </summary>
        private string? _InfoCustom;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="parseCoverData">If set to <c>true</c> all covers data are parsed. (default: false)</param>
        MediaInfo(bool parseCoverData = false)
        {
            this._MediaInfoNative = IMediaInfoNative.Create(parseCoverData);

            this._ParseCoverData = parseCoverData;
        }

        /// <summary>
        /// Creates a new MediaInfo object instance by copying all information from the input MediaInfo object.
        /// </summary>
        /// <param name="mediaInfo">The MediaInfo object</param>
        public MediaInfo(MediaInfo mediaInfo) : this()
        {
            this._General = mediaInfo._General is not null ? [.. mediaInfo._General] : null;
            this._GeneralCount = mediaInfo._GeneralCount;
            this._Video = mediaInfo._Video is not null ? [.. mediaInfo._Video] : null;
            this._VideoCount = mediaInfo._VideoCount;
            this._Audio = mediaInfo._Audio is not null ? [.. mediaInfo._Audio] : null;
            this._AudioCount = mediaInfo._AudioCount;
            this._Text = mediaInfo._Text is not null ? [.. mediaInfo._Text] : null;
            this._TextCount = mediaInfo._TextCount;
            this._Image = mediaInfo._Image is not null ? [.. mediaInfo._Image] : null;
            this._ImageCount = mediaInfo._ImageCount;
            this._Menu = mediaInfo._Menu is not null ? [.. mediaInfo._Menu] : null;

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
        public MediaInfo(string path, bool parseCoverData = false, bool parseReports = false) : this(parseCoverData)
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

                RetrieveInformation(parseReports);

#if DEBUG
                sw.Stop();
                System.Diagnostics.Debug.WriteLine(string.Format("GetMediaInfo data parsing time: {0} ms", sw.ElapsedMilliseconds));
#endif     

            }
            catch (System.Exception ex)
            {
                throw new MediaInfoAnalysisException("An error occurred while trying to analyze the file.", ex);
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Creates a new MediaInfo object instance filled with all parsed information from the specified stream.
        /// </summary>
        /// <param name="stream">The stream that contains the file content to analyze.</param>
        /// <param name="parseCoverData">If set to <c>true</c> all covers data are parsed. (default: false)</param>
        /// <param name="parseReports">If set to <c>true</c> both specific and complete reports are immediately parsed. (default: false)</param>
        public MediaInfo(Stream stream, bool parseCoverData = false, bool parseReports = false) : this(parseCoverData)
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

                RetrieveInformation(parseReports);

                //check if mediainfo can get filename
                this._FileName = General?.Count > 0 && !string.IsNullOrEmpty(General[0].FileNameExtension) ? General[0].FileNameExtension : "stream";

#if DEBUG
                sw.Stop();
                System.Diagnostics.Debug.WriteLine(string.Format("GetMediaInfo data parsing time: {0} ms", sw.ElapsedMilliseconds));
#endif     

            }
            catch (System.Exception ex)
            {
                throw new MediaInfoAnalysisException("An error occurred while trying to analyze input stream.", ex);
            }
            finally
            {
                Close();
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

            this._MediaInfoNative.OpenFile(path);
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
            this._MediaInfoNative.OpenStream(stream);
        }
        #endregion

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
                    GeneralTrack tmp = PopulteGeneralTrack(_MediaInfoNative, index);

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
                    VideoTrack tmp = PopulteVideoTrack(_MediaInfoNative, index);

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
                    AudioTrack tmp = PopulteAudioTrack(_MediaInfoNative, index);

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
                    TextTrack tmp = PopulteTextTrack(_MediaInfoNative, index);

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
                    ImageTrack tmp = PopulteImageTrack(_MediaInfoNative, index);

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
                    MenuTrack tmp = PopulteMenuTrack(_MediaInfoNative, index);

                    //get chapters count (diff between start position and end position)
                    int endpos = tmp.ChaptersPosEnd.GetValueOrDefault();
                    int startpos = tmp.ChaptersPosBegin.GetValueOrDefault();

                    var chaptersCount = endpos - startpos;

                    //get chapters list inside each menu
                    if (chaptersCount > 0)
                    {
                        tmp.ChaptersInternal = [];

                        for (int innerindex = startpos; innerindex < endpos; innerindex++)
                        {
                            ChapterTrack chapt = new ChapterTrack
                            {
                                Index = innerindex - startpos
                            };

                            string tmpstr = _MediaInfoNative.GetSpecificParameter(StreamKind.Menu, index, innerindex, InfoKind.Text);

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

                            chapt.Timecode = _MediaInfoNative.GetSpecificParameter(StreamKind.Menu, index, innerindex, InfoKind.Name);
                            tmp.ChaptersInternal.Add(chapt);
                        }
                    }

                    this._Menu.Add(tmp);
                }
            }
        }

        /// <summary>
        /// Parses channels count from channels string.
        /// </summary>
        /// <param name="channelstring">The formatted channel string.</param>
        /// <returns>Numbers of channels parsed from the input string, 0 in case of failure.</returns>
        internal static int? GetChannelsCount(string channelstring)
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
        /// Parses the specified string and returns its value as a <see cref="TimeSpan"/>. Returns <see
        /// cref="TimeSpan.Zero"/> if the input is null, empty, or not a valid time interval.
        /// </summary>
        /// <param name="input">The string representation of the time interval to parse. Can be in any format supported by <see
        /// cref="TimeSpan.TryParse(string, out TimeSpan)"/>.</param>
        /// <returns>A <see cref="TimeSpan"/> value parsed from the input string, or <see cref="TimeSpan.Zero"/> if the input is
        /// null, empty, or invalid.</returns>
        internal static TimeSpan GetTimeSpanValue(string input)
        {
            if (string.IsNullOrEmpty(input)) return TimeSpan.Zero;
            if (TimeSpan.TryParse(input, out TimeSpan value))
            {
                return value;
            }
            else
            {
                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Parses the specified string and returns its value as a nullable TimeSpan.
        /// </summary>
        /// <param name="input">The string representation of a time interval to parse. Can be null or empty.</param>
        /// <returns>A TimeSpan value if the input is a valid time interval; otherwise, null.</returns>
        private static TimeSpan? GetTimeSpanValueNullable(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            if (TimeSpan.TryParse(input, out TimeSpan value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Helper method to parse an integer from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed integer or 0 in case of failure.</returns>
        private static int? GetIntValueNullable(string input)
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
        /// Converts the specified string representation of a number to its 32-bit signed integer equivalent.
        /// </summary>
        /// <param name="input">The string to convert to an integer. Can be null or empty.</param>
        /// <returns>The 32-bit signed integer value equivalent to the input string, or 0 if the input is null, empty, or not a
        /// valid integer.</returns>
        private static int GetIntValue(string input)
        {
            if (string.IsNullOrEmpty(input)) return 0;

            if (int.TryParse(input, out int value))
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Helper method to parse an ULong from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed ULong or 0 in case of failure.</returns>
        private static long? GetLongValueNullable(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            if (long.TryParse(input, out long value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the specified string representation of a number to its 64-bit signed integer equivalent.
        /// </summary>
        /// <param name="input">The string containing the number to convert. Can be null or empty.</param>
        /// <returns>A 64-bit signed integer equivalent to the numeric value contained in the input string, or 0 if the input is
        /// null, empty, or not a valid number.</returns>
        private static long GetLongValue(string input)
        {
            if (string.IsNullOrEmpty(input)) return 0;

            if (long.TryParse(input, out long value))
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Helper method to parse an double from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed double or 0 in case of failure.</returns>
        private static double? GetDoubleValueNullable(string input)
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
        /// Parses the specified string as a double-precision floating-point number. Returns 0.0 if the input is null,
        /// empty, or not a valid number.
        /// </summary>
        /// <remarks>This method attempts to handle both period and comma as decimal separators to
        /// accommodate different cultural formats. If parsing fails, the method returns 0.0 instead of throwing an
        /// exception.</remarks>
        /// <param name="input">The string representation of the number to parse. May use either a period or comma as the decimal separator.</param>
        /// <returns>The parsed double-precision floating-point value if the input is valid; otherwise, 0.0.</returns>
        private static double GetDoubleValue(string input) 
        { 
            if (string.IsNullOrEmpty(input)) return 0.0;

            if (double.TryParse(input.Replace(".", ","), out double value))
            {
                return value;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Parses the specified string and returns its numeric value as a BigInteger. Returns zero if the input is
        /// null, empty, or not a valid number.
        /// </summary>
        /// <param name="input">The string representation of the number to parse. Can be null or empty.</param>
        /// <returns>A BigInteger representing the parsed numeric value of the input string, or zero if the input is null, empty,
        /// or not a valid number.</returns>
        private static BigInteger GetBigIntegerValue(string input)
        {
            if (string.IsNullOrEmpty(input)) return BigInteger.Zero;
            if (BigInteger.TryParse(input, out BigInteger value))
            {
                return value;
            }
            else
            {
                return BigInteger.Zero;
            }
        }

        /// <summary>
        /// Parses the specified string and returns its value as a nullable <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="input">The string representation of a number to parse. Can be null or empty.</param>
        /// <returns>A <see cref="BigInteger"/> value if the input is a valid representation of a number; otherwise, <see
        /// langword="null"/>.</returns>
        private static BigInteger? GetBigIntegerValueNullable(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            if (BigInteger.TryParse(input, out BigInteger value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        private static FrameRateInfo? GetFrameRateValueNullable(double? frameRate, long? num, long? den)
        {
            return (frameRate.HasValue || (num.HasValue && den.HasValue)) ? new FrameRateInfo(frameRate, num, den) : null;
        }

        private static FrameRateInfo? GetFrameRateValueNullable(double? frameRate, double? num, double? den)
        {
            return (frameRate.HasValue || (num.HasValue && den.HasValue)) ? new FrameRateInfo(frameRate, (long?)num, (long?)den) : null;
        }

        /// <summary>
        /// Gets all stream types count.
        /// </summary>
        private void GetStreamCount()
        {
            _AudioCount = _MediaInfoNative.GetStreamCount(StreamKind.Audio);
            _VideoCount = _MediaInfoNative.GetStreamCount(StreamKind.Video);
            _GeneralCount = _MediaInfoNative.GetStreamCount(StreamKind.General);
            _TextCount = _MediaInfoNative.GetStreamCount(StreamKind.Text);
            _ImageCount = _MediaInfoNative.GetStreamCount( StreamKind.Image);
            _MenuCount = _MediaInfoNative.GetStreamCount(StreamKind.Menu);
        }


        /// <summary>
        /// Retrieves all informations from Mediainfo library.
        /// </summary>
        /// <param name="parseReports">Flag to determine whether to parse reports or not.</param>
        private void RetrieveInformation(bool parseReports)
        {
            //retrieve all information
            GetStreamCount();
            ParseInformation();

            if (parseReports)
            {
                GetCompleteReport();
                GetStandardReport();
            }
        }

        /// <summary>
        /// Gets the complete report.
        /// </summary>
        private void GetCompleteReport()
        {
            _InfoComplete = _MediaInfoNative.GetCompleteReport();
        }

        /// <summary>
        /// Gets the standard report.
        /// </summary>
        private void GetStandardReport()
        {
            _InfoStandard = _MediaInfoNative.GetStandardReport();
        }


        /// <summary>
        /// Helper method to parse an bool from a string, if fails return default value
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The parsed bool or "false" in case of failure.</returns>
        private static bool? GetBoolValueNullable(string input)
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

        private static bool GetBoolValue(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            if (input.ToLower().Contains("yes")) { return true; }

            if (input.ToLower().Contains("no")) { return false; }

            if (bool.TryParse(input, out bool value))
            {
                return value;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Helper method to parse a list from a string. Splits the input string using the input separator and removes trailing and leading whitespaces, if fails return an empty list.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The list with all splitted strings.</returns>
        private static List<string> GetStringListValue(string input, string separator = "/")
        {
            List<string> output = new List<string>();

            if (separator.Length > 0 && !string.IsNullOrEmpty(input))
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
        private static List<byte[]> GetCoverDataValue(string input)
        {
            List<byte[]> list = new List<byte[]>();

            if (string.IsNullOrEmpty(input)) { return list; }

            if (input.Contains(" / "))
            {
                foreach (string str in input.Split([" / "], StringSplitOptions.RemoveEmptyEntries))
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

        /// <summary>
        /// Gets every property for every track in a tracklist and get its value.
        /// </summary>
        /// <typeparam name="T">Type of tracklist, for instance List'VideoTrack'</typeparam>
        /// <param name="tracks">The track list</param>
        /// <param name="builder">The StringBuilder where append all properties strings.</param>
        private void ListEveryAvailablePropery<T>(List<T>? tracks, StringBuilder builder)
        {
            if (tracks is null) { return; }
            foreach (T track in tracks)
            {
                foreach (PropertyInfo p in track?.GetType().GetProperties() ?? [])
                {
                    object? value = p.GetValue(track, null);

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

        /// <summary>
        /// Gets the formatted bitrate string.
        /// </summary>
        /// <param name="bitrate">The bitrate.</param>
        /// <returns>the formatted bitrate string.</returns>
        internal static string? GetFormattedBitRate(long? bitrate)
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

        public bool InfoAvailable()
        {
            return _GeneralCount > 0 ? true : false;

        }

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
                return this._General!;
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
                return this._Video!;
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
                return this._Audio!;
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
                return this._Text!;
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
                return this._Image!;
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
                return this._Menu!;
            }
        }
        #endregion

        /// <summary>
        /// Complete information (will throw exception if accessed from a Mediainfo instance created from a stream without specifying 'parseReports' argument to true or if the file does not exists anymore).
        /// </summary>
        public string? InfoComplete
        {
            get
            {
                if (string.IsNullOrEmpty(this._InfoComplete))
                {
                    this.GetCompleteReport();
                }
                return _InfoComplete;
            }
        }

        /// <summary>
        /// Standard information (will throw exception if accessed from a Mediainfo instance created from a stream without specifying 'parseReports' argument to true or if the file does not exists anymore).
        /// </summary>
        public string? InfoStandard
        {
            get
            {
                if (string.IsNullOrEmpty(this._InfoStandard))
                {
                    this.GetStandardReport();
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

        IReadOnlyList<GeneralTrack> IMediaInfo.General => General;

        IReadOnlyList<VideoTrack> IMediaInfo.Video => Video;

        IReadOnlyList<AudioTrack> IMediaInfo.Audio => Audio;

        IReadOnlyList<TextTrack> IMediaInfo.Text => Text;

        IReadOnlyList<ImageTrack> IMediaInfo.Image => Image;

        IReadOnlyList<MenuTrack> IMediaInfo.Menu => Menu;

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            Close();
            _disposed = true;
            _General = null;
            _Video = null;
            _Audio = null;
            _Text = null;
            _Image = null;
            _Menu = null;
        }

        private void Close()
        {
            _MediaInfoNative?.Close();
        }
    }
}
    #endregion

