using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MafiaRules;
using ServerHelper;
using ServerHelper.Commands;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace GodFox
{
    
    internal class VkLongpollServer : IServerTools
    {

        public string Token => "92fd1df6e8c17f7be58e23c6bb27268214c826ebf3a25309fec69ecf682f5e40c57f93ec4ffd809649f49";
        public string GroupName => "@" + GroupNameWithoutAT;
        public string GroupNameWithoutAT => "krestoffox";
        public string GroupNameTech => "[club196065880|]";
        public long GroupID => 196065880L;
        public long CreatorID => 312950325L;

        public string CreatorID2 => "@grloboda";

        public int MaximumRepeatsOfCommand => 15;

        private readonly VkApi vk;
        private readonly ServerController controller;

        public VkLongpollServer()
        {
            vk = new VkApi();
            controller = new ServerController(this);
        }

        public void Start()
        {
            Console.WriteLine("Инициализация команд... ");
            InitCommands();

            Console.WriteLine("Команды проинициализированны, авторизация... ");
            vk.Authorize(new ApiAuthParams() { AccessToken = Token });
            Console.WriteLine("Бот авторизован! ");
            Console.WriteLine("Запуск второго потока... ");
            var thread = new Thread(GameChecking) { Name = "NightnDayChanging" };
            thread.Start();
            Console.WriteLine("Второй поток запущен... ");
            Console.WriteLine("Первый цикл... ");

            while (true)
            {
                var longpoll = vk.Groups.GetLongPollServer((ulong)GroupID);
                var poll = vk.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams
                {
                    Server = longpoll.Server,
                    Ts = longpoll.Ts,
                    Key = longpoll.Key,
                    Wait = 25
                });

                if (poll?.Updates == null)
                    continue;

                foreach (var e in poll.Updates)
                {
                    if (e.Type == GroupUpdateType.MessageNew)
                    {
                        var text = e.MessageNew.Message.Text;
                        var chat = (long)e.MessageNew.Message.PeerId;
                        var user = (long)e.MessageNew.Message.FromId;
                        var source = e.MessageNew.Message.Id == 0 ? MessageSource.Chat : MessageSource.Private;

                        MessageNewProccesingAsync(new MessageEvent(text, chat, user, source));
                    }
                    else if (e.Type == GroupUpdateType.MessageDeny)
                    {
                        if (e.MessageDeny.UserId.HasValue)
                            controller.UserDeniesMessages((long)e.MessageDeny.UserId);
                    }
                }
            }
        }

        private async void MessageNewProccesingAsync(MessageEvent boxedEvent)
            => await Task.Run(() => MessageNewProccesing(boxedEvent));

        private void MessageNewProccesing(MessageEvent boxedEvent)
        {
            var result = controller.MessageInputEvent(boxedEvent);
            switch (result)
            {
                case MessageInputResults.SystemException:
                    SendMessage(boxedEvent.peer, "Ошибка!");
                    SendSticker(boxedEvent.peer, 55);
                    break;
                case MessageInputResults.InvalidSource:
                    SendMessage(boxedEvent.peer, "Эту команду можно использовать только в " + boxedEvent.source.Invert().ToString2());
                    break;
            }
        }

        private void InitCommands()
        {
            controller.AddCommand(new ICommand[] {
                new BotAppendingCommand(), new BotRemovingCommand(), new ForsiblyStartGameCommand(),
                new GameAcceptingCommand(), new GameRejectingCommand(), new GameResetCommand(), 
                new GameStartCommand(), new GetGameInfoCommand(), new GetMyRoleCommand(), 
                new MafiaChatCommand(), new PetFoxCommand(), new PlayerVoiceCommand(), new ShippingCommand(), 
                new GetHelpCommand(), new DebugCommand(),
            });

            GetHelpCommand.LinkToMafiaRules = "https://";
            GetHelpCommand.LinkToMafiaWithThisBot = "https://";
            GetHelpCommand.LinkToBotCommandsHelp = "https://";
            GetHelpCommand.LinkToRolesOfTheBot = "https://";
        }

        private void GameChecking()
        {
            while (true)
            {
                controller.CheckGames();
                Thread.Sleep(5000);
            }
        }

        public string GetUserLastname(long userID) 
            => userID > 0 ? GetUser(userID).LastName : "";

        public string GetUserName(long userID)
        {
            if (userID > 0)
                return GetUser(userID).FirstName;
            return Settings.GetBotName((int)-userID);
        }

        private User GetUser(long userID) 
            => vk.Users.Get(new long[] { userID }).FirstOrDefault();

        public bool IsMessageFromBotAllowed(long userID) =>
            vk.Messages.IsMessagesFromGroupAllowed((ulong) GroupID, (ulong) userID);

        public void SendMessage(in long peer, in string message)
        {
            try
            {
                vk.Messages.Send(new MessagesSendParams() { 
                    Message = Program.Rand() % 1000 < 3 ? "Лис рычит:\n" + message : message, PeerId = peer, RandomId = Program.Rand() 
                });
            }
            catch (PermissionToPerformThisActionException) { throw new MessageDeniedException(); }
        }

        public List<string> GetChatUsersNames(in long chatID)
        {
            GetConversationMembersResult chatMembers;
            try
            {
                chatMembers = vk.Messages.GetConversationMembers(chatID);
            }
            catch(ConversationAccessDeniedException)
            {
                return new List<string>() { "denied" };
            }
            var names = new List<string>();

            foreach (var item in chatMembers.Items)
                names.Add(GetUserName(item.MemberId) + " " + GetUserLastname(item.MemberId));

            return names;
        }

        public void SendSticker(in long peer, in uint sticker)
        {
            try
            {
                vk.Messages.Send(new MessagesSendParams() { StickerId = sticker, PeerId = peer, RandomId = Program.Rand() });
            }
            catch (PermissionToPerformThisActionException)
            {
                throw new MessageDeniedException();
            }
        }

        public void SendMessageAndKeyboard(in long peer, in string message, BotButton[][] buttons, bool inline = true)
        {
            var keyboard = ShapeKeyboard(buttons, inline);
            try
            {
                vk.Messages.Send(new MessagesSendParams()
                {
                    PeerId = peer,
                    Message = message,
                    Keyboard = keyboard,
                    RandomId = Program.Rand(),
                });
            }
            catch (PermissionToPerformThisActionException)
            {
                throw new MessageDeniedException();
            }
        }

        private MessageKeyboard ShapeKeyboard(BotButton[][] keybuttons, bool inline)
        {
            var buttons = new List<List<MessageKeyboardButton>>();

            foreach (var botbuttons in keybuttons)
            {
                var buttonsList = new List<MessageKeyboardButton>();
                foreach (var botbutton in botbuttons)
                    buttonsList.Add(new MessageKeyboardButton() 
                        { Color = botbutton.color.ConvertToVkNetFormat(), Action = new MessageKeyboardButtonAction() 
                            { Label = botbutton.text, Type = KeyboardButtonActionType.Text }
                    });
                buttons.Add(buttonsList);
            }

            return new MessageKeyboard { Inline = inline, Buttons = buttons, };
        }

    }

    internal static class BotKeyboardColorsExtension
    {
        internal static KeyboardButtonColor ConvertToVkNetFormat(this BotButtonColors self)
        => self switch
        {
            BotButtonColors.Default => KeyboardButtonColor.Default,
            BotButtonColors.Primary => KeyboardButtonColor.Primary,
            BotButtonColors.Positive => KeyboardButtonColor.Positive,
            _ => KeyboardButtonColor.Negative,
        };
    }

}
