namespace MafiaRules
{
    public enum GamePhases
    {

        PlayersSet,
        Morning,
        Day,
        Night,

    }

    public static class GamePhasesWork
    {

        public static string ToWord(this GamePhases phase)
        {
            switch (phase)
            {
                case GamePhases.PlayersSet:
                    return "набор";
                case GamePhases.Morning:
                    return "утро";
                case GamePhases.Day:
                    return "день";
                case GamePhases.Night:
                default:
                    return "ночь";
            }
        }

    }

}
