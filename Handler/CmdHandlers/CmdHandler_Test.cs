using Base;
using Base.Attributes;
using Base.BaseData;
using Base.Client;
using ConnmonMessage;
using Google.Protobuf;
using System;
using System.Collections.Generic;
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
        [CmdHandlerAttribute(CmdID = CMDS.FrameSynchronization)]
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
        /// 客户端用 客户端之间的加入房间消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.JionRoom)]
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

        [CmdHandlerAttribute(CmdID = CMDS.SCJionRoom)]
        public static void ProcessSCJionRoom(CommonClient client, CommonMessage message)
        {
            CSJoinRoom jionRoom = message.GetObject<CSJoinRoom>();


        }
    }
}
