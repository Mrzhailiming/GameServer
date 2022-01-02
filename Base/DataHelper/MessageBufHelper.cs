using Base.Logger;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base.DataHelper
{
    public class MessageBufHelper
    {
        public static byte[] GetBytes(IMessage message)
        {
            byte[] result = new byte[message.CalculateSize()];
            try
            {
                using (CodedOutputStream rawOutput = new CodedOutputStream(result))
                {
                    message.WriteTo(rawOutput);
                }

                return result;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"MessageBufHelper GetBytes() 异常\r\n" +
                    $"{ex}");
            }

            return null;
        }
    }
}
