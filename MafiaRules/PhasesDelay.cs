namespace MafiaRules
{
    public struct PhasesDelay
    {

        public readonly int[] delay; // phases delay in seconds
        public int this[int index] => delay[index];
        public int this[GamePhases index] => delay[(int)index];

        public PhasesDelay(params int[] delay)
        {
            if (delay.Length != 4)
                throw new MafiaException("Invalid phases times set");

            this.delay = delay;
        }

        public string ToString(int delayNumber)
        {
            return delay[delayNumber].ToString();
        }

    }
}
