namespace ServerHelper
{
    public enum MessageSource : byte
    {
        Private,
        Chat,
    }

    public static class MessageSourceWork
    {

        public static MessageSource Invert(this MessageSource source)
        {
            switch (source)
            {
                case MessageSource.Chat:
                    return MessageSource.Private;
                case MessageSource.Private:
                default:
                    return MessageSource.Chat;
            }
        }

        public static string ToString2(this MessageSource source)
        {
            switch (source)
            {
                case MessageSource.Chat:
                    return "беседах";
                case MessageSource.Private:
                default:
                    return "личных сообщениях";
            }
        }

    }

}
