using ESocket.Common;
using IMCommon;

namespace IMClient.Controllers
{
    public abstract class ControllerBase
    {
        public abstract OperationCode OperationCode { get; }
        public virtual void OnOperationRequest(OperationRequest request) { }
        public abstract void OnOperationResponse(OperationResponse response);
    }
}