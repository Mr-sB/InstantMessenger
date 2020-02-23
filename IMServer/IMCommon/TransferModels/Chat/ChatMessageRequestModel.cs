using IMCommon.DB.Models;

namespace IMCommon.TransferModels
{
    public class ChatMessageRequestModel : ChatRequestModelBase
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public Chat.MessageCode MessageType;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message;

        public ChatMessageRequestModel() : base() { }
        public ChatMessageRequestModel(string receiveUsername, Chat.MessageCode messageType, string message) : base(receiveUsername)
        {
            MessageType = messageType;
            Message = message;
        }
    }
}
