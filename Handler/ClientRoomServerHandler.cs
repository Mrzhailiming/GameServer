﻿using Base;
using Base.BaseData;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.IO;
using System.Threading;

namespace Handler
{
    /// <summary>
    /// 房间服务器  9 个客户端连接过来
    /// </summary>
    public class ClientRoomServerHandler : SimpleChannelInboundHandler<CommonMessage>
    {
        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            // 
            ClientManager.Instance().AddCient(ctx);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, CommonMessage msg)
        {
            Console.WriteLine($"roomserver recv from client success {msg.mCMD}");
            // 投递
            CmdHelper.Fire(ctx, msg);
        }
    }
}