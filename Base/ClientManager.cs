using Base.BaseData;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public class ClientManager : Singletion<ClientManager>
    {
        private Dictionary<IChannelHandlerContext, CommonClient> mClientDic =
            new Dictionary<IChannelHandlerContext, CommonClient>();

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
    }
}
