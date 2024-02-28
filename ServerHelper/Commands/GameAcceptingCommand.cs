using MafiaRules;
using System.Runtime.InteropServices;

namespace ServerHelper.Commands
{
    public class GameAcceptingCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    { " принять ", };

        public override bool Repeatable => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            if (!server.IsMessagesFromBotAllowed(e.user))
            {
                server.SendMessage(e.peer, "Чтобы принять игру, Вам нужно разрешить сообществу писать Вам сообщения. Для этого, перейдите в меню сообщества" +
                    "и нажмите кнопку \"Разрешить сообщения\". ");
                return;
            }

            var game = server.FindGameByChatID(e.peer);

            if (game == null)
                return;

            if (server.FindGameByPlayerID(e.user) != null)
            {
                server.SendMessage(e.peer, "Вы уже приняли игру в этой или другой беседе! ");
                return;
            }

            lock (game.playersListLocker)
            {
                if (game.AppendPlayer(server.GetFLName(e.user), server.GetUserNickname(e.user), e.user))
                {
                    server.SendMessage(e.peer, $"{server.GetUserNickname(e.user)}, Вы приняты!\n{game.players.Count} {InclinePlayer(game.players.Count)} " +
                        "готовы к игре. ");
                    if (game.players.Count == Settings.MaxPlayersCount)
                    {
                        server.SendMessage(e.peer, "Максимальное количество игроков было собрано.\nИгра начинается автоматически! ");
                        game.StartGame();
                    }
                }
                else
                {
                    server.SendMessage(e.peer, "Игра уже идёт. Дождитесь окончания этой игры, и начинайте новую!) ");
                }
            }
        }

        internal static string InclinePlayer(int number)
        {
            if (number == 1)
                return "игрок";

            switch(number % 10)
            {
                case 2 when number % 100 != 12:
                case 3 when number % 100 != 13:
                case 4 when number % 100 != 14:
                    return "игрока";
                default:
                    return "игроков";
            }
        }

        public override bool IsSourceValid(MessageSource source)
        {
            return source == MessageSource.Chat;
        }

    }
}
