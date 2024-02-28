using System;

namespace ServerHelper
{
    public interface ICommand
    {
        string[] Synonyms { get; }
        bool Repeatable { get; }
        bool WithoutParams { get; }

        void Execute(MessageEvent e, string formattedCommand, ServerController server);
        bool IsSourceValid(MessageSource source);
    }

}
