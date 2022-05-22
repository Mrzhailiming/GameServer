using Base.Logger;
using Entity;
using Entity.Component;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MySystem
{
    public class SocketSystem
    {
        public void RunServer(IEntity entity, IPEndPoint iPEndPoint, SocketType socketType, ProtocolType protocolType
            , string ip, int port, int backlog)
        {
            SocketEntity socketEntity = entity as SocketEntity;
            if (null == socketEntity)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"SocketSystem Init SocketEntity null");
                return;
            }

            SocketComponent socketComponent = entity.GetComponent<SocketComponent>() as SocketComponent;
            if (null == socketComponent)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"SocketSystem Init SocketComponent null");
                return;
            }


            CreateSocket(socketComponent, iPEndPoint, socketType, protocolType);

            Bind(socketComponent, ip, port);

            Listen(socketComponent, backlog);

            AcceptAsync(socketComponent);

            LoggerHelper.Instance().Log(LogType.Console, $"RunServer 开始监听 IP:{ip} Port:{port}");
        }


        public void RunClient(IEntity entity, IPEndPoint iPEndPoint, SocketType socketType, ProtocolType protocolType
            , string ip, int port, object userToken)
        {
            SocketEntity socketEntity = entity as SocketEntity;
            if (null == socketEntity)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"SocketSystem Init SocketEntity null");
                return;
            }

            SocketComponent socketComponent = entity.GetComponent<SocketComponent>() as SocketComponent;
            if (null == socketComponent)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"SocketSystem Init SocketComponent null");
                return;
            }

            LoggerHelper.Instance().Log(LogType.Console, $"连接server IP:{ip} Port:{port}");

            CreateSocket(socketComponent, iPEndPoint, socketType, protocolType);

            Connect(socketComponent, iPEndPoint, userToken);
        }


        public void CreateSocket(SocketComponent socketComponent, IPEndPoint iPEndPoint, SocketType socketType, ProtocolType protocolType)
        {
            // 创建socket
            socketComponent.mSocket = new Socket(iPEndPoint.AddressFamily, socketType, protocolType);
        }

        public void Connect(SocketComponent socketComponent, IPEndPoint iPEndPoint, object userToken)
        {
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
            socketAsyncEventArgs.SetBuffer(new byte[1024 * 1024], 0, 1024 * 1024);
            socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Connect_Completed);
            socketAsyncEventArgs.RemoteEndPoint = iPEndPoint;
            socketAsyncEventArgs.UserToken = userToken;

            socketComponent.mSocketArg = socketAsyncEventArgs;
            socketComponent.mSocket.ConnectAsync(socketAsyncEventArgs);
        }

        public void Bind(SocketComponent socketComponent, string ip, int port)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socketComponent.mSocket.Bind(iPEndPoint);
        }

        public void Listen(SocketComponent socketComponent, int backlog)
        {
            socketComponent.mSocket.Listen(backlog);
        }

        public void AcceptAsync(SocketComponent socketComponent)
        {
            socketComponent.mSocketArg = new SocketAsyncEventArgs();
            socketComponent.mSocketArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptCompleted);
            //socketComponent.mSocketArg.SetBuffer(new byte[1024 * 1024], 0, 1024 * 1024);

            socketComponent.mSocketArg.UserToken = socketComponent;
            //socketComponent.mSocketInvild = true;
            socketComponent.mSocket.AcceptAsync(socketComponent.mSocketArg);
        }
        public void Accept(SocketAsyncEventArgs e)
        {
            SocketComponent socketComponent = e.UserToken as SocketComponent;
            if (null == socketComponent)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"Accept SocketComponent null");
            }

            socketComponent.mSocket.AcceptAsync(socketComponent.mSocketArg);
        }

        public void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            LoggerHelper.Instance().Log(LogType.Console, $"AcceptCompleted");
            ProcessAccept(e);
        }

        public void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (null == e.AcceptSocket)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"ProcessAccept e.AcceptSocket null");
                return;
            }

            LoggerHelper.Instance().Log(LogType.Console, $"ProcessAccept e.AcceptSocket != null");

            // 接收到新链接
            SocketAsyncEventArgs clientArg = new SocketAsyncEventArgs();
            // 设置结束缓冲区
            clientArg.SetBuffer(new byte[1024 * 1024], 0, 1024 * 1024);

            SocketComponent socketComponent = new SocketComponent();
            socketComponent.mSocketArg = clientArg;
            socketComponent.mSocket = e.AcceptSocket;

            clientArg.UserToken = socketComponent;
            clientArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

            e.AcceptSocket = null;

            bool isAsync = socketComponent.mSocket.ReceiveAsync(socketComponent.mSocketArg);
            if (!isAsync)
            {
                ProcessReceive(socketComponent.mSocketArg);
            }

            // 继续接收下一个连接
            Accept(e);
        }
        public void Connect_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a Connect");
            }
        }
        public void ProcessConnect(SocketAsyncEventArgs e)
        {
            SocketComponent socketComponent = e.UserToken as SocketComponent;
            if (null == socketComponent)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"client ProcessConnect SocketComponent null");
            }
            else
            {
                LoggerHelper.Instance().Log(LogType.Console, $"client ProcessConnect SocketComponent != null");
            }

            socketComponent.mSocketInvild = true;
        }
        public void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] recv = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, e.Offset, recv, 0, e.BytesTransferred);
                string recvstr = Encoding.Default.GetString(recv);

                LoggerHelper.Instance().Log(LogType.Console, $"ProcessReceive recvstr:{e.BytesTransferred} recvstr:{recvstr}");

                // 继续异步接收
                SocketComponent socketComponent = e.UserToken as SocketComponent;
                socketComponent.mSocket.ReceiveAsync(socketComponent.mSocketArg);
            }
            else
            {
                if (e.SocketError == SocketError.SocketError)
                {
                    Close(e);
                    LoggerHelper.Instance().Log(LogType.Console, "ProcessReceive() SocketError");
                }
                else
                {
                    LoggerHelper.Instance().Log(LogType.Console, $"ProcessReceive ReceiveAsync SocketError:{e.SocketError}");
                }
            }
        }
        public void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                LoggerHelper.Instance().Log(LogType.Console, "ProcessSend()");

                // 恢复缓冲区的大小
                e.SetBuffer(0, 1024 * 1024);
            }
            else
            {
                Close(e);
                LoggerHelper.Instance().Log(LogType.Console, "ProcessSend()");
            }
        }

        public void Close(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            try
            {
                socketAsyncEventArgs.ConnectSocket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance().Log(LogType.Exception, ex.ToString());
            }
            socketAsyncEventArgs.ConnectSocket.Close();

        }
    }

    public delegate void Asyncdelegate(object sender, SocketAsyncEventArgs e);
}
