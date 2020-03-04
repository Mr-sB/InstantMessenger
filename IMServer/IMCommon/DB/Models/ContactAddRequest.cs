namespace IMCommon.DB.Models
{
    /// <summary>
    /// 添加联系人请求
    /// </summary>
    public class ContactAddRequest
    {
        public enum ContactAddResponseCode
        {
            Waite,
            Accept,
            Refuse
        }
        
        public virtual int ID { get; set; }
        /// <summary>
        /// 请求用户
        /// </summary>
        public virtual User RequestUser { get; set; }
        /// <summary>
        /// 被添加对象用户名
        /// </summary>
        public virtual string ContactUsername { get; set; }
        /// <summary>
        /// 添加请求是否被同意
        /// </summary>
        public virtual int ResponseCode { get; set; }

        public virtual ContactAddResponseCode GetResponseCode()
        {
            return (ContactAddResponseCode) ResponseCode;
        }
    }
}
