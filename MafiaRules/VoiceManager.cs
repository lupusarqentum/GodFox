using System.Collections.Generic;
using System.Text;

namespace MafiaRules
{
    public sealed class VoiceManager
    {
        public List<int> allowList;
        public List<int> voices;

        public List<int> arresting_all;

        public int tour;
        public bool voiceCrash;

        public List<long> yetVoiced; // проголосовавшие

        public VoiceManager()
        {
            voices = new List<int>();
            allowList = new List<int>();

            tour = 0;
            voiceCrash = false;

            arresting_all = new List<int>();
            yetVoiced = new List<long>();
        }

        public bool IsAllowVoice(int man, long userID)
            => allowList.IndexOf(man - 1) != -1 && yetVoiced.IndexOf(userID) == -1;

        public void Voice(int man, long userID)
        {
            voices[man - 1]++;
            yetVoiced.Add(userID);
        }

        public List<int> GetBestMans()
        {
            if (voiceCrash)
                return new List<int> { voices[0] > voices[1] ? Settings.ARREST_ALL : Settings.FREE_ALL };

            int max = voices[0];
            for (int i = 0; i < voices.Count; i++)
                if (voices[i] > max)
                        max = voices[i];

            var result = new List<int>();
            for (int i = 0; i < voices.Count; i++)
                if (voices[i] == max)
                    result.Add(i);

            return result;
        }

        public void Init(int manCount)
        {
            for (int i = 0; i < manCount; i++)
                voices.Add(0);
            for (int i = 0; i < manCount; i++)
                allowList.Add(i);
        }

        public void Retour(List<int> newAllow, int mansCount)
        {
            Reset();

            tour++;

            if (tour > 1 && newAllow.Count == 2 && mansCount > 3)
            {
                voiceCrash = true;
                allowList.Add(0);
                allowList.Add(1);
                voices.Add(0);
                voices.Add(1);

                arresting_all = newAllow;

                return;
            }

            for (int i = 0; i < newAllow.Count; i++)
                allowList.Add(newAllow[i]);

            for (int i = 0; i < mansCount; i++)
                voices.Add(0);
        }

        public void Reset()
        {
            voices = new List<int>();
            allowList = new List<int>();
            yetVoiced = new List<long>();
        }

        public void TrueReset()
        {
            Reset();
            voiceCrash = false;
            tour = 0;
            arresting_all = new List<int>();
        }

        public string GetCandidats()
        {
            var buffer = new StringBuilder();
            for (int i = 0; i < allowList.Count; i++)
                buffer.Append(i + ": " + voices[i] + ", ");
            return buffer.ToString();
        }

        public string ToString(List<IPlayer> players)
        {
            if (voiceCrash)
                return "1. Арестовать всех \n2. Никого не арестовать";

            var buffer = new StringBuilder();
            foreach (var allowed in allowList)
                buffer.Append($"{allowed + 1}. {players[allowed]}\n");

            return buffer.ToString();
        }

    }
}
