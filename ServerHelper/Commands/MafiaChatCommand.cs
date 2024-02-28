namespace ServerHelper.Commands
{
    public class MafiaChatCommand : EmptyCommand
    {
        
        public override string[] Synonyms => new string[]
                    {" !чат ", " !мафия "};

        public override bool Repeatable => false;
        public override bool WithoutParams => false;

        public override void Execute(MessageEvent e, string formattedCommand, ServerController server)
        {
            if (server.IsUserPlays(e.user))
                server.FindGameByPlayerID(e.user).PlayerSendsMessageToMafia(e.user, formattedCommand);
        }

        public override bool IsSourceValid(MessageSource source)
        {
            return source == MessageSource.Private;
        }

    }

}
