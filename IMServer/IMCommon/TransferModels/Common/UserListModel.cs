using System.Collections.Generic;

namespace IMCommon.TransferModels
{
    public class UserListModel
    {
        public List<UserModel> Users;
        public UserListModel() { }
        public UserListModel(List<UserModel> users)
        {
            Users = users;
        }
    }
}
