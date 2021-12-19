using Base;
using Base.Attributes;
using Base.BaseData;
using Base.Client;
using Base.DataHelper;
using ConnmonMessage;
using System;
using System.Collections.Generic;
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
        [CmdHandlerAttribute(CmdID = CMDS.RoomServerFrameSynchronization)]
        public static void ProcessServerFrameSynchronization(CommonClient client, CommonMessage message)
        {
            RoomServerSynchronousInfo roomServerSynchronousInfo = message.GetObject<RoomServerSynchronousInfo>();
            Console.WriteLine($"RoomClient recv roomserver  FrameSynchronization:{roomServerSynchronousInfo.OperationInfo}");
        }

        /// <summary>
        /// 接受房间服务器返回的消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.RoomServerJionRoomRsp)]
        public static void ProcessRoomServerJionRoomRsp(CommonClient client, CommonMessage message)
        {
            RoomServerJionRoomRsp jionRoom = message.GetObject<RoomServerJionRoomRsp>();

            if (jionRoom.Result == 0)
            {

            }

            SynchronousInfo synchronousInfo = new SynchronousInfo()
            {
                Name = "client",
                OperationInfo = "帧同步"
            };

            CommonMessage commonMessage = new CommonMessage()
            {
                mCMD = CMDS.FrameSynchronization,
                mMessageBuffer = MessageBufHelper.GetBytes(synchronousInfo)
            };

            Test.RunAsync();
        }

        /// <summary>
        /// 房间客户端 接收房间服务器 返回的登录结果
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        [CmdHandlerAttribute(CmdID = CMDS.RoomServerSCLogIn)]
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

            JionRoom joinRoom = new JionRoom()
            {
                JionType = 0,
                RoleID = n
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
                    mCMD = CMDS.FrameSynchronization,
                    mMessageBuffer = MessageBufHelper.GetBytes(synchronousInfo)
                };

                RoomPlayersManager.Instance().BroadCast(commonMessage);

                --i;
                Thread.Sleep(2 * 1000);
            }
        }
    }
}
