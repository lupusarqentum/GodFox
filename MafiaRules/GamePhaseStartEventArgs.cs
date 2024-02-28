using System;
using System.Collections.Generic;

namespace MafiaRules
{
    public sealed class GamePhaseStartEventArgs : EventArgs
    {
        public GamePhases Phase { get; private set; }
        public Killing[] Killings { get; private set; }

        public GamePhaseStartEventArgs() : base() 
        {
            Phase = GamePhases.PlayersSet;
            Killings = null;
        }

        public GamePhaseStartEventArgs(GamePhases Phase)
        {
            this.Phase = Phase;
            Killings = null;
        }

        public GamePhaseStartEventArgs(GamePhases Phase, List<Killing> Killings) : this(Phase)
        {
            this.Killings = Killings.ToArray();
        }
    }
}
