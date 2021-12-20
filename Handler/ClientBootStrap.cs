using Base.Client;
using CommonProtocol;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Handler;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public class ClientBootStrap : Singletion<ClientBootStrap>
    {
        public async Task RunClientAsync(IPEndPoint iPEndPoint)
        {

            var group = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast(new MessageDecoder());
                        pipeline.AddLast(new MessageEncoder());
                        pipeline.AddLast(new ClientHandler());
                    }));

                IChannel bootstrapChannel = await bootstrap.ConnectAsync(iPEndPoint);

                ChannelManager.Instance().ChannelToCenterServer = new MyChannel()
                {
                    Channel = bootstrapChannel,
                    channelType = ChannelType.Server,
                    Groups = new List<MultithreadEventLoopGroup>() { group }
                };

                //Console.ReadLine();

                //await bootstrapChannel.CloseAsync();
            }
            finally
            {
                //group.ShutdownGracefullyAsync().Wait(1000);
            }
        }


        /// <summary>
        /// 房间服务器 
        /// </summary>
        /// <returns></returns>
        public async Task RunClientRoomServerAsync(IPEndPoint EndPoint)
        {
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100)
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        // 粘包 半包问题
                        pipeline.AddLast(new LengthFieldBasedFrameDecoder(1024 * 1024, 0, 4));
                        // 自定义序列化
                        pipeline.AddLast(new MessageEncoder());
                        // 自定义反序列化
                        pipeline.AddLast(new MessageDecoder());
                        // 消息处理
                        pipeline.AddLast(new ClientRoomServerHandler());
                    }));

                IChannel bootstrapChannel = await bootstrap.BindAsync(EndPoint);

                ChannelManager.Instance().ChannelRoomServer = new MyChannel()
                {
                    Channel = bootstrapChannel,
                    Groups = new List<MultithreadEventLoopGroup>() { bossGroup, workerGroup }
                };

                //Console.WriteLine("key to quit");
                //Console.ReadKey();

                //await bootstrapChannel.CloseAsync();
            }
            finally
            {
                //Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
            }
        }

        static int RoomServerCount = 0;
        /// <summary>
        /// 客户端连接其他房间服务器
        /// </summary>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        public async void RunClientRoomClientAsync(IPEndPoint EndPoint)
        {

            var group = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast(new MessageDecoder());
                        pipeline.AddLast(new MessageEncoder());
                        pipeline.AddLast(new ClientRoomClientHandler());
                    }));

                IChannel bootstrapChannel = await bootstrap.ConnectAsync(EndPoint);

                if (bootstrapChannel.Active)
                {

                }

                ChannelManager.Instance().ChannelToRoomServers.Add(RoomServerCount++, new MyChannel()
                {
                    Channel = bootstrapChannel,
                    channelType = ChannelType.RoomServer,
                    Groups = new List<MultithreadEventLoopGroup>() { group }
                });

                //Console.ReadLine();

                //await bootstrapChannel.CloseAsync();
            }
            finally
            {
                //group.ShutdownGracefullyAsync().Wait(1000);
            }
        }
    }
}
