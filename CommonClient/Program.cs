namespace Client
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Base;
    using Base.Client;
    using Base.Interface;
    using Base.Logger;
    using Base.Tick;

    class Program
    {
        public static void Main()
        {
            InitClientServer();

            Random ran = new Random();
            int n = ran.Next(100, 1000);
            ClientInfo.MyClientServerPort = n.ToString();

            // 服务器 端口 8888  连接中心服务器
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            Task task = ClientBootStrap.Instance().RunClientAsync(iPEndPoint);
            bool b = task.IsCompleted;
            // 客户端的房间服务器 监听 随机吧
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(ClientInfo.MyClientServerPort));
            ClientBootStrap.Instance().RunClientRoomServerAsync(EndPoint);

            LoggerHelper.Instance().Log(LogType.Console, $"本房间服务器的监听端口:{n}");
        }

        public static void InitClientServer()
        {
            // 新方式, 自动调用, 只需要继承 StartInitInterface 接口即可
            StartInitManager.Instance().StartInit(InitType.Client, Directory.GetCurrentDirectory());

            //TickManager.Instance().RunAsync();
            //CMDHelperManager.Instance().Init();
        }
    }
}