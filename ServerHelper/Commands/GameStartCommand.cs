using MafiaRules;

namespace ServerHelper.Commands
{
    public class GameStartCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[] 
                    { " начать ", };

        public override bool Repeatable => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            if (!server.IsGameExist(e.peer))
            {
                var delay = new PhasesDelay(10, 15, 45, 45);
                server.AddGame(e.peer, delay, Settings.GetNormalDistributor());
                server.SendMessageAndKeyboard(e.peer, $"Набор на игру начался, он длится {delay[0]} секунд. Для начала игры необходимо {Settings.MinimumPlayersCount} игроков. " +
                    "Чтобы принять игру, используйте команду \"Принять\"", new BotButton[][] {
                    new BotButton[] {
                        new BotButton() { color = BotButtonColors.Positive, text = "Принять", },
                        new BotButton() { color = BotButtonColors.Positive, text = "Добавить бота", }
                    }, new BotButton[] {
                        new BotButton() { color = BotButtonColors.Negative, text = "Отказаться", },
                        new BotButton() { color = BotButtonColors.Negative, text = "Удалить бота", }
                    }, new BotButton[] {
                        new BotButton() { color = BotButtonColors.Default, text = "Принудительно", }
                    } }, true);
            }
            else
            {
                server.SendMessage(e.peer, "Игра уже идёт, дождитесь её окончания, чтобы начать новую! ");
            }
        }

        public override bool IsSourceValid(MessageSource source)
        {
            return source == MessageSource.Chat;
        }

    }

}
