using ESocket.Common;
using IMClient.Tools;
using IMCommon;
using IMCommon.Tools;

namespace IMClient.Controllers
{
    public class LoginController : ControllerBase
    {
        public override OperationCode OperationCode => OperationCode.Login;
        
        public override void OnOperationResponse(OperationResponse response)
        {
            if ((ReturnCode)response.ReturnCode != ReturnCode.Success)
            {
                ((ReturnCode)response.ReturnCode).ToString().ToastOnSubThread();
                return;
            }
            if(response.Parameters.TryGetSubCode(out var subCode))
                Messenger.Broadcast(OperationCode, subCode);
        }
    }
}