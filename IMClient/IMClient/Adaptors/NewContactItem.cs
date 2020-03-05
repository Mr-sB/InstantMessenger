using IMCommon.DB.Models;
using IMCommon.TransferModels;

namespace IMClient.Adaptors
{
    public class NewContactItem
    {
        public readonly bool IsRequest;//请求方还是被请求方
        public ContactAddRequest.ContactAddResponseCode ResponseCode;
        public readonly string Username;
        public readonly string Nickname;
        public NewContactItem(bool isRequest, ContactAddRequest.ContactAddResponseCode responseCode, string username, string nickname)
        {
            IsRequest = isRequest;
            ResponseCode = responseCode;
            Username = username;
            Nickname = nickname;
        }

        public NewContactItem(ContactAddServerResponseModel serverResponseModel) :
            this(serverResponseModel.IsRequest, serverResponseModel.ResponseCode, serverResponseModel.ContactUser.Username, serverResponseModel.ContactUser.Nickname){}
    }
}