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

        public void OnListRequest(IMClientPeer peer, OperationRequest request)
        {
            //DB查询联系人列表
            var contacts = ContactManager.GetContactsByUsername(peer.LoginUser?.Username);
            var parameters = InitParameters(SubCode.Contact_List);
            if(contacts != null)
            {
                List<UserModel> users = new List<UserModel>();
                foreach (var c in contacts)
                    users.Add(new UserModel(c.ContactUser));
                parameters.AddParameter(ParameterKeys.USER_MODEL_LIST, new UserListModel(users));
            }
            //回应
            peer.SendResponse(ReturnCode.Success, parameters);
        }

        public void OnSearchRequest(IMClientPeer peer, OperationRequest request)
        {
            SubCode subCode = SubCode.Contact_Search;
            if (!TryInitResponse(subCode, peer, request, out var parameters,
                ParameterKeys.USERNAME, out string contactUsername)) return;
            //DB模糊查询
            var user = UserManager.FuzzySearchByUsername(contactUsername);
            if (user != null)
                parameters.AddParameter(ParameterKeys.USER_MODEL, user);
            //回应
            peer.SendResponse(ReturnCode.Success, parameters);
        }

        public void OnAddRequest(IMClientPeer peer, OperationRequest request)
        {
            SubCode subCode = SubCode.Contact_Add;
            if (!TryInitResponse(subCode, peer, request, out var parameters,
                ParameterKeys.USERNAME, out string contactUsername)) return;
            //DB添加请求
            ContactRequestManager.AddContactRequest(new ContactRequest {
                RequestUser = peer.LoginUser,
                ContactUsername = contactUsername
            });
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

        public void OnDeleteRequest(IMClientPeer peer, OperationRequest request)
        {
            SubCode subCode = SubCode.Contact_Delete;
            if (!TryInitResponse(subCode, peer, request, out var parameters,
                ParameterKeys.USERNAME, out string contactUsername)) return;
            //DB删除
            ContactManager.DeleteContact(peer.LoginUser?.Username, contactUsername);
            //回应
            peer.SendResponse(ReturnCode.Success, parameters);
        }

        public void OnAddResponse(IMClientPeer peer, OperationResponse response)
        {
            
        }
    }
}
