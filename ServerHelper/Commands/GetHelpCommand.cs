using MafiaRules;

namespace ServerHelper.Commands
{
    public sealed class GetHelpCommand : EmptyCommand
    {

        public override string[] Synonyms => new string[]
                    { " помощь ", };

        public override bool Repeatable => false;

        #region public-links
        public static string LinkToMafiaRules; // https://............: page with rules of mafia game (for help command)
        public static string LinkToBotCommandsHelp; // https://.......: page with help of bot commands (for help command)
        public static string LinkToMafiaWithThisBot; // https://......: page with help of bot playing
        public static string LinkToRolesOfTheBot; // https://.........: page with help of bot roles
        #endregion

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            MafiaGame game;
            if (e.source == MessageSource.Private && (game = server.FindGameByPlayerID(e.user)) != null)
                ExecuteRoleHelp(e, game, server);
            else
                ExecuteBotHelp(e, server);
        }

        private void ExecuteBotHelp(MessageEvent e, ServerController server) =>
            server.SendMessage(e.peer, $"Помощь по боту:\n\nПравила мафии: {LinkToMafiaRules}\nИгра с ботом: " +
                $"{LinkToMafiaWithThisBot}\nКоманды бота: {LinkToBotCommandsHelp}\n" +
                $"Роли: {LinkToRolesOfTheBot}\n\nПо всем вопросам и предложениям писать в {server.GroupName} (обсуждения группы бота )" +
                $"или {server.Str_CreatorId} (создателю).\nВы также можете оставить отзыв, используя команду \"Отзыв\". ");

        private void ExecuteRoleHelp(MessageEvent e, MafiaGame game, ServerController server)
        {
            var player = game.GetPlayer(e.user);
            server.SendMessage(e.peer, $"{player.Role}.\n{player.Role.Description}");
        }

    }
}
