using IMCommon.DB.Models;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace IMServer.DB.Managers
{
    public static class UserManager
    {
        public static User GetUser(string username)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<User>().Where(x => x.Username == username);
                var list = res.List();
                if (list != null && list.Count > 0) return list[0];
                return null;
            });
        }

        public static void AddUser(User user)
        {
            //重复添加
            if (GetUser(user.Username) != null) return;
            NHibernateHelper.OpenDB(session =>
            {
                session.Save(user);
            });
        }

        public static void UpdateUser(User user)
        {
            NHibernateHelper.OpenDB(session =>
            {
                session.Update(user);
            });
        }

        public static List<User> FuzzySearchByUsername(string username)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<User>().Where(Restrictions.Like(nameof(User.Username), username.ToLower(), MatchMode.Anywhere));
                if (res.List() is List<User> list && list.Count > 0) return list;
                return null;
            });
        }
    }
}
