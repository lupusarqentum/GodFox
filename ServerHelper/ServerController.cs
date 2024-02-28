using System.Collections.Generic;
using MafiaRules;

namespace ServerHelper
{
    public sealed partial class ServerController
    {
        private readonly IServerTools vk;
        
        private readonly Dictionary<string, ICommand> commandsWithoutParameters;
        private readonly List<ICommand> commandsWithParameters;
        
        public long CreatorId => vk.CreatorID;
        public string Str_CreatorId => vk.CreatorID2;

        public string GroupName => vk.GroupName;

        public ServerController(IServerTools vk)
        {
            this.vk = vk;
            
            mafiaGames = new List<MafiaGame>();
            
            commandsWithoutParameters = new Dictionary<string, ICommand>();
            commandsWithParameters = new List<ICommand>();
        }

        public bool IsMessagesFromBotAllowed(long userID) => vk.IsMessageFromBotAllowed(userID);

        public string GetUserNickname(long userID) => GetFLName(userID);

        public string GetFLName(long userID) => GetUserName(userID) + " " + GetUserLastname(userID);

        private string GetUserName(long userID) => vk.GetUserName(userID);

        private string GetUserLastname(long userID) => vk.GetUserLastname(userID);

        public void SendMessage(long peer, string message)
        {
            if (peer > 0)
                vk.SendMessage(peer, message);
        }

        public void SendMessageAndKeyboard(in long peer, in string message, in BotButton[][] buttons, in bool inline = true) 
            => vk.SendMessageAndKeyboard(peer, message, buttons, inline);

        public List<string> GetChatUsersNames(in long peer) => vk.GetChatUsersNames(peer);
        
    }
}
