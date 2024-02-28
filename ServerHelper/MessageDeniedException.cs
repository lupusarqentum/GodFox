using System;

namespace ServerHelper
{
    public class MessageDeniedException : Exception
    {
        
        public MessageDeniedException() : base() {}
        public MessageDeniedException(string message) : base(message) {}

    }
}
