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
    public class MacthManager : Singletion<MacthManager>, StartInitInterface
    {
        /// <summary>
        /// 登录成功的玩家
        /// </summary>
        private ConcurrentDictionary<IChannelHandlerContext, CommonClient> mOnLineClientDic
            => ClientManager.Instance().GetAllClient();

        /// <summary>
        /// 离线玩家
        /// </summary>
        private ConcurrentDictionary<IChannelHandlerContext, CommonClient> mOffLineClientDic
            => ClientManager.Instance().GetAllOfflineClient();

        /// <summary>
        /// 正在匹配的玩家
        /// </summary>
        private LinkedList<CommonClient> mOnMatchClients =
            new LinkedList<CommonClient>();

        private TickInfos mServerTickInfos;
        private void BeginTick()
        {
            mServerTickInfos = new TickInfos(this);
            // 增加匹配的tick
            mServerTickInfos.AddTick(new TickInfo(Match, 3 * 1000, mServerTickInfos));

            TickManager.Instance().AddTickInfo(new TickInfo(Update, 1 * 1000, mServerTickInfos));
            LoggerHelper.Instance().Log(LogType.Console, $"begin match tick");
        }

        public const int PerMatchNum = 2;


        public void AddMatchClient(CommonClient client)
        {
            mOnMatchClients.AddLast(client);
        }

        bool Update(long ticks)
        {
            mServerTickInfos.DoTick(ticks);
            return true;
        }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        bool Match(long ticks)
        {
            if (mOnMatchClients.Count < PerMatchNum)
            {
                return true;
            }

            var tmp = mOnMatchClients;

            LinkedListNode<CommonClient> node = tmp.First;

            List<CommonClient> RedTeam = new List<CommonClient>();
            List<CommonClient> BlueTeam = new List<CommonClient>();

            for (int count = 0; count < PerMatchNum; ++count)
            {
                if (null == node)
                {
                    break;
                }

                if (count % 2 == 0)
                {
                    RedTeam.Add(node.Value);
                }
                else
                {
                    BlueTeam.Add(node.Value);
                }

                var RemoveNode = node;
                node = node.Next;

                mOnMatchClients.Remove(RemoveNode);
            }

            StringBuilder stringBuilder = new StringBuilder();

            foreach (CommonClient client in RedTeam)
            {
                stringBuilder.Append(client.RoomServerIP).Append("&");
                stringBuilder.Append(client.RoomServerPort).Append("&");
                stringBuilder.Append("Red").Append("|");
            }

            foreach (CommonClient client in BlueTeam)
            {
                stringBuilder.Append(client.RoomServerIP).Append("&");
                stringBuilder.Append(client.RoomServerPort).Append("&");
                stringBuilder.Append("Blue").Append("|");
            }

            SCMatch scJoinRoom = new SCMatch()
            {
                AllClient = stringBuilder.ToString()
            };

            byte[] result = MessageBufHelper.GetBytes(scJoinRoom);

            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.SCMatch,
                mMessageBuffer = result
            };

            foreach (var client in mOnMatchClients)
            {
                client.Send(message);
            }
            LoggerHelper.Instance().Log(LogType.Console, $"match success");

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
