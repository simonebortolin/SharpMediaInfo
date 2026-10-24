using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpMediaInfo.Generator
{
    public static class TrackFieldInfoParser
    {
        public static List<TrackFieldInfo> ParseCsv(string csvFile)
        {
            var fields = new List<TrackFieldInfo>();
            var lines = csvFile.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                var columns = lines[i].Split(';'); // MediaInfo CSV are separated by ';'
                if (columns.Length < 7) continue;

                var field = new TrackFieldInfo(
                    csvName: columns[0].Trim(),
                    name: CreateSharpName(columns[0].Trim()),
                    type: MapType(columns[3].Trim()),
                    description: columns[6].Trim(),
                    uoM: columns[2].Trim(),
                    parseFunction: MapParseType(columns[3]),
                    deprecated: columns[6].Trim().StartsWith("Deprecated")
                );
                fields.Add(field);
            }

            return fields;
        }

        private static string CreateSharpName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return string.Empty;
            }

            // Use StringInfo to properly handle non-BMP Unicode characters (surrogate pairs)
            var textElements = GetTextElements(rawName);
            var tokens = new List<string>();
            var current = new StringBuilder();

            for (int idx = 0; idx < textElements.Count; idx++)
            {
                var element = textElements[idx];

                // Check if element contains only letters or digits (works with non-BMP characters)
                bool isLetterOrDigit = element.All(char.IsLetterOrDigit);

                if (isLetterOrDigit)
                {
                    // Split before uppercase letter if previous char is lowercase
                    if (current.Length > 0 && 
                        char.IsUpper(element[0]) && 
                        char.IsLower(current[current.Length - 1]))
                    {
                        tokens.Add(current.ToString());
                        current.Clear();
                    }
                    current.Append(element);
                }
                else
                {
                    if (current.Length > 0)
                    {
                        tokens.Add(current.ToString());
                        current.Clear();
                    }
                    // Special handling for decimal point: add "Dot" as a token only if surrounded by digits
                    if (element == "." && 
                        tokens.Count > 0 && 
                        tokens[tokens.Count - 1].All(StringExtensions.IsDigitOrMinus) &&
                        idx + 1 < textElements.Count && 
                        textElements[idx + 1].All(StringExtensions.IsDigitOrMinus))
                    {
                        tokens.Add("Dot");
                    }

                    if (element == "-" &&
                        idx + 1 < textElements.Count &&
                        textElements[idx + 1].All(char.IsDigit))
                    {
                        current.Append(element);
                    }
                }
            }

            if (current.Length > 0)
            {
                tokens.Add(current.ToString());
            }

            if (tokens.Count == 0)
            {
                return string.Empty;
            }

            var first = tokens[0];

            if (StringExtensions.IsDigitOrMinus(first[0]))
            {
                int i = 1;
                // Case: Purely numeric token (e.g. "10")
                if (first.All(StringExtensions.IsDigitOrMinus))
                {
                    var numberPart = NumberToIdentifier.FromNumber(int.Parse(first));
                    tokens[0] = numberPart[0].ToTitleCase();
                    for (; i< numberPart.Length; i++)
                    {
                        tokens.Insert(i, numberPart[i].ToTitleCase());
                    }
                }
                else
                {
                    // Case: 3D, 10bit
                    var match = Regex.Match(first, @"^(-?\d+)(.*)");
                    if (match.Success)
                    {
                        var numberPart = NumberToIdentifier.FromNumber(int.Parse(match.Groups[1].Value));

                        tokens[0] = numberPart[0].ToTitleCase();
                        for (int j = 1; j < numberPart.Length; j++, i++)
                        {
                            tokens.Insert(i, numberPart[j].ToTitleCase());
                        }
                        tokens.Insert(i, match.Groups[2].Value);
                    }
                }
                for (; i < tokens.Count; i++)
                {
                    if (tokens[i].Any(StringExtensions.IsDigitOrMinus))
                    {
                        var number = int.Parse(tokens[i]); 
                        var numberPart = NumberToIdentifier.FromNumber(number);

                        tokens[i] = numberPart[0].ToTitleCase();
                        for (int j = 1; j < numberPart.Length; j++, i++)
                        {
                            tokens.Insert(i, numberPart[j].ToTitleCase());
                        }
                        continue;
                    }
                    if (tokens[i] == "Dot" || tokens[i] == "Minus")
                    {
                        continue;
                    }
                    break;
                }
            }

            // PascalCase + acronym management
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                //  acronym(all capital letters and > 1 long)
                if (token.Length > 1 && token.All(char.IsUpper))
                {
                    tokens[i] = token;
                }
                else
                {
                    tokens[i] = token.ToTitleCase();
                }
            }

            var result = string.Concat(tokens);

            // Final safety: cannot start with a digit
            if (char.IsDigit(result[0]))
                result = "Value" + result;

            return result;
        }

        private static string MapType(string csvType)
        {
            // https://stackoverflow.com/a/79216294
            var nullable = csvType[0] == 'N';
            string type = "string";
            switch (csvType[3])
            {
                case 'I':
                    type = "long";
                    break;
                case 'F':
                    type = "double";
                    break;
                case 'T':
                    type = "string";
                    break;
                case 'D':
                    type = "DateTimeOffset";
                    break;
                case 'C':
                    type = "TimeSpan";
                    break;
                case 'B':
                    type = "string";
                    break;
            }

            return nullable && type != "string" ? $"{type}?" : type;
        }

        private static string? MapParseType(string csvType)
        {
            // https://stackoverflow.com/a/79216294
            string? type = null;
            switch (csvType[3])
            {
                case 'I':
                    type = "GetLongValue";
                    break;
                case 'F':
                    type = "GetDoubleValue";
                    break;
                case 'D':
                    type = "GetDateTimeOffsetValue";
                    break;
                case 'C':
                    type = "GetTimeSpanValue";
                    break;
            }

            if (csvType[0] == 'N')
            {
                type = type is null ? null : $"{type}Nullable";
            }

            return type;
        }

        /// <summary>
        /// Extracts text elements from a string while properly handling non-BMP Unicode characters (surrogate pairs).
        /// Uses StringInfo to ensure each element represents a complete grapheme cluster.
        /// </summary>
        private static List<string> GetTextElements(string input)
        {
            var elements = new List<string>();
            var enumerator = StringInfo.GetTextElementEnumerator(input);

            while (enumerator.MoveNext())
            {
                elements.Add((string)enumerator.Current);
            }

            return elements;
        }

    }

    internal static class NumberToIdentifier
    {
        private static readonly string[] Units =
        {
            "Zero", "One", "Two", "Three", "Four", "Five",
            "Six", "Seven", "Eight", "Nine", "Ten",
            "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen",
            "Sixteen", "Seventeen", "Eighteen", "Nineteen"
        };

        private static readonly string[] Tens =
        {
            "", "", "Twenty", "Thirty", "Forty",
            "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
        };

        public static string[] FromNumber(int number)
        {
            if (number < 0)
                return ["Minus", .. FromNumber(-number)];

            if (number < 20)
                return [Units[number]];

            if (number < 100)
                return [Tens[number / 10] +
                       (number % 10 == 0 ? "" : Units[number % 10])];

            if (number < 1000)
                return [Units[number / 100],  "Hundred", ..(number % 100 == 0 ? [""] : FromNumber(number % 100))];

            return ["N", number.ToString(CultureInfo.InvariantCulture)];
        }
    }

    internal static class StringExtensions
    {
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var sb = new StringBuilder(input.Length);
            bool capitalizeNext = true;

            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(capitalizeNext ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c));
                    capitalizeNext = false;
                }
                else
                {
                    sb.Append(c);
                    capitalizeNext = true; 
                }
            }

            return sb.ToString();
        }





        public static bool IsDigitOrMinus(this char c) => char.IsDigit(c) || c == '-';
    }
}
