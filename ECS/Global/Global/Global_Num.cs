using System;
using System.Text;

namespace Global
{
    public partial class Global
    {
        /// <summary>
        /// 设置cmd
        /// </summary>
        void SetCMD(byte[] srcBuf, int srcOffset, byte[] tarBuf, int tarOffset, int count)
        {
            Array.Copy(srcBuf, srcOffset, tarBuf, tarOffset, count);
        }

        /// <summary>
        /// 确保 从offset 到len 有 4 字节
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int Byte2Int(byte[] buf, int offset)
        {
            return ((buf[3 + offset] & 0xff) << 24) | ((buf[2 + offset] & 0xff) << 16) | ((buf[1 + offset] & 0xff) << 8) | (buf[0 + offset] & 0xff);
        }
    }
}
