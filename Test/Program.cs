﻿using Base;
using Base.Interface;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string ips = "::ffff:127.0.0.1&100|::ffff:127.0.0.1&200";

            string[] ret = ips.Split('|');

            int beginIndex = ret[0].LastIndexOf(':');

            string ipPor = ret[0].Substring(beginIndex + 1, ret[0].Length - 7);
            string[] ipt = ipPor.Split('&');

            Console.WriteLine("Hello World!");

            StartInitManager.Instance().ToString();
        }
    }


    public class testInitInterface1 : Singletion<testInitInterface1>, StartInitInterface
    {
        object StartInitInterface.Instance { get => testInitInterface1.Instance(); }

        public void Init(/*params string[] param*/)
        {
            Console.WriteLine("testInitInterface1 success");
        }
    }

    public class testInitInterface2 : Singletion<testInitInterface2>, StartInitInterface
    {
        object StartInitInterface.Instance { get => testInitInterface2.Instance(); }

        public void Init(/*params string[] param*/)
        {
            Console.WriteLine("testInitInterface2 success");
        }
    }

    public class testInitInterface3 : Singletion<testInitInterface3>, StartInitInterface
    {
        object StartInitInterface.Instance { get => testInitInterface3.Instance(); }

        public void Init(/*params string[] param*/)
        {
            Console.WriteLine("testInitInterface3 success");
        }
    }
}
