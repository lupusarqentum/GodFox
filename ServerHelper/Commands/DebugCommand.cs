using MafiaRules;
using System.Text;

namespace ServerHelper.Commands
{

    public sealed class DebugCommand : EmptyCommand
    {
        
        public override string[] Synonyms => new string[]
                    { " !отладинфо ", };

        public override bool Repeatable => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            //new GameStartCommand().Execute(e, "", server);
            //new GameAcceptingCommand().Execute(e, "", server);
            //for (int i = 0; i < 6; i++)
            //    new BotAppendingCommand().Execute(e, "", server);
            //new ForsiblyStartGameCommand().Execute(e, "", server);

            //return;

            if (e.user != server.CreatorId)
                server.SendMessage(e.peer, "Эта команда предназначена для отладки и тестирования бота. Не используйте её, фыр ");

            MafiaGame game;
            if ((game = server.FindGameByChatID(e.peer)) != null)
            {
                server.SendMessage(e.peer, game.ToString());
                server.SendMessage(e.peer, game.GetNightPickPlayers());
                server.SendMessage(e.peer, game.GetDayChoicePlayers());
                server.SendMessage(e.peer, game.GetPlayersRoleDebug());
            }
        }

        public override bool IsSourceValid(MessageSource source)
        {
            return source == MessageSource.Chat;
        }

    }

}
