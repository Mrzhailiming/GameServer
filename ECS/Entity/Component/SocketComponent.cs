using Global;
using Singleton.Manager;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Entity.Component
{
    public  class SocketComponent : IComponent, IComponentOwner
    {
        public ConnectType mConnectType { get; set; }
        public Socket mSocket { get; set; }
        /// <summary>
        /// 异步事件
        /// </summary>
        public SocketAsyncEventArgs mRecvSocketArg { get; set; }
        /// <summary>
        /// 异步事件
        /// </summary>
        public SocketAsyncEventArgs mSendSocketArg { get; set; }
        /// <summary>
        /// 发送缓冲区偏移
        /// </summary>
        public int mSendBufOffset { get; set; }
        /// <summary>
        /// 发送缓冲区长度
        /// </summary>
        public int mSendBufLength { get; set; }
        /// <summary>
        /// 接收缓冲区偏移
        /// </summary>
        public int mRecvBufOffset { get; set; }
        /// <summary>
        /// 接收缓冲区长度
        /// </summary>
        public int mRecvBufLength { get; set; }
        public IEntity mOwner { get; set; }

        public bool mSocketInvild { get; set; } = false;

        public int cmdID = -1;
        public byte[] recvBuff = null;
        public byte[] headerBuff = null;
        public int hadRecvNum = 0;
        public int needRecvNum = 0;

        public byte[] sendBuf = null;
    }
}
