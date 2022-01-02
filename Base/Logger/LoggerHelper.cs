using Base.Interface;
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
        Info,
        Exception,
        HeartBeat,
    }

    public class LoggerHelper : Singletion<LoggerHelper>, StartInitInterface
    {
        ConcurrentQueue<LogMessage> mMessageQueue = new ConcurrentQueue<LogMessage>();

        Dictionary<LogType, FileStream> mStreams = new Dictionary<LogType, FileStream>();

        // 用这种方式同步 (有任务的时候, 唤醒处理线程即可)
        static ManualResetEvent ResetEvent = new ManualResetEvent(false);
        public InitType InitType => InitType.Both;

        object StartInitInterface.Instance => Instance();

        public void Init()
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

                FileStream stream = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                mStreams.Add(type, stream);
            }
        }
        public void Log(LogType logType, string msg)
        {
            mMessageQueue.Enqueue(new LogMessage(logType, msg));
            ResetEvent.Set(); // 唤醒 (发信号)
        }

        private void Execute()
        {
            while (true)
            {
                try
                {
                    LogMessage message;
                    while (mMessageQueue.TryDequeue(out message))
                    {
                        OutPut(message);
                    }

                    foreach (var stream in mStreams.Values)
                    {
                        stream.Flush();
                    }
                }
                catch
                {
                    ResetEvent.Reset(); // 重置 信号
                    ResetEvent.WaitOne(10); // 阻塞当前线程, 直到收到信号
                }
                
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
