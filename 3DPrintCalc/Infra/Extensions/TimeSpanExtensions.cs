using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class TimeSpanExtensions
    {
        private static KeyValuePair<string, double>[] TIME_UNITS;

        public static TimeSpan? TryParseNatural(string input)
        {
            var time = new TimeSpan(0);
            foreach (var segment in input.Split("+"))
                try
                {
                    var parsed = string.Join(string.Empty, segment.Where(char.IsLetterOrDigit));
                    var parts = Regex.Matches(parsed, @"(\d*)(d|h|m|s)");
                    if (parts.Count == 0)
                        return null;

                    foreach (Match part in parts)
                        time += (part.Groups[2].Value switch
                        {
                            "d" => (Func<int, TimeSpan>)(i => TimeSpan.FromDays(i)),
                            "h" => i => TimeSpan.FromHours(i),
                            "m" => i => TimeSpan.FromMinutes(i),
                            "s" => i => TimeSpan.FromSeconds(i),
                            _ => throw new Exception()
                        }).Invoke(int.Parse(part.Groups[1].Value));
                }
                catch (Exception)
                {
                    return null;
                }
            return time;
        }

        public static string Format(this TimeSpan span)
        {
            if (TIME_UNITS == null)
            {
                TIME_UNITS = new Dictionary<string, double>
                {
                    { "s", 1 },
                    { "m", 60 },
                    { "h", 60 },
                    { "d", 24 },
                    { "w", 7 },
                    { "mo", (365.25/12)/7 },
                    { "y", 12 },
                    { "dec", 10 },
                    { "c", 10 }
                }.ToArray();
                for (var i = 1; i < TIME_UNITS.Length; i++)
                    TIME_UNITS[i] = new KeyValuePair<string, double>(TIME_UNITS[i].Key, TIME_UNITS[i].Value * TIME_UNITS[i - 1].Value);
            }

            var seconds = span.TotalSeconds;

            if (seconds < 0)
                return $"-({span.Multiply(-1).Format()})";

            var values = new List<string>();
            for (var i = TIME_UNITS.Length - 1; i >= 0; i--)
            {
                var result = Math.Floor(seconds / TIME_UNITS[i].Value);
                if (result >= 1)
                {
                    seconds %= TIME_UNITS[i].Value;
                    values.Add($"{result}{TIME_UNITS[i].Key}");
                }
            }
            if (values.Count == 0)
                values.Add($"0{TIME_UNITS.First().Key}");
            return string.Join(" ", values);
        }
    }
}