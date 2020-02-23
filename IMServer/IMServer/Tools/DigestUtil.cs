using System;
using System.Security.Cryptography;
using System.Text;

namespace IMServer.Tools
{
    public static class DigestUtil
    {
        public enum EncodeType
        {
            MD5,
            SHA256
        }

        /// <summary>
        /// 加盐加密
        /// </summary>
        /// <param name="message">被加密消息</param>
        /// <param name="encodeType">加密类型</param>
        /// <param name="saltLength">随机字符串(盐)的长度，最大32</param>
        /// <param name="salt">随机字符串(盐)</param>
        /// <returns>加密结果(大写十六进制形式)</returns>
        public static string SaltEncodeString(this string message, EncodeType encodeType, out string salt, int saltLength = 10)
        {
            salt = GetRandomString(saltLength);
            return SaltEncodeString(message, encodeType, salt);
        }

        /// <summary>
        /// 加盐加密
        /// </summary>
        /// <param name="message">被加密消息</param>
        /// <param name="encodeType">加密类型</param>
        /// <param name="salt">随机字符串(盐)</param>
        /// <returns>加密结果(大写十六进制形式)</returns>
        public static string SaltEncodeString(this string message, EncodeType encodeType, string salt)
        {
            switch (encodeType)
            {
                case EncodeType.MD5:
                    return (message + salt).GetMD5();
                default:
                    return (message + salt).GetSHA256();
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="message">被加密消息</param>
        /// <returns>加密结果(大写十六进制形式)</returns>
        public static string GetMD5(this string message)
        {
            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(message)).ToHex();
        }

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="message">被加密消息</param>
        /// <returns>加密结果(大写十六进制形式)</returns>
        public static string GetSHA256(this string message)
        {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(message)).ToHex();
        }

        /// <summary>
        /// 将字节数组转为大写十六进制字符串
        /// </summary>
        /// <param name="byteArray">字节数组</param>
        /// <returns>大写十六进制字符串</returns>
        public static string ToHex(this byte[] byteArray)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var b in byteArray)
                stringBuilder.Append(b.ToString("X2"));
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 获得随机的字符串
        /// </summary>
        /// <param name="length">字符串长度，最大32</param>
        /// <returns>随机字符串</returns>
        public static string GetRandomString(int length = 10)
        {
            string guid = Guid.NewGuid().ToString("N");//去除'-'连接符
            //给定长度小于0，返回空字符串
            if (length <= 0) return string.Empty;

            int guidLength = guid.Length;
            //给定长度超过最大长度，直接返回全部
            if (length >= guidLength) return guid;

            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder(length);
            double rate = (double)length / guidLength;//某个字符被选中的概率
            bool forceAdd = false;//是否直接添加
            for (int i = 0, randomStrRemain = length; i < guidLength; i++)
            {
                //剩余guid字符串长度和剩余随机字符串长度相同时，直接添加
                if(!forceAdd && guidLength - i - 1 == randomStrRemain) forceAdd = true;
                if (forceAdd || random.NextDouble() <= rate)
                {
                    stringBuilder.Append(guid[i]);
                    if (--randomStrRemain <= 0) break;
                }
            }
            return stringBuilder.ToString();
        }
    }
}
