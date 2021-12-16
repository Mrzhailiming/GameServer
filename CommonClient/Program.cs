// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Client
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Base;
    using Base.Client;
    using Base.Tick;
    using CommonProtocol;
    using DotNetty.Handlers.Logging;
    using DotNetty.Handlers.Tls;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;
    using Handler;

    class Program
    {
        public static void Main()
        {
            InitClientServer();

            // 服务器 端口 8888  连接中心服务器
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), ClientInfo.MyClientServerPort);
            ClientBootStrap.Instance().RunClientAsync(iPEndPoint);

            // 客户端的房间服务器 监听 9999
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            ClientBootStrap.Instance().RunClientRoomServerAsync(EndPoint);
        }

        public static void InitClientServer()
        {
            TickManager.Instance().RunAsync(); 

            CmdHelper.Init(CMDType.Client);
        }
    }
}