using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Entity.Component
{
    /// <summary>
    /// 连接组件
    /// 1.socket
    /// </summary>
    public class ConnectionComponent : IComponent, IComponentOwner
    {
        public Socket mSocket;

        public string mName = "666";

        public IEntity mOwner { get; set; }
    }

}
