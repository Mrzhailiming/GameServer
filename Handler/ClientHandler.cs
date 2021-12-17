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
    public class ClientHandler : SimpleChannelInboundHandler<CommonMessage>
    {
        public void TestSend(IChannelHandlerContext ctx)
        {
            Person person = new Person()
            {
                Name = "777",
                Id = 10010,
                Email = "44@qq.com"
            };

            SynchronousInfo synchronousInfo = new SynchronousInfo()
            {
                Name = "888",
                OperationInfo = "frame sync"
            };

            byte[] result = new byte[synchronousInfo.CalculateSize()];
            try
            {
                using (CodedOutputStream rawOutput = new CodedOutputStream(result))
                {
                    person.WriteTo(rawOutput);
                }
            }
            catch (Exception ex)
            {
            }


            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.FrameSynchronization,
                mMessageBuffer = result
            };


            ctx.WriteAsync(message);

            ctx.Flush();
        }

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

            // 在这把服务器当做自己的维护的客户端, 加到 clientmanager 里, 能行
            ClientManager.Instance().AddCient(ctx);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, CommonMessage msg)
        {
            Console.WriteLine($"recv from server success {msg.mCMD}");

            CmdHelper.Fire(ctx, msg);
            //Thread.Sleep(100000000);
            //TestSend(ctx);
        }
    }
}
