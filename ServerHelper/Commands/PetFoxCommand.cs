using MafiaRules;

namespace ServerHelper.Commands
{
    public class PetFoxCommand : EmptyCommand
    {
        private static readonly string[] messages;
        private static int pointer;

        public override string[] Synonyms => new string[]
                    { " !погладить ", };

        public override bool Repeatable => true;
        public override bool WithoutParams => false;

        static PetFoxCommand()
        {
            pointer = 0;
            messages = new string[] { " погладил лиса-мафиозника ", " погладил лисика ", " погладил лису ", " рррх ", };
        }

        private static string GetMessage()
        {
            pointer = (++pointer) % messages.Length;
            return messages[pointer];
        }

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            server.SendMessage(e.peer, HtmlSmiles.Fox + server.GetUserNickname(e.user) + GetMessage() + formattedCommand);
        }

    }
}
