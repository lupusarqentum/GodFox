using System.Collections.Generic;

namespace MafiaRules
{
    public static class Helper
    {
        
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var j = Settings.Rand(0, list.Count);
                var temp = list[j];
                list[j] = list[i];
                list[i] = temp;
            }
        }

        public static string GetWinTextFromTeam(Teams win)
        {
            switch (win)
            {
                case Teams.Civillian:
                    return HtmlSmiles.Party + "В городе больше не осталось преступников. " +
                        "Победил город, поздравляем! " + HtmlSmiles.Party;
                case Teams.Mafia:
                    return HtmlSmiles.Party + "Образовалась безвыходная ситуация для города. " +
                        "Победила мафия, поздравляем! " + HtmlSmiles.Party;
                case Teams.Yakudza:
                    return HtmlSmiles.Party + "Образовалась безвыходная ситуация для города. " +
                        "Победила якудза, поздравляем! " + HtmlSmiles.Party;
                case Teams.Single:
                    return HtmlSmiles.Party + "Образовалась безвыходная ситуация для города. " +
                        "Победил маньяк, поздравляем! " + HtmlSmiles.Party;
                case Teams.None:
                default:
                    return HtmlSmiles.Party + HtmlSmiles.Party;
            }
        }

    }
}
