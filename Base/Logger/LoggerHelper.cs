using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Logger
{

    public enum LogType
    {
        Console,
        Exception,
    }

    public class LoggerHelper : Singletion<LoggerHelper>
    {

        public void Log(LogType logType, string msg)
        {

        }
    }
}
