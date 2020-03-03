using IMCommon.DB.Models;

namespace IMCommon.TransferModels
{
    public class ContactAddServerResponseModel
    {
        public ContactAddRequest.ContactAddResponseCode ResponseCode;
        public UserModel ContactUser;
        
        public ContactAddServerResponseModel(){}

        public ContactAddServerResponseModel(ContactAddRequest.ContactAddResponseCode responseCode, UserModel contactUser)
        {
            ResponseCode = responseCode;
            ContactUser = contactUser;
        }
    }
}