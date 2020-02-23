using System;

namespace IMCommon.DB.Models
{
    public class Chat
    {
        public enum MessageCode
        {
            Word,//文字(包含小的emoji)
            Emoji,//大表情
            File,//文件
        }

        /// <summary>
        /// ID唯一标识
        /// </summary>
        public virtual int ID { get; set; }
        /// <summary>
        /// 发送方用户名
        /// </summary>
        public virtual string SendUsername { get; set; }
        /// <summary>
        /// 接收方用户名
        /// </summary>
        public virtual string ReceiveUsername { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public virtual int MessageType { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public virtual string Message { get; set; }
        /// <summary>
        /// 消息时间
        /// </summary>
        public virtual DateTime Time { get; set; }

        public virtual MessageCode GetMessageCode()
        {
            return (MessageCode)MessageType;
        }
    }
}
