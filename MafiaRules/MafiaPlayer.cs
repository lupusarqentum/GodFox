namespace MafiaRules
{
    public sealed class MafiaPlayer : IPlayer
    {
        private IRole _role;
        public IRole Role 
        {
            get => _role;
            set
            {
                if (_role != null)
                {
                    _role.Clear();
                }

                _role = value;
                
                if (_role != null)
                {
                    if (mafiaGame != null)
                        _role.SetPlayer(this);

                    Team = _role.Team;
                }
            }
        }
        public Teams Team { get; set; }

        public MafiaGame Game => mafiaGame;
        public MafiaGame mafiaGame;

        private readonly string vkName;
        private readonly string nickname;

        public long userID { get; set; }
        public bool IsAlive { get; set; }

        public MafiaPlayer(string vkName, string nickname, long userID, MafiaGame mafiaGame)
        {
            _role = null;
            Team = Teams.None;
            
            this.mafiaGame = mafiaGame;
            this.vkName = vkName;
            this.nickname = nickname;
            this.userID = userID;

            IsAlive = true;
        }

        public void SetRole(IRole role)
        {
            if (Role == null)
                mafiaGame.SendMessage(userID, $"{HtmlSmiles.ExclamationPoint}Ваша роль - \"{role}\", Поздравляем!" +
                    "\nЧтобы узнать больше информации о Вашей роли, используйте команду \"Помощь\" ");

            Role = role;
        }

        public void Clear()
        {
            Role?.Clear();
            mafiaGame = null;
        }

        public override string ToString()
        {
            return nickname.Equals(vkName) ? vkName : $"{nickname} ({vkName})";
        }

    }
}