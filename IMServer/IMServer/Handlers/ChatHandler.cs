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
    public class ChatHandler : HandlerBase
    {
        public override OperationCode OperationCode => OperationCode.Chat;
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
                case SubCode.Chat_Record:
                    OnRecord(peer, request);
                    break;
                case SubCode.Chat_Message:
                    OnMessage(peer, request);
                    break;
                default:
                    mLogger.ErrorFormat("消息错误！客户端:{0},用户名:{1},SubCode未知:{2}", peer, peer.LoginUser, subCode);
                    peer.SendResponse(ReturnCode.SubCodeException,
                        ESocketParameterTool.NewParameters.AddOperationCode(OperationCode));
                    break;
            }
        }

        private void OnRecord(IMClientPeer peer, OperationRequest request)
        {
            var subCode = SubCode.Chat_Record;
            if (!TryInitResponse(subCode, peer, request, out var parameters,
                ParameterKeys.CHAT_RECORD_REQUEST, out ChatRecordRequestModel model)) return;
            //DB查询聊天记录
            var records = ChatManager.GetRecordsBySendAndRecUsername(peer.LoginUser?.Username, model.ReceiveUsername, model.MinDay, model.MaxDay);
            var tmpRec = ChatManager.GetRecordsBySendAndRecUsername(model.ReceiveUsername, peer.LoginUser?.Username, model.MinDay, model.MaxDay);
            if(tmpRec != null)
            {
                if (records == null)
                    records = tmpRec;
                else
                    records.AddRange(tmpRec);
            }
            if(records != null)
                records.Sort((left, right) => left.Time.CompareTo(right));
            //响应
            peer.SendResponse(ReturnCode.Success, parameters.AddParameter(ParameterKeys.CHAT_RECORD_RESPONSE, new ChatRecordResponseModel(records)));
        }

        private void OnMessage(IMClientPeer peer, OperationRequest request)
        {
            var subCode = SubCode.Chat_Message;
            if (!TryInitResponse(subCode, peer, request, out var parameters,
                ParameterKeys.CHAT_MESSAGE_REQUEST, out ChatMessageRequestModel model)) return;
            Chat chat = new Chat
            {
                SendUsername = peer.LoginUser?.Username,
                ReceiveUsername = model.ReceiveUsername,
                MessageType = (int)model.MessageType,
                Message = model.Message,
                Time = TimeUtil.GetCurrentUtcTime().GetTotalMilliseconds()
            };
            switch (model.MessageType)
            {
                case Chat.MessageCode.Word:
                    //记录到DB中
                    ChatManager.AddChat(chat);
                    OnWord(peer, parameters, chat);
                    break;
                case Chat.MessageCode.Emoji:
                    //记录到DB中
                    ChatManager.AddChat(chat);
                    OnEmoji(peer, parameters, chat);
                    break;
                case Chat.MessageCode.File:
                    //记录到DB中
                    ChatManager.AddChat(chat);
                    OnFile(peer, parameters, chat);
                    break;
                default:
                    mLogger.ErrorFormat("消息错误！客户端:{0},用户名:{1},mSubCode:{2},ChatMessageCode{3}", peer, peer.LoginUser, subCode, model.MessageType);
                    peer.SendResponse(ReturnCode.ChatMessageCodeException,
                        ESocketParameterTool.NewParameters.AddOperationCode(OperationCode));
                    break;
            }
        }

        private void OnWord(IMClientPeer peer, Dictionary<string, object> parameters, Chat chat)
        {
            //给发送方响应消息发送成功
            peer.SendResponse(ReturnCode.Success, parameters);
            //给接收方放发送消息
            if(IMApplication.Instance.TryGetPeerByUsername(chat.ReceiveUsername, out var receivePeer))
            {
                receivePeer.SendRequest(parameters.AddParameter(ParameterKeys.CHAT_INFO, chat));
            }
        }

        private void OnEmoji(IMClientPeer peer, Dictionary<string, object> parameters, Chat chat)
        {
            //TODO
        }

        private void OnFile(IMClientPeer peer, Dictionary<string, object> parameters, Chat chat)
        {
            //TODO
        }

        private new bool TryInitResponse<T>(SubCode subCode, IMClientPeer peer, OperationRequest request,
            out Dictionary<string, object> parameters, string key, out T model) where T : ChatRequestModelBase
        {
            if(!base.TryInitResponse(subCode, peer, request, out parameters, key, out model)) return false;
            //DB判断接收方有没有问题
            var user = UserManager.GetUser(model.ReceiveUsername);
            if (user == null)
            {
                mLogger.ErrorFormat("消息错误UsernameDoesNotExist！客户端:{0},用户名:{1},SubCode:{2},ReceiveUsername:{3}", peer, peer.LoginUser, subCode, model.ReceiveUsername);
                peer.SendResponse(ReturnCode.UsernameDoesNotExist, parameters);
                return false;
            }
            return true;
        }
    }
}
