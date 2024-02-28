using System;

namespace ServerHelper.Commands
{
    public class PlayerVoiceCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    {  " голос за ", " голос ", " мой голос за ", " мой голос в ",
                       " я голосую за ", " я голосую в ", " мой голос ", " мой волос ",
                    };

        public override bool Repeatable => false;
        public override bool WithoutParams => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            if (server.IsUserPlays(e.user))
            {
                int voice;
                try
                {
                    voice = int.Parse(formattedCommand);
                }
                catch (Exception)
                {
                    server.SendMessage(e.peer, $"Некорректный номер игрока \"{formattedCommand}\". ");
                    return;
                }

                server.FindGameByPlayerID(e.user).PlayerVoiceFor(e.user, voice);
            }
        }

        public override bool IsSourceValid(MessageSource source)
        {
            return source == MessageSource.Chat;
        }

    }
}
