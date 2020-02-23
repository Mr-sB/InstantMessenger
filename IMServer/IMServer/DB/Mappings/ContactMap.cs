using FluentNHibernate.Mapping;
using IMCommon.DB.Models;

namespace IMServer.DB.Mappings
{
    public class ContactMap : ClassMap<Contact>
    {
        public ContactMap()
        {
            Table("contact");
            Id(x => x.ID, "id");
            Map(x => x.Username, "username");
            References(x => x.ContactUser, "contactusername");
        }
    }
}
