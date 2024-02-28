using System.Text;

namespace ServerHelper.Commands
{
    public class GameResetCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    { " !сброс ", };

        public override bool Repeatable => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            var game = server.FindGameByChatID(e.peer);

            if (game == null)
                return;

            string message = "Игра была сброшена. ";
            if (game.players.Count != 0)
            {
                var buffer = new StringBuilder(message);
                buffer.Append("Роли игроков: ");
                
                foreach (var player in game.players)
                {
                    server.SendMessage(player.userID, "Игра в Вашей беседе была сброшена. ");
                    buffer.Append($"{player} - {player.Role}, ");
                }
                
                buffer.Remove(buffer.Length - 2, 2);
                message = buffer.ToString();
            }
            
            server.SendMessage(e.peer, message);

            server.RemoveFinishedGame(game, null);
        }

        public override bool IsSourceValid(MessageSource source)
        {
            return source == MessageSource.Chat;
        }

    }

}
