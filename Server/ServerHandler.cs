using Base;
using Base.BaseData;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ServerHandler : SimpleChannelInboundHandler<CommonMessage>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, CommonMessage msg)
        {
            // 投递
            CmdHelper.Fire(new CommonClient()
            {
                ctx = ctx,
                ClientEndPoint = ctx.Channel.RemoteAddress,
                Name = ctx.Channel.RemoteAddress.ToString()
            },
            msg); ;
            //Console.WriteLine($"{msg}");
        }
        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            //Console.WriteLine("recv connect");
        }
    }
}
