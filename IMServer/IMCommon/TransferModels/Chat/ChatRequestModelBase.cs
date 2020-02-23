namespace IMCommon.TransferModels
{
    public class ChatRequestModelBase
    {
        /// <summary>
        /// 接收方用户名
        /// </summary>
        public string ReceiveUsername;

        public ChatRequestModelBase() { }
        public ChatRequestModelBase(string receiveUsername)
        {
            ReceiveUsername = receiveUsername;
        }
    }
}
