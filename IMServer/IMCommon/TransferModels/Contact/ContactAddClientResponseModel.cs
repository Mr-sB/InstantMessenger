namespace IMCommon.TransferModels
{
    public class ContactAddClientResponseModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string RequestUsername;
        /// <summary>
        /// 是否同意添加
        /// </summary>
        public bool Accept;
        
        public ContactAddClientResponseModel() { }
        public ContactAddClientResponseModel(string requestUsername, bool accept)
        {
            RequestUsername = requestUsername;
            Accept = accept;
        }
    }
}