using System.Reflection;
using System.Text;

namespace SharpMediaInfo.Generator
{
    public static class TrackCodeBuilder
    {
        public static string GenerateClass(string className, IEnumerable<TrackFieldInfo> fields)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Numerics;");
            sb.AppendLine("using SharpMediaInfo.Types;");
            sb.AppendLine();
            sb.AppendLine("namespace SharpMediaInfo.Tracks");
            sb.AppendLine("{");
            sb.AppendLine($"    public partial class {className}");
            sb.AppendLine("    {");


            foreach (var f in fields)
            {
                sb.AppendLine("        /// <summary>");
                sb.AppendLine($"        /// {f.Description}");
                sb.AppendLine("        /// </summary>");
                if (f.Deprecated)
                {
                    sb.AppendLine("        [Obsolete(\"This field is deprecated and may be removed in future versions.\")]");
                }
                sb.AppendLine($"        {f.Modifier} {f.Type} {f.Name} {{ get; init; }} //{f.CsvName}");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
