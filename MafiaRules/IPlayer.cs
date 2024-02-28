namespace MafiaRules
{
    public interface IPlayer
    {
        void SetRole(IRole role);
        bool IsAlive { get; set; }
        long userID { get; set; }
        IRole Role { get; set; }
        MafiaGame Game { get; }
        Teams Team { get; set; }
        void Clear();

    }
}
