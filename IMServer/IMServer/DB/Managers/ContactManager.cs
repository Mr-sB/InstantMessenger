using IMCommon.DB.Models;
using System.Collections.Generic;

namespace IMServer.DB.Managers
{
    public static class ContactManager
    {
        public static Contact GetContact(string username, string contactUsername)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<Contact>().Where(x => x.Username == username
                    && x.ContactUsername == contactUsername);
                var list = res.List();
                if (list != null && list.Count > 0) return list[0];
                return null;
            });
        }

        public static List<Contact> GetContactsByUsername(string username)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<Contact>().Where(x => x.Username == username);
                if (res.List() is List<Contact> list && list.Count >= 0) return list;
                return null;
            });
        }

        public static void AddContact(Contact contact)
        {
            //重复添加
            if (GetContact(contact.Username, contact.ContactUsername) != null) return;
            NHibernateHelper.OpenDB(session =>
            {
                session.Save(contact);
            });
        }

        public static void DeleteContact(string username, string contactUsername)
        {
            NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<Contact>().Where(x => x.Username == username
                    && x.ContactUsername == contactUsername);
                var list = res.List();
                if (list == null || list.Count <= 0) return;
                session.Delete(list[0]);
            });
        }
    }
}
