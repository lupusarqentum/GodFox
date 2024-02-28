using System;

namespace MafiaRules.Roles
{
    internal class ManiacRole : AdvancedCivillian
    {
        public override string Description => "Безумный преступник, выбирающий ночью свою жертву, и убивающего её. Для" +
                                              " победы должен остаться последним выжившим в городе. ";
        public override Teams Team => Teams.Single;

        protected bool killing;

        public ManiacRole() : base() => killing = false;

        public override string Pick(GamePhases phase, int pick)
        {
            if (phase != GamePhases.Night)
                return base.Pick(phase, pick);

            if (NightPick != -1)
            {
                return HtmlSmiles.Scissors + "Ложитесь спать. ";
            }
            else
            {
                NightPick = pick - 1;
                return HtmlSmiles.CheckMark2 + "Принято. ";
            }
        }

        protected void NightStarted(object sender, EventArgs e)
        {
            NightPick = -1;
            
            if (isBot)
            {
                var sacrifice = Settings.Rand(0, game.players.Count);

                if (sacrifice == game.GetPlayerIndex(player.userID)) 
                    sacrifice = (++sacrifice) % game.players.Count;

                Pick(GamePhases.Night, sacrifice);
            }
            else
            {
                game.SendMessage(player.userID, HtmlSmiles.Clown + "Настала ночь, пора выбирать жертву.... " +
                    "для этого, выберите любого игрока из списка ниже: \n\n" + 
                    game.GetPlayersList(" \n", ". \n", true) + "\n...и если его никто не спасёт, он погибнет!" + HtmlSmiles.Demon);
            }
        }

        protected override void ListenEvents() => game.NightStart += NightStarted;
        
        protected override void UnlistenEvents() => game.NightStart -= NightStarted;

        public override string ToString() => "Маньяк";
    }
}
