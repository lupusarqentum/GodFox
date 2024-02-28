using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaRules.Roles
{
    internal class MafiaRole : AdvancedCivillian
    {
        public override string Description => "Мафия.\nИграет за преступную группировку. Вместе с остальными членами мафии, " +
                                              "выбирают одну жертву и убивают её. ";
        public override Teams Team => Teams.Mafia;

        protected bool shooted;
        protected IPlayer[] teamates;
        protected bool onlyBots;

        public MafiaRole() : base()
        {
            shooted = false;

            onlyBots = false;
            teamates = null;
        }

        protected override void ListenEvents() => game.NightStart += NightStarted;
        protected override void UnlistenEvents() => game.NightStart -= NightStarted;

        public override string Pick(GamePhases phase, int pick)
        {
            if (phase != GamePhases.Night)
                return base.Pick(phase, pick);

            if (shooted)
                return HtmlSmiles.Scissors + "Ложитесь спать. ";

            NightPick = pick - 1;

            foreach (var teamate in teamates)
                if (teamate is BotPlayer)
                    teamate.Role.Pick(phase, pick); // Боты будут убивать тех же, кого убивают живые люди

            shooted = true;

            return HtmlSmiles.CheckMark2 + "Выбор сделан! Можете идти спать. ";
        }

        protected virtual void NightStarted(object sender, EventArgs e)
        {
            shooted = false;
            NightPick = -1;
            UpdateTeamates();

            if (!isBot)
            {
                var buffer = new StringBuilder(HtmlSmiles.Demon + "Наступает ночь, а мафия выходит на охоту. " +
                    "Ниже список игроков, из которого Вы можете выбрать жертву. " +
                    "Главное, помнить, что команда мафии сможет убить человека, только в том случае, если все участники мафии выберут одного человека. " +
                    "Поэтому договаривайтесь со своими сообщниками с помощью команды \"Чат\". Список игроков: \n\n" + game.GetPlayersList(" \n", "\n\n", true) + 
                    "Состав мафии на сегодня: \n\n");

                foreach (var teamate in teamates)
                    buffer.Append(teamate.ToString() + " \n");

                buffer.Append("\nСотрудничайте с ними, чтобы победить! ");

                game.SendMessage(player.userID, buffer.ToString());
            }
            else
            {
                if (onlyBots && NightPick == -1)
                {
                    var pick = Settings.Rand(0, game.players.Count);

                    while (NotCivillian(pick)) 
                        pick = (++pick) % game.players.Count;

                    Pick(GamePhases.Night, pick);
                }
            }
        }
        
        protected bool NotCivillian(int pick)
        {
            return game.players[pick].Team != Teams.Mafia;
        }

        protected void UpdateTeamates()
        {
            var livingMafias = false;
            var _teamates = new List<IPlayer>();
            
            foreach (var _player in game.players)
            {
                if (_player.Team == Teams.Mafia && _player.userID != player.userID)
                {
                    _teamates.Add(_player);
                    if (_player is MafiaPlayer)
                        livingMafias = true;
                }
            }

            onlyBots = isBot && (!livingMafias);

            teamates = _teamates.ToArray();
        }

        public override string ToString() => "Мафия";
    }
}
