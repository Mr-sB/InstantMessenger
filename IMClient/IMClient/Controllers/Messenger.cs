using IMCommon;

namespace IMClient.Controllers
{
    public delegate void SuccessDelegate(OperationCode operationCode, SubCode subCode);
    public delegate void SuccessDelegate<in T>(OperationCode operationCode, SubCode subCode, T arg);
    public delegate void SuccessDelegate<in TT, in TU>(OperationCode operationCode, SubCode subCode, TT arg, TU arg2);

    public static class Messenger
    {
        public static event SuccessDelegate OnSuccessEvent;

        public static void Broadcast(OperationCode operationCode, SubCode subCode)
        {
            OnSuccessEvent?.Invoke(operationCode, subCode);
        }
    }
}