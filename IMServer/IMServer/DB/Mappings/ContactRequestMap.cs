using FluentNHibernate.Mapping;
using IMCommon.DB.Models;

namespace IMServer.DB.Mappings
{
    public class ContactRequestMap : ClassMap<ContactRequest>
    {
        public ContactRequestMap()
        {
            Table("contactrequest");
            Id(x => x.ID, "id");
            References(x => x.RequestUser, "requestusername");
            Map(x => x.ContactUsername, "contactusername");
        }
    }
}
