using ESocket.Common;
using IMClient.Tools;
using IMCommon;
using IMCommon.Tools;

namespace IMClient.Controllers
{
    public class ContactController : ControllerBase
    {
        public override OperationCode OperationCode => OperationCode.Contact;
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