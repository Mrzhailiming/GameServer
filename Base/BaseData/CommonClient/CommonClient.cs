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
    /// <summary>
    /// 整理一下, 抽出来一个接口
    /// </summary>
    public partial class CommonClient
    {
        public long RoleID { get; set; }
        public string Name { get; set; }

        public EndPoint ClientEndPoint { get; set; }

        public IChannelHandlerContext ctx { get; set; }

        

        public CommonClient()
        {
            mClientTickInfos = new TickInfos(this);

            // 让 TickManager tick CommonClient 的 Update
            TickManager.Instance().AddTickInfo(new TickInfo(Update, 1 * 1000, mClientTickInfos));
        }

        public void Send(CommonMessage message)
        {
            ctx.WriteAndFlushAsync(message);
        }
    }
}
