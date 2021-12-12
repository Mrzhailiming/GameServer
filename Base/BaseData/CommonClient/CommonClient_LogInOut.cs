using Base.Tick;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Base.BaseData
{
    public partial class CommonClient
    {

        /// <summary>
        /// 角色登录的时候要做的一些事情
        /// </summary>
        public void LogIn()
        {
            
        }

        /// <summary>
        /// 角色登出的时候要做的一些事情
        /// </summary>
        public void LogOut()
        {
            // 角色登出的时候, 不再tick其 TickInfos
            mClientTickInfos.SetRemoveFlag();
        }
    }
}
