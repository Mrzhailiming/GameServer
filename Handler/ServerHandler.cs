using Base;
using Base.BaseData;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Handler
{
    public class ServerHandler : SimpleChannelInboundHandler<CommonMessage>
    {
        
        protected override void ChannelRead0(IChannelHandlerContext ctx, CommonMessage msg)
        {
            // 投递
            CmdHelper.Fire(ctx, msg);
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            ClientManager.Instance().AddCient(ctx);
        }

        
    }
}
