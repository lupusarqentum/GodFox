using System;

namespace MafiaRules
{
    internal sealed class PlayerQuitGameEventArgs : EventArgs
    {
        public IPlayer player;

        public PlayerQuitGameEventArgs() : base()
        {
            this.player = null;
        }

        public PlayerQuitGameEventArgs(IPlayer player) : base()
        {
            this.player = player;
        }
    }
}
