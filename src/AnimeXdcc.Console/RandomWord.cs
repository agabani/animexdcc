using System;

namespace AnimeXdcc.Console
{
    internal class RandomWord
    {
        private readonly Random _random = new Random();

        public string GetString(int length)
        {
            var str = string.Empty;
            for (var i = 0; i < length; i++)
            {
                str += GetLetter();
            }
            return str;
        }

        private char GetLetter()
        {
            var num = _random.Next(0, 26);
            return (char) ('a' + num);
        }
    }
}