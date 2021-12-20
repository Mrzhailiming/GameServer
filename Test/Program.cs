using Base.Logger;
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

            LoggerHelper.Instance().Log(LogType.Console, ips);
            LoggerHelper.Instance().Log(LogType.Console, "22222222222");

            Console.WriteLine("Hello World!");
        }
    }
}
