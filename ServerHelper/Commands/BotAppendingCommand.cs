using MafiaRules;

namespace ServerHelper.Commands
{
    public class BotAppendingCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    { " добавить бота ", " +бот ", " + бот ", };

        public override bool Repeatable => true;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            var game = server.FindGameByChatID(e.peer);

            if (game == null)
                return;

            if (game.Phase != GamePhases.PlayersSet)
            {
                server.SendMessage(e.peer, "Игра уже идёт. Добавить бота нельзя! ");
                return;
            }

            if (game.botsCount == Settings.MaxBotsCount)
            {
                server.SendMessage(e.peer, "Максимальное количество ботов уже достигнуто, добавить больше нельзя! ");
                return;
            }

            lock (game.playersListLocker)
            {
                if (game.AppendBot())
                {
                    server.SendMessage(e.peer, $"{HtmlSmiles.Robot}Бот {game.GetLastBotInGameNickname()} добавлен.\n" +
                        $"{game.players.Count} {GameAcceptingCommand.InclinePlayer(game.players.Count)} готовы к игре! ");
                }
                
                if (game.players.Count == Settings.MaxPlayersCount)
                {
                    server.SendMessage(e.peer, "Максимальное количество игроков было собрано.\nИгра начинается автоматически! ");
                    game.StartGame();
                }
            }
        }

        public override bool IsSourceValid(MessageSource source)
        {
            return source == MessageSource.Chat;
        }

    }

}
