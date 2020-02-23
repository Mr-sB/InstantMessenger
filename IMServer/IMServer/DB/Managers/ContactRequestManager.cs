using IMCommon.DB.Models;
using System.Collections.Generic;

namespace IMServer.DB.Managers
{
    public static class ContactRequestManager
    {
        public static ContactRequest GetContactRequest(string requestUsername, string contactUsername)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<ContactRequest>().Where(x => x.RequestUser.Username == requestUsername
                    && x.ContactUsername == contactUsername);
                var list = res.List();
                if (list != null && list.Count > 0) return list[0];
                return null;
            });
        }

        public static void AddContactRequest(ContactRequest contactRequest)
        {
            //重复添加
            if (GetContactRequest(contactRequest.RequestUser.Username, contactRequest.ContactUsername) != null) return;
            NHibernateHelper.OpenDB(session =>
            {
                session.Save(contactRequest);
            });
        }

        public static void DeleteContactRequest(string requestUsername, string contactUsername)
        {
            NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<ContactRequest>().Where(x => x.RequestUser.Username == requestUsername
                    && x.ContactUsername == contactUsername);
                var list = res.List();
                if (list == null || list.Count <= 0) return;
                session.Delete(list[0]);
            });
        }

        public static List<ContactRequest> GetContactRequestList(string contactUsername)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<ContactRequest>().Where(x => x.ContactUsername == contactUsername);
                if (res.List() is List<ContactRequest> list && list.Count > 0) return list;
                return null;
            });
        }
    }
}
