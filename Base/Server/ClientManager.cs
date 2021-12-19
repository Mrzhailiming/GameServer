using Base.BaseData;
using Base.DataHelper;
using Base.Tick;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Base
{
    public class ClientManager : Singletion<ClientManager>
    {
        /// <summary>
        /// 未执行login的玩家
        /// </summary>
        private Dictionary<IChannelHandlerContext, CommonClient> mClientDic =
            new Dictionary<IChannelHandlerContext, CommonClient>();

        /// <summary>
        /// 登录成功的玩家
        /// </summary>
        private Dictionary<IChannelHandlerContext, CommonClient> mOnLineClientDic =
            new Dictionary<IChannelHandlerContext, CommonClient>();

        /// <summary>
        /// 正在匹配的玩家
        /// </summary>
        private LinkedList<CommonClient> mOnMatchClients =
            new LinkedList<CommonClient>();

        private TickInfos mClientTickInfos;
        public void BeginMatchTick()
        {
            mClientTickInfos = new TickInfos(this);
            TickManager.Instance().AddTickInfo(new TickInfo(Match, 1 * 1000, mClientTickInfos));
            Console.WriteLine($"begin match tick");
        }

        /// <summary>
        /// 添加一个新的客户端连接, 并没有完成登录
        /// </summary>
        /// <param name="ctx"></param>
        public void AddCient(IChannelHandlerContext ctx)
        {
            mClientDic.Add(ctx, new CommonClient()
            {
                ctx = ctx,
                ClientEndPoint = ctx.Channel.RemoteAddress,
                Name = ctx.Channel.RemoteAddress.ToString()
            });

            Console.WriteLine($"Add Client {ctx.Channel.RemoteAddress}");
        }

        /// <summary>
        /// 玩家执行了登录
        /// </summary>
        /// <param name="ctx"></param>
        public void AddOnLogInCient(CommonClient client)
        {
            mOnLineClientDic.Add(client.ctx, client);

            Console.WriteLine($"Client login success Address:{client.ctx.Channel.RemoteAddress}");
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
                //Console.WriteLine($"not find Client {ctx.Channel.RemoteAddress}");
            }

            return client;
        }

        public Dictionary<IChannelHandlerContext, CommonClient> GetAllClient()
        {
            return mOnLineClientDic;
        }

        public const int PerMatchNum = 4;

        public void AddMatchClient(CommonClient client)
        {
            mOnMatchClients.AddLast(client);
        }

        bool Match(long ticks)
        {
            if (mOnMatchClients.Count >= PerMatchNum)
            {
                List<string> ips = new List<string>();
                List<string> ports = new List<string>();

                LinkedListNode<CommonClient> node = mOnMatchClients.First;

                List<CommonClient> RedTeam = new List<CommonClient>();
                List<CommonClient> BlueTeam = new List<CommonClient>();

                for(int count = 0; count < PerMatchNum; ++count)
                {
                    if(null == node)
                    {
                        break;
                    }

                    if(count % 2 == 0)
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

                foreach(CommonClient client in RedTeam)
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

                foreach (var client in mClientDic.Values)
                {
                    client.Send(message);
                }
                Console.WriteLine($"match success");
            }

            return true;
        }
    }
}
