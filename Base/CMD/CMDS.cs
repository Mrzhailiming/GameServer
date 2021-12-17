using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public enum CMDS : long
    {
        Test = 1,
        FrameSynchronization = 2,
        JionRoom = 3,
        SCJionRoom = 4,
        CSLogIn = 5,    // 玩家发起登录
        SCLogIn = 6,    // 服务器回复登录结果
        RoomServerCSLogIn = 7,    // 玩家发起登录
        RoomServerSCLogIn = 8,    // 服务器回复登录结果
    }
    public enum CMDType : long
    {
        Server = 1,
        Client = 2,
        ServerAndClient = 3,
    }
}
