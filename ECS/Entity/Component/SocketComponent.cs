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
        public SocketAsyncEventArgs mSocketArg { get; set; }
        /// <summary>
        /// 缓冲区偏移
        /// </summary>
        public int mBufOffset { get; set; }
        /// <summary>
        /// 缓冲区长度
        /// </summary>
        public int mBufLength { get; set; }
        public IEntity mOwner { get; set; }

        public bool mSocketInvild { get; set; } = false;
    }
}
