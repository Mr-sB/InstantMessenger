namespace IMCommon.DB.Models
{
    public class Contact
    {
        /// <summary>
        /// ID唯一标识
        /// </summary>
        public virtual int ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string Username { get; set; }
        /// <summary>
        /// 联系人用户
        /// </summary>
        public virtual string ContactUsername { get; set; }
    }
}
