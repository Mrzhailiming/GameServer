using System;
using System.Collections.Generic;
using System.Text;

namespace Base.DataHelper
{
    public class IPPort
    {
        public string IP;
        public string Port;
    }

    public class IPParserHelper
    {
        public static List<IPPort> StringIPPort(string str)
        {
            List<IPPort> ret = new List<IPPort>();

            //string ips = "::ffff:127.0.0.1&100|::ffff:127.0.0.1&200";

            string[] items = str.Split('|');

            foreach(string item in items)
            {
                //int beginIndex = item.LastIndexOf(':');

                //string ipPor = item.Substring(beginIndex + 1, item.Length - 7);

                string[] ipt = item.Split('&');
                IPPort tmp = new IPPort()
                {
                    IP = ipt[0],
                    Port = ipt[1]
                };
                ret.Add(tmp);
            }

            return ret;
        }
    }
}
