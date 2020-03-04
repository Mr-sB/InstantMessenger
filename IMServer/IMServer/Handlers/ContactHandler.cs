using ESocket.Common;
using ESocket.Common.Tools;
using IMCommon;
using IMCommon.DB.Models;
using IMCommon.Tools;
using IMCommon.TransferModels;
using IMServer.DB.Managers;
using log4net;
using System.Collections.Generic;
using System.Reflection;
using OperationCode = IMCommon.OperationCode;

namespace IMServer.Handlers
{
    public class ContactHandler : HandlerBase
    {
        public override OperationCode OperationCode => OperationCode.Contact;
        protected override ILog mLogger => mLog ?? (mLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType));
        private ILog mLog;

        public override void OnOperationRequest(IMClientPeer peer, OperationRequest request)
        {
            if (!request.Parameters.TryGetSubCode(out var subCode))
            {
                mLogger.ErrorFormat("消息错误！客户端:{0},用户名:{1},OperationCode:{2},获取SubCode失败", peer, peer.LoginUser, OperationCode);
                peer.SendResponse(ReturnCode.SubCodeException,
                    ESocketParameterTool.NewParameters.AddOperationCode(OperationCode));
                return;
            }
            switch (subCode)
            {
                case SubCode.Contact_List:
                    OnListRequest(peer, request);
                    break;
                case SubCode.Contact_Search:
                    OnSearchRequest(peer, request);
                    break;
                case SubCode.Contact_Add:
                    OnAddRequest(peer, request);
                    break;
                case SubCode.Contact_Delete:
                    OnDeleteRequest(peer, request);
                    break;
                case SubCode.Contact_Add_List:
                    OnAddListRequest(peer, request);
                    break;
                default:
                    mLogger.ErrorFormat("消息错误！客户端:{0},用户名:{1},SubCode未知:{2}", peer, peer.LoginUser, subCode);
                    peer.SendResponse(ReturnCode.SubCodeException,
                        ESocketParameterTool.NewParameters.AddOperationCode(OperationCode));
                    break;
            }
        }

        public override void OnOperationResponse(IMClientPeer peer, OperationResponse response)
        {
            if (!response.Parameters.TryGetSubCode(out var subCode))
            {
                mLogger.ErrorFormat("消息错误！客户端:{0},用户名:{1},OperationCode:{2},获取SubCode失败", peer, peer.LoginUser, OperationCode);
                peer.SendResponse(ReturnCode.SubCodeException,
                    ESocketParameterTool.NewParameters.AddOperationCode(OperationCode));
                return;
            }
            if ((ReturnCode)response.ReturnCode != ReturnCode.Success)
            {
                mLogger.ErrorFormat("请求失败！客户端:{0},用户名:{1},SubCode:{2},ReturnCode:{3}", peer, peer.LoginUser, subCode, (ReturnCode)response.ReturnCode);
                return;
            }
            switch (subCode)
            {
                case SubCode.Contact_Add:
                    OnAddResponse(peer, response);
                    break;
                default:
                    mLogger.ErrorFormat("SubCode错误！客户端:{0},用户名:{1},SubCode:{2}", peer, peer.LoginUser, subCode);
                    peer.SendResponse(ReturnCode.SubCodeException,
                        ESocketParameterTool.NewParameters.AddOperationCode(OperationCode));
                    break;
            }
        }

        private void OnListRequest(IMClientPeer peer, OperationRequest request)
        {
            //DB查询联系人列表
            var contacts = ContactManager.GetContactsByUsername(peer.LoginUser?.Username);
            var parameters = InitParameters(SubCode.Contact_List);
            if(contacts != null)
            {
                List<UserModel> users = new List<UserModel>();
                foreach (var c in contacts)
                    users.Add(new UserModel(UserManager.GetUser(c.ContactUsername)));
                parameters.AddParameter(ParameterKeys.USER_MODEL_LIST, new UserListModel(users));
            }
            //回应
            peer.SendResponse(ReturnCode.Success, parameters);
        }

        private void OnSearchRequest(IMClientPeer peer, OperationRequest request)
        {
            const SubCode subCode = SubCode.Contact_Search;
            if (!TryInitResponse(subCode, peer, request, out var parameters,
                ParameterKeys.USERNAME, out string contactUsername)) return;
            //DB模糊查询
            var users = UserManager.FuzzySearchByUsername(contactUsername);
            if (users != null)
                parameters.AddParameter(ParameterKeys.USER_MODEL_LIST, new UserListModel(users));
            //回应
            peer.SendResponse(ReturnCode.Success, parameters);
        }

        private void OnAddRequest(IMClientPeer peer, OperationRequest request)
        {
            const SubCode subCode = SubCode.Contact_Add;
            if (!TryInitResponse(subCode, peer, request, out var parameters,
                ParameterKeys.USERNAME, out string contactUsername)) return;
            
            var contactAddRequest = ContactAddRequestManager.GetContactAddRequest(peer.LoginUser?.Username, contactUsername);
            if (contactAddRequest != null)
            {
                const int wait = (int) ContactAddRequest.ContactAddResponseCode.Waite;
                if (contactAddRequest.ResponseCode != wait)
                {
                    //DB更新
                    contactAddRequest.ResponseCode = wait;
                    ContactAddRequestManager.UpdateContactAddRequest(contactAddRequest);
                }
            }
            else
            {
                //DB添加请求
                ContactAddRequestManager.AddContactAddRequest(new ContactAddRequest
                {
                    RequestUsername = peer.LoginUser.Username,
                    ContactUsername = contactUsername,
                    ResponseCode = (int) ContactAddRequest.ContactAddResponseCode.Waite
                });
            }

            //回应
            peer.SendResponse(ReturnCode.Success, parameters);
            //如果对方在线，直接发送请求
            if(IMApplication.Instance.TryGetPeerByUsername(contactUsername, out var contactPeer))
            {
                contactPeer.SendRequest(ESocketParameterTool.NewParameters
                    .AddOperationCode(OperationCode)
                    .AddSubCode(SubCode.Contact_Add)
                    .AddParameter(ParameterKeys.USER_MODEL, new UserModel(peer.LoginUser)));
            }
        }

        private void OnDeleteRequest(IMClientPeer peer, OperationRequest request)
        {
            const SubCode subCode = SubCode.Contact_Delete;
            if (!TryInitResponse(subCode, peer, request, out var parameters,
                ParameterKeys.USERNAME, out string contactUsername)) return;
            //DB删除
            ContactManager.DeleteContact(peer.LoginUser?.Username, contactUsername);
            ContactManager.DeleteContact(contactUsername, peer.LoginUser?.Username);
            //回应
            peer.SendResponse(ReturnCode.Success, parameters.AddParameter(ParameterKeys.USERNAME, contactUsername));
            //如果请求在线，发送响应
            if (IMApplication.Instance.TryGetPeerByUsername(contactUsername, out var responsePeer))
            {
                parameters[ParameterKeys.USERNAME] = peer.LoginUser.Username;
                responsePeer.SendRequest(parameters);
            }
        }

        private void OnAddListRequest(IMClientPeer peer, OperationRequest request)
        {
            var requestList = ContactAddRequestManager.GetContactAddRequestList(peer.LoginUser?.Username);
            var contactedList = ContactAddRequestManager.GetContactAddContactedList(peer.LoginUser?.Username);
            var parameters = InitParameters(SubCode.Contact_Add_List);
            List<ContactAddServerResponseModel> list = new List<ContactAddServerResponseModel>();
            if(requestList != null)
            {
                foreach (var item in requestList)
                    list.Add(new ContactAddServerResponseModel(true, item.GetResponseCode(), new UserModel(UserManager.GetUser(item.ContactUsername))));
            }
            if(contactedList != null)
            {
                foreach (var item in contactedList)
                    list.Add(new ContactAddServerResponseModel(false, item.GetResponseCode(), new UserModel(UserManager.GetUser(item.RequestUsername))));
            }
            parameters.AddParameter(ParameterKeys.CONTACT_ADD_SERVER_RESPONSE_LIST, new ContactAddServerResponseModelList(list));
            //回应
            peer.SendResponse(ReturnCode.Success, parameters);
        }

        private void OnAddResponse(IMClientPeer peer, OperationResponse response)
        {
            const SubCode subCode = SubCode.Contact_Add;
            if (!TryInitResponse(subCode, peer, response, out var parameters,
                ParameterKeys.CONTACT_ADD_CLIENT_RESPONSE, out ContactAddClientResponseModel model)) return;
            if (peer.LoginUser == null)
            {
                mLogger.ErrorFormat("响应失败!客户端:{0}未登陆！", peer);
                return;
            }
            var contactAddRequest = ContactAddRequestManager.GetContactAddRequest(model.RequestUsername, peer.LoginUser.Username);
            if (contactAddRequest == null)
            {
                peer.SendResponse(ReturnCode.UsernameDoesNotExist, parameters);
                return;
            }
            
            var responseCode = model.Accept
                ? ContactAddRequest.ContactAddResponseCode.Accept
                : ContactAddRequest.ContactAddResponseCode.Refuse;
            //DB更新
            contactAddRequest.ResponseCode = (int)responseCode;
            ContactAddRequestManager.UpdateContactAddRequest(contactAddRequest);
            var requestUser = UserManager.GetUser(model.RequestUsername);
            if (model.Accept)
            {
                //DB添加
                ContactManager.AddContact(new Contact
                {
                    Username = model.RequestUsername,
                    ContactUsername = peer.LoginUser.Username
                });
                ContactManager.AddContact(new Contact
                {
                    Username = peer.LoginUser.Username,
                    ContactUsername = requestUser.Username
                });
            }
            //响应
            ContactAddServerResponseModel responseModel1 = new ContactAddServerResponseModel(false, responseCode, new UserModel(requestUser));
            peer.SendResponse(ReturnCode.Success, parameters.AddParameter(ParameterKeys.CONTACT_ADD_SERVER_RESPONSE, responseModel1));
            //如果请求方在线，发送响应
            if (IMApplication.Instance.TryGetPeerByUsername(model.RequestUsername, out var requestPeer))
            {
                ContactAddServerResponseModel responseModel2 = new ContactAddServerResponseModel(true, responseCode, new UserModel(peer.LoginUser));
                parameters[ParameterKeys.CONTACT_ADD_SERVER_RESPONSE] = responseModel2;
                requestPeer.SendRequest(parameters);
            }
        }
    }
}
