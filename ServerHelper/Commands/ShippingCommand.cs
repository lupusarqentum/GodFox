using MafiaRules;

namespace ServerHelper.Commands
{
    public class ShippingCommand : EmptyCommand
    {
        public override string[] Synonyms => new string[]
                    { " !шипперить ", };

        public override bool Repeatable => true;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            var names = server.GetChatUsersNames(e.peer);

            if (names.Count == 1 && names[0] == "denied")
            {
                server.SendMessage(e.peer, "Ошибка доступа. Чтобы получить список участников беседы, боту нужны права администратора! ");
                return;
            }

            var index1 = names[Settings.Rand(0, names.Count)];
            var index2 = names[Settings.Rand(0, names.Count)];
            server.SendMessage(e.peer, $"{index1} + {index2}. Любите друг друга и берегите, фырк!");
        }

        public override bool IsSourceValid(MessageSource source)
        {
            return source == MessageSource.Chat;
        }

    }

}
