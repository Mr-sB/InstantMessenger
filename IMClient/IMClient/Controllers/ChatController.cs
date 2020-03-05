using ESocket.Common;
using IMClient.Tools;
using IMCommon;
using IMCommon.Tools;
using IMCommon.TransferModels;

namespace IMClient.Controllers
{
    public class ChatController : ControllerBase
    {
        public override OperationCode OperationCode => OperationCode.Chat;
        public static ChatController Instance { private set; get; }
        public UserModel CurChatUser;

        public ChatController()
        {
            Instance = this;
        }
        
        public override void OnOperationRequest(OperationRequest request)
        {
            if(request.Parameters.TryGetSubCode(out var subCode))
                Messenger.Broadcast(OperationCode, subCode, request);
        }

        public override void OnOperationResponse(OperationResponse response)
        {
            if ((ReturnCode)response.ReturnCode != ReturnCode.Success)
            {
                ((ReturnCode)response.ReturnCode).ToString().ToastOnSubThread();
                return;
            }
            if(response.Parameters.TryGetSubCode(out var subCode))
                Messenger.Broadcast(OperationCode, subCode, response);
        }
    }
}