namespace ServerHelper
{
    public abstract class EmptyCommand : ICommand
    {
        public abstract string[] Synonyms { get; }

        public abstract bool Repeatable { get; }

        public virtual bool WithoutParams => true;

        public virtual void Execute(MessageEvent e, string formattedCommand, ServerController server) {}

        public virtual bool IsSourceValid(MessageSource source) => true;

    }

}
