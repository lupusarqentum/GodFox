using System;

namespace MafiaRules
{
    public sealed class GameEndedEventArgs : EventArgs
    {
        public Teams WinTeam { get; set; }

        public GameEndedEventArgs(Teams WinTeam)
        {
            this.WinTeam = WinTeam;
        }

    }
}