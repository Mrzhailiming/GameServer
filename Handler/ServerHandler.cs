using Base;
using Base.BaseData;
using Base.Logger;
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
            CMDHelperManager.Instance().FireServer(ctx, msg);
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            LoggerHelper.Instance().Log(LogType.Console, $"server recv a connect");
            ClientManager.Instance().AddCient(ctx);
        }

        
    }
}
