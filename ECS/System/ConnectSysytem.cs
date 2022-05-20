using Base.Logger;
using Entity;
using Entity.Component;
using System;

namespace MySystem
{
    /// <summary>
    /// 连接系统
    /// 1.打印 ConnectionComponent 的 name
    /// </summary>
    public class ConnectSysytem : ISystem
    {
        public void Send(IEntity entity)
        {
            ConnectionComponent cp = entity.GetComponent<ConnectionComponent>() as ConnectionComponent;

            if (null == cp)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"ConnectSysytem Send GetComponent null");
                return;
            }

            byte[] buf = new byte[100];
            cp?.mSocket.Send(buf);
        }

        public void Tick(long tick, IEntity entity)
        {
            ConnectionComponent connection = entity.GetComponent<ConnectionComponent>() as ConnectionComponent;

            if (null == connection)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"ConnectSysytem Tick GetComponent null");
                return;
            }
            PrintName(connection);
            LoggerHelper.Instance().Log(LogType.Console, $"ConnectSysytem tick");
        }


        public void PrintName(ConnectionComponent connection)
        {
            LoggerHelper.Instance().Log(LogType.Console, $"ConnectSysytem PrintName entityOwner:{((IEntityOwner)connection.mOwner)?.mOwner} ComponentOwner:{connection?.mOwner} {connection?.mName}");
        }
    }
}
