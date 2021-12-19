using System;
using System.Collections.Generic;
using System.Text;

namespace Base.DataHelper
{
    public class IPPort
    {
        public string IP;
        public string Port;
        public string Camp;
    }

    public class IPParserHelper
    {
        /// <summary>
        /// 解析
        /// "127.0.0.1&8888&camp|127.0.0.1&8888&camp"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<IPPort> StringIPPortCamp(string str)
        {
            List<IPPort> ret = new List<IPPort>();

            string[] items = str.Split('|');

            foreach(string item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                string[] ipt = item.Split('&');
                IPPort tmp = new IPPort()
                {
                    IP = ipt[0],
                    Port = ipt[1],
                    Camp = ipt[2]
                };
                ret.Add(tmp);
            }

            return ret;
        }

        /// <summary>
        /// 解析
        /// ::ffff:127.0.0.1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringIP(string str)
        {
            string ret = str.Substring(str.LastIndexOf(':') + 1, str.Length - str.LastIndexOf(':') - 1);

            return ret;
        }
    }
}
