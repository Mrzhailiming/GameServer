using Base.BaseData;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

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
        public Dictionary<IChannelHandlerContext, CommonClient> mRoomServer { get; set; }
            = new Dictionary<IChannelHandlerContext, CommonClient>();


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
    }


    public class ClientInfo
    {
        /// <summary>
        /// 房间服务器的监听端口
        /// </summary>
        public static string MyClientServerPort = "9999";
    }
}
