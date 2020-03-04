using FluentNHibernate.Mapping;
using IMCommon.DB.Models;

namespace IMServer.DB.Mappings
{
    public class ContactAddRequestMap : ClassMap<ContactAddRequest>
    {
        public ContactAddRequestMap()
        {
            Table("contactaddrequest");
            Id(x => x.ID, "id");
            Map(x => x.RequestUsername, "requestusername");
            Map(x => x.ContactUsername, "contactusername");
            Map(x => x.ResponseCode, "responsecode");
        }
    }
}
