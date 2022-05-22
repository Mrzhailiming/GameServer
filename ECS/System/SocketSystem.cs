using Base.Logger;
using Entity;
using Entity.Component;
using Singleton.Manager;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MySystem
{
    public class SocketSystem
    {
        public void Init()
        {
            // 初始化缓冲区
            for (int index = 0; index < BufferManager.MaxConnect; ++index)
            {
                BufferManager.Instance().SendBufUseMap.Add(index, false);
                BufferManager.Instance().RecvBufUseMap.Add(index, false);
            }

        }

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
            , string ip, int port)
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

            Connect(socketComponent, iPEndPoint);
        }


        public void CreateSocket(SocketComponent socketComponent, IPEndPoint iPEndPoint, SocketType socketType, ProtocolType protocolType)
        {
            // 创建socket
            socketComponent.mSocket = new Socket(iPEndPoint.AddressFamily, socketType, protocolType);
        }

        public void Connect(SocketComponent socketComponent, IPEndPoint iPEndPoint)
        {
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();

            socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Connect_Completed);
            socketAsyncEventArgs.RemoteEndPoint = iPEndPoint;
            socketAsyncEventArgs.UserToken = socketComponent;
            socketComponent.mSocketArg = socketAsyncEventArgs;

            // 主动连接 设置sendbuf
            if (!PopBuf(socketComponent, socketAsyncEventArgs))
            {
                LoggerHelper.Instance().Log(LogType.Console, $"SocketSystem Connect PopBuf false");
                return;
            }

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

            SocketComponent socketComponent = new SocketComponent();
            socketComponent.mSocketArg = clientArg;
            socketComponent.mSocket = e.AcceptSocket;
            socketComponent.mConnectType = ConnectType.Accept;

            clientArg.UserToken = socketComponent;
            clientArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

            e.AcceptSocket = null;

            // 被动连接 设置 recvbuf
            if (!PopBuf(socketComponent, clientArg))
            {
                LoggerHelper.Instance().Log(LogType.Console, $"SocketSystem ProcessAccept PopBuf false");
                return;
            }

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
            socketComponent.mSocket = e.ConnectSocket;
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
                // 再次设置接收缓冲区
                socketComponent.mSocketArg.SetBuffer(socketComponent.mSocketArg.Offset, 1024 * 1024);
                bool isAsync = socketComponent.mSocket.ReceiveAsync(socketComponent.mSocketArg);
                if (!isAsync)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                if (e.SocketError == SocketError.SocketError)
                {
                    LoggerHelper.Instance().Log(LogType.Console, "ProcessReceive() SocketError");
                }
                else
                {
                    LoggerHelper.Instance().Log(LogType.Console, $"ProcessReceive ReceiveAsync SocketError:{e.SocketError}");
                }

                Close(e);
            }
        }
        public void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                LoggerHelper.Instance().Log(LogType.Console, "ProcessSend()");

                // 恢复缓冲区的大小
                e.SetBuffer(e.Offset, 1024 * 1024);
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
                SocketComponent socketComponent = socketAsyncEventArgs.UserToken as SocketComponent;
                if (null == socketComponent)
                {
                    LoggerHelper.Instance().Log(LogType.Console, "SocketSystem Close PushBuf SocketComponent null");
                    return;
                }

                PushBuf(socketComponent);

                socketComponent.mSocket?.Shutdown(SocketShutdown.Send);
                socketComponent.mSocket?.Close();
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance().Log(LogType.Exception, ex.ToString());
            }
        }

        #region 管理缓冲区
        public void PushBuf(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            SocketComponent socketComponent = socketAsyncEventArgs.UserToken as SocketComponent;
            if (null == socketComponent)
            {
                LoggerHelper.Instance().Log(LogType.Console, "SocketSystem Close PushBuf SocketComponent null");
                return;
            }

            PushBuf(socketComponent);
        }
        public void PushBuf(SocketComponent socketComponent)
        {
            try
            {
                Dictionary<int, bool> map;
                if (socketComponent.mConnectType == ConnectType.Connect)
                {
                    map = BufferManager.Instance().SendBufUseMap;
                }
                else if (socketComponent.mConnectType == ConnectType.Accept)
                {
                    map = BufferManager.Instance().RecvBufUseMap;
                }
                else
                {
                    LoggerHelper.Instance().Log(LogType.Console, "SocketSystem PushBuf ConnectType undefined");
                    return;
                }

                int freeIndex = socketComponent.mBufOffset / BufferManager.PerBufLen;

                map[freeIndex] = false;

                LoggerHelper.Instance().Log(LogType.Console, $"SocketSystem PushBuf freeIndex:{freeIndex}");
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance().Log(LogType.Exception, $"SocketSystem PushBuf ex:{ex}");
            }
        }

        public bool PopBuf(SocketComponent socketComponent, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            try
            {
                Dictionary<int, bool> map;
                byte[] buf;
                int freeIndex;
                // 设置发送缓冲区
                if (socketComponent.mConnectType == ConnectType.Connect)
                {
                    map = BufferManager.Instance().SendBufUseMap;
                    buf = BufferManager.Instance().SendTotalBuffer;
                }
                // 设置接收缓冲区
                else if (socketComponent.mConnectType == ConnectType.Accept)
                {
                    map = BufferManager.Instance().RecvBufUseMap;
                    buf = BufferManager.Instance().RecvTotalBuffer;
                }
                else
                {
                    LoggerHelper.Instance().Log(LogType.Console, "SocketSystem PopBuf ConnectType undefined");
                    return false;
                }

                freeIndex = GetFreeIndex(map);
                if (freeIndex < 0)
                {
                    LoggerHelper.Instance().Log(LogType.Console, "SocketSystem PopBuf buf is full");
                    return false;
                }
                map[freeIndex] = true;
                int offset = freeIndex * BufferManager.PerBufLen;

                socketAsyncEventArgs.SetBuffer(buf, offset, BufferManager.PerBufLen);
                socketComponent.mBufOffset = offset;
                socketComponent.mBufLength = BufferManager.PerBufLen;

                LoggerHelper.Instance().Log(LogType.Console, $"SocketSystem PopBuf freeIndex:{freeIndex}");
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance().Log(LogType.Exception, $"SocketSystem PopBuf ex:{ex}");
                return false;
            }
        }


        public int GetFreeIndex(Dictionary<int, bool> map)
        {
            foreach (var pair in map)
            {
                if (pair.Value)
                {
                    continue;
                }

                return pair.Key;
            }

            return -1;
        }

        #endregion
    }

    public delegate void Asyncdelegate(object sender, SocketAsyncEventArgs e);
}
