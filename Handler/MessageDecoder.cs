using Base;
using Base.BaseData;
using ConnmonMessage;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonProtocol
{
    /// <summary>
    /// 反序列化
    /// </summary>
    public class MessageDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (input.ReadableBytes < 4)
            {
                return;
            }
            input.MarkReaderIndex();

            Array array = input.Array;
            int dataLength = input.ReadInt();
            if (input.ReadableBytes < dataLength)
            {
                input.ResetReaderIndex();
                return;
            }
            var decoded = new byte[dataLength];
            input.ReadBytes(decoded);

            byte[] cmdArr = new byte[8];

            Array.Copy(decoded, 0, cmdArr, 0, 8);

            CMDS cmd = (CMDS)BitConverter.ToInt64(cmdArr, 0);

            byte[] messageArr = new byte[decoded.Length - 8];
            Array.Copy(decoded, 8, messageArr,  0, decoded.Length - 8);

            CommonMessage message = new CommonMessage()
            {
                mCMD = cmd,
                mMessageBuffer = messageArr
            };

            output.Add(message);
        }
    }
}
