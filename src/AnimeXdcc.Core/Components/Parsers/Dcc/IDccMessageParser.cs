namespace AnimeXdcc.Core.Components.Parsers.Dcc
{
    public interface IDccMessageParser
    {
        bool IsDccMessage(string message);
        DccMessageParser.DccSendMessage Parse(string dccMessageString);
    }
}