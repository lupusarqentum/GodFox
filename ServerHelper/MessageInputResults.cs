namespace ServerHelper
{
    public enum MessageInputResults : sbyte
    {
        SystemException = -5,
        CommandNotFound,
        InvalidSource,
        PlayerPicks,
        SucessWithRepeatsCountMismatch,
        Sucess,
    }
}
