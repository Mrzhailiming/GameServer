using Base.Logger;
using Entity.Component;
using Global;
using Singleton.Manager;
using System;
using System.Net.Sockets;
using System.Text;

namespace MySystem
{
    public partial class SocketSystem
    {


        public static void SendAsync(SocketComponent socketComponent, TCPCMDS cmd, string str)
        {
            SendAsync(socketComponent, GenBuf(cmd, str));
        }

        public static void SendAsync(SocketComponent socketComponent, byte[] buf)
        {
            SetBuffer(socketComponent.mSendSocketArg, buf);

            if (!socketComponent.mSocket.SendAsync(socketComponent.mSendSocketArg))
            {
                LoggerHelper.Instance().Log(LogType.Console, $"send sync");
            }
        }

        public static void SetBuffer(SocketAsyncEventArgs AsyncEventArgs, byte[] buf)
        {
            Buffer.BlockCopy(buf, 0, AsyncEventArgs.Buffer, AsyncEventArgs.Offset, buf.Length);

            AsyncEventArgs.SetBuffer(AsyncEventArgs.Offset, buf.Length);
        }

        public static byte[] GenBuf(TCPCMDS cmd, string str)
        {
            byte[] bodyBuf = Encoding.Default.GetBytes(str);
            int packetLen = Proto.protoHeadLen + bodyBuf.Length;

            byte[] buf = new byte[packetLen];

            // cmd
            Array.Copy(BitConverter.GetBytes((int)cmd), 0, buf, Proto.cmdIDOffset, 4);
            // paklen
            Array.Copy(BitConverter.GetBytes(packetLen), 0, buf, Proto.PacketLenOffset, 4);
            // body
            Array.Copy(bodyBuf, 0, buf, Proto.protoHeadLen, bodyBuf.Length);

            return buf;
        }

        public static bool BuffCopy(SocketComponent socketComponent)
        {
            try
            {
                SocketAsyncEventArgs AsyncEventArgs = socketComponent.mRecvSocketArg;
                // 接收命令头 4位cmdID 4位包长度
                if (socketComponent.needRecvNum <= 0)
                {
                    socketComponent.headerBuff = new byte[AsyncEventArgs.BytesTransferred];

                    Buffer.BlockCopy(AsyncEventArgs.Buffer, AsyncEventArgs.Offset, socketComponent.headerBuff, 0, AsyncEventArgs.BytesTransferred);
                    socketComponent.hadRecvNum += AsyncEventArgs.BytesTransferred;
                    if (socketComponent.hadRecvNum >= 8)
                    {
                        socketComponent.cmdID = Global.Global.Byte2Int(socketComponent.headerBuff, Proto.cmdIDOffset);//获取 cmdid
                        socketComponent.needRecvNum = Global.Global.Byte2Int(socketComponent.headerBuff, Proto.PacketLenOffset);//获取命令包长度

                        socketComponent.recvBuff = new byte[socketComponent.needRecvNum]; // 搞个内存池
                        Buffer.BlockCopy(socketComponent.headerBuff, 0, socketComponent.recvBuff, 0, socketComponent.headerBuff.Length);
                    }
                }

                // 接收包体
                else if (socketComponent.hadRecvNum < socketComponent.needRecvNum)
                {
                    //AsyncEventArgs.BytesTransferred 大于剩余需要拷贝的字节数，说明下一条数据来了, 咋处理
                    int realCopy = AsyncEventArgs.BytesTransferred <= (socketComponent.needRecvNum - socketComponent.hadRecvNum) ? AsyncEventArgs.BytesTransferred : (socketComponent.needRecvNum - socketComponent.hadRecvNum);
                    Buffer.BlockCopy(AsyncEventArgs.Buffer, AsyncEventArgs.Offset, socketComponent.recvBuff, socketComponent.hadRecvNum, realCopy);

                    socketComponent.hadRecvNum += realCopy;
                }

                if (socketComponent.hadRecvNum < socketComponent.needRecvNum)
                {
                    return false;
                }

                return true;// 接收完毕
            }
            catch (Exception ex)
            {
                Console.WriteLine("BuffCopy异常{0}：", ex.ToString());
                return true;
            }
        }

        public static void ResetComponentCmdBuf(SocketComponent socketComponent)
        {
            socketComponent.recvBuff = null;
            socketComponent.headerBuff = null;
            socketComponent.hadRecvNum = 0;
            socketComponent.needRecvNum = 0;
        }
    }
}
