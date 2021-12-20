using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Base.Logger
{

    public enum LogType
    {
        Console,
        Exception,
    }

    public class LoggerHelper : Singletion<LoggerHelper>
    {
        ConcurrentQueue<LogMessage> mMessageQueue = new ConcurrentQueue<LogMessage>();

        Dictionary<LogType, FileStream> mStreams = new Dictionary<LogType, FileStream>();
        public LoggerHelper()
        {
            InitStream();

            Thread thread = new Thread(Execute);
            thread.Name = "LoggerHelper";

            thread.Start();
        }

        private void InitStream()
        {
            string exePath = $"{Directory.GetCurrentDirectory()}\\Log";

            if (!Directory.Exists(exePath))
            {
                Directory.CreateDirectory(exePath);
            }


            foreach (LogType type in Enum.GetValues(typeof(LogType)))
            {
                string fileFullPath = $"{exePath}\\{type}.txt";

                //if (!File.Exists(fileFullPath))
                //{
                //    File.Create(fileFullPath);
                //}

                FileStream stream = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                mStreams.Add(type, stream);
            }
        }
        public void Log(LogType logType, string msg)
        {
            mMessageQueue.Enqueue(new LogMessage(logType, msg));
        }

        private void Execute()
        {
            while (true)
            {
                LogMessage message;
                while (mMessageQueue.TryDequeue(out message))
                {
                    OutPut(message);
                }

                foreach(var stream in mStreams.Values)
                {
                    stream.Flush();
                }

                Thread.Sleep(5 * 1000);
            }
        }

        private void OutPut(LogMessage message)
        {
            FileStream stream;
            mStreams.TryGetValue(message.LogType, out stream);

            if(null == stream)
            {
                return;
            }

            byte[] buff = Encoding.Default.GetBytes($"{message.Message}\r\n");

            stream.WriteAsync(buff);
        }
    }

    internal class LogMessage
    {
        public LogType LogType { get; }

        public string Message { get; }

        public LogMessage(LogType logType, string msg)
        {
            LogType = logType;
            Message = msg;
        }
    }
}
