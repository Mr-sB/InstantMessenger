namespace IMCommon.DB.Models
{
    /// <summary>
    /// 添加联系人请求
    /// </summary>
    public class ContactRequest
    {
        public virtual int ID { get; set; }
        /// <summary>
        /// 请求用户
        /// </summary>
        public virtual User RequestUser { get; set; }
        /// <summary>
        /// 被添加对象用户名
        /// </summary>
        public virtual string ContactUsername { get; set; }
    }
}
