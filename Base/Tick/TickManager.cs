using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Base.Tick
{
    public class TickManager : Singletion<TickManager>
    {
        private ConcurrentQueue<TickInfos> mTickInfos = new ConcurrentQueue<TickInfos>();

        private Semaphore sema = new Semaphore(0, int.MaxValue);

        public void Init()
        {
            Thread thread = new Thread(Execute);
            thread.IsBackground = true;
            thread.Name = "threadtick";

            thread.Start();
        }

        public void AddTickInfos(TickInfos tickInfos)
        {
            mTickInfos.Enqueue(tickInfos);
            sema.Release();
        }

        public void Execute()
        {
            while (true)
            {
                sema.WaitOne();
                // 会不会迭代器失效?
                foreach (var tickInfos in mTickInfos)
                {
                    try
                    {
                        tickInfos.DoTicks(0);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"TickManager.Execute tickInfos.DoTicks()\r\n" +
                            $"{ex}");
                    }
                }

                sema.Release();
            }
        }
    }
}
