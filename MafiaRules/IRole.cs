using System;

namespace MafiaRules
{
    public interface IRole
    {
        string Description { get; }
        Teams Team { get; }
        int NightPick { get; set; }
        
        void SetPlayer(IPlayer player, bool isBot = false);
        string Pick(GamePhases phase, int pick); // answer to player
        KillingResult Kill();
        bool Arrest();
        void AutoBotVoice(object sender, EventArgs e);
        void Clear();
        string ToString(); // returns name of role (Mafia, Maniac, etc...)
    }
}
