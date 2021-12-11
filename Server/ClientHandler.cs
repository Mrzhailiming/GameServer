﻿using Base;
using Base.BaseData;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.IO;

namespace Server
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

            byte[] result = new byte[person.CalculateSize()];
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
                mCMD = CMDS.Test,
                mMessageBuffer = result
            };


            ctx.WriteAsync(message);

            ctx.Flush();
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            Console.WriteLine("connect success");
            TestSend(ctx);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, CommonMessage msg)
        {
            Console.WriteLine("send success");
            TestSend(ctx);
        }
    }
}
