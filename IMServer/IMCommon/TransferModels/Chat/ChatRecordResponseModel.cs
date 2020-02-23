using IMCommon.DB.Models;
using System.Collections.Generic;

namespace IMCommon.TransferModels
{
    public class ChatRecordResponseModel
    {
        //聊天记录
        public List<Chat> Records;
        public ChatRecordResponseModel() { }
        public ChatRecordResponseModel(List<Chat> records)
        {
            Records = records;
        }
    }
}
