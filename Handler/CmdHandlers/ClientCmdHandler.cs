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
using System.Threading.Tasks;

namespace Handler.CmdHandlers
{

    [CMDTypeAttribute(CMDType = CMDType.Client)]
    public class ClientCmdHandler
    {
        [CmdHandlerAttribute(CmdID = CMDS.Test)]
        public static void Process(CommonClient client, CommonMessage message)
        {
            Person person = message.GetObject<Person>();
        }

        /// <summary>
        /// 客户用 服务器告诉客户端连接其他的客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.SCJionRoom)]
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
        }
    }
}
