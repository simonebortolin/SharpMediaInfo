using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Models.Report;
using SharpMediaInfo.Tracks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMediaInfo.GUI.Converters;

/// <summary>
/// Converts MediaInfo tracks to platform-agnostic report sections
/// </summary>
public class TrackToReportConverter
{
    private readonly AppConfiguration _config;

    public TrackToReportConverter(AppConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Converts all tracks to report sections
    /// </summary>
    public List<ReportSection> Convert(MediaInfo mediaInfo)
    {
        var sections = new List<ReportSection>();

        // General section
        if (mediaInfo.General.Count > 0)
        {
            var section = ConvertGeneral(mediaInfo.General[0], mediaInfo);
            if (section.Items.Count > 0)
                sections.Add(section);
        }

        // Video sections
        foreach (var video in mediaInfo.Video)
        {
            var section = ConvertVideo(video);
            if (section.Items.Count > 0)
                sections.Add(section);
        }

        // Audio sections
        foreach (var audio in mediaInfo.Audio)
        {
            var section = ConvertAudio(audio);
            if (section.Items.Count > 0)
                sections.Add(section);
        }

        // Text sections
        foreach (var text in mediaInfo.Text)
        {
            var section = ConvertText(text);
            if (section.Items.Count > 0)
                sections.Add(section);
        }

        // Menu sections
        foreach (var menu in mediaInfo.Menu)
        {
            var section = ConvertMenu(menu);
            if (section.Items.Count > 0)
                sections.Add(section);
        }

        // Image sections
        foreach (var image in mediaInfo.Image)
        {
            var section = ConvertImage(image);
            if (section.Items.Count > 0)
                sections.Add(section);
        }

        return sections;
    }

    private ReportSection ConvertGeneral(GeneralTrack general, MediaInfo mediaInfo)
    {
        var items = new List<ReportItem>();

        if (!string.IsNullOrWhiteSpace(general.FileName) && !string.IsNullOrWhiteSpace(general.FileExtension))
            items.Add(new ReportItem("File Name", $"{general.FileName}.{general.FileExtension}"));

        if (!string.IsNullOrWhiteSpace(general.Format))
            items.Add(new ReportItem("Format", general.Format));

        if (general.FileSize.Bytes.HasValue)
            items.Add(new ReportItem("File Size", general.FileSize.ToString()));

        if (general.Duration != default)
            items.Add(new ReportItem("Duration", FormatDuration(general.Duration.ToString())));

        if (mediaInfo.Video.Count > 0 && mediaInfo.Video[0].FrameCount.HasValue)
            items.Add(new ReportItem("Frame Count", mediaInfo.Video[0].FrameCount.ToString()));

        if (general.OverallBitRate.BitsPerSecond.HasValue)
            items.Add(new ReportItem("Overall Bitrate", general.OverallBitRate.ToString()));

        if (mediaInfo.Video.Count > 0 && !string.IsNullOrEmpty(mediaInfo.Video[0].MultiViewLayout))
            items.Add(new ReportItem("3D Mode", mediaInfo.Video[0].MultiViewLayout));

        if (!string.IsNullOrWhiteSpace(general.Title))
            items.Add(new ReportItem("Title", general.Title));

        if (!string.IsNullOrWhiteSpace(general.Album) && general.Title != general.Album)
            items.Add(new ReportItem("Album", general.Album));

        if (!string.IsNullOrWhiteSpace(general.AlbumPerformer))
            items.Add(new ReportItem("Artist", general.AlbumPerformer));

        if (!string.IsNullOrWhiteSpace(general.Genre))
            items.Add(new ReportItem("Genre", general.Genre));

        if (!string.IsNullOrWhiteSpace(general.EncodedDate))
            items.Add(new ReportItem("Date Encoded", general.EncodedDate));

        if (!string.IsNullOrWhiteSpace(general.EncodedLibrary))
            items.Add(new ReportItem("Created with", general.EncodedLibrary));

        return new ReportSection("General", items);
    }

    private ReportSection ConvertVideo(VideoTrack video)
    {
        var items = new List<ReportItem>();
        var sb = new StringBuilder();

        // Header line
        sb.Append("> ");
        sb.Append(string.IsNullOrEmpty(video.Language.Code) ? "Undetermined" : video.Language.ToString());

        if (!string.IsNullOrWhiteSpace(video.Title))
            sb.Append($" - [{video.Title}]");

        if (video.Duration != default)
            sb.Append($" - {FormatDuration(video.Duration.ToString())} -");
        else
            sb.Append(" -");

        if (video.Width.HasValue && video.Height.HasValue && video.Width > 0 && video.Height > 0)
            sb.Append($" {video.Width}×{video.Height}, ");

        if (!string.IsNullOrWhiteSpace(video.FormatCommercial))
        {
            if (!string.IsNullOrEmpty(video.Format) && !video.FormatCommercial.Equals(video.Format))
                sb.Append($"{video.Format}/");
            
            sb.Append(video.FormatCommercial);

            if (!string.IsNullOrWhiteSpace(video.BitDepthString))
                sb.Append($" {video.BitDepthString}");
        }

        if (!string.IsNullOrWhiteSpace(video.FormatProfile) && !string.IsNullOrWhiteSpace(video.FormatSettings))
            sb.Append($" ({video.FormatProfile}, {video.FormatSettings}), ");
        else if (!string.IsNullOrWhiteSpace(video.FormatProfile))
            sb.Append($" ({video.FormatProfile}), ");
        else if (!string.IsNullOrWhiteSpace(video.FormatSettings))
            sb.Append($" ({video.FormatSettings}), ");
        else
            sb.Append(", ");

        if (video.BitRate.BitsPerSecond.HasValue)
            sb.Append($"{video.BitRate.ToString()}, ");

        if (video.FrameRate.HasValue)
            sb.Append($"{video.FrameRate.Value.Value.Value:F3} fps, ");

        if (video.DisplayAspectRatio.Value.HasValue)
            sb.Append($"{video.DisplayAspectRatio.ToString()}, ");

        if (!string.IsNullOrWhiteSpace(video.ColorSpace))
            sb.Append($"{video.ColorSpace}, ");

        if (!string.IsNullOrWhiteSpace(video.ChromaSubsampling))
            sb.Append($"{video.ChromaSubsampling}, ");

        if (!string.IsNullOrWhiteSpace(video.ColourPrimaries))
            sb.Append($"{video.ColourPrimaries}, ");

        var headerText = sb.ToString().TrimEnd(',', ' ', '-');

        if (video.Delay.HasValue && video.Delay.Value.TotalMilliseconds > 0)
            headerText += $", Delay: {video.Delay.Value.TotalMilliseconds:F0}ms";

        if (video.Default.GetValueOrDefault())
            headerText += ", Default";

        if (video.Forced.GetValueOrDefault())
            headerText += ", Forced";

        // Add summary as first item (will be displayed at top of GroupBox)
        items.Add(new ReportItem("", headerText.TrimEnd(',', ' '), ReportItemType.Normal));

        // HDR metadata
        if (video.HDR)
        {
            items.Add(new ReportItem("HDR metadata", "", ReportItemType.Subheader, 1));

            if (!string.IsNullOrWhiteSpace(video.HDRFormat))
            {
                var hdrText = video.HDRFormat;
                if (!string.IsNullOrWhiteSpace(video.HDRFormatCommercial) && !video.HDRFormat.Equals(video.HDRFormatCommercial))
                    hdrText += $" ({video.HDRFormatCommercial})";
                items.Add(new ReportItem("Format", hdrText, ReportItemType.Normal, 2));
            }

            if (!string.IsNullOrWhiteSpace(video.ColourRange))
                items.Add(new ReportItem("Color range", video.ColourRange, ReportItemType.Normal, 2));

            if (!string.IsNullOrWhiteSpace(video.ColourPrimaries))
                items.Add(new ReportItem("Color primaries", video.ColourPrimaries, ReportItemType.Normal, 2));

            if (!string.IsNullOrWhiteSpace(video.TransferCharacteristics))
                items.Add(new ReportItem("Transfer characteristics", video.TransferCharacteristics, ReportItemType.Normal, 2));

            if (!string.IsNullOrWhiteSpace(video.MatrixCoefficients))
                items.Add(new ReportItem("Matrix coefficients", video.MatrixCoefficients, ReportItemType.Normal, 2));

            if (!string.IsNullOrWhiteSpace(video.MasteringDisplayColorPrimaries))
                items.Add(new ReportItem("Mastering display color primaries", video.MasteringDisplayColorPrimaries, ReportItemType.Normal, 2));

            if (!string.IsNullOrWhiteSpace(video.MasteringDisplayLuminance))
                items.Add(new ReportItem("Mastering display luminance", video.MasteringDisplayLuminance, ReportItemType.Normal, 2));

            if (!string.IsNullOrWhiteSpace(video.MaximumContentLightLevel))
                items.Add(new ReportItem("Maximum content light level", video.MaximumContentLightLevel, ReportItemType.Normal, 2));

            if (!string.IsNullOrWhiteSpace(video.MaximumFrameAverageLightLevel))
                items.Add(new ReportItem("Maximum frame-average light level", video.MaximumFrameAverageLightLevel, ReportItemType.Normal, 2));
        }

        return new ReportSection("Video", items);
    }

    private ReportSection ConvertAudio(AudioTrack audio)
    {
        var items = new List<ReportItem>();
        var sb = new StringBuilder();

        sb.Append("> ");
        sb.Append(string.IsNullOrEmpty(audio.Language.Code) ? "Undetermined" : audio.Language.ToString());

        if (!string.IsNullOrWhiteSpace(audio.Title))
            sb.Append($" - [{audio.Title}]");

        if (audio.Duration != default)
            sb.Append($" - {FormatDuration(audio.Duration.ToString())} - ");
        else
            sb.Append(" - ");

        // Check for subtracks
        int numberOfSubTracks = 0;
        if (!string.IsNullOrWhiteSpace(audio.FormatProfileNormalized))
            numberOfSubTracks = audio.FormatProfileNormalized.Split(new string[] { " / " }, System.StringSplitOptions.None).Length;
        else if (!string.IsNullOrWhiteSpace(audio.FormatNormalized))
            numberOfSubTracks = audio.FormatNormalized.Split(new string[] { " / " }, System.StringSplitOptions.None).Length;

        if (numberOfSubTracks > 1)
        {
            // Multiple subtracks
            for (int j = 0; j < numberOfSubTracks; j++)
            {
                var subtrackText = new StringBuilder();
                if (j > 0) subtrackText.Append("\t");

                if (!string.IsNullOrWhiteSpace(GetSubTrack(audio.FormatNormalized, j)))
                    subtrackText.Append(GetSubTrack(audio.FormatNormalized, j));

                if (!string.IsNullOrWhiteSpace(GetSubTrack(audio.FormatProfileNormalized, j)))
                {
                    if (subtrackText.Length > 0 && !string.IsNullOrWhiteSpace(audio.Format))
                        subtrackText.Append($"-{GetSubTrack(audio.FormatProfileNormalized, j)}");
                    else
                        subtrackText.Append(GetSubTrack(audio.FormatProfileNormalized, j));
                }

                if (!string.IsNullOrWhiteSpace(GetSubTrack(audio.BitRateDescription, j)))
                    subtrackText.Append($", {GetSubTrack(audio.BitRateDescription, j)}");

                if (!string.IsNullOrEmpty(GetSubTrack(audio.BitRateModeDescription, j)))
                    subtrackText.Append($", {GetSubTrack(audio.BitRateModeDescription, j)}");

                if (!string.IsNullOrEmpty(GetSubTrack(audio.ChannelsDescription, j)))
                    subtrackText.Append($", {GetSubTrack(audio.ChannelsDescription, j)}");

                if (!string.IsNullOrEmpty(GetSubTrack(audio.BitDepthDescription, j)))
                    subtrackText.Append($", {GetSubTrack(audio.BitDepthDescription, j)}");

                if (!string.IsNullOrEmpty(GetSubTrack(audio.SamplingRateDescription, j)))
                    subtrackText.Append($", {GetSubTrack(audio.SamplingRateDescription, j)}");

                var text = subtrackText.ToString().TrimEnd(',', ' ');

                if (j == 0)
                {
                    if (audio.Delay.TotalMilliseconds > 0)
                        text += $", Delay: {audio.Delay.TotalMilliseconds:F0}ms";

                    if (audio.Default.GetValueOrDefault())
                        text += ", Default";

                    if (audio.Forced.GetValueOrDefault())
                        text += ", Forced";
                }

                items.Add(new ReportItem($"Audio Track {j + 1}", text.TrimEnd(',', ' '), ReportItemType.Normal, j > 0 ? 1 : 0));
            }
        }
        else
        {
            // Single track
            if (!string.IsNullOrWhiteSpace(audio.FormatNormalized))
                sb.Append($"{audio.FormatNormalized} - ");

            if (!string.IsNullOrWhiteSpace(audio.FormatProfileNormalized))
                sb.Append($"{audio.FormatProfileNormalized}, ");

            if (audio.BitRate.BitsPerSecond.HasValue)
                sb.Append($"{audio.BitRate.ToString()}, ");

            if (!string.IsNullOrWhiteSpace(audio.BitRateMode))
                sb.Append($"{audio.BitRateMode}, ");

            if (audio.Channels.Count.HasValue)
                sb.Append($"{audio.Channels.ToString()}, ");

            if (!string.IsNullOrWhiteSpace(audio.BitDepthString))
                sb.Append($"{audio.BitDepthString}, ");

            if (!string.IsNullOrWhiteSpace(audio.SamplingRateString))
                sb.Append($"{audio.SamplingRateString}");

            var headerText = sb.ToString().TrimEnd(',', ' ');

            if (audio.Delay.TotalMilliseconds > 0)
                headerText += $", Delay: {audio.Delay.TotalMilliseconds:F0}ms";

            if (audio.Default.GetValueOrDefault())
                headerText += ", Default";

            if (audio.Forced.GetValueOrDefault())
                headerText += ", Forced";

            // Add summary as first item (will be displayed at top of GroupBox)
            items.Add(new ReportItem("", headerText.TrimEnd(',', ' '), ReportItemType.Normal));
        }

        return new ReportSection("Audio", items);
    }

    private ReportSection ConvertText(TextTrack text)
    {
        var items = new List<ReportItem>();
        var sb = new StringBuilder();

        sb.Append("> ");
        sb.Append(string.IsNullOrEmpty(text.Language.Code) ? "Undetermined" : text.Language.ToString());

        if (!string.IsNullOrWhiteSpace(text.Title))
            sb.Append($" - [{text.Title}]");

        if (!string.IsNullOrWhiteSpace(text.FormatCommercial))
            sb.Append($" - {text.FormatCommercial}");

        var headerText = sb.ToString();

        if (text.Delay.TotalMilliseconds > 0)
            headerText += $", Delay: {text.Delay.TotalMilliseconds:F0}ms";

        if (text.Default.GetValueOrDefault())
            headerText += ", Default";

        if (text.Forced.GetValueOrDefault())
            headerText += ", Forced";

        items.Add(new ReportItem("Text", headerText.Trim(), ReportItemType.Subheader));

        return new ReportSection("Text", items);
    }

    private ReportSection ConvertMenu(MenuTrack menu)
    {
        var items = new List<ReportItem>();
        var sb = new StringBuilder();

        sb.Append($"> Menu {menu.ID}");
        if (menu.ChaptersCount.HasValue)
            sb.Append($" ({menu.ChaptersCount} chapters)");

        if (!string.IsNullOrWhiteSpace(menu.Language))
            sb.Append($" - {menu.Language}");

        if (!string.IsNullOrWhiteSpace(menu.Title))
            sb.Append($" - [{menu.Title}]");

        var headerText = sb.ToString();

        if (menu.Default.GetValueOrDefault())
            headerText += ", Default";

        if (menu.Forced.GetValueOrDefault())
            headerText += ", Forced";

        items.Add(new ReportItem("Menu", headerText.Trim(), ReportItemType.Subheader));

        // Chapters
        foreach (var chapter in menu.Chapters)
        {
            var chapterText = new StringBuilder();

            if (!ValidateChapterTitle(chapter.Timecode, chapter.Title, ':', '.', ' '))
                chapterText.Append($"{chapter.Title.Trim(':')}: ");

            chapterText.Append(chapter.Timecode);

            if (chapter.Language != ":0" && !string.IsNullOrEmpty(chapter.Language))
                chapterText.Append($" ({chapter.Language})");

            items.Add(new ReportItem($"Chapter {chapter.Index + 1}", chapterText.ToString(), ReportItemType.Normal, 1));
        }

        return new ReportSection("Menu", items);
    }

    private ReportSection ConvertImage(ImageTrack image)
    {
        var items = new List<ReportItem>();
        var sb = new StringBuilder();

        sb.Append("> ");

        if (!string.IsNullOrWhiteSpace(image.Format))
            sb.Append($"{image.Format} (");

        if (!string.IsNullOrWhiteSpace(image.FormatCompression))
            sb.Append($"{image.FormatCompression} ");

        if (!string.IsNullOrWhiteSpace(image.FormatProfile))
            sb.Append(image.FormatProfile);

        var text = sb.ToString().TrimEnd();
        
        if (text.EndsWith("("))
            text = text.Substring(0, text.Length - 2) + " - ";
        else if (!text.EndsWith(">"))
            text += ") - ";
        else
            text += " ";

        if (image.Width.HasValue && image.Height.HasValue && image.Width > 0 && image.Height > 0)
            text += $"{image.Width}×{image.Height}, ";

        if (!string.IsNullOrWhiteSpace(image.CompressionMode))
            text += $"{image.CompressionMode}, ";

        if (!string.IsNullOrWhiteSpace(image.BitDepthString))
            text += $"{image.BitDepthString}, ";

        if (!string.IsNullOrWhiteSpace(image.ColorSpace))
            text += $"{image.ColorSpace}, ";

        if (!string.IsNullOrWhiteSpace(image.ChromaSubsampling))
            text += image.ChromaSubsampling;

        items.Add(new ReportItem("Image", text.TrimEnd(' ', ','), ReportItemType.Subheader));

        return new ReportSection("Image", items);
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

    private bool ValidateChapterTitle(string title, string timecode, params char[] ignoreChars)
    {
        if (title.Length < 3 || timecode.Length < 3) return false;

        foreach (var c in ignoreChars)
        {
            title = title.Replace(c.ToString(), "--");
            timecode = timecode.Replace(c.ToString(), "--");
        }

        return title.Contains(timecode) || timecode.Contains(title);
    }
}
