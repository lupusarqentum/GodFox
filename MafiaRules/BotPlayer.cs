using System;

namespace MafiaRules
{
    public sealed class BotPlayer : IPlayer
    {
        private IRole _role;
        public IRole Role
        {
            get => _role;
            set
            {
                _role = value;

                if (_role != null)
                {
                    if (mafiaGame != null)
                        _role.SetPlayer(this, true);

                    Team = _role.Team;
                }
            }
        }
        public Teams Team { get; set; }

        public MafiaGame Game => mafiaGame;
        public MafiaGame mafiaGame;

        private readonly string nickname;

        public long userID { get; set; }
        public bool IsAlive { get; set; }

        public BotPlayer(string nickname, long userID, MafiaGame mafiaGame)
        {
            _role = null;
            Team = Teams.None;

            this.mafiaGame = mafiaGame;
            this.nickname = nickname;
            this.userID = userID;

            IsAlive = true;

            mafiaGame.VoiceStepStart += AutoVoicing;
        }

        private void AutoVoicing(object sender, EventArgs e)
        {
            Role.AutoBotVoice(sender, e);
        }

        public void SetRole(IRole role) => Role = role;

        public void Clear()
        {
            mafiaGame.VoiceStepStart -= AutoVoicing;
            Role?.Clear();
            mafiaGame = null;
        }

        public override string ToString() => nickname;
    }

}
