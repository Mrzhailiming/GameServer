using Base;
using Base.Attributes;
using Base.BaseData;
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

        [CmdHandlerAttribute(CmdID = CMDS.FrameSynchronization)]
        public static void ProcessFrameSynchronization(CommonClient client, CommonMessage message)
        {

            var allClient = ClientManager.Instance().GetAllClient();
            int count = 0;
            while(true)
            {
                foreach (var cli in allClient.Values)
                {
                    if (client != cli)
                    {
                        cli.Send(message);
                    }
                }

                Thread.Sleep(1 * 1000);
            }
            
        }
    }
}
