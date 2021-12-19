﻿using Base;
using Base.Attributes;
using Base.BaseData;
using Base.Client;
using Base.DataHelper;
using Base.Tick;
using ConnmonMessage;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Handler.CmdHandlers
{

    [CMDTypeAttribute(CMDType = CMDType.Client)]
    public class ClientCmdHandler
    {
        [CmdHandlerAttribute(CmdID = CMDS.Test)]
        public static void Process(CommonClient client, CommonMessage message)
        {
            //Person person = message.GetObject<Person>();
        }

        /// <summary>
        /// 客户用 服务器告诉客户端连接其他的客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.SCMatch)]
        public static void ProcessSCJionRoom(CommonClient client, CommonMessage message)
        {
            SCMatch jionRoom = message.GetObject<SCMatch>();

            List<IPPort> toConnect = IPParserHelper.StringIPPortCamp(jionRoom.AllClient);

            foreach(IPPort iPPort in toConnect)
            {
                // 不要连接自己的 房间服务器
                if(iPPort.Port == ClientInfo.MyClientServerPort
                    && iPPort.IP == ClientInfo.MyClientServerIP)
                {
                    ClientInfo.MyCamp = iPPort.Camp; // 记录我的阵营
                    continue;
                }
                IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(iPPort.IP),Convert.ToInt32(iPPort.Port));

                ClientBootStrap.Instance().RunClientRoomClientAsync(EndPoint);
            }

        }


        /// <summary>
        /// 客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.SCLogIn)]
        public static void ProcessSCLogIn(CommonClient client, CommonMessage message)
        {
            SCLogIn SCLogIn = message.GetObject<SCLogIn>();

            if(SCLogIn.Result == 0)
            {
                // 
            }

            Console.WriteLine($"登录成功");

            // 开启心跳
            TickManager.Instance().AddTickInfo(new TickInfo(HeartBeatHandler, 1 * 1000, null));

            // 开始发送匹配请求
            CSMatch match = new CSMatch()
            {
                RoleID = Convert.ToInt32(ClientInfo.MyClientServerPort)
            };

            CommonMessage matchMsg = new CommonMessage()
            {
                mCMD = CMDS.CSMatch,
                mMessageBuffer = MessageBufHelper.GetBytes(match)
            };

            client.Send(matchMsg);
            Console.WriteLine($"开始匹配...");
        }


        private static bool HeartBeatHandler(long ticks)
        {
            HeartBeat heartBeat = new HeartBeat()
            {
                State = "1",
                Tick = DateTime.Now.Ticks
            };

            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.HeartBeat,
                mMessageBuffer = MessageBufHelper.GetBytes(heartBeat)
            };

            SocketInfo.Instance().mCenterServer.Send(message);
            Console.WriteLine($"client send heartbeat");
            return true;
        }
    }
}
