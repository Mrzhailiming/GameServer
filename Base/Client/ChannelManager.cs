using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Client
{
    public class ChannelManager : Singletion<ChannelManager>
    {
        /// <summary>
        /// 连接中心服的 channel
        /// </summary>
        public MyChannel ChannelToCenterServer { get; set; }
        /// <summary>
        /// 房间服务器的 channel
        /// </summary>
        public MyChannel ChannelRoomServer { get; set; }
        /// <summary>
        /// 连接房间服务器的 channel 多个
        /// </summary>
        public Dictionary<int, MyChannel> ChannelToRoomServers { get; set; } = new Dictionary<int, MyChannel>();
    }

    public class MyChannel
    {
        public IChannel Channel { get; set; } = null;

        public ChannelType channelType { get; set; }

        public List<MultithreadEventLoopGroup> Groups { get; set; } = null;

        public void ShutDown()
        {
            if(null != Channel)
            {
                Channel.CloseAsync();
            }

            if(null != Groups)
            {
                foreach(var group in Groups)
                {
                    group.ShutdownGracefullyAsync().Wait(1000);
                }
            }
        }
    }

    public enum ChannelType
    {
        Server,
        RoomServer,
    }
}
