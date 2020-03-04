using System.Collections.Generic;
using IMCommon.DB.Models;

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
        
        public UserListModel(List<User> users)
        {
            Users = new List<UserModel>();
            foreach (var c in users)
                Users.Add(new UserModel(c));
        }
    }
}
