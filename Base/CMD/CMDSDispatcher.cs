using Base.BaseData;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Base
{

    public class MyTimer
    {
        private Stopwatch timer = null;
        private object mutex = new object();

        private int mMnterval;

        public MyTimer(int interval)
        {
            mMnterval = interval;
        }

        public bool Tick
        {
            get
            {
                if (null == timer)
                {
                    lock (mutex)
                    {
                        if (null == timer)
                        {
                            timer = new Stopwatch();
                            timer.Start();
                        }
                    }
                }
                else if(timer.ElapsedMilliseconds - cur > mMnterval)
                {
                    Console.WriteLine($"DoFunc 间隔{timer.ElapsedMilliseconds - cur}");
                    cur = timer.ElapsedMilliseconds;
                    return true;
                }

                return false;
            }
            set
            {

            }
        }
        public double cur = 0;
    }

    internal class Task
    {
        private Action<CommonClient, CommonMessage> mAction;
        private CommonClient mClient;
        private CommonMessage mMessage;

        private MyTimer mMyTimer;
        public Task(Action<CommonClient, CommonMessage> action, CommonClient client, CommonMessage message)
        {
            mAction = action;
            mClient = client;
            mMessage = message;

            mMyTimer = new MyTimer(1000);
        }

        public void DoAction()
        {
            try
            {
                mAction.Invoke(mClient, mMessage);
                //Console.WriteLine($"Task.DoAction() Success:\r\n" +
                //    $"client:{mClient} action:{mAction} message:{mMessage}\r\n");

                //if (mMyTimer.Tick)
                //{
                //    Console.WriteLine($"process task num * s {CMDSDispatcher.num}");
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task.DoAction() Exception:\r\n" +
                    $"client:{mClient.Name} action:{mAction} message:{mMessage}\r\n" +
                    $"{ex}");
            }
        }

        private Task() { }
    }
    /// <summary>
    /// 处理协议, 在这做异步吧
    /// </summary>
    public class CMDSDispatcher
    {
        static ConcurrentQueue<Task> mTaskQueue = new ConcurrentQueue<Task>();

        static Semaphore sema = new Semaphore(0, int.MaxValue);

        public static int num = 0;

        public CMDSDispatcher()
        {
            //设置正在等待线程的事件为终止
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            int workerThreads;

            int portThreads;

            //获取处于活动状态的线程池请求的数目
            ThreadPool.GetMaxThreads(out workerThreads, out portThreads);

            //在控制台中显示处于活动状态的线程池请求的数目
            Console.WriteLine("设置前，线程池中辅助线程的最大数为：" + workerThreads.ToString() + "；线程池中异步I/O线程的最大数为：" + portThreads.ToString());

            workerThreads = 10;//设置辅助线程的最大数

            portThreads = 500;//设置线程池中异步I/O线程的最大数

            //设置处于活动状态的线程池请求的数目

            ThreadPool.SetMaxThreads(workerThreads, portThreads);



            //在控制台中显示设置后的处于活动状态的线程池请求的数目

            Console.WriteLine("设置后，线程池中辅助线程的最大数为：" + workerThreads.ToString() + "；线程池中异步I/O线程的最大数为：" + portThreads.ToString());



            Thread thread = new Thread(Execute);
            thread.Name = "CMDSDispatcher";
            thread.IsBackground = true;
            thread.Start();
        }

        public void Dispatch(Action<CommonClient, CommonMessage> action, CommonClient client, CommonMessage message)
        {
            mTaskQueue.Enqueue(new Task(action, client, message));
            sema.Release();
        }


        static void TimerCall(object o)
        {

        }

        private static void Execute()
        {
            
            while (true)
            {
                try
                {
                    sema.WaitOne();
                    Task task = null;
                    while (mTaskQueue.TryDequeue(out task))
                    {
                        Interlocked.Increment(ref num);
                        ThreadPool.QueueUserWorkItem(Do, task);//执行线程池
                        //task.DoAction();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CMDSDispatcher.Execute() Exception:\r\n" +
                                        $"{ex}");
                }
                finally
                {
                    
                }
                
            }
        }

        static void Do(object task)
        {
            Task task1 = (Task)task;

            task1.DoAction();
        }
    }
}
