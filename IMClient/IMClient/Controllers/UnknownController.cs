using ESocket.Common;
using IMCommon;
using System;
using IMClient.Tools;

namespace IMClient.Controllers
{
    public class UnknownController : ControllerBase
    {
        public override OperationCode OperationCode => OperationCode.Unknow;

        public override void OnOperationResponse(OperationResponse response)
        {
            (OperationCode.ToString() + response.ReturnCode).ToastOnSubThread();
        }
    }
}