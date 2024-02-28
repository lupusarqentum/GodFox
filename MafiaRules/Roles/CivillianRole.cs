using System;

namespace MafiaRules.Roles
{
    public class CivillianRole : IRole
    {
        public virtual string Description => "Ночью спит, днём участвует в обсуждении и голосует. ";
        public virtual Teams Team => Teams.Civillian;
        public int NightPick { get; set; }

        protected IPlayer player;
        protected MafiaGame game;

        public CivillianRole() => NightPick = -1;

        public virtual bool Arrest() => true;

        public virtual void Clear() => player = null;

        public virtual KillingResult Kill()
        {
            var myId = game.GetPlayerIndex(player.userID);

            foreach (var player in game.players)
                if (player.Role is DoctorRole)
                    if (player.Role.NightPick == myId)
                        return KillingResult.Saved;
            
            return KillingResult.Killed;
        }
        
        public virtual void AutoBotVoice(object sender, EventArgs e)
        {
            var args = e as VoiceStepEventArgs;

            var pickIndex = Settings.Rand(0, args.allowList.Count);
            int pick;

            if (args.allowList[pickIndex] == game.GetPlayerIndex(player.userID))
                pick = args.allowList[++pickIndex % args.allowList.Count];
            else
                pick = args.allowList[pickIndex];

            game.PlayerVoiceFor(player.userID, pick + 1);
        }

        public virtual string Pick(GamePhases phase, int pick) 
            => HtmlSmiles.Fox + "Чтобы погладить лисика, используйте команду \"Погладить\"";

        public virtual void SetPlayer(IPlayer player, bool isBot = false)
        {
            this.player = player;
            game = player.Game;
        }

        public override string ToString() => "Мирный";
    }
}
