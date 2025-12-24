using SharpMediaInfo.Exception;
using SharpMediaInfo.Types;
using SharpMediaInfo.Native;
using SharpMediaInfo.Native.Enum;
using SharpMediaInfo.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace SharpMediaInfo
{
    /// <summary>
    /// Immutable representation of media file information parsed by MediaInfo library.
    /// Use <see cref="MediaInfoBuilder"/> to create instances.
    /// </summary>
    public sealed partial class MediaInfo : IMediaInfo
    {
        #region Immutable Fields
        private readonly List<GeneralTrack>? _general;
        private readonly List<VideoTrack>? _video;
        private readonly List<AudioTrack>? _audio;
        private readonly List<TextTrack>? _text;
        private readonly List<ImageTrack>? _image;
        private readonly List<MenuTrack>? _menu;
        private readonly int _generalCount;
        private readonly int _videoCount;
        private readonly int _audioCount;
        private readonly int _textCount;
        private readonly int _imageCount;
        private readonly int _menuCount;
        private readonly string? _infoComplete;
        private readonly string? _infoStandard;
        private string? _infoCustom;
        private readonly string? _fileName;
        private readonly bool _parseCoverData;
        #endregion

        /// <summary>
        /// Internal constructor used by MediaInfoBuilder to create immutable instances.
        /// </summary>
        internal MediaInfo(MediaInfoBuilder builder)
        {
            _general = builder.General;
            _generalCount = builder.GeneralCount;
            _video = builder.Video;
            _videoCount = builder.VideoCount;
            _audio = builder.Audio;
            _audioCount = builder.AudioCount;
            _text = builder.Text;
            _textCount = builder.TextCount;
            _image = builder.Image;
            _imageCount = builder.ImageCount;
            _menu = builder.Menu;
            _menuCount = builder.MenuCount;
            _infoComplete = builder.InfoComplete;
            _infoStandard = builder.InfoStandard;
            _fileName = builder.FileName;
            _parseCoverData = builder.ParseCoverData;
        }

        /// <summary>
        /// Factory method for creating MediaInfo from a builder.
        /// </summary>
        internal static MediaInfo CreateFromBuilder(MediaInfoBuilder builder)
        {
            return new MediaInfo(builder);
        }

        #region Helper Methods (used by Builder and Track classes)
        /// <summary>
        /// Gets every property for every track in a tracklist and get its value.
        /// </summary>
        /// <typeparam name="T">Type of tracklist, for instance IReadOnlyList'VideoTrack'</typeparam>
        /// <param name="tracks">The track list</param>
        /// <param name="builder">The StringBuilder where append all properties strings.</param>
        private static void ListEveryAvailablePropery<T>(IReadOnlyList<T> tracks, StringBuilder builder)
        {
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

        public bool InfoAvailable()
        {
            return _generalCount > 0;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the filename or path of the analyzed media file.
        /// </summary>
        public string? FileName => _fileName;

        /// <summary>
        /// Gets whether cover data parsing was enabled.
        /// </summary>
        public bool ParseCoverData => _parseCoverData;

        /// <summary>
        /// Gets the count of General streams.
        /// </summary>
        public int GeneralCount => _generalCount;

        /// <summary>
        /// Gets the count of Video streams.
        /// </summary>
        public int VideoCount => _videoCount;

        /// <summary>
        /// Gets the count of Audio streams.
        /// </summary>
        public int AudioCount => _audioCount;

        /// <summary>
        /// Gets the count of Text streams.
        /// </summary>
        public int TextCount => _textCount;

        /// <summary>
        /// Gets the count of Image streams.
        /// </summary>
        public int ImageCount => _imageCount;

        /// <summary>
        /// Gets the count of Menu streams.
        /// </summary>
        public int MenuCount => _menuCount;

        /// <summary>
        /// List of all the General streams available in the file.
        /// </summary>
        public IReadOnlyList<GeneralTrack> General => _general ?? new List<GeneralTrack>();

        /// <summary>
        /// List of all the Video streams available in the file.
        /// </summary>
        public IReadOnlyList<VideoTrack> Video => _video ?? new List<VideoTrack>();

        /// <summary>
        /// List of all the Audio streams available in the file.
        /// </summary>
        public IReadOnlyList<AudioTrack> Audio => _audio ?? new List<AudioTrack>();

        /// <summary>
        /// List of all the Text streams available in the file.
        /// </summary>
        public IReadOnlyList<TextTrack> Text => _text ?? new List<TextTrack>();

        /// <summary>
        /// List of all the Image streams available in the file.
        /// </summary>
        public IReadOnlyList<ImageTrack> Image => _image ?? new List<ImageTrack>();

        /// <summary>
        /// List of all the Menu streams available in the file.
        /// </summary>
        public IReadOnlyList<MenuTrack> Menu => _menu ?? new List<MenuTrack>();

        /// <summary>
        /// Complete information report from MediaInfo.
        /// </summary>
        public string? InfoComplete => _infoComplete;

        /// <summary>
        /// Standard information report from MediaInfo.
        /// </summary>
        public string? InfoStandard => _infoStandard;

        /// <summary>
        /// Lists all available properties for each track.
        /// </summary>
        public string InfoCustom
        {
            get
            {
                if (!string.IsNullOrEmpty(_infoCustom))
                {
                    return _infoCustom;
                }

                var sb = new StringBuilder();

                sb.AppendLine("General");
                ListEveryAvailablePropery(General, sb);
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine("Video");
                ListEveryAvailablePropery(Video, sb);
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine("Audio");
                ListEveryAvailablePropery(Audio, sb);
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine("Text");
                ListEveryAvailablePropery(Text, sb);
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine("Image");
                ListEveryAvailablePropery(Image, sb);
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine("Menu");
                ListEveryAvailablePropery(Menu, sb);

                return _infoCustom = sb.ToString();
            }
        }

        #endregion

        #region Static Library Information Methods
        /// <summary>
        /// Gets the list of MediaInfo library capacities.
        /// </summary>
        /// <returns>A formatted string containing the library's capabilities.</returns>
        public static string Capacities()
        {
            return IMediaInfoNative.GetLibraryInfo("Info_Capacities");
        }

        /// <summary>
        /// Lists all info parameters available in the MediaInfo library.
        /// </summary>
        /// <param name="csvFormat">If true, returns the list in CSV format; otherwise returns in human-readable format.</param>
        /// <returns>A formatted string containing all available parameters.</returns>
        public static string ParametersList(bool csvFormat = false)
        {
            return IMediaInfoNative.GetLibraryInfo(csvFormat ? "Info_Parameters_CSV" : "Info_Parameters");
        }

        /// <summary>
        /// Gets the list of all codecs supported by the MediaInfo library.
        /// </summary>
        /// <returns>A formatted string containing all supported codecs.</returns>
        public static string KnownCodecs()
        {
            return IMediaInfoNative.GetLibraryInfo("Info_Codecs");
        }

        /// <summary>
        /// Gets the MediaInfo library version.
        /// </summary>
        /// <returns>The version string of the MediaInfo library.</returns>
        public static string LibraryVersion()
        {
            return IMediaInfoNative.GetLibraryInfo("Info_Version");
        }

        /// <summary>
        /// Gets the MediaInfo library URL.
        /// </summary>
        /// <returns>The URL of the MediaInfo library website.</returns>
        public static string LibraryUrl()
        {
            return IMediaInfoNative.GetLibraryInfo("Info_Url");
        }
        #endregion
    }
}

