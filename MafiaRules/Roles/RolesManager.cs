using System;
using System.Collections.Generic;
using System.Reflection;

namespace MafiaRules.Roles
{

    public sealed class RolesManager
    {

        public static RolesManager instance;

        private readonly List<ConstructorInfo> roles;

        public IRole this[int id] => roles[id].Invoke(new object[] { }) as IRole;

        static RolesManager() => instance = new RolesManager();

        private RolesManager() => roles = new List<ConstructorInfo>();

        public void AddRole(Type role) => roles.Add(role.GetConstructor(new Type[] { }));

    }
}
