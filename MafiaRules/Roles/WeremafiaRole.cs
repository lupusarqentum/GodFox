using System;
using System.Text;

namespace MafiaRules.Roles
{
    internal class WeremafiaRole : AdvancedCivillian
    {
        public override string Description => "Является обычным мирным жителем, однако " +
                                              "когда кто-то из мафии погибает, оборотень заменяет его. ";

        public override Teams Team => Teams.Civillian;

        public WeremafiaRole() : base() {}

        private void MafiaKilledEventHandler(object sender, EventArgs e)
        {
            var args = e as PlayerQuitGameEventArgs;

            var buffer = new StringBuilder($"{HtmlSmiles.WeremafiaMoon}Вы заняли место погибшей мафии {args.player}. " +
                "\nСостав мафии на сегодня: \n\n");
            foreach (var item in game.players)
            {
                if (item.Team == Teams.Mafia)
                {
                    buffer.Append(item.ToString() + ", \n");
                    game.SendMessage(item.userID, $"{HtmlSmiles.WeremafiaMoon}Место погибшего {args.player} занял {player}. " +
                        "Сотрудничайте с ним, чтобы победить !) ");
                }
            }
            buffer.Remove(buffer.Length - 2, 2).Append(". \n\nСотрудничайте с ними, и у Вас всё получится!) ");
            game.SendMessage(player.userID, buffer.ToString());

            player.SetRole(new MafiaRole());
        }


        protected override void ListenEvents()
        {
            game.MafiaDeath += MafiaKilledEventHandler;
        }

        protected override void UnlistenEvents()
        {
            game.MafiaDeath -= MafiaKilledEventHandler;
        }

        public override string ToString() => "Оборотень";
    }
}
