namespace MafiaRules.Roles
{
    public class JesterRole : CivillianRole
    {
        public override string Description => "Отличается от мирного одной весёлой особенностью - при аресте, выигрывает. ";

        public override bool Arrest()
        {
            game.MakePlayerWinner(player.userID);
            game.SendMessage($"{player} был шутом{HtmlSmiles.Clown}. " +
                $"Его арестовали и он одержал победу в этой игре!{HtmlSmiles.CoolMan}");
            return false;
        }

        public override string ToString() => "Шут";
    }
}
