using System;

namespace ServerHelper
{
    public sealed partial class ServerController
    {

        public void AddCommand(ICommand command)
        {
            if (command.WithoutParams)
            {
                foreach (var syno in command.Synonyms)
                    commandsWithoutParameters.Add(syno, command);
            }
            else
            {
                commandsWithParameters.Add(command);
            }
        }

        public void AddCommand(ICommand[] command)
        {
            foreach (var item in command)
                AddCommand(item);
        }

        public void UserDeniesMessages(long userID)
        {
            var itsGame = FindGameByPlayerID(userID);

            if (itsGame != null)
            {
                var itsIndex = itsGame.GetPlayerIndex(userID);
                SendMessage(itsGame.chatID, $"{itsGame.players[itsIndex]} запретил сообществу писать ему сообщения, поэтому он больше не участвует в игре!");
                itsGame.MakePlayerLoser(userID, false);
            }
        }



        public MessageInputResults MessageInputEvent(MessageEvent e)
        {
            var parseResult = ParseCommand(e, out int repeats, out string formattedCommand);
            if (parseResult != MessageInputResults.Sucess)
                return parseResult;

            if (commandsWithoutParameters.TryGetValue(formattedCommand, out ICommand commandToExe))
                return CommandExecute(commandToExe, e, "", repeats);

            foreach (var command in commandsWithParameters)
                foreach (var syno in command.Synonyms)
                    if (formattedCommand.Contains(syno))
                        return CommandExecute(command, e, formattedCommand.Replace(syno, ""), repeats);

            return UserPick(e, formattedCommand);
        }

        private MessageInputResults UserPick(MessageEvent e, string formattedCommand)
        {
            if (IsUserPlays(e.user) && e.source == MessageSource.Private)
            {
                formattedCommand = formattedCommand.Trim();

                int picking;
                try
                {
                    picking = int.Parse(formattedCommand);
                }
                catch (Exception)
                {
                    return MessageInputResults.CommandNotFound;
                }

                FindGameByPlayerID(e.user).PlayerPick(e.user, picking);
                return MessageInputResults.PlayerPicks;
            }

            return MessageInputResults.CommandNotFound;
        }

        internal MessageInputResults ParseCommand(MessageEvent e, out int repeats, out string formattedCommand)
        {
            formattedCommand = e.message.Trim(' ', '\n');
            repeats = 1;

            formattedCommand = formattedCommand.ToLower().
                            Replace(vk.GroupName, ""). // @krestoffox
                            Replace("*" + vk.GroupNameWithoutAT, ""). // *krestoffox
                            Replace(vk.GroupNameTech, "");

            if (formattedCommand.IndexOf('*') != -1)
            {
                var parts = formattedCommand.Split(new char[] { '*' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return MessageInputResults.SystemException;

                parts[0] = parts[0].Trim();
                parts[1] = parts[1].Trim();

                formattedCommand = parts[1];
                try
                {
                    repeats = int.Parse(parts[0]);
                }
                catch (Exception)
                {
                    return MessageInputResults.SystemException;
                }
            }

            formattedCommand = " " + formattedCommand.Trim() + " ";

            if (repeats > vk.MaximumRepeatsOfCommand)
                repeats = vk.MaximumRepeatsOfCommand;

            return MessageInputResults.Sucess;
        }

        internal MessageInputResults CommandExecute(ICommand command, MessageEvent e, string formattedCommand, int repeats)
        {
            if (!command.IsSourceValid(e.source))
                return MessageInputResults.InvalidSource;

            MessageInputResults result;
            if ((!command.Repeatable) && repeats > 1)
            {
                result = MessageInputResults.SucessWithRepeatsCountMismatch;
                repeats = 1;
            }
            else
            {
                result = MessageInputResults.Sucess;
            }

            for (int i = 0; i < repeats; i++)
            {
                try
                {
                    command.Execute(e, formattedCommand, this);
                }
                catch (MessageDeniedException)
                {
                    return result;
                }
            }

            return result;
        }

    }
}
