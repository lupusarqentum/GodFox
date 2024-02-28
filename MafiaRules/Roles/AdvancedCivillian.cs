namespace MafiaRules.Roles
{
    internal abstract class AdvancedCivillian : CivillianRole
    {
        public abstract override string Description { get; }
        public abstract override Teams Team { get; }

        private bool subscribed;
        protected bool isBot;

        public AdvancedCivillian() : base()
        { 
            subscribed = false;
            isBot = false;
        }

        protected abstract void ListenEvents();
        protected abstract void UnlistenEvents();

#pragma warning disable IDE1006
        private void _SubscribeToEvents()
        {
            ListenEvents();
            subscribed = true;
        }

        private void _DesubscribeToEvents()
        {
            UnlistenEvents();
            subscribed = false;
        }
#pragma warning restore IDE1006

        public sealed override void SetPlayer(IPlayer player, bool isBot = false)
        {
            if (subscribed)
                _DesubscribeToEvents();

            base.SetPlayer(player, isBot);
            
            this.isBot = isBot;
            NightPick = -1;

            _SubscribeToEvents();
        }

        public sealed override void Clear()
        {
            if (subscribed)
                _DesubscribeToEvents();

            base.Clear();
        }

        public abstract override string ToString();

    }
}
