using System;

namespace IMCommon.DB.Models
{
    public class User
    {
        /// <summary>
        /// ID唯一标识
        /// </summary>
        public virtual int ID { get; set; }
        /// <summary>
        /// 用户名，不能重复
        /// </summary>
        public virtual string Username { get; set; }
        /// <summary>
        /// 密码，加盐之后hash散列结果
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// 盐(随机字符串)
        /// </summary>
        public virtual string Salt { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public virtual string Nickname { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public virtual DateTime SignUpTime { get; set; }
    }
}
