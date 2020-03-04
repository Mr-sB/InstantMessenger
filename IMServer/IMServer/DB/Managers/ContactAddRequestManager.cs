using IMCommon.DB.Models;
using System.Collections.Generic;

namespace IMServer.DB.Managers
{
    public static class ContactAddRequestManager
    {
        public static ContactAddRequest GetContactAddRequest(string requestUsername, string contactUsername)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<ContactAddRequest>().Where(x => x.RequestUsername == requestUsername
                    && x.ContactUsername == contactUsername);
                var list = res.List();
                if (list != null && list.Count > 0) return list[0];
                return null;
            });
        }

        public static void AddContactAddRequest(ContactAddRequest contactAddRequest)
        {
            //重复添加
            if (GetContactAddRequest(contactAddRequest.RequestUsername, contactAddRequest.ContactUsername) != null) return;
            NHibernateHelper.OpenDB(session =>
            {
                session.Save(contactAddRequest);
            });
        }

        public static void DeleteContactAddRequest(string requestUsername, string contactUsername)
        {
            NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<ContactAddRequest>().Where(x => x.RequestUsername == requestUsername
                    && x.ContactUsername == contactUsername);
                var list = res.List();
                if (list == null || list.Count <= 0) return;
                session.Delete(list[0]);
            });
        }

        public static void UpdateContactAddRequest(ContactAddRequest contactAddRequest)
        {
            NHibernateHelper.OpenDB(session =>
            {
                session.Update(contactAddRequest);
            });
        }

        public static List<ContactAddRequest> GetContactAddRequestList(string requestUsername)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<ContactAddRequest>().Where(
                    x => x.RequestUsername == requestUsername);
                if (res.List() is List<ContactAddRequest> list && list.Count > 0) return list;
                return null;
            });
        }
        
        public static List<ContactAddRequest> GetContactAddContactedList(string contactUsername)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var res = session.QueryOver<ContactAddRequest>().Where(
                    x => x.ContactUsername == contactUsername);
                if (res.List() is List<ContactAddRequest> list && list.Count > 0) return list;
                return null;
            });
        }
    }
}
