using System;

namespace MafiaRules.Roles
{
    internal class SheriffRole : AdvancedCivillian
    {
        public override string Description => "Ночью просыпается и выбирает самого подозрительного человека, " +
                                              "выясняя имеет ли он отношение к преступникам. ";

        public override Teams Team => Teams.Civillian;

        public SheriffRole() : base() {}
        
        public override string Pick(GamePhases phase, int pick)
        {
            if (phase != GamePhases.Night)
                return base.Pick(phase, pick);

            if (NightPick == -1)
            {
                NightPick = pick - 1;
                return HtmlSmiles.CheckMark + "Принято. А информацию узнаете утром, так что ложитесь спать) ";
            }

            return HtmlSmiles.Scissors + "Больше Вы сегодня ничего не сможете узнать. ";
        }

        public void NightStarted(object sender, EventArgs e)
        {
            NightPick = -1;

            if (!isBot)
                game.SendMessage(player.userID, HtmlSmiles.Lens + 
                     "Не забыли, что Вы шериф? Выберите любого игрока из списка ниже, чтобы проверить его на чистоту: " +
                    "\n\n" + game.GetPlayersList(", \n", ".\n", true) + HtmlSmiles.MirroredLens);
        }

        public void MorningStarted(object sender, EventArgs e)
        {
            if (NightPick != -1 && !isBot)
            {
                var args = e as GamePhaseStartEventArgs;
                foreach (var kill in args.Killings)
                    if (kill.sacrificeID == game.GetPlayerIndex(player.userID))
                        return;

                game.SendMessage(player.userID, $"{HtmlSmiles.CheckMark} Игрок {NightPick + 1}. {game.players[NightPick]} " +
                    $"{(game.players[NightPick].Team == Teams.Civillian ? "" : "НЕ ")}играет за мирных. ");
            }
        }

        protected override void ListenEvents()
        {
            game.NightStart += NightStarted;
            game.MorningStart += MorningStarted;
        }

        protected override void UnlistenEvents()
        {
            game.NightStart -= NightStarted;
            game.MorningStart -= MorningStarted;
        }

        public override string ToString() => "Шериф";
    }
}
