using Base.Attributes;
using Base.BaseData;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Base
{

    /// <summary>
    /// 
    /// </summary>
    public class CmdHelper
    {
        /// <summary>
        /// 对应消息的解析
        /// Person -> ParseFrom 方法的映射 (根据Person 找到 反序列化根据Person的方法)
        /// </summary>
        static private Dictionary<string, MessageParser> mMessageTypes;

        // 与 cmd 对应的 handler
        static private Dictionary<CMDS, Action<CommonClient, CommonMessage>> mActions;

        static private CMDSDispatcher mCMDSDispatcher;
        static public void Init()
        {
            mMessageTypes = new Dictionary<string, MessageParser>();
            mActions = new Dictionary<CMDS, Action<CommonClient, CommonMessage>>();
            mCMDSDispatcher = new CMDSDispatcher();

            //InitMessageTypesParser();

            InitMessageHandler();

            
        }

        private static void InitMessageHandler()
        {
            string exePath = Directory.GetCurrentDirectory();

            string path = $"{exePath}\\Handler.dll";
            Assembly assembly = Assembly.LoadFile(path);
            var types = assembly.GetTypes();

            foreach (Type type in types)
            {
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
                        Console.WriteLine($"注册消息处理失败 CmdID:{arrr.CmdID}, handler{method.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"注册消息处理成功 CmdID:{arrr.CmdID}, handler{method.Name}");
                    }
                }
            }

            Console.WriteLine($"共注册消息处理:{mActions.Count}个");
        }

        /// <summary>
        /// 查找所有继承 IMessage 的类, 生成对应的 Parser
        /// </summary>
        //private static void InitMessageTypesParser()
        //{
        //    string exePath = Directory.GetCurrentDirectory();
        //    string path = $"exePath\\..\\..\\..\\..\\Gen\\bin\\Debug\\netcoreapp3.1\\Gen.dll";
        //    path = $"D:\\Code\\CommonServer\\CommonServer\\Gen\\bin\\Debug\\netcoreapp3.1\\Gen.dll";
        //    Assembly assembly = Assembly.LoadFile(path);
        //    var types = assembly.GetTypes();

        //    foreach (Type type in types)
        //    {

        //        if (null != type.GetInterface($"IMessage"))
        //        {
        //            AddMessageParser(type);
        //        }
        //    }

        //    foreach (var type in mMessageTypes)
        //    {
        //        Console.WriteLine($"找到IMessage:{type}");
        //    }
        //    Console.WriteLine($"共找到IMessage:{mMessageTypes.Count}个");
        //}

        /// <summary>
        /// 投递消息
        /// </summary>
        public static void Fire(IChannelHandlerContext ctx, CommonMessage message)
        {
            Action<CommonClient, CommonMessage> action = null;

            var client = ClientManager.Instance().FindClient(ctx);

            if(null == client)
            {
                //return;
            }

            if (mActions.TryGetValue(message.mCMD, out action))
            {
                //Console.WriteLine($"cmd:{message.mCMD} find");
                mCMDSDispatcher.Dispatch(action, client, message);
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
                Console.WriteLine($"faild 未找到:{type} 对应的 Parser");
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
                Console.WriteLine($"AddMessageParser type:{messageTypeName} success");
            }
            else
            {
                Console.WriteLine($"AddMessageParser type:{messageTypeName} faild");
            }
        }

        /// <summary>
        /// 目的: 找到自定义 Message 对应的 Parser 从而进行反序列化
        /// 先建立缓存 map 饿汉模式
        /// </summary>
        /// <param name="type"></param>
        //private static void AddMessageParser(Type type)
        //{
        //    // 获取 Person 的成员 parser 的 ParseFrom 方法
        //    BindingFlags flag = BindingFlags.Static | BindingFlags.NonPublic;
        //    FieldInfo f_key = type.GetField("_parser", flag);

        //    byte[] tmp = new byte[1];
        //    Type paramType = tmp.GetType();
        //    Type[] methodParamType = new Type[] { paramType };
        //    MethodInfo[] methodInfos = f_key.FieldType.GetMethods();
        //    MethodInfo methodInfo = f_key.FieldType.GetMethod("ParseFrom", methodParamType);


        //    MessageParser messageParser = (MessageParser)type.GetField("_parser", flag).GetValue(null);


        //    object obj = type.Assembly.CreateInstance(type.FullName);
        //    var message = (IMessage)obj;

        //    // Person -> ParseFrom 方法的映射 (根据Person 找到 反序列化根据Person的方法)
        //    if (mMessageTypes.TryAdd(type.FullName, messageParser))
        //    {
        //        Console.WriteLine($"success type:{type} 生成对应的 Parser:{f_key}");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"faild type:{type} 生成对应的 Parser:{f_key}");
        //    }
        //}
    }
}
