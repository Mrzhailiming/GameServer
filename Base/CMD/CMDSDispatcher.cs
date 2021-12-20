using Base.BaseData;
using Base.Tick;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Base
{
    internal class CMDTask
    {
        private Action<CommonClient, CommonMessage> mAction;
        private CommonClient mClient;
        private CommonMessage mMessage;

        public CMDTask(Action<CommonClient, CommonMessage> action, CommonClient client, CommonMessage message)
        {
            mAction = action;
            mClient = client;
            mMessage = message;
        }

        public void DoAction()
        {
            try
            {
                mAction.Invoke(mClient, mMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task.DoAction() Exception:\r\n" +
                    $"client:{mClient.Name} action:{mAction} message:{mMessage}\r\n" +
                    $"{ex}");
            }
        }

        private CMDTask() { }
    }

    /// <summary>
    /// 处理协议
    /// </summary>
    public class CMDSDispatcher
    {
        static ConcurrentQueue<CMDTask> mTaskQueue = new ConcurrentQueue<CMDTask>();

        // 这种方式同步, 不太好吧 好像也没啥问题 这个是有计数的问题的 每次release都会+1, 本逻辑不太需要计数
        //static Semaphore sema = new Semaphore(0, int.MaxValue); 

        // 用这种方式同步 (有任务的时候, 唤醒处理线程即可)
        static ManualResetEvent ResetEvent = new ManualResetEvent(false);

        public CMDSDispatcher()
        {
            SetThreadPool(10, 10);

            Thread thread = new Thread(Execute);
            thread.Name = "CMDSDispatcher";
            thread.IsBackground = true;
            thread.Start();
        }

        public void Dispatch(Action<CommonClient, CommonMessage> action, CommonClient client, CommonMessage message)
        {
            mTaskQueue.Enqueue(new CMDTask(action, client, message));
            //sema.Release();
            ResetEvent.Set(); // 唤醒 (发信号)
        }


        void SetThreadPool(int workerThreads,int portThreads)
        {
            //设置正在等待线程的事件为终止
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            //获取处于活动状态的线程池请求的数目
            //ThreadPool.GetMaxThreads(out workerThreads, out portThreads);
            //在控制台中显示处于活动状态的线程池请求的数目
            //Console.WriteLine("设置前，线程池中辅助线程的最大数为：" + workerThreads.ToString() + "；线程池中异步I/O线程的最大数为：" + portThreads.ToString());

            workerThreads = 10;//设置辅助线程的最大数

            portThreads = 100;//设置线程池中异步I/O线程的最大数

            //设置处于活动状态的线程池请求的数目

            ThreadPool.SetMaxThreads(workerThreads, portThreads);

            //在控制台中显示设置后的处于活动状态的线程池请求的数目

            Console.WriteLine("设置后，线程池中辅助线程的最大数为：" + workerThreads.ToString() + "；线程池中异步I/O线程的最大数为：" + portThreads.ToString());

        }

        private static void Execute()
        {
            string name = Thread.CurrentThread.Name;
            while (true)
            {
                try
                {
                    CMDTask task = null;

                    //sema.WaitOne();
                    do
                    {
                        if (mTaskQueue.TryDequeue(out task))
                        {
                            ThreadPool.QueueUserWorkItem(Do, task);// 投递
                        }
                    } while (mTaskQueue.TryDequeue(out task));


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CMDSDispatcher.Execute() Exception:\r\n" +
                                        $"{ex}");
                }
                finally
                {
                    ResetEvent.Reset(); // 重置 信号
                    ResetEvent.WaitOne(10); // 阻塞当前线程, 直到收到信号
                }
                
            }
        }

        static void Do(object task)
        {
            CMDTask task1 = (CMDTask)task;

            task1.DoAction();
        }
    }
}
