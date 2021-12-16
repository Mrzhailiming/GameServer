using Base;
using Base.BaseData;
using ConnmonMessage;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.IO;
using System.Threading;

namespace Handler
{
    /// <summary>
    /// 连接其他客户端的房间服务器
    /// </summary>
    public class ClientRoomClientHandler : SimpleChannelInboundHandler<CommonMessage>
    {
        void JionRoom(IChannelHandlerContext ctx)
        {
            JionRoom joinRoom = new JionRoom()
            {
                JionType = 0,
                RoleID = 001
            };
            byte[] result = new byte[joinRoom.CalculateSize()];
            try
            {
                using (CodedOutputStream rawOutput = new CodedOutputStream(result))
                {
                    joinRoom.WriteTo(rawOutput);
                }
            }
            catch (Exception ex)
            {
            }


            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.JionRoom,
                mMessageBuffer = result
            };

            ctx.WriteAsync(message);
            ctx.Flush();
        }

        /// <summary>
        /// 与 其他房间服务器建立连接后, 发送joinroom消息
        /// </summary>
        /// <param name="ctx"></param>
        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            // 加入其他房间服务器
            JionRoom(ctx);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, CommonMessage msg)
        {
            Console.WriteLine($"recv from clientRoomServer success {msg.mCMD}");
            // 投递
            CmdHelper.Fire(ctx, msg);
        }
    }
}
