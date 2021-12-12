using Base.Tick;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Base.BaseData
{
    public class CommonClient
    {
        public string Name { get; set; }

        public EndPoint ClientEndPoint { get; set; }

        public IChannelHandlerContext ctx { get; set; }

        private TickInfos mClientTickInfos;

        public CommonClient()
        {
            mClientTickInfos = new TickInfos(this);
            TickManager.Instance().AddTickInfos(mClientTickInfos);

            TickInfo testTick = new TickInfo(Update, 1000);
            mClientTickInfos.AddTick(testTick);

        }

        public void Send(CommonMessage message)
        {
            ctx.WriteAndFlushAsync(message);
        }


        public void AddClientTick(TickInfo tick)
        {
            mClientTickInfos.AddTick(tick);
        }

        private bool Update(long time)
        {
            Console.WriteLine($"client {Name} update {(new DateTimeOffset(DateTime.Now)).ToUnixTimeMilliseconds()}");
            //Thread.Sleep(3 * 1000);
            return true;
        }

        public void LogOut()
        {
            mClientTickInfos.IsEffective = false;
        }
    }
}
