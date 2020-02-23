using ESocket.Common.Tools;
using IMCommon.DB.Models;
using System.Collections.Generic;

namespace IMServer.DB.Managers
{
    public class ChatManager
    {
        public static List<Chat> GetRecordsBySendAndRecUsername(string sendUsername, string recUsername, int minDay, int maxDay)
        {
            return NHibernateHelper.OpenDB(session =>
            {
                var now = TimeUtil.GetCurrentUtcTime();
                var maxTime = now.AddDays(-minDay);
                var minTime = now.AddDays(-maxDay);
                var res = session.QueryOver<Chat>().Where(
                    x => x.SendUsername == sendUsername
                    && x.ReceiveUsername == recUsername
                    && x.Time <= maxTime
                    && x.Time > minTime);
                if (res.List() is List<Chat> list && list.Count > 0) return list;
                return null;
            });
        }

        public static void AddChat(Chat chat)
        {
            NHibernateHelper.OpenDB(session =>
            {
                session.Save(chat);
            });
        }
    }
}
