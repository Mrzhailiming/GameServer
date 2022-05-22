using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Entity.Component
{
    public  class SocketComponent : IComponent, IComponentOwner
    {
        public Socket mSocket;

        public SocketAsyncEventArgs mSocketArg;

        public IEntity mOwner { get; set; }

        public bool mSocketInvild = false;
    }
}
