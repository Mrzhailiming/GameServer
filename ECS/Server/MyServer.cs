using Entity;
using MySystem;
using Singleton;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class MyServer : Singleton<MyServer>
    {
        public List<ISystem> mSystems = new List<ISystem>();

        public SocketSystem mSocketSystem = new SocketSystem();

        public void Run(IEntity entity, IPEndPoint iPEndPoint, SocketType socketType, ProtocolType protocolType
            , string ip, int port, int backlog)
        {
            mSocketSystem.Init();

            mSocketSystem.RunServer(entity, iPEndPoint, socketType, protocolType, ip, port, backlog);
        }

        public void AddSystem(ISystem system)
        {
            mSystems.Add(system);
            LoggerHelper.Instance().Log(LogType.Console, $"AddSystem");
        }

        public void Tick(long tick)
        {
            foreach (ISystem system in mSystems)
            {
                foreach (Entity.IEntity entity in EntityManager.Instance())
                {
                    system.Tick(tick, entity);
                }
            }
        }
    }
}
