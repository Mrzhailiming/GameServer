using Base;
using Base.Attributes;
using Base.BaseData;
using Base.DataHelper;
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
