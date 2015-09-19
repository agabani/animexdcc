using System;

namespace AnimeXdcc.Core.Components.HumanReadable
{
    public class BytesConverter : IBytesConverter
    {
        private static readonly string[] SizeSuffixes = {"B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        public string ToHumanReadable(long value)
        {
            if (value < 0)
            {
                return "-" + ToHumanReadable(-value);
            }

            var i = 0;
            decimal dValue = value;
            while (Math.Floor(dValue/1024) >= 1)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:N1} {1}", dValue, SizeSuffixes[i]);
        }
    }
}