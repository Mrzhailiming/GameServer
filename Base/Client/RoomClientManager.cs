using Base.BaseData;
using Base.DataHelper;
using Base.Tick;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Base
{
    /// <summary>
    /// RoomServer用
    /// 管理房间玩家
    /// </summary>
    public class RoomClientManager : Singletion<RoomClientManager>
    {
        /// <summary>
        /// 未执行login的玩家
        /// </summary>
        private ConcurrentDictionary<IChannelHandlerContext, CommonClient> mClientDic =
            new ConcurrentDictionary<IChannelHandlerContext, CommonClient>();

        /// <summary>
        /// 登录成功的玩家
        /// </summary>
        private ConcurrentDictionary<IChannelHandlerContext, CommonClient> mOnLineClientDic =
            new ConcurrentDictionary<IChannelHandlerContext, CommonClient>();

        private TickInfos mRoomTickInfos;

        /// <summary>
        /// 等所有玩家都进入好彼此的房间服务器之后, 执行gaming
        /// 这个信号由中心服发?
        /// </summary>
        public void BeginGameTick()
        {
            mRoomTickInfos = new TickInfos(this);
            TickManager.Instance().AddTickInfo(new TickInfo(Gaming, 1 * 1000, mRoomTickInfos));
            Console.WriteLine($"begin Begin Game Tick");
        }

        public bool Gaming(long tick)
        {
            mRoomTickInfos.DoTick(tick);

            Console.WriteLine($"Gaming Tick");
            return true;
        }

        /// <summary>
        /// 添加一个新的客户端连接, 并没有完成登录
        /// </summary>
        /// <param name="ctx"></param>
        public void AddCient(IChannelHandlerContext ctx)
        {
            mClientDic.TryAdd(ctx, new CommonClient()
            {
                ctx = ctx,
                ClientEndPoint = ctx.Channel.RemoteAddress,
                Name = ctx.Channel.RemoteAddress.ToString()
            });

            Console.WriteLine($"roomserver Add roomClient {ctx.Channel.RemoteAddress}");
        }

        /// <summary>
        /// 玩家执行了登录
        /// </summary>
        /// <param name="ctx"></param>
        public void AddOnLogInCient(CommonClient client)
        {
            mOnLineClientDic.TryAdd(client.ctx, client);

            Console.WriteLine($"roomserver roomClient login success Address:{client.ctx.Channel.RemoteAddress}");
        }

        /// <summary>
        /// 查找 包括为完成登录的
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public CommonClient FindClient(IChannelHandlerContext ctx)
        {
            CommonClient client;

            if (!mClientDic.TryGetValue(ctx, out client))
            {
                Console.WriteLine($"not find Client {ctx.Channel.RemoteAddress}");
            }

            return client;
        }

        public ConcurrentDictionary<IChannelHandlerContext, CommonClient> GetAllClient()
        {
            return mOnLineClientDic;
        }

    }
}
