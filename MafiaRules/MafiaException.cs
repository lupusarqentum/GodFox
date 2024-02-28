using System;

namespace MafiaRules
{
    public sealed class MafiaException : Exception
    {
        public MafiaException() : base() {}
        public MafiaException(string message):base(message) {}
    }
}
