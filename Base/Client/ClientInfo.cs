using Base.BaseData;
using Base.DataHelper;
using Base.Logger;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Base.Client
{
    /// <summary>
    /// 保存与 中心服 和 房间服 的连接
    /// </summary>
    public class SocketInfo : Singletion<SocketInfo>
    {
        /// <summary>
        /// 客户端连接的中心服
        /// </summary>
        public CommonClient mCenterServer { get; set; }
        /// <summary>
        /// 客户端连接的roomserver, 妈的会连接 9 个, 这弄一个也没有用啊
        /// </summary>
        public ConcurrentDictionary<IChannelHandlerContext, CommonClient> mRoomServer { get; set; }
            = new ConcurrentDictionary<IChannelHandlerContext, CommonClient>();

        private int ConnectRoomServerSuccessCount = 0;

        public RoomPlayersManager mRoomPlayersManager { get; }

        /// <summary>
        /// 客户端给中心服务器发消息用这个
        /// 这是只连接一个server
        /// </summary>
        /// <param name="message"></param>
        public void SendToCenterServer(CommonMessage message)
        {
            mCenterServer.Send(message);
        }

        public void Add()
        {
            if (++ConnectRoomServerSuccessCount >= 3)
            {
                ConnectRoomServerSuccessCount = 0;
                JionRoom();
            }
        }

        private void JionRoom()
        {
            Thread.Sleep(3 * 1000);

            Random ran = new Random();
            int n = ran.Next(100, 1000);

            RCRSJionRoom joinRoom = new RCRSJionRoom()
            {
                Camp = ClientInfo.MyCamp, // 告诉房间服务器 我的阵营
                RoleID = n
            };

            LoggerHelper.Instance().Log(LogType.Console, $"my roleid:{n}");
            byte[] result = MessageBufHelper.GetBytes(joinRoom);

            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.RCRSJionRoom,
                mMessageBuffer = result
            };

            foreach (var RoomServer in mRoomServer.Values)
            {
                RoomServer.Send(message);
                LoggerHelper.Instance().Log(LogType.Console, $"发送加入房间消息 to {RoomServer.ClientEndPoint}");
            }
        }
    }

    /// <summary>
    /// 目前有:
    /// 房间服务器的监听端口
    /// 房间服务器的监听IP
    /// 我的阵营
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// 房间服务器的监听端口
        /// </summary>
        public static string MyClientServerPort = "9999";
        /// <summary>
        /// 房间服务器的监听IP
        /// </summary>
        public static string MyClientServerIP = "127.0.0.1";
        /// <summary>
        /// 我的阵营
        /// </summary>
        public static string MyCamp = "common";
    }
}
