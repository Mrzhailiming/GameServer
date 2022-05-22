using Global;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Singleton.Manager
{

    public class CMDHandler
    {
        public Action<TCPPacket> _action = null;

    }
    public class CMDDispatcher : Singleton<CMDDispatcher>
    {
        static int threadNum = 1;
        static int semaphoreMaximumCount = 10000;//100个不够吧
        Semaphore semaphore = new Semaphore(0, semaphoreMaximumCount);
        static int count = 0;
        static Dictionary<string, int> threadCount = new Dictionary<string, int>();

        Dictionary<TCPCMDS, CMDHandler> _CMD2Action = new Dictionary<TCPCMDS, CMDHandler>();
        ConcurrentQueue<TCPPacket> _taskQueue = new ConcurrentQueue<TCPPacket>();
        public CMDDispatcher()
        {
            for (int i = 0; i < threadNum; ++i)
            {
                Thread thread = new Thread(Execute);
                thread.Name = $"taskThread_{i}";
                threadCount[thread.Name] = 0;
                thread.Start();
            }
        }

        private void Execute()
        {
            TCPPacket task = null;
            CMDHandler outHandler;
            TCPCMDS cmdID;
            //使用生产者消费者模型?
            while (true)
            {
                try
                {
                    semaphore.WaitOne();
                    //
                    if (_taskQueue.TryDequeue(out task))
                    {
                        cmdID = (TCPCMDS)Global.Global.Byte2Int(task.mBuff, 0);

                        if (!_CMD2Action.TryGetValue(cmdID, out outHandler))
                        {
                            LoggerHelper.Instance().Log(LogType.Console, $"CMDDispatcher cmdID:{cmdID} handler null");
                        }
                        else
                        {
                            LoggerHelper.Instance().Log(LogType.Console, $"CMDDispatcher Dequeue cmdID:{cmdID}");

                            outHandler._action(task);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Instance().Log(LogType.Exception, ex.ToString());
                }
                finally
                {
                    task = null;
                    outHandler = null;
                    Interlocked.Increment(ref count);
                    LoggerHelper.Instance().Log(LogType.Console, $"{Thread.CurrentThread.Name}  CurThreadProcessedTaskCount:{++threadCount[Thread.CurrentThread.Name]} packetIndex:{count}");
                }
            }
        }
        public void RegisterCMD(TCPCMDS cmdID, Action<TCPPacket> action)
        {
            CMDHandler newHandler = new CMDHandler()
            {
                _action = action
            };
            CMDHandler outHandler;
            if (_CMD2Action.TryGetValue(cmdID, out outHandler))
            {
                LoggerHelper.Instance().Log(LogType.Console, cmdID.ToString());
                throw new Exception("cmd重复");
            }
            else
            {
                _CMD2Action[cmdID] = newHandler;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        public void Dispatcher(TCPPacket task)
        {
            LoggerHelper.Instance().Log(LogType.Console, $"Dispatcher task cmd:{task.mCmd}");
            _taskQueue.Enqueue(task);

            semaphore.Release();
        }
    }
}