using Base.Logger;
using Entity;
using Entity.Component;
using MySystem;
using MySystem.Global;
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

            IEntity entity = new SocketEntity();
            SocketComponent socketComponent = new SocketComponent();
            socketComponent.mOwner = entity;
            entity.AddComponent(typeof(SocketComponent), socketComponent);

            string ip = "127.0.0.1";
            int port = 8888;
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            SocketType socketType = SocketType.Stream;
            ProtocolType protocolType = ProtocolType.Tcp;

            socketSystem.RunClient(entity, iPEndPoint, socketType, protocolType, ip, port, socketComponent);

            while (true)
            {
                if (!socketComponent.mSocketInvild)
                {
                    Thread.Sleep(1000 * 2);
                }
                else
                {
                    break;
                }
            }

            Thread.Sleep(1000 * 2);
            string s = "hello";

            byte[] buf = Encoding.Default.GetBytes(Str);


            Console.WriteLine($"str:{Str.Length}");

            Global.SendAsync(socketComponent, buf);

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
