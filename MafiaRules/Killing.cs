namespace MafiaRules
{
    public struct Killing
    {
        public KillingResult killingResult;
        public long sacrificeID;

        public Killing(KillingResult killingResult, long sacrificeID)
        {
            this.killingResult = killingResult;
            this.sacrificeID = sacrificeID;
        }

        public Killing(KillingResult killingResult)
        {
            this.killingResult = killingResult;
            sacrificeID = -1;
        }

        public string ToString(string name, string sacrifice)
        {
            switch (killingResult)
            {
                case KillingResult.Miss:
                    if (name == "Маньяк")
                        return HtmlSmiles.ExclamationPoint + "Маньяк промахнулся и никого не убил. \n";
                    return HtmlSmiles.ExclamationPoint + name + " промахнулась и никого не убила. \n";
                case KillingResult.Saved:
                    if (name == "Маньяк")
                        return HtmlSmiles.Syringe + "Маньяк не смог никого убить " + ".\n";
                    return HtmlSmiles.MedicinePill + name + " не смогла никого убить " + ".\n";
                case KillingResult.Killed:
                    if (name == "Маньяк")
                        return HtmlSmiles.Knife + "Маньяк убил " + sacrifice + ".\n";
                    return HtmlSmiles.Gun + name + " убила " + sacrifice + ".\n";
                case KillingResult.Undefined:
                default:
                    return "";
            }
        }

    }
}
