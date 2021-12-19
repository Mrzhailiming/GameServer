﻿using Base;
using Base.BaseData;
using Base.Client;
using Base.DataHelper;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.IO;
using System.Threading;

namespace Handler
{
    /// <summary>
    /// 连接其他客户端的房间服务器
    /// </summary>
    public class ClientRoomClientHandler : SimpleChannelInboundHandler<CommonMessage>
    {
        void LogIn(IChannelHandlerContext ctx)
        {
            CSLogIn logIn = new CSLogIn()
            {
                RoomServerIP = "127.0.0.1",
                RoomServerPort = Convert.ToInt32(ClientInfo.MyClientServerPort)
            };

            // 发送消息的步骤可以在简化一下
            byte[] result = MessageBufHelper.GetBytes(logIn);

            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.RoomServerCSLogIn,
                mMessageBuffer = result
            };

            ctx.WriteAsync(message);
            ctx.Flush();

        }
        /// <summary>
        /// 与 其他房间服务器建立连接后, 发送joinroom消息
        /// </summary>
        /// <param name="ctx"></param>
        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            // 设置自己连接的房间服务器
            SocketInfo.Instance().mRoomServer.Add(ctx, new CommonClient()
            {
                ctx = ctx,
                Name = "roomserver"
            });
            // 登录 加入其他房间服务器
            LogIn(ctx);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, CommonMessage msg)
        {
            Console.WriteLine($"recv from clientRoomServer success {msg.mCMD}");
            // 投递
            CMDHelperManager.Instance().FireRoomClient(ctx, msg);
        }
    }
}
