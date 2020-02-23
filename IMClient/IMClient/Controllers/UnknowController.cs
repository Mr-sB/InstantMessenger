using ESocket.Common;
using IMCommon;
using System;

namespace IMClient.Controllers
{
    public class UnknowController : ControllerBase
    {
        public override OperationCode OperationCode => OperationCode.Unknow;

        public override void OnOperationResponse(OperationResponse response)
        {
            Console.WriteLine(response.ReturnCode);
        }
    }
}