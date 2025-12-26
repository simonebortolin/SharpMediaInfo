using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMediaInfo.GUI.Avalonia;

public class CommandLineParser
{
    public class ParsedArguments
    {
        public (int X, int Y)? WindowPosition { get; set; }
        public string[] Files { get; set; } = Array.Empty<string>();
        public bool HasErrors { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public static ParsedArguments Parse(string[] args)
    {
        var result = new ParsedArguments();
        var files = new List<string>();

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg == "--window-position" || arg == "-p")
            {
                if (i + 1 >= args.Length)
                {
                    result.Errors.Add($"Missing value for {arg}");
                    result.HasErrors = true;
                    i++;
                    continue;
                }

                var posValue = args[i + 1];
                var coords = posValue.Split(',');

                if (coords.Length != 2)
                {
                    result.Errors.Add($"Invalid window position format: '{posValue}'. Expected: X,Y");
                    result.HasErrors = true;
                }
                else if (!int.TryParse(coords[0], out int x))
                {
                    result.Errors.Add($"Invalid X coordinate: '{coords[0]}'");
                    result.HasErrors = true;
                }
                else if (!int.TryParse(coords[1], out int y))
                {
                    result.Errors.Add($"Invalid Y coordinate: '{coords[1]}'");
                    result.HasErrors = true;
                }
                else
                {
                    result.WindowPosition = (x, y);
                }

                i++;
            }
            else if (!arg.StartsWith("-"))
            {
                if (File.Exists(arg))
                {
                    files.Add(arg);
                }
                else
                {
                    result.Errors.Add($"File not found: '{arg}'");
                }
            }
            else
            {
                result.Errors.Add($"Unknown option: '{arg}'");
                result.HasErrors = true;
            }
        }

        result.Files = files.ToArray();
        return result;
    }

    public static bool HasWindowPosition(string[] args)
    {
        return args.Contains("--window-position") || args.Contains("-p");
    }
}
