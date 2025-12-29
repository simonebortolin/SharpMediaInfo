using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Models.Report;
using SharpMediaInfo.Tracks;
using System.Collections.Generic;
using System.Linq;

namespace SharpMediaInfo.GUI.Converters;

/// <summary>
/// Converts MediaInfo tracks to platform-agnostic tree structure
/// </summary>
public class TrackToTreeConverter
{
    private readonly AppConfiguration _config;

    public TrackToTreeConverter(AppConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Converts all tracks to tree nodes
    /// </summary>
    public List<TreeNode> Convert(IMediaInfo mediaInfo)
    {
        var nodes = new List<TreeNode>();

        // General
        if (mediaInfo.General.Count > 0)
        {
            var node = ConvertGeneral(mediaInfo.General[0], mediaInfo);
            if (node.Children.Count > 0)
                nodes.Add(node);
        }

        // Videos
        int videoIndex = 0;
        foreach (var video in mediaInfo.Video)
        {
            var header = mediaInfo.Video.Count == 1 ? "Video" : $"Video #{++videoIndex}";
            var node = ConvertVideo(video, header);
            if (node.Children.Count > 0)
                nodes.Add(node);
        }

        // Audio
        int audioIndex = 0;
        foreach (var audio in mediaInfo.Audio)
        {
            var header = mediaInfo.Audio.Count == 1 ? "Audio" : $"Audio #{++audioIndex}";
            var node = ConvertAudio(audio, header);
            if (node.Children.Count > 0)
                nodes.Add(node);
        }

        // Text
        int textIndex = 0;
        foreach (var text in mediaInfo.Text)
        {
            var header = mediaInfo.Text.Count == 1 ? "Text" : $"Text #{++textIndex}";
            var node = ConvertText(text, header);
            if (node.Children.Count > 0)
                nodes.Add(node);
        }

        // Menu
        int menuIndex = 0;
        foreach (var menu in mediaInfo.Menu)
        {
            var header = mediaInfo.Menu.Count == 1 ? "Menu" : $"Menu #{++menuIndex}";
            var node = ConvertMenu(menu, header);
            if (node.Children.Count > 0)
                nodes.Add(node);
        }

        // Image
        int imageIndex = 0;
        foreach (var image in mediaInfo.Image)
        {
            var header = mediaInfo.Image.Count == 1 ? "Image" : $"Image #{++imageIndex}";
            var node = ConvertImage(image, header);
            if (node.Children.Count > 0)
                nodes.Add(node);
        }

        return nodes;
    }

    private TreeNode ConvertGeneral(GeneralTrack general, IMediaInfo mediaInfo)
    {
        var children = new List<TreeNode>();

        if (!string.IsNullOrWhiteSpace(general.FileName) && !string.IsNullOrWhiteSpace(general.FileExtension))
            children.Add(new TreeNode($"File Name: {general.FileName}.{general.FileExtension}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(general.Format))
            children.Add(new TreeNode($"Format: {general.Format}") { Type = TreeNodeType.Property });

        if (general.FileSize.Bytes.HasValue)
            children.Add(new TreeNode($"File Size: {general.FileSize}") { Type = TreeNodeType.Property });

        if (general.Duration != default)
            children.Add(new TreeNode($"Duration: {FormatDuration(general.Duration.ToString())}") { Type = TreeNodeType.Property });

        if (mediaInfo.Video.Count > 0 && mediaInfo.Video[0].FrameCount.HasValue && mediaInfo.Video[0].FrameCount > 0)
            children.Add(new TreeNode($"Frame Count: {mediaInfo.Video[0].FrameCount}") { Type = TreeNodeType.Property });

        if (general.OverallBitRate.BitsPerSecond.HasValue)
            children.Add(new TreeNode($"Overall BitRate: {general.OverallBitRate}") { Type = TreeNodeType.Property });

        if (mediaInfo.Video.Count > 0 && !string.IsNullOrEmpty(mediaInfo.Video[0].MultiViewLayout))
            children.Add(new TreeNode($"3D Mode: {mediaInfo.Video[0].MultiViewLayout}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(general.Title))
            children.Add(new TreeNode($"Title: {general.Title}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(general.Album) && general.Title != general.Album)
            children.Add(new TreeNode($"Album: {general.Album}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(general.AlbumPerformer))
            children.Add(new TreeNode($"Artist: {general.AlbumPerformer}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(general.Genre))
            children.Add(new TreeNode($"Genre: {general.Genre}") { Type = TreeNodeType.Property });

        children.Add(new TreeNode($"Cover: {(general.Cover.GetValueOrDefault() ? "Yes" : "No")}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(general.EncodedDate))
            children.Add(new TreeNode($"Date Encoded: {general.EncodedDate}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(general.EncodedLibrary))
            children.Add(new TreeNode($"Created with: {general.EncodedLibrary}") { Type = TreeNodeType.Property });

        return new TreeNode("General", children) { Type = TreeNodeType.Section, IsExpanded = true };
    }

    private TreeNode ConvertVideo(VideoTrack video, string header)
    {
        var children = new List<TreeNode>();

        var language = string.IsNullOrEmpty(video.Language.Code) ? "Undetermined Language" : $"Language: {video.Language}";
        children.Add(new TreeNode(language) { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(video.Title))
            children.Add(new TreeNode($"Track Title: {video.Title}") { Type = TreeNodeType.Property });

        if (video.Width.HasValue && video.Height.HasValue && video.Width > 0 && video.Height > 0)
            children.Add(new TreeNode($"Resolution: {video.Width}×{video.Height}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(video.FormatCommercial))
        {
            var codecText = video.FormatCommercial;
            if (!string.IsNullOrEmpty(video.Format) && !video.FormatCommercial.Equals(video.Format))
                codecText = $"{video.Format}/{codecText}";
            
            if (!string.IsNullOrWhiteSpace(video.BitDepthString))
                codecText += $" {video.BitDepthString}";

            children.Add(new TreeNode($"Codec: {codecText}") { Type = TreeNodeType.Property });
        }

        if (!string.IsNullOrWhiteSpace(video.FormatProfile))
            children.Add(new TreeNode($"Format Profile: {video.FormatProfile}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(video.FormatSettings))
            children.Add(new TreeNode($"Format Setting: {video.FormatSettings}") { Type = TreeNodeType.Property });

        if (video.BitRate.BitsPerSecond.HasValue)
            children.Add(new TreeNode($"BitRate: {video.BitRate}") { Type = TreeNodeType.Property });

        if (video.FrameRate.HasValue)
            children.Add(new TreeNode($"Framerate: {video.FrameRate} fps") { Type = TreeNodeType.Property });

        if (video.DisplayAspectRatio.Value.HasValue)
            children.Add(new TreeNode($"Aspect Ratio: {video.DisplayAspectRatio}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(video.ColorSpace))
            children.Add(new TreeNode($"Color Space: {video.ColorSpace}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(video.ChromaSubsampling))
            children.Add(new TreeNode($"Chroma Subsampling: {video.ChromaSubsampling}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(video.ColourPrimaries))
            children.Add(new TreeNode($"Color Primaries: {video.ColourPrimaries}") { Type = TreeNodeType.Property });

        if (video.Delay.HasValue && video.Delay.Value.TotalMilliseconds > 0)
            children.Add(new TreeNode($"Delay: {video.Delay.Value.TotalMilliseconds:F0}ms") { Type = TreeNodeType.Property });

        if (video.Default.HasValue)
            children.Add(new TreeNode($"Default: {(video.Default.GetValueOrDefault() ? "Yes" : "No")}") { Type = TreeNodeType.Property });

        if (video.Forced.HasValue)
            children.Add(new TreeNode($"Forced: {(video.Forced.GetValueOrDefault() ? "Yes" : "No")}") { Type = TreeNodeType.Property });

        // HDR metadata
        if (video.HDR)
        {
            var hdrChildren = new List<TreeNode>();

            if (!string.IsNullOrWhiteSpace(video.HDRFormat))
            {
                var hdrText = $"Format: {video.HDRFormat}";
                if (!string.IsNullOrWhiteSpace(video.HDRFormatCommercial) && !video.HDRFormat.Equals(video.HDRFormatCommercial))
                    hdrText += $" ({video.HDRFormatCommercial})";
                hdrChildren.Add(new TreeNode(hdrText) { Type = TreeNodeType.Property });
            }

            if (!string.IsNullOrWhiteSpace(video.ColourRange))
                hdrChildren.Add(new TreeNode($"Color range: {video.ColourRange}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrWhiteSpace(video.ColourPrimaries))
                hdrChildren.Add(new TreeNode($"Color primaries: {video.ColourPrimaries}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrWhiteSpace(video.TransferCharacteristics))
                hdrChildren.Add(new TreeNode($"Transfer characteristics: {video.TransferCharacteristics}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrWhiteSpace(video.MatrixCoefficients))
                hdrChildren.Add(new TreeNode($"Matrix coefficients: {video.MatrixCoefficients}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrWhiteSpace(video.MasteringDisplayColorPrimaries))
                hdrChildren.Add(new TreeNode($"Mastering display color primaries: {video.MasteringDisplayColorPrimaries}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrWhiteSpace(video.MasteringDisplayLuminance))
                hdrChildren.Add(new TreeNode($"Mastering display luminance: {video.MasteringDisplayLuminance}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrWhiteSpace(video.MaximumContentLightLevel))
                hdrChildren.Add(new TreeNode($"Maximum content light level: {video.MaximumContentLightLevel}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrWhiteSpace(video.MaximumFrameAverageLightLevel))
                hdrChildren.Add(new TreeNode($"Maximum frame-average light level: {video.MaximumFrameAverageLightLevel}") { Type = TreeNodeType.Property });

            if (hdrChildren.Count > 0)
                children.Add(new TreeNode("HDR metadata", hdrChildren) { Type = TreeNodeType.Section });
        }

        return new TreeNode(header, children) { Type = TreeNodeType.Track, IsExpanded = true };
    }

    private TreeNode ConvertAudio(AudioTrack audio, string header)
    {
        var children = new List<TreeNode>();

        var language = string.IsNullOrEmpty(audio.Language.Code) ? "Undetermined Language" : $"Language: {audio.Language}";
        children.Add(new TreeNode(language) { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(audio.Title))
            children.Add(new TreeNode($"Track Title: {audio.Title}") { Type = TreeNodeType.Property });

        if (audio.Duration != default)
            children.Add(new TreeNode($"Duration: {FormatDuration(audio.Duration.ToString())}") { Type = TreeNodeType.Property });

        // Check for subtracks
        int numberOfSubTracks = 0;
        if (!string.IsNullOrWhiteSpace(audio.FormatProfileNormalized))
            numberOfSubTracks = audio.FormatProfileNormalized.Split(new string[] { " / " }, System.StringSplitOptions.None).Length;
        else if (!string.IsNullOrWhiteSpace(audio.FormatNormalized))
            numberOfSubTracks = audio.FormatNormalized.Split(new string[] { " / " }, System.StringSplitOptions.None).Length;

        if (numberOfSubTracks > 1)
        {
            // Multiple subtracks
            for (int i = 0; i < numberOfSubTracks; i++)
            {
                var subtrackChildren = new List<TreeNode>();

                var format = GetSubTrack(audio.Format, i);
                var formatProfile = GetSubTrack(audio.FormatProfile, i);
                var subtrackHeader = !string.IsNullOrWhiteSpace(format) && !string.IsNullOrWhiteSpace(formatProfile) 
                    ? $"{format}-{formatProfile}" 
                    : !string.IsNullOrWhiteSpace(format) ? format 
                    : !string.IsNullOrWhiteSpace(formatProfile) ? formatProfile 
                    : $"Subtrack {i + 1}";

                var bitrateMode = GetSubTrack(audio.BitRateModeDescription, i);
                var bitrate = GetSubTrack(audio.BitRateDescription, i);

                if (!string.IsNullOrWhiteSpace(bitrate))
                {
                    var bitrateText = bitrateMode.Contains("CBR") ? $"BitRate: {bitrate}" : 
                        audio.BitRateMaximum.BitsPerSecond.HasValue ? $"BitRate: {audio.BitRateMaximum}" : $"BitRate: {bitrate}";
                    subtrackChildren.Add(new TreeNode(bitrateText) { Type = TreeNodeType.Property });
                }

                if (!string.IsNullOrWhiteSpace(bitrateMode))
                    subtrackChildren.Add(new TreeNode($"BitRate Mode: {bitrateMode}") { Type = TreeNodeType.Property });

                var channels = GetSubTrack(audio.ChannelsDescription, i);
                if (!string.IsNullOrWhiteSpace(channels))
                    subtrackChildren.Add(new TreeNode($"Channel: {channels}") { Type = TreeNodeType.Property });

                var bitDepth = GetSubTrack(audio.BitDepthDescription, i);
                if (!string.IsNullOrWhiteSpace(bitDepth))
                    subtrackChildren.Add(new TreeNode($"Bit Depth: {bitDepth}") { Type = TreeNodeType.Property });

                var samplingRate = GetSubTrack(audio.SamplingRateDescription, i);
                if (!string.IsNullOrWhiteSpace(samplingRate))
                    subtrackChildren.Add(new TreeNode($"Sampling Rate: {samplingRate}") { Type = TreeNodeType.Property });

                children.Add(new TreeNode(subtrackHeader, subtrackChildren) { Type = TreeNodeType.Section });
            }
        }
        else
        {
            // Single track
            if (!string.IsNullOrWhiteSpace(audio.FormatNormalized))
                children.Add(new TreeNode($"Format: {audio.FormatNormalized.Trim()}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrEmpty(audio.FormatProfileNormalized))
                children.Add(new TreeNode($"Profile: {audio.FormatProfileNormalized}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrEmpty(audio.BitRateDescription))
                children.Add(new TreeNode($"BitRate: {audio.BitRateDescription}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrEmpty(audio.BitRateModeDescription))
                children.Add(new TreeNode($"BitRate Mode: {audio.BitRateModeDescription}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrEmpty(audio.ChannelsDescription))
                children.Add(new TreeNode($"Channels: {audio.ChannelsDescription}") { Type = TreeNodeType.Property });

            if (!string.IsNullOrEmpty(audio.SamplingRateDescription))
                children.Add(new TreeNode($"Sampling Rate: {audio.SamplingRateDescription}") { Type = TreeNodeType.Property });
        }

        if (audio.Delay.TotalMilliseconds > 0)
            children.Add(new TreeNode($"Delay: {audio.Delay.TotalMilliseconds:F0}ms") { Type = TreeNodeType.Property });

        if (!string.IsNullOrEmpty(audio.DefaultString))
            children.Add(new TreeNode($"Default: {audio.DefaultString}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrEmpty(audio.ForcedString))
            children.Add(new TreeNode($"Forced: {audio.ForcedString}") { Type = TreeNodeType.Property });

        return new TreeNode(header, children) { Type = TreeNodeType.Track, IsExpanded = true };
    }

    private TreeNode ConvertText(TextTrack text, string header)
    {
        var children = new List<TreeNode>();

        var language = string.IsNullOrEmpty(text.Language.Code) ? "Undetermined Language" : $"Language: {text.Language}";
        children.Add(new TreeNode(language) { Type = TreeNodeType.Property });

        if (!string.IsNullOrEmpty(text.Title))
            children.Add(new TreeNode($"Track Title: {text.Title}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(text.FormatCommercial))
            children.Add(new TreeNode($"Codec: {text.FormatCommercial}") { Type = TreeNodeType.Property });

        return new TreeNode(header, children) { Type = TreeNodeType.Track, IsExpanded = true };
    }

    private TreeNode ConvertMenu(MenuTrack menu, string header)
    {
        var children = new List<TreeNode>();

        if (menu.ChaptersCount.HasValue)
            children.Add(new TreeNode($"Chapters Count: {menu.ChaptersCount}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(menu.Language))
            children.Add(new TreeNode($"Language: {menu.Language}") { Type = TreeNodeType.Property });

        children.Add(new TreeNode($"Default: {(menu.Default.GetValueOrDefault() ? "Yes" : "No")}") { Type = TreeNodeType.Property });
        children.Add(new TreeNode($"Forced: {(menu.Forced.GetValueOrDefault() ? "Yes" : "No")}") { Type = TreeNodeType.Property });

        // Chapters
        if (menu.Chapters.Count > 0)
        {
            var chaptersNode = menu.Chapters.Count == 1 
                ? new TreeNode("Chapter") { Type = TreeNodeType.Section }
                : new TreeNode("Chapters") { Type = TreeNodeType.Section };

            foreach (var chapter in menu.Chapters)
            {
                var chapterChildren = new List<TreeNode>();

                if (!ValidateChapterTitle(chapter.Timecode, chapter.Title, ':', '.', ' '))
                    chapterChildren.Add(new TreeNode($"Title: {chapter.Title!.Trim(':')}") { Type = TreeNodeType.Property });

                chapterChildren.Add(new TreeNode($"Time: {chapter.Timecode}") { Type = TreeNodeType.Property });

                var chapterLang = (chapter.Language != ":0" && !string.IsNullOrEmpty(chapter.Language)) 
                    ? $"Language: {chapter.Language}" 
                    : "Language: Undetermined";
                chapterChildren.Add(new TreeNode(chapterLang) { Type = TreeNodeType.Property });

                var chapterHeader = menu.Chapters.Count == 1 ? "Chapter" : $"Chapter #{chapter.Index + 1}";
                chaptersNode.Children.Add(new TreeNode(chapterHeader, chapterChildren) { Type = TreeNodeType.Section });
            }

            children.Add(chaptersNode);
        }

        return new TreeNode(header, children) { Type = TreeNodeType.Track, IsExpanded = true };
    }

    private TreeNode ConvertImage(ImageTrack image, string header)
    {
        var children = new List<TreeNode>();

        if (!string.IsNullOrWhiteSpace(image.Format))
            children.Add(new TreeNode($"Format: {image.Format}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(image.FormatCompression))
            children.Add(new TreeNode($"Format compression: {image.FormatCompression}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(image.FormatProfile))
            children.Add(new TreeNode($"Format profile: {image.FormatProfile}") { Type = TreeNodeType.Property });

        if (image.Width.HasValue && image.Height.HasValue && image.Width > 0 && image.Height > 0)
            children.Add(new TreeNode($"Resolution: {image.Width}×{image.Height}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(image.CompressionMode))
            children.Add(new TreeNode($"Compression mode: {image.CompressionMode}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(image.BitDepthString))
            children.Add(new TreeNode($"Bit depth: {image.BitDepthString}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(image.ColorSpace))
            children.Add(new TreeNode($"Color space: {image.ColorSpace}") { Type = TreeNodeType.Property });

        if (!string.IsNullOrWhiteSpace(image.ChromaSubsampling))
            children.Add(new TreeNode($"Chroma subsampling: {image.ChromaSubsampling}") { Type = TreeNodeType.Property });

        return new TreeNode(header, children) { Type = TreeNodeType.Track, IsExpanded = true };
    }

    private string FormatDuration(string duration)
    {
        if (!_config.ShowMilliseconds && duration.Contains('.'))
        {
            return duration.Split('.')[0];
        }
        return duration;
    }

    private string GetSubTrack(string? str, int index)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;

        if (str.Contains(" / "))
        {
            var subTracks = str.Split(new string[] { " / " }, System.StringSplitOptions.None);
            if (subTracks.Length > index && !string.IsNullOrWhiteSpace(subTracks[index]))
            {
                var track = subTracks[index].Trim();
                if (track.Contains("Core"))
                    return string.Empty;
                if (track.Contains("HR") || track.Contains("MA"))
                    return $"HD {track}";
                return track;
            }
            return subTracks.LastOrDefault()?.Trim() ?? string.Empty;
        }

        return str;
    }

    private bool ValidateChapterTitle(string? title, string? timecode, params char[] ignoreChars)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(timecode))
            return false;
        if (title.Length < 3 || timecode.Length < 3) return false;

        foreach (var c in ignoreChars)
        {
            title = title.Replace(c.ToString(), "--");
            timecode = timecode.Replace(c.ToString(), "--");
        }

        return title.Contains(timecode) || timecode.Contains(title);
    }
}
