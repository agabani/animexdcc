using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generic.IrcClient.Dcc
{
    public class DccMessageParser
    {
        private readonly IpConverter _ipConverter;

        public DccMessageParser(IpConverter ipConverter)
        {
            _ipConverter = ipConverter;
        }

        public bool IsDccMessage(string message)
        {
            return message.StartsWith("\x01" + "DCC SEND") && message.EndsWith("\x01");
        }

        public DccSendMessage Parse(string dccMessageString)
        {
            var sanitizedString = SanitizeString(dccMessageString);

            var parameters = SeperateParameters(sanitizedString);

            return new DccSendMessage
            {
                FileName = ParseFileName(parameters),
                FileSize = ParseFileSize(parameters),
                IpAddress = ParseIpAddress(parameters),
                Port = ParsePort(parameters)
            };
        }

        private static string SanitizeString(string dccMessageString)
        {
            return dccMessageString.Replace("\x01", string.Empty);
        }

        private static string[] SeperateParameters(string sanitizedString)
        {
            return sanitizedString.Split(' ');
        }

        private static string ParseFileName(IReadOnlyCollection<string> parameters)
        {
            var fileName = string.Join(" ", parameters.Skip(2).Take(parameters.Count - 5));

            if (IsWrappedInDoubleQuotes(fileName))
            {
                fileName = RemoveDoubleQuoteWrapper(fileName);
            }

            return RemoveInvalidCharacters(fileName);
        }

        private string ParseIpAddress(IReadOnlyList<string> parameters)
        {
            return _ipConverter.UintAddressToIpAddress(uint.Parse(parameters[parameters.Count - 3]));
        }

        private static int ParsePort(IReadOnlyList<string> parameters)
        {
            return int.Parse(parameters[parameters.Count - 2]);
        }

        private static uint ParseFileSize(IReadOnlyList<string> parameters)
        {
            return uint.Parse(parameters[parameters.Count - 1]);
        }

        private static bool IsWrappedInDoubleQuotes(string fileName)
        {
            return fileName.StartsWith("\"") && fileName.EndsWith("\"");
        }

        private static string RemoveDoubleQuoteWrapper(string fileName)
        {
            return fileName.Substring(1, fileName.Length - 2);
        }

        private static string RemoveInvalidCharacters(string fileName)
        {
            return Path.GetInvalidFileNameChars()
                .Aggregate(fileName, (current, chararacter) => current.Replace(chararacter, '_'));
        }
    }
}