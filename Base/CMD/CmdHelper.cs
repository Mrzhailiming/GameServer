using Base.Attributes;
using Base.BaseData;
using Base.Client;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Base
{

    public class CMDHelperManager : Singletion<CMDHelperManager>
    {
        CmdHelper ClientCmdHelper { get; } = new CmdHelper();
        CmdHelper RoomClientCmdHelper { get; } = new CmdHelper();
        CmdHelper ServerCmdHelper { get; } = new CmdHelper();
        CmdHelper RoomServerCmdHelper { get; } = new CmdHelper();

        private CMDSDispatcher mCMDSDispatcher;

        /// <summary>
        /// 对中心服务器来说, 没必要把所有的客户端要用的消息处理函数都注册了
        /// </summary>
        public void Init()
        {
            mCMDSDispatcher = new CMDSDispatcher();

            ClientCmdHelper.Init(CMDType.Client);
            RoomClientCmdHelper.Init(CMDType.RoomClient);
            ServerCmdHelper.Init(CMDType.Server);
            RoomServerCmdHelper.Init(CMDType.RoomServer);
        }
        public void FireClient(IChannelHandlerContext ctx, CommonMessage message)
        {
            ClientCmdHelper.Fire(mCMDSDispatcher, message, SocketInfo.Instance().mCenterServer);
        }
        public void FireRoomClient(IChannelHandlerContext ctx, CommonMessage message)
        {
            CommonClient roomServer;
            SocketInfo.Instance().mRoomServer.TryGetValue(ctx, out roomServer);
            RoomClientCmdHelper.Fire(mCMDSDispatcher, message, roomServer);
        }

        /// <summary>
        /// 中心服务器用这个
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="message"></param>
        public void FireServer(IChannelHandlerContext ctx, CommonMessage message)
        {
            CommonClient client = ClientManager.Instance().FindClient(ctx);
            ServerCmdHelper.Fire(mCMDSDispatcher, message, client);
        }
        public void FireRoomServer(IChannelHandlerContext ctx, CommonMessage message)
        {
            CommonClient client = RoomClientManager.Instance().FindClient(ctx);
            RoomServerCmdHelper.Fire(mCMDSDispatcher, message, client);
        }
        public void AddMessageParser(string messageTypeName, MessageParser messageParser)
        {
            CmdHelper.AddMessageParser(messageTypeName, messageParser);
        }
        public MessageParser GetMessageParser(IMessage type)
        {
            return CmdHelper.GetMessageParser(type);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class CmdHelper
    {
        /// <summary>
        /// 对应消息的解析
        /// Person -> ParseFrom 方法的映射 (根据Person 找到 反序列化根据Person的方法)
        /// </summary>
        static private Dictionary<string, MessageParser> mMessageTypes = new Dictionary<string, MessageParser>();

        // 与 cmd 对应的 handler
        private Dictionary<CMDS, Action<CommonClient, CommonMessage>> mActions;

        public void Init(CMDType cMDType)
        {
            mActions = new Dictionary<CMDS, Action<CommonClient, CommonMessage>>();

            InitMessageHandler(cMDType);
        }

        private void InitMessageHandler(CMDType cMDType)
        {
            string exePath = Directory.GetCurrentDirectory();

            string path = $"{exePath}\\Handler.dll";
            Assembly assembly = Assembly.LoadFile(path);
            var types = assembly.GetTypes();

            foreach (Type type in types)
            {
                var arr = type.GetCustomAttribute<CMDTypeAttribute>();

                if (null == arr || arr.CMDType != cMDType)
                {
                    continue;
                }

                MethodInfo[] methodInfos = type.GetMethods();

                foreach (MethodInfo method in methodInfos)
                {
                    var arrr = method.GetCustomAttribute<CmdHandlerAttribute>();

                    if (null == arrr)
                    {
                        continue;
                    }

                    var handler = Delegate.CreateDelegate(typeof(Action<CommonClient, CommonMessage>), method) as Action<CommonClient, CommonMessage>;
                    if (!mActions.TryAdd(arrr.CmdID, handler))
                    {
                        Console.WriteLine($"注册{cMDType}消息处理失败 CmdID:{arrr.CmdID}, handler:{method.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"注册{cMDType}消息处理成功 CmdID:{arrr.CmdID}, handler:{method.Name}");
                    }
                }
            }

            Console.WriteLine($"共注册{cMDType}消息处理:{mActions.Count}个");
        }

        /// <summary>
        /// 投递消息
        /// </summary>
        public void Fire(CMDSDispatcher mCMDSDispatcher, CommonMessage message, CommonClient toWho)
        {
            Action<CommonClient, CommonMessage> action = null;

            if (mActions.TryGetValue(message.mCMD, out action))
            {
                //Console.WriteLine($"cmd:{message.mCMD} find");
                mCMDSDispatcher.Dispatch(action, toWho, message);
            }
            else
            {
                Console.WriteLine($"cmd:{message.mCMD} not find");
            }
        }

        public static MessageParser GetMessageParser(IMessage type)
        {
            MessageParser parser = null;
            Type realType = type.GetType();
            if (!mMessageTypes.TryGetValue(realType.ToString(), out parser))
            {
                //Console.WriteLine($"faild 未找到:{type} 对应的 Parser");
            }
            return parser;
        }

        /// <summary>
        /// 建立 messagetype-parser 的缓存
        /// </summary>
        /// <param name="messageTypeName"></param>
        /// <param name="messageParser"></param>
        public static void AddMessageParser(string messageTypeName, MessageParser messageParser)
        {
            // Person -> ParseFrom 方法的映射 (根据Person 找到 反序列化根据Person的方法)
            if (mMessageTypes.TryAdd(messageTypeName, messageParser))
            {
                //Console.WriteLine($"AddMessageParser type:{messageTypeName} success");
            }
            else
            {
                Console.WriteLine($"AddMessageParser type:{messageTypeName} faild");
            }
        }
    }
}
