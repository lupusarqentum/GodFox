using MafiaRules;
using System;
using System.Collections.Generic;

namespace ServerHelper
{
    public sealed partial class ServerController
    {

        public readonly List<MafiaGame> mafiaGames;

        public MafiaGame FindGameByPlayerID(long userID)
        {
            foreach (var item in mafiaGames)
                if (item.IsUserPlaying(userID))
                    return item;

            return null;
        }

        public bool IsUserPlays(long userID)
        {
            foreach (var item in mafiaGames)
                if (item.IsUserPlaying(userID))
                    return true;

            return false;
        }

        public MafiaGame FindGameByChatID(long chatID)
        {
            foreach (var item in mafiaGames)
                if (item.chatID == chatID)
                    return item;

            return null;
        }

        public bool IsGameExist(long chatID)
        {
            foreach (var item in mafiaGames)
                if (item.chatID == chatID)
                    return true;

            return false;
        }

        private void RemoveGameByIndex(int index) => mafiaGames.RemoveAt(index);

        public void RemoveFinishedGame(object sender, EventArgs e) // only event
        {
            var endedGameID = (sender as MafiaGame).chatID;

            for (int i = 0; i < mafiaGames.Count; i++)
            {
                if (mafiaGames[i].chatID == endedGameID)
                {
                    RemoveGameByIndex(i);
                    return;
                }
            }
        }

        public void AddGame(long chatID, PhasesDelay delay, RolesDistributor roles)
        {
            var game = new MafiaGame(chatID, delay, roles);

            game.MessageSending += SendMessage;
            game.GameEnded += RemoveFinishedGame;

            mafiaGames.Add(game);
        }

        public void CheckGames()
        {
            int i = 0;
            while (i < mafiaGames.Count)
            {
                try
                {
                    mafiaGames[i].CheckTime();
                }
                catch (MessageDeniedException)
                {
                    RemoveGameByIndex(i);
                    continue;
                }

                i++;
            }
        }

    }
}
