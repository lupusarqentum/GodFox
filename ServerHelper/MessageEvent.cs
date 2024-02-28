namespace ServerHelper
{
    public class MessageEvent
    {

        public string message;
        public long peer;
        public long user;
        public MessageSource source;

        public MessageEvent(string message, long peer, long user, MessageSource source)
        {
            this.message = message;
            this.peer = peer;
            this.user = user;
            this.source = source;
        }

        public override string ToString() => $"Пользователь {user} в чате {peer} написал {message} из {source.ToString2()}";

    }

}
