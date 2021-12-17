using Base;
using Base.Attributes;
using Base.BaseData;
using Base.Client;
using Base.DataHelper;
using ConnmonMessage;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Handler.CmdHandlers
{
    public class CmdHandler_Test
    {
        [CmdHandlerAttribute(CmdID = CMDS.Test)]
        public static void Process(CommonClient client, CommonMessage message)
        {
            Person person = message.GetObject<Person>();
            //Console.WriteLine($"recv {person}");
            //client.Send(message);
        }

        /// <summary>
        /// 服务器和客户端用, 接受客户端的帧同步消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.FrameSynchronization,
            CMDType = CMDType.ServerAndClient)]
        public static void ProcessFrameSynchronization(CommonClient client, CommonMessage message)
        {
            Console.WriteLine($"server and client recv client FrameSynchronization");

            //var allClient = ClientManager.Instance().GetAllClient();
            //int count = 0;
            //while(true)
            //{
            //    foreach (var cli in allClient.Values)
            //    {
            //        if (client != cli)
            //        {
            //            cli.Send(message);
            //        }
            //    }
            //    Thread.Sleep(1 * 1000);
            //}

        }

        /// <summary>
        /// 房间服务器用 客户端之间的加入房间消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.JionRoom,
            CMDType = CMDType.Client)]
        public static void ProcessJionRoom(CommonClient client, CommonMessage message)
        {
            JionRoom jionRoom = message.GetObject<JionRoom>();

            if(jionRoom.JionType == 0)
            {
                RoomPlayersManager.Instance().AddTeamer(client);
            }
            else
            {
                RoomPlayersManager.Instance().AddEnemy(client);
            }

        }

        /// <summary>
        /// 客户用 服务器告诉客户端连接其他的客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.SCJionRoom,
            CMDType = CMDType.Client)]
        public static void ProcessSCJionRoom(CommonClient client, CommonMessage message)
        {
            SCJoinRoom jionRoom = message.GetObject<SCJoinRoom>();


            List<IPPort> toConnect = IPParserHelper.StringIPPort(jionRoom.AllClient);

            foreach(IPPort iPPort in toConnect)
            {
                // 不要连接自己的房间
                if(iPPort.Port == ClientInfo.MyClientServerPort)
                {
                    continue;
                }
                IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(iPPort.IP),Convert.ToInt32(iPPort.Port));

                ClientBootStrap.Instance().RunClientRoomClientAsync(EndPoint);
            }

        }

        /// <summary>
        /// 玩家登录
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.CSLogIn,
            CMDType = CMDType.ServerAndClient)]
        public static void ProcessCSLogIn(CommonClient client, CommonMessage message)
        {
            CSLogIn jionRoom = message.GetObject<CSLogIn>();
            client.RoomServerIP = jionRoom.RoomServerIP;
            client.RoomServerPort = jionRoom.RoomServerPort.ToString();

            ClientManager.Instance().AddOnLogInCient(client);

            BackLogin(client);
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

        /// <summary>
        /// 收到
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.SCLogIn,
            CMDType = CMDType.Client)]
        public static void ProcessSCLogIn(CommonClient client, CommonMessage message)
        {
            SCLogIn SCLogIn = message.GetObject<SCLogIn>();

            if(SCLogIn.Result == 0)
            {
                // 
            }

            Console.WriteLine($"登录成功");
        }

        [CmdHandlerAttribute(CmdID = CMDS.RoomServerCSLogIn,
            CMDType = CMDType.Client)]
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

        [CmdHandlerAttribute(CmdID = CMDS.RoomServerSCLogIn,
            CMDType = CMDType.Client)]
        public static void ProcessRoomServerSCLogIn(CommonClient client, CommonMessage message)
        {
            SCLogIn SCLogIn = message.GetObject<SCLogIn>();

            if (SCLogIn.Result == 0)
            {
                // 
            }

            JionRoom(client);
        }

        static void JionRoom(CommonClient client)
        {
            JionRoom joinRoom = new JionRoom()
            {
                JionType = 0,
                RoleID = 001
            };
            byte[] result = MessageBufHelper.GetBytes(joinRoom);
            
            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.JionRoom,
                mMessageBuffer = result
            };

            client.Send(message);
        }
    }
}
