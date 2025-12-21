using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMediaInfo.Tracks
{
    public partial class MenuTrack
    {
        internal List<ChapterTrack>? ChaptersInternal { get; set; }
        public IReadOnlyList<ChapterTrack>? Chapters => ChaptersInternal;
        public int? ChaptersCount => ChaptersInternal?.Count;
    }
}
