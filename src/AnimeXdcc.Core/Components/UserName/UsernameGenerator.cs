using System;

namespace AnimeXdcc.Core.Components.UserName
{
    public class UserNameGenerator : IUserNameGenerator
    {
        private readonly Random _random = new Random();

        public string Create(int length)
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