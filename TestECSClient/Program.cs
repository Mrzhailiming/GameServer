using Base.Logger;
using Entity;
using Entity.Component;
using Global;
using MySystem;
using Singleton.Manager;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestECSClient
{
    class Program
    {
        static void Main(string[] args)
        {

            SocketSystem socketSystem = new SocketSystem();
            socketSystem.Init();

            IEntity entity = new SocketEntity();
            SocketComponent socketComponent = new SocketComponent();
            socketComponent.mOwner = entity;
            socketComponent.mConnectType = ConnectType.Send;
            entity.AddComponent(typeof(SocketComponent), socketComponent);

            string ip = "127.0.0.1";
            int port = 8888;
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            SocketType socketType = SocketType.Stream;
            ProtocolType protocolType = ProtocolType.Tcp;

            socketSystem.RunClient(entity, iPEndPoint, socketType, protocolType, ip, port);

            while (true)
            {
                if (!socketComponent.mSocketInvild)
                {
                    Thread.Sleep(1000 * 1);
                }
                else
                {
                    break;
                }
            }

            //Thread.Sleep(1000 * 2);

            Console.WriteLine($"str:{Str.Length}");

            SocketSystem.SendAsync(socketComponent, TCPCMDS.TEST, Str);

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }

        public static string Str = $"hellohellohellohellohel" +
            $"lohellohellohellohellohellohellohellohellohellohe" +
            $"llohellohellohel" +
            $"lohellohellohellohellohellohellohellohel" +
            $"lohellohellohellohellohellohellohellohel" +
            $"lohellohellohellohellohellohellohellohel" +
            $"lohellohellohellohellohellohellohellohel" +
            $"lohellohellohellohellohellohe" +
            $"llohellohellohellohellohellohellohellohe" +
            $"llohellohellohellohellohellohellohellohell" +
            $"ohellohellohellohellohellohellohellohellohe" +
            $"llohellohellohellohellohellohellohellohelloh" +
            $"ellohellohellohellohellohellohellohellohellohell" +
            $"ohellohellohellohellohellohellohellohellohellohe" +
            $"llohellohellohellohellohellohellohellohellohello" +
            $"hellohellohellohellohellohellohellohellohellohellohell" +
            $"ohellohellohellohellohellohellohellohell" +
            $"ohellohellohellohellohellohellohellohelloh" +
            $"ellohellohellohellohellohellohellohellohellohell" +
            $"ohellohellohellohellohellohellohellohellohellohe" +
            $"llohellohellohellohellohellohellohellohellohelloh" +
            $"ellohellohellohellohellohellohellohellohellohellohe" +
            $"llohellohellohellohellohellohellohellohellohellohelloh" +
            $"ellohellohellohellohellohellohellohellohellohellohelloh" +
            $"ellohellohellohellohellohellohellohellohellohellohell" +
            $"ohellohellohellohellohellohe" +
            $"llohellohellohellohellohellohellohello";
    }
}
