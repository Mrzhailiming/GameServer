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
    /// 序列化
    /// </summary>
    public class MessageEncoder : MessageToMessageEncoder<CommonMessage>
    {
        protected override void Encode(IChannelHandlerContext context, CommonMessage message, List<object> output)
        {
            IByteBuffer buffer = context.Allocator.Buffer();

            buffer.WriteInt(message.mTotalLength);
            buffer.WriteBytes(message.ToBytes());

            output.Add(buffer);
        }
    }
}
