using System;
using MafiaRules.Roles;

namespace MafiaRules
{

    public static class Settings
    {
        public static Func<int> RandomInteger;
        
        public static int MaxBotsCount { get; private set; }
        
        public static int MaxPlayersCount { get; private set; }
        
        public static int MinimumPlayersCount { get; private set; }
        
        public static string LoseText { get; private set; }
        
        public static string WinText { get; private set; }

        private static readonly string[] botNicknames;
        public static int BotNicknamesCount => botNicknames.Length;

        public static sbyte ARREST_ALL = -10;
        public static sbyte FREE_ALL = -100;
        public static sbyte BOT_ID_MODIFICATOR = -50;
        
        static Settings() 
        {
            LoseText = "К сожалению, Вы проиграли.\n Но не отчаивайтесь, в следующий раз Вы обязательно выиграйте!" + HtmlSmiles.CoolMan;
            WinText = HtmlSmiles.CoolMan + "Поздравляем, Вы выиграли!";

            botNicknames = new string[] { "Соня Долкайо", "Елизавета Анансо", "Марьяна Брайо", "Леонид Прутковский", "Клеопатра",
                "Борис Владимирович", "Мелания Соно", "Корней Корнеевич", "Рая Лавлейс", "Бонтик Вазовских",
                "Клара Печенька", "Константин Константинович", "Надежда Ада", "Доктор Лис", "Леди Корова",
                "Электричка Томасия", "Ренар Гуд", "Пётр Ботович I", "Ржавчик", "Марлон Вито", };

            MaxPlayersCount = 20;
            MaxBotsCount = 15;
            MinimumPlayersCount = 7;

            InitRoles();
        }

        private enum RolesIds
        {
            CivillianRoleId,
            DonRoleId,
            MafiaRoleId,
            SheriffRoleId,
            ManiacRoleId,
            DoctorRoleId,
            DeathlessRoleId,
            WeremafiaRoleId,
            JesterRoleId,
        }

        private static void InitRoles()
        {
            RolesManager.instance.AddRole(typeof(CivillianRole));
            RolesManager.instance.AddRole(typeof(DonRole));
            RolesManager.instance.AddRole(typeof(MafiaRole));
            RolesManager.instance.AddRole(typeof(SheriffRole));
            RolesManager.instance.AddRole(typeof(ManiacRole));
            RolesManager.instance.AddRole(typeof(DoctorRole));
            RolesManager.instance.AddRole(typeof(DeathlessRole));
            RolesManager.instance.AddRole(typeof(WeremafiaRole));
            RolesManager.instance.AddRole(typeof(JesterRole));
        }

        public static string GetBotName(int botsCount) => botNicknames[botsCount % botNicknames.Length];

        public static RolesDistributor GetNormalDistributor()
        {
            var roles = new RolesDistributor();
            
            roles.AddRolesLine(06, new IRole[] {
                GetRole(RolesIds.DonRoleId), GetRole(RolesIds.MafiaRoleId), GetRole(RolesIds.SheriffRoleId), 
            }); // from 6: don, mafia, sheriff
            roles.AddRolesLine(10, new IRole[] {
                GetRole(RolesIds.DonRoleId), GetRole(RolesIds.MafiaRoleId), GetRole(RolesIds.MafiaRoleId), GetRole(RolesIds.SheriffRoleId), 
            }); // from 10: don, 2 mafia, sheriff
            roles.AddRolesLine(12, new IRole[] {
                GetRole(RolesIds.DonRoleId), GetRole(RolesIds.MafiaRoleId), GetRole(RolesIds.MafiaRoleId), GetRole(RolesIds.SheriffRoleId),
                GetRole(RolesIds.DoctorRoleId), GetRole(RolesIds.ManiacRoleId), 
            }); // from 12: maniac, don, 2 mafia, sheriff, doctor
            roles.AddRolesLine(15, new IRole[] {
                GetRole(RolesIds.ManiacRoleId), GetRole(RolesIds.DonRoleId), GetRole(RolesIds.MafiaRoleId), GetRole(RolesIds.MafiaRoleId),
                GetRole(RolesIds.JesterRoleId), GetRole(RolesIds.WeremafiaRoleId), GetRole(RolesIds.SheriffRoleId), GetRole(RolesIds.DoctorRoleId),
                GetRole(RolesIds.DeathlessRoleId), 
            }); // from 15: maniac, don, 2 mafia, jester, weremafia, sheriff, doctor, deathless

            return roles;
        }

        private static IRole GetRole(RolesIds id) => RolesManager.instance[(int)id];

        public static int Rand(int min, int max) => min + (RandomInteger.Invoke() % (max - min));

    }
}
