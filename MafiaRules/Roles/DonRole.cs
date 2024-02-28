using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaRules.Roles
{
    internal class DonRole : MafiaRole
    {
        protected enum DonPickStages
        {
            None,
            Shooted,
            SheriffChecked,
        }

        public override string Description => "Дон мафии.\nЯвляется главным мафиози, помимо этого, он может искать ночью шерифа. Выбирая " +
                                              "по 1 игроку за ночь и узнавая, является ли он шерифом. ";
        public override Teams Team => Teams.Mafia;
        
        protected DonPickStages pickStage;
        protected IPlayer sheriffChecking;

        protected Dictionary<long, bool> botChecks; // <id, isSheriff?>

        public DonRole() : base()
        {
            pickStage = DonPickStages.None;
            sheriffChecking = null;
            botChecks = new Dictionary<long, bool>();
        }

        public override void AutoBotVoice(object sender, EventArgs e)
        {
            BotChecksClear();

            var args = e as VoiceStepEventArgs;
            if (args.allowList.Count < 3)
            {
                base.AutoBotVoice(sender, e);
                return;
            }

            if (Settings.Rand(0, 2) == 0)
            {
                foreach (var pair in botChecks)
                {
                    if (pair.Value)
                    {
                        game.PlayerVoiceFor(player.userID, game.GetPlayerIndex(pair.Key) + 1);
                        return;
                    }
                }
            }

            base.AutoBotVoice(sender, e);
        }

        public override string Pick(GamePhases phase, int pick)
        {
            if (phase != GamePhases.Night)
                return base.Pick(phase, pick);

            if (pickStage == DonPickStages.SheriffChecked)
                return HtmlSmiles.Scissors + "Ложитесь спать. ";

            if (pickStage == DonPickStages.None)
            {
                NightPick = pick - 1;

                foreach (var teamate in teamates)
                    if (teamate is BotPlayer)
                        teamate.Role.Pick(phase, pick); // Боты будут убивать тех же, кого убивают живые люди

                pickStage++;
                return HtmlSmiles.CheckMark + "Выбор сделан! Но Вы - дон, и поэтому можете попробовать найти шерифа. " +
                    "Выберите любого игрока, чтобы совершить проверку. ";
            }
            else if (pickStage == DonPickStages.Shooted)
            {
                // Проверка на шерифа
                sheriffChecking = game.players[pick - 1];
                pickStage++;
                return HtmlSmiles.CheckMark2 + "Принято. А информацию узнаете утром. ";
            }

            throw new MafiaException();
        }

        protected void MorningStarted(object sender, EventArgs e)
        {
            if (sheriffChecking != null)
            {
                var result = $"грок {sheriffChecking}{(sheriffChecking.Role is SheriffRole ? " " : " НЕ ")}шериф! ";

                if (isBot)
                {
                    game.PlayerSendsMessageToMafia(player.userID, "Привет, я узнал, что и" + result);
                    botChecks.Add(sheriffChecking.userID, sheriffChecking is SheriffRole);
                }
                else
                    game.SendMessage(player.userID, "И" + result);

                sheriffChecking = null;
            }
        }
        
        protected override void ListenEvents()
        {
            game.MorningStart -= MorningStarted;
            base.ListenEvents();
        }

        protected override void UnlistenEvents()
        {
            game.MorningStart += MorningStarted;
            base.UnlistenEvents();
        }

        protected override void NightStarted(object sender, EventArgs e)
        {
            return;
            //pickStage = DonPickStages.None;
            //NightPick = -1;
            //UpdateTeamates();

            //if (!isBot)
            //{
            //    var buffer = new StringBuilder("Наступает ночь, а мафия выходит на охоту. Ниже список игроков, из которого Вы можете выбрать жертву. " +
            //        "Главное, помнить, что команда мафии сможет убить человека, только в том случае, если все участники мафии выберут одного человека. " +
            //        "Поэтому договаривайтесь со своими сообщниками с помощью команды \"Чат\". Список игроков: \n\n" + game.GetPlayersList(" \n", "\n\n", true) +
            //        "Состав мафии на сегодня: \n\n");

            //    foreach (var teamate in teamates)
            //        buffer.Append(teamate.ToString() + " \n");

            //    buffer.Append("\nСотрудничайте с ними, чтобы победить! ");

            //    game.SendMessage(player.userID, buffer.ToString());
            //}
            //else
            //{
            //    BotChecksClear();

            //    // Выстрел
            //    if (onlyBots)
            //    {
            //        foreach (var pair in botChecks)
            //        {
            //            if (pair.Value)
            //            {
            //                var sheriff = game.GetPlayerIndex(pair.Key);
            //                NightPick = sheriff;
            //                foreach (var teamate in teamates)
            //                    teamate.Role.NightPick = sheriff;

            //                return;
            //            }
            //        }
                    
            //        var pick = Settings.Rand(0, game.players.Count);

            //        while (NotCivillian(pick))
            //            pick = (++pick) % game.players.Count;

            //        Pick(GamePhases.Night, pick);
            //    }

            //    // Поиск шерифа
            //    var sheriffPick = Settings.Rand(0, game.players.Count);

            //    var iterations = 0;
            //    while (NotChecked(sheriffPick))
            //    {
            //        sheriffPick = (++sheriffPick) % game.players.Count;
                    
            //        iterations++;
            //        if (iterations == game.players.Count)
            //        {
            //            sheriffPick = Settings.Rand(0, game.players.Count);

            //            while (NotCivillian(sheriffPick))
            //                sheriffPick = (++sheriffPick) % game.players.Count;

            //            break;
            //        }
            //    }

            //    sheriffChecking = game.players[sheriffPick];
            //}
        }

        protected void BotChecksClear()
        {
            foreach (var pair in botChecks)
                if (!game.IsUserPlaying(pair.Key))
                    botChecks.Remove(pair.Key);
        }

        protected bool NotChecked(int pick)
        {
            return NotCivillian(pick) && (!botChecks.ContainsKey(game.players[pick].userID));
        }

        public override string ToString() => "Дон мафии";
    }
}
