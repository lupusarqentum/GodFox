using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MafiaRules
{
    public class RolesDistributor
    {

        public class RolesLine
        {

            public readonly int playersCount;
            public readonly IRole[] roles;

            public int Length => roles.Length;

            public IRole this[int index] => roles[index];

            public RolesLine(int playersCount, IRole[] roles)
            {
                this.playersCount = playersCount;
                this.roles = roles;
            }

            public override string ToString()
            {
                var buffer = new StringBuilder();

                foreach (var role in roles)
                {
                    buffer.Append(role.ToString() + ", ");
                }

                buffer.Append("остальные мирные жители. ");
                return buffer.ToString();
            }

        }
        
        public readonly List<RolesLine> roleLines;

        public RolesDistributor()
        {
            roleLines = new List<RolesLine>();
        }

        public RolesDistributor AddRolesLine(int players, IRole[] roles)
        {
            roleLines.Add(new RolesLine(players, roles));
            return this; // new RolesDistributor().AddRolesLine().AddRolesLine()....
        }
        
        public RolesLine GetSpecialRolesFor(int players)
        {
            for (int i = roleLines.Count - 1; i >= 0; i--)
                if (roleLines[i].playersCount <= players)
                    return roleLines[i];

            throw new MafiaException("Invalid players count. ");
        }

        public string ToStringFor(int players)
        {
            return GetSpecialRolesFor(players).ToString();
        }

    }
}
