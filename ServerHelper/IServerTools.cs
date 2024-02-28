using System.Collections.Generic;

namespace ServerHelper
{
    public interface IServerTools
    {
        string Token { get; } // to vk connecting token
        string GroupName { get; } // @krestoffox
        string GroupNameWithoutAT { get; } // krestoffox
        string GroupNameTech { get; } // [club196065880|]
        long GroupID { get; } // 196065880
        long CreatorID { get; } // 312950325
        string CreatorID2 { get; } // @grloboda
        int MaximumRepeatsOfCommand { get; }

        void SendMessage(in long peer, in string message);
        void SendMessageAndKeyboard(in long peer, in string message, BotButton[][] buttons, bool inline = true);
        string GetUserName(long userID);
        string GetUserLastname(long userID);
        List<string> GetChatUsersNames(in long chatID);
        bool IsMessageFromBotAllowed(long userID);
    }

}
