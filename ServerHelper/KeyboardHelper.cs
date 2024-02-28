namespace ServerHelper
{
    public struct BotButton
    {
        public BotButtonColors color;
        public string text;
    }

    public enum BotButtonColors
    {
        Default,
        Primary,
        Positive,
        Negative,
    }
}
