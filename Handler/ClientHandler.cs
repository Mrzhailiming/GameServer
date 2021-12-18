using Base;
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
    /// 与中心服连接的Handler
    /// </summary>
    public class ClientHandler : SimpleChannelInboundHandler<CommonMessage>
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
                mCMD = CMDS.CSLogIn,
                mMessageBuffer = result
            };

            ctx.WriteAsync(message);
            ctx.Flush();

        }
        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            Console.WriteLine("connect success begin login");
            LogIn(ctx);

            // 设置自己连接的 server
            SocketInfo.Instance().mCenterServer = new CommonClient()
            {
                ctx = ctx,
                Name = "centerserver",
            };
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, CommonMessage msg)
        {
            //Console.WriteLine($"recv from server success {msg.mCMD}");

            CmdHelper.Fire(ctx, msg, SocketInfo.Instance().mCenterServer);
        }
    }
}
