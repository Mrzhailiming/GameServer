using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public enum CMDS : long
    {
        Test = 1,

        #region Client Server

        CSLogIn,    // 玩家发起登录
        SCLogIn,    // 服务器 回复登录结果
        CSMatch,    // 客户端 向 服务器 发起匹配请求
        SCMatch,    // 服务器 返回匹配结果

        #endregion


        #region RoomClient RoomServer

        RCRSLogIn,    // 玩家发起登录
        RSRCLogIn,    // 服务器 回复登录结果
        
        RCRSJionRoom,   // 房间客户端 向 房间服务器 发起加入房间的请求
        RSRCJionRoomRsp, // 房间服务器 回复 房间客户端 加入房间结果

        RSRCFrameSynchronization, // 房间服务器 广播的消息
        RCRSFrameSynchronization,

        #endregion
    }
    public enum CMDType : long
    {
        Server,
        Client,
        RoomServer,
        RoomClient,
    }
}
