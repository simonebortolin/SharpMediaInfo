using SharpMediaInfo.Exception;
using SharpMediaInfo.Info;
using SharpMediaInfo.Native;
using SharpMediaInfo.Native.Enum;
using SharpMediaInfo.Tracks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SharpMediaInfo
{
    /// <summary>
    /// Builder for creating immutable MediaInfo instances from files or streams.
    /// </summary>
    public sealed partial class MediaInfoBuilder
    {
        private IMediaInfoNative? _mediaInfoNative;
        private string? _fileName;
        private bool _parseCoverData;
        private bool _parseCompleteReport;
        private bool _parseStandardReport;
        private bool _isBuilt;

        // Parsed data
        private List<GeneralTrack>? _general;
        private List<VideoTrack>? _video;
        private List<AudioTrack>? _audio;
        private List<TextTrack>? _text;
        private List<ImageTrack>? _image;
        private List<MenuTrack>? _menu;
        private int _generalCount;
        private int _videoCount;
        private int _audioCount;
        private int _textCount;
        private int _imageCount;
        private int _menuCount;
        private string? _infoComplete;
        private string? _infoStandard;

        private MediaInfoBuilder()
        {
        }

        /// <summary>
        /// Creates a builder to analyze a media file.
        /// </summary>
        /// <param name="path">The path to the media file.</param>
        /// <returns>A MediaInfoBuilder instance for method chaining.</returns>
        public static MediaInfoBuilder FromFile(string path)
        {
            var builder = new MediaInfoBuilder();
            builder.InitializeFromFile(path);
            return builder;
        }

        /// <summary>
        /// Creates a builder to analyze a media stream.
        /// </summary>
        /// <param name="stream">The stream containing media data.</param>
        /// <returns>A MediaInfoBuilder instance for method chaining.</returns>
        public static MediaInfoBuilder FromStream(Stream stream)
        {
            var builder = new MediaInfoBuilder();
            builder.InitializeFromStream(stream);
            return builder;
        }

        /// <summary>
        /// Creates a builder by copying data from an existing MediaInfo instance.
        /// </summary>
        /// <param name="mediaInfo">The MediaInfo instance to copy.</param>
        /// <returns>A MediaInfoBuilder instance for method chaining.</returns>
        public static MediaInfoBuilder FromCopy(MediaInfo mediaInfo)
        {
            var builder = new MediaInfoBuilder();
            builder.InitializeFromCopy(mediaInfo);
            return builder;
        }

        /// <summary>
        /// Enables parsing of cover image data as base64.
        /// </summary>
        /// <returns>The builder instance for method chaining.</returns>
        public MediaInfoBuilder WithCoverData()
        {
            ThrowIfBuilt();
            _parseCoverData = true;
            return this;
        }

        /// <summary>
        /// Enables parsing of both complete and standard reports.
        /// </summary>
        /// <returns>The builder instance for method chaining.</returns>
        public MediaInfoBuilder WithReports()
        {
            ThrowIfBuilt();
            _parseCompleteReport = true;
            _parseStandardReport = true;
            return this;
        }

        /// <summary>
        /// Enables parsing of the complete report only.
        /// </summary>
        /// <returns>The builder instance for method chaining.</returns>
        public MediaInfoBuilder WithCompleteReport()
        {
            ThrowIfBuilt();
            _parseCompleteReport = true;
            return this;
        }

        /// <summary>
        /// Enables parsing of the standard report only.
        /// </summary>
        /// <returns>The builder instance for method chaining.</returns>
        public MediaInfoBuilder WithStandardReport()
        {
            ThrowIfBuilt();
            _parseStandardReport = true;
            return this;
        }

        /// <summary>
        /// Builds an immutable MediaInfo instance with all parsed data.
        /// </summary>
        /// <returns>An immutable MediaInfo instance.</returns>
        public MediaInfo Build()
        {
            ThrowIfBuilt();

            try
            {
                // Parse all stream information
                GetStreamCount();
                ParseAllStreams();

                // Parse reports if requested
                if (_parseCompleteReport)
                {
                    GetCompleteReport();
                }
                if (_parseStandardReport)
                {
                    GetStandardReport();
                }

                _isBuilt = true;

                // Create immutable MediaInfo instance
                return MediaInfo.CreateFromBuilder(this);
            }
            finally
            {
                // Close native handle
                _mediaInfoNative?.Close();
            }
        }

        #region Initialization Methods

        private void InitializeFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified file could not be found.");
            }

            _fileName = path;
            _mediaInfoNative = IMediaInfoNative.Create(_parseCoverData);
            _mediaInfoNative.OpenFile(path);
        }

        private void InitializeFromStream(Stream stream)
        {
            if (!stream.CanRead || !stream.CanSeek || stream.Length <= 0)
            {
                throw new ArgumentException("The stream cannot be read or it's empty");
            }

            _mediaInfoNative = IMediaInfoNative.Create(_parseCoverData);
            _mediaInfoNative.OpenStream(stream);

            // Try to get filename from General track after opening
            _fileName = string.Empty;
        }

        private void InitializeFromCopy(MediaInfo source)
        {
            _general = source.General?.ToList();
            _generalCount = source.GeneralCount;
            _video = source.Video?.ToList();
            _videoCount = source.VideoCount;
            _audio = source.Audio?.ToList();
            _audioCount = source.AudioCount;
            _text = source.Text?.ToList();
            _textCount = source.TextCount;
            _image = source.Image?.ToList();
            _imageCount = source.ImageCount;
            _menu = source.Menu?.ToList();
            _menuCount = source.MenuCount;
            _infoComplete = source.InfoComplete;
            _infoStandard = source.InfoStandard;
            _fileName = source.FileName;
            _parseCoverData = source.ParseCoverData;
        }

        #endregion

        #region Parsing Methods

        private void GetStreamCount()
        {
            if (_mediaInfoNative == null) return;

            _audioCount = _mediaInfoNative.GetStreamCount(StreamKind.Audio);
            _videoCount = _mediaInfoNative.GetStreamCount(StreamKind.Video);
            _generalCount = _mediaInfoNative.GetStreamCount(StreamKind.General);
            _textCount = _mediaInfoNative.GetStreamCount(StreamKind.Text);
            _imageCount = _mediaInfoNative.GetStreamCount(StreamKind.Image);
            _menuCount = _mediaInfoNative.GetStreamCount(StreamKind.Menu);
        }

        private void ParseAllStreams()
        {
            if (_mediaInfoNative == null) return;

            ParseGeneralInfo();
            ParseVideoInfo();
            ParseAudioInfo();
            ParseTextInfo();
            ParseImageInfo();
            ParseMenuInfo();

            // Update filename from General track if from stream
            if (string.IsNullOrEmpty(_fileName) && _general?.Count > 0)
            {
                _fileName = !string.IsNullOrEmpty(_general[0].FileNameExtension)
                    ? _general[0].FileNameExtension
                    : "stream";
            }
        }

        private void ParseGeneralInfo()
        {
            _general = new List<GeneralTrack>();
            for (int index = 0; index < _generalCount; index++)
            {
                _general.Add(PopulateGeneralTrack(_mediaInfoNative, index));
            }
        }

        private void ParseVideoInfo()
        {
            _video = new List<VideoTrack>();
            for (int index = 0; index < _videoCount; index++)
            {
                _video.Add(PopulateVideoTrack(_mediaInfoNative, index));
            }
        }

        private void ParseAudioInfo()
        {
            _audio = new List<AudioTrack>();
            for (int index = 0; index < _audioCount; index++)
            {
                _audio.Add(PopulateAudioTrack(_mediaInfoNative, index));
            }
        }

        private void ParseTextInfo()
        {
            _text = new List<TextTrack>();
            for (int index = 0; index < _textCount; index++)
            {
                _text.Add(PopulateTextTrack(_mediaInfoNative, index));
            }
        }

        private void ParseImageInfo()
        {
            _image = new List<ImageTrack>();
            for (int index = 0; index < _imageCount; index++)
            {
                _image.Add(PopulateImageTrack(_mediaInfoNative, index));
            }
        }

        private void ParseMenuInfo()
        {
            _menu = new List<MenuTrack>();
            for (int index = 0; index < _menuCount; index++)
            {
                MenuTrack tmp = PopulateMenuTrack(_mediaInfoNative, index);

                // Get chapters count (diff between start position and end position)
                int endpos = tmp.ChaptersPosEnd.GetValueOrDefault();
                int startpos = tmp.ChaptersPosBegin.GetValueOrDefault();
                var chaptersCount = endpos - startpos;

                // Get chapters list inside each menu
                if (chaptersCount > 0)
                {
                    tmp.ChaptersInternal = [];

                    for (int innerindex = startpos; innerindex < endpos; innerindex++)
                    {
                        ChapterTrack chapt = new ChapterTrack
                        {
                            Index = innerindex - startpos
                        };

                        string tmpstr = _mediaInfoNative!.GetSpecificParameter(StreamKind.Menu, index, innerindex, InfoKind.Text);

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

                        chapt.Timecode = _mediaInfoNative.GetSpecificParameter(StreamKind.Menu, index, innerindex, InfoKind.Name);
                        tmp.ChaptersInternal.Add(chapt);
                    }
                }

                _menu.Add(tmp);
            }
        }

        private void GetCompleteReport()
        {
            if (_mediaInfoNative != null)
            {
                _infoComplete = _mediaInfoNative.GetCompleteReport();
            }
        }

        private void GetStandardReport()
        {
            if (_mediaInfoNative != null)
            {
                _infoStandard = _mediaInfoNative.GetStandardReport();
            }
        }

        #endregion

        #region Internal Access for MediaInfo

        internal List<GeneralTrack>? General => _general;
        internal List<VideoTrack>? Video => _video;
        internal List<AudioTrack>? Audio => _audio;
        internal List<TextTrack>? Text => _text;
        internal List<ImageTrack>? Image => _image;
        internal List<MenuTrack>? Menu => _menu;
        internal int GeneralCount => _generalCount;
        internal int VideoCount => _videoCount;
        internal int AudioCount => _audioCount;
        internal int TextCount => _textCount;
        internal int ImageCount => _imageCount;
        internal int MenuCount => _menuCount;
        internal string? InfoComplete => _infoComplete;
        internal string? InfoStandard => _infoStandard;
        internal string? FileName => _fileName;
        internal bool ParseCoverData => _parseCoverData;

        #endregion

        #region Parsing Helper Methods

        private TimeSpan GetTimeSpanValue(string input)
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

        private TimeSpan? GetTimeSpanValueNullable(string input)
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

        private int? GetIntValueNullable(string input)
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

        private int GetIntValue(string input)
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

        private long? GetLongValueNullable(string input)
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

        private long GetLongValue(string input)
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

        private double? GetDoubleValueNullable(string input)
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

        private double GetDoubleValue(string input)
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

        private BigInteger GetBigIntegerValue(string input)
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

        private BigInteger? GetBigIntegerValueNullable(string input)
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

        private FrameRateInfo? GetFrameRateValueNullable(double? frameRate, long? num, long? den)
        {
            return (frameRate.HasValue || (num.HasValue && den.HasValue)) ? new FrameRateInfo(frameRate, num, den) : null;
        }

        private FrameRateInfo? GetFrameRateValueNullable(double? frameRate, double? num, double? den)
        {
            return (frameRate.HasValue || (num.HasValue && den.HasValue)) ? new FrameRateInfo(frameRate, (long?)num, (long?)den) : null;
        }

        private bool? GetBoolValueNullable(string input)
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

        private bool GetBoolValue(string input)
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

        private List<string> GetStringListValue(string input, string separator = "/")
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

        private List<byte[]> GetCoverDataValue(string input)
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

        private MeasuredValue<int> GetMeasuredIntValue(string input, string unit)
        {
            return new MeasuredValue<int>(GetIntValueNullable(input), unit);
        }

        private MeasuredValue<long> GetMeasuredLongValue(string input, string unit)
        {
            return new MeasuredValue<long>(GetLongValueNullable(input), unit);
        }

        private MeasuredValue<BigInteger> GetMeasuredBigIntegerValue(string input, string unit)
        {
            return new MeasuredValue<BigInteger>(GetBigIntegerValueNullable(input), unit);
        }

        private BitRateInfo GetBitRateInfo(string input)
        {
            return new BitRateInfo(GetLongValueNullable(input));
        }

        private StreamSizeInfo GetStreamSizeInfo(string sizeInput, string proportionInput)
        {
            var size = GetLongValueNullable(sizeInput);
            var proportion = GetDoubleValueNullable(proportionInput);
            return new StreamSizeInfo(size, proportion);
        }

        private TimeSpan? GetDurationAsTimeSpan(string input)
        {
            var milliseconds = GetLongValueNullable(input);
            return milliseconds.HasValue ? TimeSpan.FromMilliseconds(milliseconds.Value) : null;
        }

        #endregion

        private void ThrowIfBuilt()
        {
            if (_isBuilt)
            {
                throw new InvalidOperationException("This builder has already been used to build a MediaInfo instance. Create a new builder for additional operations.");
            }
        }
    }
}
