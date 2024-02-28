using MafiaRules;

namespace ServerHelper.Commands
{
    public class GameRejectingCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    { " отказаться " };

        public override bool Repeatable => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            var game = server.FindGameByPlayerID(e.user);

            if (game == null)
                return;

            if (game.Phase != GamePhases.PlayersSet)
            {
                server.SendMessage(e.peer, "Игра уже идёт. Отказаться больше нельзя! ");
                return;
            }

            string who;
            int playersCount;

            lock (game.playersListLocker)
            {
                who = game.GetPlayer(e.user).ToString();
                playersCount = game.players.Count - 1;

                game.RemovePlayer(e.user);
            }

            game.SendMessage(game.chatID, $"{who} больше не участвует в игре.\n{playersCount} " +
                $"{GameAcceptingCommand.InclinePlayer(playersCount)} готовы к игре! ");
        }

    }
}
