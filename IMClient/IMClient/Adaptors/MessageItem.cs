using IMCommon.TransferModels;

namespace IMClient.Adaptors
{
    public class MessageItem
    {
        public readonly UserModel UserModel;
        public readonly string Content;

        public MessageItem(UserModel userModel, string content)
        {
            UserModel = userModel;
            Content = content;
        }
    }
}