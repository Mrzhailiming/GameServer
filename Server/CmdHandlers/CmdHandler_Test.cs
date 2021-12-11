using Base;
using Base.Attributes;
using Base.BaseData;
using ConnmonMessage;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Handler.CmdHandlers
{
    public class CmdHandler_Test
    {
        [CmdHandlerAttribute(CmdID = CMDS.Test)]
        public static void Process(CommonClient client, CommonMessage message)
        {
            Person person = message.GetObject<Person>();
            //Console.WriteLine($"recv {person}");
            client.Send(message);
        }
    }
}
