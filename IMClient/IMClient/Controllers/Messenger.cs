using ESocket.Common;
using IMCommon;

namespace IMClient.Controllers
{
    public delegate void SuccessNullDelegate(OperationCode operationCode, SubCode subCode);
    public delegate void SuccessOperationRequestDelegate(OperationCode operationCode, SubCode subCode, OperationRequest request);
    public delegate void SuccessOperationResponseDelegate(OperationCode operationCode, SubCode subCode, OperationResponse response);

    public static class Messenger
    {
        public static event SuccessNullDelegate OnSuccessNullEvent;
        public static event SuccessOperationRequestDelegate OnSuccessOperationRequestEvent;
        public static event SuccessOperationResponseDelegate SuccessOperationResponseEvent;

        public static void Broadcast(OperationCode operationCode, SubCode subCode)
        {
            OnSuccessNullEvent?.Invoke(operationCode, subCode);
        }
        
        public static void Broadcast(OperationCode operationCode, SubCode subCode, OperationRequest request)
        {
            OnSuccessOperationRequestEvent?.Invoke(operationCode, subCode, request);
        }
        
        public static void Broadcast(OperationCode operationCode, SubCode subCode, OperationResponse response)
        {
            SuccessOperationResponseEvent?.Invoke(operationCode, subCode, response);
        }
    }
}