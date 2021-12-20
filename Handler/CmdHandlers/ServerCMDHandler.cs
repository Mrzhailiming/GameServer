﻿using Base;
using Base.Attributes;
using Base.BaseData;
using Base.DataHelper;
using Base.Logger;
using ConnmonMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Handler.CmdHandlers
{
    [CMDTypeAttribute(CMDType = CMDType.Server)]
    public class ServerCMDHandler
    {
        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.HeartBeat)]
        public static void ProcessHeartBeat(CommonClient client, CommonMessage message)
        {
            HeartBeat heartBeat = message.GetObject<HeartBeat>();

            client.PrevHeartBeatTick = heartBeat.Tick;

            LoggerHelper.Instance().Log(LogType.HeartBeat, $"server recv heartbeat");
        }
        /// <summary>
        /// 服务器
        /// 玩家登录
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.CSLogIn)]
        public static void ProcessCSLogIn(CommonClient client, CommonMessage message)
        {
            CSLogIn jionRoom = message.GetObject<CSLogIn>();
            client.RoomServerIP = jionRoom.RoomServerIP;
            client.RoomServerPort = jionRoom.RoomServerPort.ToString();

            ClientManager.Instance().AddOnLogInCient(client);

            BackLogin(client);
        }

        /// <summary>
        /// 客户端发起匹配
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.CSMatch)]
        public static void ProcessCSMatch(CommonClient client, CommonMessage message)
        {
            CSMatch match = message.GetObject<CSMatch>();

            ClientManager.Instance().AddMatchClient(client);
        }

        static void BackLogin(CommonClient client)
        {
            SCLogIn SCLogIn = new SCLogIn()
            {
                Result = 0,
                RoleID = 001
            };
            byte[] result = MessageBufHelper.GetBytes(SCLogIn);

            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.SCLogIn,
                mMessageBuffer = result
            };

            client.Send(message);
        }
    }
}