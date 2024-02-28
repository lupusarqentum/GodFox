namespace ServerHelper.Commands
{
    public class GetMyRoleCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    { " !мояроль ", };

        public override bool Repeatable => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            if (server.IsUserPlays(e.user))
            {
                server.SendMessage(e.user, $"Ваша роль - {server.FindGameByPlayerID(e.user).GetPlayer(e.user).Role}");
                if (e.source == MessageSource.Chat)
                    server.SendMessage(e.peer, "Ваша роль была отправлена Вам в л/с!");
            }
        }

    }
}
