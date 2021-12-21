using Base;
using Base.Attributes;
using Base.BaseData;
using Base.Client;
using Base.DataHelper;
using ConnmonMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Handler.CmdHandlers
{
    [CMDTypeAttribute(CMDType = CMDType.RoomServer)]
    class RoomServerCMDHandler
    {
        /// <summary>
        /// 服务器和房间服务器用,
        /// 服务器接受客户端的帧同步消息 x
        /// 房间服务器接收所有客户端的帧同步消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.RCRSFrameSynchronization)]
        public static void ProcessFrameSynchronization(CommonClient client, CommonMessage message)
        {
            Console.WriteLine($"roomserver recv roomclient FrameSynchronization");
        }

        /// <summary>
        /// 房间服务器用 客户端之间的加入房间消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.RCRSJionRoom)]
        public static void ProcessJionRoom(CommonClient client, CommonMessage message)
        {
            RCRSJionRoom jionRoom = message.GetObject<RCRSJionRoom>();
            client.RoleID = jionRoom.RoleID;

            if (jionRoom.Camp == ClientInfo.MyCamp) // 队友
            {
                RoomPlayersManager.Instance().AddTeamer(client);
            }
            else // 敌人
            {
                RoomPlayersManager.Instance().AddEnemy(client);
            }

            RSRCJionRoomRsp RoomServerJionRoomRsp = new RSRCJionRoomRsp()
            {
                Result = 0
            };

            CommonMessage commonMessage = new CommonMessage()
            {
                mCMD = CMDS.RSRCJionRoomRsp,
                mMessageBuffer = MessageBufHelper.GetBytes(RoomServerJionRoomRsp)
            };
            // RoomServerJionRoomRsp 发回复的
            client.Send(commonMessage);

            Console.WriteLine($"RoomServer 回复 RSRCJionRoomRsp roleid:{client.RoleID}");
        }

        /// <summary>
        /// 房间服务器 接收 房间客户端 的登录请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.RCRSLogIn)]
        public static void ProcessRoomServerCSLogIn(CommonClient client, CommonMessage message)
        {
            CSLogIn jionRoom = message.GetObject<CSLogIn>();
            client.RoomServerIP = jionRoom.RoomServerIP;
            client.RoomServerPort = jionRoom.RoomServerPort.ToString();

            RoomClientManager.Instance().AddOnLogInCient(client);

            RoomServerBackLogin(client);
        }

        static void RoomServerBackLogin(CommonClient client)
        {
            SCLogIn SCLogIn = new SCLogIn()
            {
                Result = 0,
                RoleID = 001
            };
            byte[] result = MessageBufHelper.GetBytes(SCLogIn);

            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.RSRCLogIn,
                mMessageBuffer = result
            };

            Console.WriteLine($"roome server 回复 roomclient 登录结果:{client.ctx.Channel.RemoteAddress}");
            client.Send(message);
        }
    }
}
