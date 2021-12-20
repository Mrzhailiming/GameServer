using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Base.Interface
{
    public class StartInitManager : Singletion<StartInitManager>
    {
        public StartInitManager()
        {
            Init();
        }


        private void Init()
        {
            List<string> dlls = GetDlls();

            if(null == dlls || dlls.Count <= 0)
            {
                Console.WriteLine($"not find any dll");
                return;
            }

            foreach(string dllName in dlls)
            {
                ProcessDll(dllName);
            }
        }


        private List<string> GetDlls()
        {
            string exePath = Directory.GetCurrentDirectory();

            string[] files = Directory.GetFiles(exePath);


            List<string> dlls = new List<string>();
            foreach(string file in files)
            {
                string ext = Path.GetExtension(file).ToLower();

                if(".dll" == ext)
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
            if(null == assembly)
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

                foreach(var intface in Interfaces)
                {
                    if (intface == typeof(StartInitInterface))
                    {
                        result.Add(type);
                        break;
                    }
                }
            }

            // 拿到类的实例 以及 Init函数
        }
    }
}
