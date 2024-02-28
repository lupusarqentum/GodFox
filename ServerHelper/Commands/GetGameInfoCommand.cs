namespace ServerHelper.Commands
{
    public class GetGameInfoCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    { " !процесс ", " !инфо ", " !инфа ", " !информация ", };

        public override bool Repeatable => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            if (e.source == MessageSource.Chat)
            {
                if (server.IsGameExist(e.peer))
                    server.SendMessage(e.peer, server.FindGameByChatID(e.peer).ToString());
            }
            else if (server.IsUserPlays(e.user))
                server.SendMessage(e.user, server.FindGameByPlayerID(e.user).ToString());
            else
                server.SendMessage(e.user, "Вы же не играете! ");
        }

    }

}
