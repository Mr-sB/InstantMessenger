using FluentNHibernate.Mapping;
using IMCommon.DB.Models;

namespace IMServer.DB.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("user");
            Id(x => x.ID, "id");
            Map(x => x.Username, "username");
            Map(x => x.Password, "password");
            Map(x => x.Salt, "salt");
            Map(x => x.Nickname, "nickname");
            Map(x => x.SignUpTime, "signuptime");
        }
    }
}
