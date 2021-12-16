using Base.BaseData;
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
        private Dictionary<IChannelHandlerContext, CommonClient> mClientDic =
            new Dictionary<IChannelHandlerContext, CommonClient>();

        private TickInfos mClientTickInfos;
        public void BeginMatchTick()
        {
            mClientTickInfos = new TickInfos(this);
            TickManager.Instance().AddTickInfo(new TickInfo(Match, 1 * 1000, mClientTickInfos));
            Console.WriteLine($"begin match tick");
        }
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

        public CommonClient FindClient(IChannelHandlerContext ctx)
        {
            CommonClient client;

            if (!mClientDic.TryGetValue(ctx, out client))
            {
                Console.WriteLine($"not find Client {ctx.Channel.RemoteAddress}");
            }

            return client;
        }

        public Dictionary<IChannelHandlerContext, CommonClient> GetAllClient()
        {
            return mClientDic;
        }


        public bool Match(long ticks)
        {
            if (mClientDic.Count >= 1)
            {
                List<string> ips = new List<string>();
                List<string> ports = new List<string>();
                foreach (var client in mClientDic.Values)
                {
                    ips.Add(client.RoomServerIP);
                    ports.Add(client.RoomServerPort);
                }

                SCJoinRoom scJoinRoom = new SCJoinRoom()
                {
                    //AllClient = $"{ips[0]}&{ports[0]}|{ips[1]}&{ports[1]}"
                    AllClient = $"{ips[0]}&{ports[0]}"
                };

                byte[] result = new byte[scJoinRoom.CalculateSize()];
                try
                {
                    using (CodedOutputStream rawOutput = new CodedOutputStream(result))
                    {
                        scJoinRoom.WriteTo(rawOutput);
                    }
                }
                catch (Exception ex)
                {
                }

                CommonMessage message = new CommonMessage()
                {
                    mCMD = CMDS.SCJionRoom,
                    mMessageBuffer = result
                };

                foreach (var client in mClientDic.Values)
                {
                    client.Send(message);
                }
                Console.WriteLine($"match success, end match");

                return false; // 先匹配一次, 逻辑还得改
            }

            return true;
        }
    }
}
