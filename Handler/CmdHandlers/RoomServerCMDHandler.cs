﻿using Base;
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
        [CmdHandlerAttribute(CmdID = CMDS.FrameSynchronization)]
        public static void ProcessFrameSynchronization(CommonClient client, CommonMessage message)
        {
            Console.WriteLine($"roomserver recv roomclient FrameSynchronization");
        }

        /// <summary>
        /// 房间服务器用 客户端之间的加入房间消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.JionRoom)]
        public static void ProcessJionRoom(CommonClient client, CommonMessage message)
        {
            JionRoom jionRoom = message.GetObject<JionRoom>();
            client.RoleID = jionRoom.RoleID;

            if (jionRoom.JionType == 0)
            {
                RoomPlayersManager.Instance().AddTeamer(client);
            }
            else
            {
                RoomPlayersManager.Instance().AddEnemy(client);
            }

            RoomServerJionRoomRsp RoomServerJionRoomRsp = new RoomServerJionRoomRsp()
            {
                Result = 0
            };

            CommonMessage commonMessage = new CommonMessage()
            {
                mCMD = CMDS.RoomServerJionRoomRsp,
                mMessageBuffer = MessageBufHelper.GetBytes(RoomServerJionRoomRsp)
            };
            // RoomServerJionRoomRsp 发回复的
            client.Send(commonMessage);
        }

        /// <summary>
        /// 房间服务器 接收 房间客户端 的登录请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.RoomServerCSLogIn)]
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
                mCMD = CMDS.RoomServerSCLogIn,
                mMessageBuffer = result
            };

            client.Send(message);
        }
    }
}
