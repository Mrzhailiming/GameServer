using Base.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Base.Interface
{
    /// <summary>
    /// 通过反射获取程序集所有继承 StartInitInterface 的类
    /// 调用 Init 函数
    /// </summary>
    public class StartInitManager : Singletion<StartInitManager>
    {

        //Dictionary<PropertyInfo, MethodInfo> mInstance2Method = new Dictionary<PropertyInfo, MethodInfo>();

        List<StartInitInterface> mStartInitInterface = new List<StartInitInterface>();

        /// <summary>
        /// 客户端和服务器所要调用的init函数的类可能不一样, 也有重合的类
        /// </summary>
        InitType mInitType;

        /// <summary>
        /// 服务器客户端都在用, 要不要区分一下...
        /// </summary>
        public void StartInit(InitType initType, string dllsPath)
        {
            mInitType = initType;

            Init(dllsPath);

            RunAllInit();
        }


        private void RunAllInit()
        {
            foreach(var instance in mStartInitInterface)
            {
                instance.Init();
                LoggerHelper.Instance().Log(LogType.Console, $"init {instance}");
            }
        }

        private void Init(string dllsPath)
        {
            List<string> dlls = GetDlls(dllsPath);

            if (null == dlls || dlls.Count <= 0)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"not find any dll");
                return;
            }

            foreach (string dllName in dlls)
            {
                ProcessDll(dllName);
            }
        }


        private List<string> GetDlls(string dllsPath)
        {
            string[] files = Directory.GetFiles(dllsPath);

            List<string> dlls = new List<string>();
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLower();

                if (".dll" == ext)
                {
                    dlls.Add(file);
                    LoggerHelper.Instance().Log(LogType.Info, $"add dll {file}");
                }
            }

            LoggerHelper.Instance().Log(LogType.Info, $"add dll sucess path:{dllsPath}");
            return dlls;
        }


        private void ProcessDll(string dllName)
        {
            if (!File.Exists(dllName))
            {
                LoggerHelper.Instance().Log(LogType.Console, $"not find dll:{dllName}");
                return;
            }

            var assembly = Assembly.LoadFrom(dllName);
            if (null == assembly)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"Assembly.LoadFrom dll is null:{dllName}");
                return;
            }

            var types = assembly.GetTypes();
            if (null == types)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"assembly.GetTypes is null:{dllName}");
                return;
            }

            // 查找继承 StartInitInterface 的类
            List<Type> result = new List<Type>();
            foreach (Type type in types)
            {

                string typeName = type.Name;

                var Interfaces = ((TypeInfo)type).ImplementedInterfaces;

                foreach (var intface in Interfaces)
                {
                    if (intface == typeof(StartInitInterface))
                    {
                        result.Add(type);
                        break;
                    }
                }
            }


            // 找类的实例 以及 Init函数
            foreach (var type in result)
            {
                ProcessType(type);
            }
        }



        /// <summary>
        /// 拿到类的实例
        /// 找不到就不能初始化, 并输出信息
        /// </summary>
        /// <param name="type"></param>

        private void ProcessType(Type type)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            PropertyInfo[] Propertys = type.GetProperties(flags);
            if (null == Propertys || Propertys.Length < 1)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"ProcessType {type} has no property");
            }

            StartInitInterface Instance = null;
            InitType initType = InitType.Both;
            foreach (PropertyInfo property in Propertys)
            {
                // 找到
                if (property.Name.Contains("StartInitInterface.Instance"))
                {
                    // 获取属性值
                    var ret = property.GetValue(Activator.CreateInstance(type), null);
                    Instance = (StartInitInterface)ret;
                    continue;
                }

                if (property.Name.Contains("InitType"))
                {
                    var ret = property.GetValue(Activator.CreateInstance(type), null);
                    initType = (InitType)ret;
                    continue;
                }
            }

            // 没找到静态实例 返回
            if (null == Instance)
            {
                LoggerHelper.Instance().Log(LogType.Console, $"ProcessType {type} has no static Instance");
                return;
            }

            // 既不是 Both 也不是 mInitType
            if (InitType.Both != initType
                && initType != mInitType)
            {
                return;
            }

            mStartInitInterface.Add(Instance);

        }
    }
}
