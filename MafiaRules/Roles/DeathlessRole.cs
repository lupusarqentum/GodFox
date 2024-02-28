namespace MafiaRules.Roles
{
    public class DeathlessRole : CivillianRole
    {
        public override string Description => "Не может умереть ночью, но может быть арестован днём.";

        public override KillingResult Kill() => KillingResult.Saved;

        public override string ToString() => "Бессмертный";
    }
}
