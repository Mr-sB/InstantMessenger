namespace IMCommon.TransferModels
{
    public class ChatRecordRequestModel : ChatRequestModelBase
    {
        /// <summary>
        /// 记录最小的天数(包含)
        /// </summary>
        public int MinDay;
        /// <summary>
        /// 记录最大的天数(不包含)
        /// </summary>
        public int MaxDay;

        public ChatRecordRequestModel() : base() { }
        public ChatRecordRequestModel(string receiveUsername, int minDay, int maxDay) : base(receiveUsername)
        {
            MinDay = minDay;
            MaxDay = maxDay;
        }
    }
}
