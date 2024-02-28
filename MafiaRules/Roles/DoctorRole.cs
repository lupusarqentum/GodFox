using System;

namespace MafiaRules.Roles
{
    internal class DoctorRole : AdvancedCivillian
    {
        private enum HealingResult : byte
        {
            RepeatedLastHealing,
            RepeatedSelfHealing,
            YetHealTonight,

            Success
        }

        public override string Description => "Ночью просыпается и выбирает человека, которого спасёт. Этот человек не сможет умереть ночью.";
        public override Teams Team => Teams.Civillian;
        
        protected bool healSelf;
        protected int lastHeal;

        public DoctorRole() : base()
        {
            lastHeal = -1;
            healSelf = false;
        }

        public override string Pick(GamePhases phase, int pick)
        {
            if (phase != GamePhases.Night)
            { 
                return base.Pick(phase, pick);
            }
            else
            {
                var savingResult = GetSavingResult(pick);

                if (savingResult == HealingResult.Success)
                {
                    NightPick = pick - 1;
                    healSelf = healSelf || (game.GetPlayerIndex(player.userID) == pick);
                }

                return HealingResultToString(savingResult);
            }
        }

        protected void NightStarted(object sender, EventArgs e)
        {
            lastHeal = NightPick + 1;

            if (!isBot)
            {
                NightPick = -1;
                game.SendMessage(player.userID, HtmlSmiles.MedicinePill + "Наступает ночь, однако Вам сегодня не до сна. " +
                    "Пришло время выполнять свой долг — лечить людей. Выберите из списка ниже, чью жизнь Вы хотите спасти" +
                    ", выбранного человека не смогут убить ночью.\nНо помните, лечить себя можно только раз за игру, а ле" +
                    "чить того, кого вы лечили в прошлый раз нельзя. Список игроков: \n\n" +
                    game.GetPlayersList("\n", ".\n\n", true) + HtmlSmiles.Syringe);
            }
            else
            {
                int pick = Settings.Rand(0, game.players.Count);
                while (GetSavingResult(pick + 1) != HealingResult.Success) 
                    pick = ++pick % game.players.Count;
                
                NightPick = pick;
                healSelf = healSelf || game.GetPlayerIndex(player.userID) == pick;
            }
        }

        private HealingResult GetSavingResult(int pick)
        {
            if (NightPick != -1)
                return HealingResult.YetHealTonight;

            var isTryingToHealSelf = game.GetPlayerIndex(player.userID) == pick;
            if (healSelf && isTryingToHealSelf)
                return HealingResult.RepeatedSelfHealing;

            if (pick - 1 == lastHeal)
                return HealingResult.RepeatedLastHealing;

            return HealingResult.Success;
        }

        private string HealingResultToString(HealingResult heal)
        {
            return heal switch
            {
                HealingResult.RepeatedLastHealing => HtmlSmiles.Scissors  + "Вы уже лечили кого-то этой ночью!",
                HealingResult.RepeatedSelfHealing => HtmlSmiles.Scissors +  "Вы уже лечили себя! Лучше спасите лисика :З",
                HealingResult.YetHealTonight => HtmlSmiles.MedicinePill +   "Вы лечили этого игрока в прошлый раз. " +
                                                                                "Если не знаете, кого спасать, спасиье лисёнка :З",
                HealingResult.Success => HtmlSmiles.CheckMark +             "Принято.",

                _ => throw new MafiaException("drhltcs"),
            };
        }

        protected override void ListenEvents() => game.NightStart += NightStarted;

        protected override void UnlistenEvents() => game.NightStart -= NightStarted;

        public override string ToString() => "Доктор";
    }
}