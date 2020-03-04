using IMCommon.DB.Models;

namespace IMCommon.TransferModels
{
    public class ContactAddServerResponseModel
    {
        public bool IsRequest;//请求方还是被请求方
        public ContactAddRequest.ContactAddResponseCode ResponseCode;
        public UserModel ContactUser;
        
        public ContactAddServerResponseModel(){}

        public ContactAddServerResponseModel(bool isRequest, ContactAddRequest.ContactAddResponseCode responseCode, UserModel contactUser)
        {
            IsRequest = isRequest;
            ResponseCode = responseCode;
            ContactUser = contactUser;
        }
    }
}