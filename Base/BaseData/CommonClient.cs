using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Base.BaseData
{
    public class CommonClient
    {
        public string Name { get; set; }

        public EndPoint ClientEndPoint { get; set; }

        public IChannelHandlerContext ctx { get; set; }

        public void Send(CommonMessage message)
        {
            ctx.WriteAndFlushAsync(message);
        }
    }
}
