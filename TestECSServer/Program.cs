using Base.Logger;
using Entity;
using Entity.Component;
using Global;
using MySystem;
using Server;
using Singleton.Manager;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestECSServer
{
    class Program
    {
        /// <summary>
        /// ECSServer
        /// 实体没有函数
        /// 系统没有字段
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // 注册协议处理函数
            CMDDispatcher.Instance().RegisterCMD(TCPCMDS.TEST, Do);

            IEntity entity = new SocketEntity();
            SocketComponent socketComponent = new SocketComponent();
            entity.AddComponent(typeof(SocketComponent), socketComponent);

            string ip = "127.0.0.1";
            int port = 8888;
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            SocketType socketType = SocketType.Stream;
            ProtocolType protocolType = ProtocolType.Tcp;
            int backlog = 10;

            MyServer.Instance().Run(entity, iPEndPoint, socketType, protocolType, ip, port, backlog);


            MyServer.Instance().AddSystem(new ConnectSysytem());
            Add("111EntityOwner", 1, "compoName111");
            Add("222EntityOwner", 2, "compoName222");
            Add("333EntityOwner", 3, "compoName333");
            Add("444EntityOwner", 4, "compoName444");
            while (false)
            {
                MyServer.Instance().Tick(DateTime.Now.Ticks);

                LoggerHelper.Instance().Log(LogType.Console, $"main tick");

                Thread.Sleep(1000 * 2);
            }

            Console.ReadKey();
        }

        public static void Add(string owner, long id, string name)
        {
            ConnectionEntity entity = new ConnectionEntity();
            entity.mOwner = owner;
            ConnectionComponent cp = new ConnectionComponent();
            cp.mOwner = entity;
            cp.mName = name;
            entity.AddComponent(typeof(ConnectionComponent), cp);

            EntityManager.Instance().AddEntity(entity, id);
        }


        public static void Do(TCPPacket packet)
        {
            byte[] recv = new byte[packet.mTotalLen - Proto.protoHeadLen];
            Buffer.BlockCopy(packet.mBuff, Proto.protoHeadLen, recv, 0, packet.mTotalLen - Proto.protoHeadLen);
            string recvstr = Encoding.Default.GetString(recv);

            LoggerHelper.Instance().Log(LogType.Console, $"ProcessCmdID:{(TCPCMDS)packet.mCmd} body:{recvstr}");
        }
    }
}
