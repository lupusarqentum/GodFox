using MafiaRules;

namespace ServerHelper.Commands
{
    public class BotRemovingCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    { " убрать бота ", " -бот ", " - бот ", " удалить бота ", };

        public override bool Repeatable => true;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            var game = server.FindGameByChatID(e.peer);

            if (game == null)
                return;

            if (game.Phase != GamePhases.PlayersSet)
            {
                server.SendMessage(e.peer, "Игра уже идёт, убрать ботов нельзя! ");
                return;
            }

            if (game.botsCount == 0)
            {
                server.SendMessage(e.peer, "Ботов и так нет! ");
                return;
            }

            bool botRemoved;
            int playersCount;
            string botName;
            lock (game.playersListLocker)
            {
                botRemoved = game.RemoveBot();
                playersCount = game.players.Count;
                botName = game.GetLastBotInGameNickname();
            }

            if (botRemoved)
                server.SendMessage(e.peer, $"{HtmlSmiles.Robot}Бот {botName} убран. \n" +
                    $"{playersCount} {GameAcceptingCommand.InclinePlayer(playersCount)} готовы к игре! ");
        }
        
        public override bool IsSourceValid(MessageSource source) => source == MessageSource.Chat;

    }
}
