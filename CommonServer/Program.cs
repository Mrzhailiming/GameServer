// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Server
{
    using Base;
    using Base.Interface;
    using Base.Tick;
    using CommonProtocol;
    using DotNetty.Codecs;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;
    using global::Handler;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    class Program
    {
        
        static async Task RunServerAsync()
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
                        pipeline.AddLast(new ServerHandler());
                    }));

                IChannel bootstrapChannel = await bootstrap.BindAsync(8888);

                //LoggerHelper.Instance().Log(LogType.Console, "key to quit");
                //Console.ReadKey();

                //await bootstrapChannel.CloseAsync();
            }
            finally
            {
                //Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
            }
        }

        public static void Main()
        {
            InitServer();

            RunServerAsync().Wait();
        }

        public static void InitServer()
        {
            // 新方式, 自动调用, 只需要继承 StartInitInterface 接口即可
            StartInitManager.Instance().StartInit(InitType.Server, Directory.GetCurrentDirectory());

            // 旧方式 每个类需要初始化的时候,都需要主动调用, 太麻烦
            //CMDHelperManager.Instance().Init();
            //ClientManager.Instance().Init();
            //TickManager.Instance().Init();
        }
    }
}