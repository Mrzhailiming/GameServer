using Base.BaseData;
using Base.DataHelper;
using Base.Interface;
using Base.Logger;
using Base.Tick;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    /// <summary>
    /// server用
    /// 管理所有玩家
    /// </summary>
    public class ClientManager : Singletion<ClientManager>, StartInitInterface
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

        /// <summary>
        /// 离线玩家
        /// </summary>
        private ConcurrentDictionary<IChannelHandlerContext, CommonClient> mOffLineClientDic =
            new ConcurrentDictionary<IChannelHandlerContext, CommonClient>();

        private TickInfos mServerTickInfos;
        private void BeginTick()
        {
            mServerTickInfos = new TickInfos(this);
            mServerTickInfos.AddTick(new TickInfo(HeartBeat, 1 * 1000, mServerTickInfos));

            TickManager.Instance().AddTickInfo(new TickInfo(Update, 1 * 1000, mServerTickInfos));
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

            LoggerHelper.Instance().Log(LogType.Console, $"Add Client {ctx.Channel.RemoteAddress}");
        }

        /// <summary>
        /// 玩家执行了登录
        /// </summary>
        /// <param name="ctx"></param>
        public void AddOnLogInCient(CommonClient client)
        {
            mOnLineClientDic.TryAdd(client.ctx, client);

            LoggerHelper.Instance().Log(LogType.Console, $"Client login success Address:{client.ctx.Channel.RemoteAddress}");
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
                LoggerHelper.Instance().Log(LogType.Console, $"not find Client {ctx.Channel.RemoteAddress}");
            }

            return client;
        }

        /// <summary>
        /// 登录成功的玩家
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<IChannelHandlerContext, CommonClient> GetAllClient()
        {
            return mOnLineClientDic;
        }
        /// <summary>
        /// 离线玩家
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<IChannelHandlerContext, CommonClient> GetAllOfflineClient()
        {
            return mOffLineClientDic;
        }

        bool Update(long ticks)
        {
            mServerTickInfos.DoTick(ticks);
            return true;
        }

        /// <summary>
        /// 检测客户端的心跳
        /// 超过几次, 就移除客户端
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        bool HeartBeat(long ticks)
        {
            long nowTick = DateTime.Now.Ticks; // 100ns
            foreach (var client in mOnLineClientDic.Values)
            {
                if (!client.IsOffLine // 不是离线状态
                    && client.PrevHeartBeatTick != 0 // 客户端同步过一次 tick (如果客户端一次都没同步过, 那不就啦垮了)
                    && nowTick - client.PrevHeartBeatTick > 2 * 1000 * 1000 * 10) // 2s 没有心跳才设置离线
                {
                    client.IsOffLine = true;
                    mOffLineClientDic.TryAdd(client.ctx, client);
                }
            }
            return true;
        }

        object StartInitInterface.Instance => Instance();

        private InitType mInitType = InitType.Server;
        public InitType InitType { get => mInitType; }

        public void Init()
        {
            BeginTick();
        }
    }
}
