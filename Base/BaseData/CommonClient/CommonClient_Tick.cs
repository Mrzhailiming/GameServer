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
    public partial class CommonClient
    {
        /// <summary>
        /// 记录客户端上次心跳的时间
        /// </summary>
        public long PrevHeartBeatTick { get; set; } = 0;

        public bool IsOffLine { get; set; } = false;

        /// <summary>
        /// 维护自己的 tickinfo
        /// 心跳应不应该在client里做?
        /// </summary>
        private TickInfos mClientTickInfos;

        /// <summary>
        /// 为 CommonClient 添加 TickInfo
        /// </summary>
        /// <param name="tick"></param>
        public void AddClientTick(TickInfo tick)
        {
            mClientTickInfos.AddTick(tick);
        }

        /// <summary>
        /// CommonClient 的 tick函数
        /// CommonClient 自己维护自己的 tickinfo
        /// </summary>
        private bool Update(long time)
        {
            mClientTickInfos.DoTick(time);
            //Console.WriteLine($"client {Name} update {(new DateTimeOffset(DateTime.Now)).ToUnixTimeMilliseconds()}");
            return true;
        }
    }
}
