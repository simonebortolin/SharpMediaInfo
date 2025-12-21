using SharpMediaInfo.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMediaInfo
{
    public interface IMediaInfo
    {
        bool InfoAvailable();
        IReadOnlyList<GeneralTrack> General { get; }
        IReadOnlyList<VideoTrack> Video { get; }
        IReadOnlyList<AudioTrack> Audio { get; }
        IReadOnlyList<TextTrack> Text { get; }
        IReadOnlyList<ImageTrack> Image { get; }
        IReadOnlyList<MenuTrack> Menu { get; }

        string? InfoComplete { get; }

        string? InfoStandard { get; }

        string InfoCustom { get; }
    }
}
