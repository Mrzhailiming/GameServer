using Base;
using Base.Attributes;
using Base.BaseData;
using Base.Client;
using Base.DataHelper;
using ConnmonMessage;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Handler.CmdHandlers
{
    [CMDTypeAttribute(CMDType = CMDType.RoomClient)]
    class RoomClientCMDHandler
    {
        /// <summary>
        /// 接受房间服务器同步的消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.RSRCFrameSynchronization)]
        public static void ProcessRSRCFrameSynchronization(CommonClient client, CommonMessage message)
        {
            RSRCSynchronousInfo roomServerSynchronousInfo = message.GetObject<RSRCSynchronousInfo>();
            Console.WriteLine($"RoomClient recv roomserver  FrameSynchronization camp:{roomServerSynchronousInfo.Camp}info:{roomServerSynchronousInfo.OperationInfo}");
        }

        /// <summary>
        /// 接受房间服务器返回的消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.RSRCJionRoomRsp)]
        public static void ProcessRSRCJionRoomRsp(CommonClient client, CommonMessage message)
        {
            RSRCJionRoomRsp jionRoom = message.GetObject<RSRCJionRoomRsp>();

            if (jionRoom.Result == 0)
            {

            }

            SynchronousInfo synchronousInfo = new SynchronousInfo()
            {
                Name = "client",
                OperationInfo = "帧同步",
            };

            CommonMessage commonMessage = new CommonMessage()
            {
                mCMD = CMDS.RCRSFrameSynchronization,
                mMessageBuffer = MessageBufHelper.GetBytes(synchronousInfo)
            };

            Test.RunAsync();
        }

        /// <summary>
        /// 房间客户端 接收房间服务器 返回的登录结果
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.RSRCLogIn)]
        public static void ProcessRoomServerSCLogIn(CommonClient client, CommonMessage message)
        {
            SCLogIn SCLogIn = message.GetObject<SCLogIn>();

            if (SCLogIn.Result == 0)
            {
                // 
            }
            Console.WriteLine($"客户端登录房间服务器成功, 准备加入房间 JionRoom");
            JionRoom(client);
        }

        static void JionRoom(CommonClient client)
        {
            Random ran = new Random();
            int n = ran.Next(100, 1000);

            string ip = IPParserHelper.StringIP(((IPEndPoint)client.ClientEndPoint).Address.ToString());
            string port = ((IPEndPoint)client.ClientEndPoint).Port.ToString();


            RCRSJionRoom joinRoom = new RCRSJionRoom()
            {
                Camp = ClientInfo.MyCamp, // 告诉房间服务器 我的阵营
                RoleID = n
            };
            byte[] result = MessageBufHelper.GetBytes(joinRoom);

            CommonMessage message = new CommonMessage()
            {
                mCMD = CMDS.RCRSJionRoom,
                mMessageBuffer = result
            };

            client.Send(message);
        }

    }
    public class Test
    {
        public static async void RunAsync()
        {
            await Task.Run(Execute);
        }

        public static void Execute()
        {
            int i = 100;
            while (i > 0)
            {
                SynchronousInfo synchronousInfo = new SynchronousInfo()
                {
                    Name = "client",
                    OperationInfo = "帧同步"
                };

                CommonMessage commonMessage = new CommonMessage()
                {
                    mCMD = CMDS.RCRSFrameSynchronization,
                    mMessageBuffer = MessageBufHelper.GetBytes(synchronousInfo)
                };

                RoomPlayersManager.Instance().BroadCast(commonMessage);

                --i;
                Thread.Sleep(2 * 1000);
            }
        }
    }
}
