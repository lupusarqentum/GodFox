using MafiaRules.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaRules 
{
    public sealed class MafiaGame
    {
        public event EventHandler GameStart = (sender, e) => { };
        public event EventHandler MorningStart = (sender, e) => { };
        public event EventHandler DayStart = (sender, e) => { };
        public event EventHandler VoiceStepStart = (sender, e) => { };
        public event EventHandler NightStart = (sender, e) => { };
        public event EventHandler PlayerWin = (sender, e) => { };
        public event EventHandler PlayerLose = (sender, e) => { };

        public event EventHandler PlayerArrested = (sender, e) => { };

        public event EventHandler MafiaDeath = (sender, e) => { };
        public event EventHandler YakudzaDeath = (sender, e) => { };
        public event EventHandler SingletonDeath = (sender, e) => { };

        public event EventHandler GameEnded = (sender, e) => { };
        public event Action<long, string> MessageSending;

        public readonly object playersListLocker = new object();
        
        public RolesDistributor specialRolesList;

        private GamePhases _phase;
        public GamePhases Phase 
        {
            get { return _phase; }
            private set
            {
                if (_phase == GamePhases.PlayersSet)
                {
                    _phase = value;
                    SetPhaseDelay();
                    SendMessage($"Наступает первое утро. Оно длится {delay.ToString((int)Phase)} секунд.");
                    return;
                }

                _phase = value;
                switch (_phase)
                {
                    case GamePhases.Morning:
                        StartMorning();
                        break;
                    case GamePhases.Day:
                        StartDay();
                        break;
                    case GamePhases.Night:
                        StartNight();
                        break;
                }
                SetPhaseDelay();
            }
        }

        public readonly List<IPlayer> players;
        public readonly VoiceManager voicer;
        public readonly long chatID;
        public readonly PhasesDelay delay;
        
        public readonly int botNicknamesOffset;

        public int maxOnline;
        public int botsCount;

        public bool isMafiaPlayed;
        public bool isSinglesPlayed;
        public bool isYakudzaPlayed;

        public bool isMafiaPlayNow;
        public bool isSinglesPlayNow;
        public bool isYakudzaPlayNow;

        public DateTime nextPhaseTime;

        public byte civilSkips;
        public byte mafiaSkips;
        public byte yakudzaSkips;
        public byte singleSkips;

        public MafiaGame(long chatID, PhasesDelay delay)
        {
            this.delay = delay;
            players = new List<IPlayer>();

            voicer = new VoiceManager();

            this.chatID = chatID;
            Phase = GamePhases.PlayersSet;
            
            isMafiaPlayed = false;
            isSinglesPlayed = false;
            isYakudzaPlayed = false;

            isMafiaPlayNow = false;
            isSinglesPlayNow = false;
            isYakudzaPlayNow = false;

            maxOnline = 0;
            botsCount = 0;

            civilSkips = 0;
            mafiaSkips = 0;
            yakudzaSkips = 0;
            singleSkips = 0;

            botNicknamesOffset = Settings.Rand(0, Settings.MaxBotsCount);
        }

        public MafiaGame(long chatID, PhasesDelay delay, RolesDistributor specialRolesList) : this(chatID, delay)
        {
            this.specialRolesList = specialRolesList;
        }

        public int GetPlayerIndex(long userID)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].userID == userID)
                    return i;
            }
            throw new MafiaException($"{userID} don't playing the game {chatID}, but trying to get game index. ");
        }

        public bool AppendPlayer(string vkName, string nickname, long userID)
        {
            if (players.Count < Settings.MaxPlayersCount && Phase == GamePhases.PlayersSet)
            {
                players.Add(new MafiaPlayer(vkName, nickname, userID, this));
                return true;
            }
            
            return false;
        }

        public bool AppendBot()
        {
            if (botsCount < Settings.MaxBotsCount &&
                players.Count < Settings.MaxPlayersCount && Phase == GamePhases.PlayersSet)
            {
                botsCount++;

                var botName = GetLastBotInGameNickname();
                players.Add(new BotPlayer(botName, Settings.BOT_ID_MODIFICATOR * botsCount, this));
                return true;
            }
            
            return false;
        }

        public string GetLastBotInGameNickname() => Settings.GetBotName(botsCount + botNicknamesOffset);

        public bool IsUserPlaying(long userID)
        {
            foreach (var player in players)
                if (player.userID == userID)
                    return true;

            return false;
        }

        public bool GetPlayer(long userID, out IPlayer outPlayer)
        {
            foreach (var player in players)
            {
                if (player.userID == userID)
                {
                    outPlayer = player;
                    return true;
                }
            }

            outPlayer = null;
            return false;
        }

        public IPlayer GetPlayer(long userID)
        {
            foreach (var player in players)
                if (player.userID == userID)
                    return player;

            throw new MafiaException($"User {userID} is not playing this game. ");
        }

        public void RemovePlayer(long userID)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].userID == userID)
                {
                    if (players[i] is BotPlayer)
                        botsCount--;

                    players[i].Clear();
                    players.RemoveAt(i);
                    return;
                }
            }

            throw new MafiaException($"Cannot find user id in {chatID} to remove in list!");
        }

        public bool RemoveBot()
        {
            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] is BotPlayer)
                {
                    RemovePlayer(players[i].userID);
                    return true;
                }
            }
            
            return false;
        }

        public void PrestartGame()
        {
            if (Phase != GamePhases.PlayersSet)
                throw new MafiaException("Game is started yet");

            Phase++;
            maxOnline = players.Count;
            GameStart.Invoke(this, EventArgs.Empty);
        }

        public bool StartGame()
        {
            if (players.Count < Settings.MinimumPlayersCount)
            {
                SendMessage($"Недостаточно игроков для начала игры. Собрано всего {players.Count}, а нужно {Settings.MinimumPlayersCount}. ");
                return false;
            }

            PrestartGame();
            DistributeRoles(specialRolesList);
            SendMessage("Игра началась. Среди Вас: " + specialRolesList.ToStringFor(players.Count));
            return true;
        }

        public void DistributeRoles(RolesDistributor rolesList)
        {
            var roles = rolesList.GetSpecialRolesFor(players.Count);

            if (roles.Length > players.Count)
                throw new MafiaException("Special roles count greater than players count");

            players.Shuffle();

            var mafias = new List<int>();
            var mafiaBuffer = new StringBuilder("Состав мафии на сегодня: \n\n");
            var yakudzas = new List<int>();
            var yakudaBuffer = new StringBuilder("Состав мафии на сегодня: \n\n");

            for (int i = 0; i < roles.Length; i++)
            {
                IRole specRole;
                try
                {
                    specRole = roles[i];
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }

                switch (specRole.Team)
                {
                    case Teams.Mafia:
                        isMafiaPlayed = true;
                        isMafiaPlayNow = true;
                        mafias.Add(i);
                        mafiaBuffer.Append(players[i].ToString() + "\n");
                        break;
                    case Teams.Yakudza:
                        isYakudzaPlayed = true;
                        isYakudzaPlayNow = true;
                        yakudzas.Add(i);
                        yakudaBuffer.Append(players[i].ToString() + "\n");
                        break;
                    case Teams.Single:
                        isSinglesPlayed = true;
                        isSinglesPlayNow = true;
                        break;
                }

                players[i].SetRole(specRole);
            }

            for (int i = roles.Length; i < players.Count; i++)
                players[i].SetRole(new CivillianRole());

            foreach (var index in mafias)
                SendMessage(players[index].userID, mafiaBuffer.ToString());
            foreach (var index in yakudzas)
                SendMessage(players[index].userID, yakudaBuffer.ToString());

            players.Shuffle();
            
            specialRolesList = rolesList;
        }

        public void CheckTime()
        {
            if (DateTime.Now > nextPhaseTime)
            {
                if (NextPhase()) // NextPhase returs false if game is players setting and it doesn't starts, otherwise true
                {
                    var win = GetWinnedTeam();
                    if (win != Teams.None)
                        EndGame(win);
                }
            }
        }

        public void EndGame(Teams win)
        {
            var buffer = new StringBuilder(Helper.GetWinTextFromTeam(win) + "\nПобедители: ");
            while (players.Count > 0)
            {
                if (players[0].Team == win)
                {
                    buffer.Append($"{players[0]} ({players[0].Role}), ");
                    MakePlayerWinner(players[0].userID);
                }
                else
                {
                    MakePlayerLoser(players[0].userID);
                }
            }
            
            buffer.Remove(buffer.Length - 2, 2);

            SendMessage(buffer.ToString());

            GameEnded.Invoke(this, new GameEndedEventArgs(win));
        }

        public Teams GetWinnedTeam()
        {
            int mafias = 0, civils = 0, yakudzas = 0, singles = 0;
            foreach (var player in players)
            {
                switch (player.Team)
                {
                    case Teams.Civillian:
                        civils++;
                        break;
                    case Teams.Mafia:
                        mafias++;
                        break;
                    case Teams.Yakudza:
                        yakudzas++;
                        break;
                    case Teams.Single:
                        singles++;
                        break;
                    case Teams.None:
                    default:
                        throw new MafiaException("Player with team none in players list: " + player.ToString());
                }
            }

            int halfPlayersCount = players.Count % 2 == 0 ? players.Count / 2 : (players.Count + 1) / 2;
            
            if (mafias >= halfPlayersCount)
                return Teams.Mafia;
            if (yakudzas >= halfPlayersCount)
                return Teams.Yakudza;
            if (singles >= halfPlayersCount)
                return Teams.Single;
            if (mafias + yakudzas + singles == 0)
                return Teams.Civillian;
            if (mafias + yakudzas + singles >= halfPlayersCount)
            {
                if (mafias > yakudzas) return Teams.Mafia;
                if (yakudzas + singles > mafias && yakudzas > 0) return Teams.Yakudza;
                if (singles > 0) return Teams.Single;
            }
            
            return Teams.None;
        }

        public bool NextPhase() // NextPhase returs false if game is players setting and it doesn't starts, otherwise true
        {
            switch (Phase)
            {
                case GamePhases.Night:
                    Phase = GamePhases.Morning;
                    return true;
                case GamePhases.PlayersSet:
                    if (players.Count >= Settings.MinimumPlayersCount)
                    {
                        StartGame();
                        return true;
                    }
                    else
                    {
                        SendMessage("Недостаточно игроков для начала игры. ");
                        GameEnded.Invoke(this, new GameEndedEventArgs(Teams.None));
                        return false;
                    }
                case GamePhases.Day:
                    FinishDay();
                    return true;
                default:
                    Phase++;
                    return true;
            }
        }

        public void PlayerPick(long userID, int pick)
        {
            if (!IsUserPlaying(userID))
                return;

            if (pick - 1 < 0 || pick - 1 >= players.Count)
            {
                SendMessage(userID, "Некорректный номер игрока. ");
                return;
            }

            var picker = GetPlayer(userID);
            SendMessage(userID, picker.Role.Pick(Phase, pick));
        }

        public void PlayerVoiceFor(long userID, int pick)
        {
            if (!IsUserPlaying(userID))
                return;

            if (Phase == GamePhases.Day)
            {
                if (GetPlayer(userID) is BotPlayer)
                {
                    if (voicer.IsAllowVoice(pick, userID))
                    {
                        voicer.Voice(pick, userID);
                        SendMessage($"{HtmlSmiles.Robot}Бот {GetPlayer(userID)} проголосовал за " +
                            $"{(voicer.voiceCrash ? pick.ToString() : players[pick - 1].ToString())}");
                    }

                    return;
                }

                if (voicer.IsAllowVoice(pick, userID))
                {
                    voicer.Voice(pick, userID);
                    SendMessage(HtmlSmiles.CheckMark + GetPlayer(userID).ToString() + ", Ваш голос учтён!");
                }
                else
                {
                    SendMessage("Некорректная команда!");
                }
            }
            else
            {
                SendMessage("Голосовать можно только днём!");
            }
        }

        public void PlayerSendsMessageToMafia(long userID, string message)
        {
            var sender = GetPlayer(userID);
            if (sender.Team != Teams.Mafia && sender.Team != Teams.Yakudza)
            {
                SendMessage(userID, "Эта команда не для мирных или одиночных игроков. ");
                return;
            }

            var finalMessage = $"Сообщение от игрока {sender}: \n{message}";
            var buffer = new StringBuilder("Ваше сообщение отправлено игрокам: ");
            var single = true;
            foreach (var player in players)
            {
                if (player.Team == sender.Team)
                {
                    SendMessage(player.userID, finalMessage);
                    buffer.Append(player.ToString() + ", ");
                    single = false;
                }
            }

            if (!single)
            {
                buffer.Remove(buffer.Length - 2, 2).Append(". ");
                SendMessage(userID, buffer.ToString());
            }
            else
            {
                SendMessage(userID, "Некому отправлять ваше сообщение. ");
            }
        }

        public void MakePlayerWinner(long userID)
        {
            SendMessage(userID, Settings.WinText);
            PlayerWin.Invoke(this, new PlayerQuitGameEventArgs(GetPlayer(userID)));
            RemovePlayer(userID);
        }

        public void MakePlayerLoser(long userID, bool sendMessageAboutLosing = true)
        {
            if (sendMessageAboutLosing) 
                SendMessage(userID, Settings.LoseText);

            var player = GetPlayer(userID);

            PlayerLose.Invoke(this, new PlayerQuitGameEventArgs(player));

            if (player.Team == Teams.Mafia)
                MafiaDeath.Invoke(this, new PlayerQuitGameEventArgs(player));
            else if (player.Team == Teams.Yakudza)
                YakudzaDeath.Invoke(this, new PlayerQuitGameEventArgs(player));
            else if (player.Team == Teams.Single)
                SingletonDeath.Invoke(this, new PlayerQuitGameEventArgs(player));

            RemovePlayer(userID);
        }

        public void StartDay()
        {
            voicer.TrueReset();
            voicer.Init(players.Count);
            
            DayStart.Invoke(this, new GamePhaseStartEventArgs(GamePhases.Day));
            SendMessage($"{HtmlSmiles.Libra}Наступает день. Проводится голосование, выберите из списка ниже, того кандидата, " +
                "которого подозреваете больше всего: \n\n" + voicer.ToString(players) + 
                $"\n\nУ вас есть {delay.ToString((int)Phase)} секунд{HtmlSmiles.Libra}.");
            
            var allowList = new List<int>();
            for (int i = 0; i < players.Count; i++)
                allowList.Add(i);
            VoiceStepStart.Invoke(this, new VoiceStepEventArgs(allowList));
        }

        public void FinishDay()
        {
            var best = voicer.GetBestMans();
            if (best.Count == 1)
            {
                if (best[0] < -1)
                {
                    if (best[0] == Settings.ARREST_ALL)
                    {
                        var buffer = new StringBuilder(HtmlSmiles.Libra + "Голосование подошло к концу. Арестованы: \n");

                        foreach (var man in voicer.arresting_all)
                            if (TryArrest(players[man]))
                                buffer.Append(players[man].ToString() + " \n");
                        
                        SendMessage(buffer.ToString());
                    }
                    else if (best[0] == Settings.FREE_ALL)
                    {
                        SendMessage(HtmlSmiles.Libra + "Голосование подошло к концу. Никто не арестован!");
                        civilSkips++;
                    }
                }
                else
                {
                    var man = players[best[0]];
                    if (TryArrest(man))
                        SendMessage(HtmlSmiles.Libra + "Голосование подошло к концу. \nАрестован: " + man.ToString());
                }

                if (civilSkips == 3)
                    KillTeam(HtmlSmiles.ExclamationPoint + "Три раза за игру никто не был арестован. " +
                        "Команда мирных проигрывает полным составом: ", Teams.Civillian);

                if (GetWinnedTeam() == Teams.None)
                    Phase++;
                else
                    SetPhaseDelay();
            }
            else
            {
                voicer.Retour(best, players.Count);
                SendMessage(HtmlSmiles.Libra + "Несколько кандидатов набрали одинаковое количество голосов. Проголосуйте, " +
                    "пожалуйста, снова: \n\n" + voicer.ToString(players) + $"\n\nУ вас есть {delay.ToString((int)Phase)} секунд.");
                VoiceStepStart.Invoke(this, new VoiceStepEventArgs(best));

                SetPhaseDelay();
            }
        }

        public bool TryArrest(IPlayer player)
        {
            if (player.Role.Arrest())
            {
                MakePlayerLoser(player.userID);
                PlayerArrested.Invoke(this, new PlayerQuitGameEventArgs(player));
                return true;
            }
            return false;
        }

        public void StartNight()
        {
            NightStart.Invoke(this, new GamePhaseStartEventArgs(GamePhases.Night));
            SendMessage(HtmlSmiles.Night + "Наступает ночь, город засыпает... " + HtmlSmiles.Night +
                "\nНо некоторые сегодня не спят, им я отправил письма в л/c, прошу ответить на них до рассвета :)\nНочь длится " +
                $"{delay.ToString((int)Phase)} секунд.");
        }

        public void StartMorning()
        {
            var killings = new List<Killing>();
            var news = new StringBuilder("Важные новости с прошедшей ночи: \n\n");

            news.Append(GetTeamKillings(killings, GetMafiaKilling(), ref mafiaSkips, "Мафия"));
            news.Append(GetTeamKillings(killings, GetYakudzaKilling(), ref yakudzaSkips, "Якудза"));

            foreach (var kill in GetSinglesKillings())
                news.Append(GetTeamKillings(killings, kill, ref singleSkips, "Маньяк"));

            var good = true;
            foreach (var kill in killings)
            {
                if (kill.killingResult == KillingResult.Killed)
                {
                    good = false;
                    break;
                }
            }

            var answer = $"{HtmlSmiles.Sunset}Наступает {(good ? " доброе " : "")}утро.{HtmlSmiles.Sunset}\n\n" 
                + news.ToString() + $"\n\nНа дневное обсуждение у Вас есть {delay.ToString((int)Phase)} секунд";
            SendMessage(answer);

            if (mafiaSkips == 3 && isMafiaPlayNow)
                KillTeam(HtmlSmiles.Alcohol + "Мафия промахнулась три раза за всю игру. " +
                    "Команда мафии проигрывает полным составом: ", Teams.Mafia);

            if (yakudzaSkips == 3 && isYakudzaPlayNow)
                KillTeam(HtmlSmiles.Alcohol + "Якудза промахнулась три раза за всю игру. " +
                    "Команда якудзы проигрывает полным составом: ", Teams.Yakudza);

            if (singleSkips == 3 && isSinglesPlayNow)
                KillTeam(HtmlSmiles.Alcohol + "Маньяк промахнулся три раза за всю игру. " +
                    "Команда маньяков проигрывает полным составом: ", Teams.Single);
            
            MorningStart.Invoke(this, new GamePhaseStartEventArgs(GamePhases.Morning, killings));
            DoMorningKills(killings);
        }

        private string GetTeamKillings(in List<Killing> killings, in Killing kill, ref byte skipsCounter, in string teamName)
        {
            killings.Add(kill);
            var sacrificeName = kill.sacrificeID != -1 ? GetPlayer(kill.sacrificeID).ToString() : "";
            if (kill.killingResult == KillingResult.Miss)
                skipsCounter++;
            return kill.ToString(teamName, sacrificeName);
        }

        private void KillTeam(in string opening, in Teams team)
        {
            var buffer = new StringBuilder(opening);

            foreach (var item in players)
            {
                if (item.Team == team)
                {
                    buffer.Append($"{item} ({item.Role}), ");
                    item.IsAlive = false;
                }
            }

            buffer.Remove(buffer.Length - 2, 2);
            SendMessage(buffer.ToString());

            ClearNotAlivePlayers();
        }

        public void DoMorningKills(List<Killing> killings)
        {
            foreach (var killing in killings)
                if (killing.killingResult == KillingResult.Killed && GetPlayer(killing.sacrificeID, out IPlayer sacrifice))
                    sacrifice.IsAlive = false;

            ClearNotAlivePlayers();
        }

        public void ClearNotAlivePlayers()
        {
            int i = 0;
            while (i < players.Count)
            {
                if (!players[i].IsAlive)
                {
                    MakePlayerLoser(players[i].userID);
                    continue;
                }
                i++;
            }
        }

        public Killing GetMafiaKilling()
        {
            return GetSomeTeamKilling(isMafiaPlayed, ref isMafiaPlayNow, Teams.Mafia);
        }

        public Killing GetYakudzaKilling()
        {
            return GetSomeTeamKilling(isYakudzaPlayed, ref isMafiaPlayNow, Teams.Yakudza);
        }

        public List<Killing> GetSinglesKillings()
        {
            if (!isSinglesPlayed)
                return new List<Killing>() { new Killing(KillingResult.Undefined) };

            List<Killing> killings = new List<Killing>();

            foreach (var player in players)
            {
                if (player.Team == Teams.Single)
                {
                    if (player.Role.NightPick == -1)
                    {
                        killings.Add(new Killing(KillingResult.Miss));
                        continue;
                    }

                    var sacrifice = player.Role.NightPick;
                    killings.Add(new Killing(players[sacrifice].Role.Kill(), sacrifice));
                }
            }

            if (killings.Count == 0)
                isSinglesPlayNow = false;

            return killings;
        }
        
        public Killing GetSomeTeamKilling(bool isExist, ref bool isAlive, Teams team)
        {
            if (!isExist)
                return new Killing(KillingResult.Undefined);

            int mafiasPick = 0;
            bool mafiaFinded = false;
            foreach (var player in players)
            {
                if (player.Team == team)
                {
                    if (!mafiaFinded)
                    {
                        mafiaFinded = true;
                        mafiasPick = player.Role.NightPick;
                    }
                    if (player.Role.NightPick != mafiasPick || player.Role.NightPick == -1)
                    {
                        return new Killing(KillingResult.Miss);
                    }
                }
            }

            if (mafiaFinded)
            {
                return new Killing(players[mafiasPick].Role.Kill(), players[mafiasPick].userID);
            }
            else
            {
                isAlive = false;
                return new Killing(KillingResult.Miss);
            }
        }

        public void SendMessage(long peer, string message)
        {
            MessageSending?.Invoke(peer, message);
        }

        public void SendMessage(string message) => SendMessage(chatID, message);

        private void SetPhaseDelay() =>
            nextPhaseTime = DateTime.Now.AddSeconds(delay[Phase]);

        public string GetPlayersList(string separator, string end, bool addNumbers)
        {
            var buffer = new StringBuilder();
            for (int i = 0; i < players.Count; i++)
            {
                buffer.Append(HtmlSmiles.CheckMark);
                if (addNumbers)
                    buffer.Append(i + 1);
                buffer.Append(". " + players[i].ToString());
                if (i + 1 != players.Count)
                    buffer.Append(separator);
            }
            return buffer.Append(end).ToString();
        }

        public string GetNightPickPlayers()
        {
            var buffer = new StringBuilder();
            foreach (var player in players)
                buffer.Append(player.ToString() + ": " + player.Role.NightPick);
            return "Ночные выборки\n" + buffer.ToString();
        }

        public string GetDayChoicePlayers() => "Голоса\n" + voicer.GetCandidats();

        public string GetPlayersRoleDebug() 
        {
            var buffer = new StringBuilder();
            foreach (var player in players)
                buffer.Append(player.ToString() + ": " + player.Role.ToString() + "|;");
            return "Роли:\n" + buffer.ToString();
        }

        public override string ToString()
        {
            var buffer = new StringBuilder(HtmlSmiles.InfoPoint + "Информация о проходящей игре: \n\n");
            buffer.Append($"Сейчас {Phase.ToWord()}. \n");
            buffer.Append($"В живых осталось {players.Count} игроков из {maxOnline}. \n\n");
            buffer.Append("Текущие игроки: \n\n");
            for (int i = 0; i < players.Count; i++)
            {
                buffer.Append(HtmlSmiles.CheckMark);
                buffer.Append($"{i + 1}. {players[i]}\n");
            }

            return buffer.ToString();
        }

    }
}
