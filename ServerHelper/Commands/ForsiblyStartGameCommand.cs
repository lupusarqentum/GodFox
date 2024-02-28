using MafiaRules;

namespace ServerHelper.Commands
{
    public class ForsiblyStartGameCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    { " !принудительно ", };

        public override bool Repeatable => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            var game = server.FindGameByChatID(e.peer);

            if (game == null || game.Phase != GamePhases.PlayersSet)
                return;

            if (game.players.Count < Settings.MinimumPlayersCount)
            {
                server.SendMessage(e.peer, $"Для начала игры необходимо {Settings.MinimumPlayersCount} игроков. К игре готовы всего {game.players.Count} игроков. ");
                return;
            }

            if (game.StartGame())
                server.SendMessage(e.peer, "Игра была начата принудительно. ");
        }

        public override bool IsSourceValid(MessageSource source)
        {
            return source == MessageSource.Chat;
        }

    }

}
