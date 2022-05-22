
using Entity.Component;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MySystem.Global
{
    public class Global
    {
        public static void SendAsync(SocketComponent socketComponent, byte[] buf)
        {
            SetBuffer(socketComponent.mSocketArg, buf);

            socketComponent.mSocket.Send(buf);
        }

        public static void SetBuffer(SocketAsyncEventArgs AsyncEventArgs, byte[] buf)
        {
            Buffer.BlockCopy(buf, 0, AsyncEventArgs.Buffer, AsyncEventArgs.Offset, buf.Length);

            AsyncEventArgs.SetBuffer(AsyncEventArgs.Offset, buf.Length);
        }
    }
}
