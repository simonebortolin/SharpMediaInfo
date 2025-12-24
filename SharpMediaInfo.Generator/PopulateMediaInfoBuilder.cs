using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMediaInfo.Generator
{
    public class PopulateMethodGenerator
    {
        public static string GeneratePopulateMethod(string className, IEnumerable<TrackFieldInfo> fields)
        {
            var track = className.Replace("Track", "");
            var sb = new StringBuilder();

            sb.AppendLine( "using System;");
            sb.AppendLine( "using System.Numerics;");
            sb.AppendLine( "using SharpMediaInfo.Tracks;");
            sb.AppendLine( "using SharpMediaInfo.Native;");
            sb.AppendLine( "using SharpMediaInfo.Native.Enum;");
            sb.AppendLine( "using SharpMediaInfo.Types;");
            sb.AppendLine();
            sb.AppendLine( "namespace SharpMediaInfo");
            sb.AppendLine( "{");
            sb.AppendLine($"  partial class MediaInfoBuilder");
            sb.AppendLine( "  {");
            sb.AppendLine($"    private {className} Populate{className}(IMediaInfoNative mediaInfoNative, int index)");
            sb.AppendLine( "    {");
            sb.AppendLine($"      return new {className}");
            sb.AppendLine( "      {");

            foreach (var f in fields)
            {
                if (f.Skip) continue;

                if (f is CombinedTrackFieldInfo ctfi)
                {
                    sb.AppendLine($"        {f.Name} = {f.ParseFunction}(");
                    for (int i = 0; i<  ctfi.TrackFieldInfos.Length; i++)
                    {
                        sb.Append($"          {GenerateFunctionCall(track, ctfi.TrackFieldInfos[i])}");
                        if(i == ctfi.TrackFieldInfos.Length - 1)
                        {
                            sb.AppendLine();
                        }
                        else
                        {
                            sb.AppendLine(",");
                        }
                    }
                    sb.AppendLine($"        ),");
                }
                else
                {
                    sb.AppendLine($"        {f.Name} = {GenerateFunctionCall(track, f)},");
                }
            }

            sb.AppendLine("      };");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string GenerateFunctionCall(string track, TrackFieldInfo tfi) => tfi.ParseFunction == null ? $"mediaInfoNative.GetParameter(StreamKind.{track},index, \"{tfi.CsvName}\")" : $"{tfi.ParseFunction}(mediaInfoNative.GetParameter(StreamKind.{track},index, \"{tfi.CsvName}\"))";
    }

}
