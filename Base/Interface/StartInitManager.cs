using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Base.Interface
{
    public class StartInitManager : Singletion<StartInitManager>
    {

        Dictionary<PropertyInfo, MethodInfo> mInstance2Method = new Dictionary<PropertyInfo, MethodInfo>();

        List<StartInitInterface> mStartInitInterface = new List<StartInitInterface>();

        public StartInitManager()
        {
            Init();

            RunAllInit();
        }

        private void RunAllInit()
        {
            foreach(var instance in mStartInitInterface)
            {
                instance.Init();
            }
        }

        private void Init()
        {
            List<string> dlls = GetDlls();

            if (null == dlls || dlls.Count <= 0)
            {
                Console.WriteLine($"not find any dll");
                return;
            }

            foreach (string dllName in dlls)
            {
                ProcessDll(dllName);
            }
        }


        private List<string> GetDlls()
        {
            string exePath = Directory.GetCurrentDirectory();

            string[] files = Directory.GetFiles(exePath);


            List<string> dlls = new List<string>();
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLower();

                if (".dll" == ext)
                {
                    dlls.Add(file);
                }
            }

            return dlls;
        }


        private void ProcessDll(string dllName)
        {
            if (!File.Exists(dllName))
            {
                Console.WriteLine($"not find dll:{dllName}");
                return;
            }

            var assembly = Assembly.LoadFrom(dllName);
            if (null == assembly)
            {
                Console.WriteLine($"Assembly.LoadFrom dll is null:{dllName}");
                return;
            }

            var types = assembly.GetTypes();
            if (null == types)
            {
                Console.WriteLine($"assembly.GetTypes is null:{dllName}");
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
                Console.WriteLine($"ProcessType {type} has no property");
            }

            StartInitInterface Instance = null;
            foreach (PropertyInfo property in Propertys)
            {
                // 找到
                if (property.Name.Contains("StartInitInterface.Instance"))
                {
                    // 获取属性值
                    var ret = property.GetValue(Activator.CreateInstance(type), null);
                    Instance = (StartInitInterface)ret;
                    break;
                }
            }

            // 没找到静态实例 返回
            if (null == Instance)
            {
                Console.WriteLine($"ProcessType {type} has no static Instance");
                return;
            }

            mStartInitInterface.Add(Instance);

        }
    }
}
