using FluentNHibernate.Mapping;
using IMCommon.DB.Models;

namespace IMServer.DB.Mappings
{
    public class ChatMap : ClassMap<Chat>
    {
        public ChatMap()
        {
            Table("chat");
            Id(x => x.ID, "id");
            Map(x => x.SendUsername, "sendusername");
            Map(x => x.ReceiveUsername, "receiveusername");
            Map(x => x.MessageType, "messagetype");
            Map(x => x.Message, "message");
            Map(x => x.Time, "time");
        }
    }
}
