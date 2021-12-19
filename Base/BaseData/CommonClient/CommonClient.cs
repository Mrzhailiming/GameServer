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

        /// <summary>
        /// 如果是客户端, 则客户端会发给服务器自己的房间服务器IP
        /// </summary>
        public string RoomServerIP { get; set; }
        /// <summary>
        /// 如果是客户端, 则客户端会发给服务器自己的房间服务器Port
        /// </summary>
        public string RoomServerPort { get; set; }

        public CommonClient()
        {
            mClientTickInfos = new TickInfos(this);

            // 让 TickManager tick CommonClient 的 Update
            TickManager.Instance().AddTickInfo(new TickInfo(Update, 1 * 1000, mClientTickInfos));
        }
        /// <summary>
        /// 服务器给客户端发消息用这个接口
        /// </summary>
        /// <param name="message"></param>
        public void Send(CommonMessage message)
        {
            ctx.WriteAndFlushAsync(message);
        }

       
    }
}
