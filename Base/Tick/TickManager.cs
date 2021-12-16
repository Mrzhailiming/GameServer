using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Base.Tick
{
    /// <summary>
    /// 可不可以 只tick CommonClient 的tick函数, 然后让 CommonClient tick自己的 tickinfo;
    /// 所以, TickManager 的 TickInfos 里只有一个需要tick 的, 就是  CommonClient.Update();
    /// 那 CommonClient.Update 的时间间隔设置多久呢? 设置为 0 还是 几毫秒
    /// 不设置吧, Update 里有需要tick的 tickinfo, 设置时间间隔的话, 肯定不能按时tick到
    /// </summary>
    public class TickManager : Singletion<TickManager>
    {
        /// <summary>
        /// 维护所有要tick的 TickInfo
        /// </summary>
        //private ConcurrentQueue<TickInfo> mTickInfos = new ConcurrentQueue<TickInfo>();

        /// <summary>
        /// 维护的 TickInfo 列表
        /// </summary>
        private LinkedList<TickInfo> mTickInfos = new LinkedList<TickInfo>();

        /// <summary>
        /// 异步 挺好使
        /// 异步调用的函数是个死循环
        /// </summary>
        public async void RunAsync()
        {
            await Task.Run(Execute);
        }

        /// <summary>
        /// 向 TickManager 增加要tick的 TickInfos
        /// </summary>
        /// <param name="tickInfos"></param>
        public void AddTickInfo(TickInfo tickInfo)
        {
            mTickInfos.AddLast(tickInfo);
        }

        public void Execute()
        {
            while (true)
            {

                var node = mTickInfos.First;

                while (null != node)
                {
                    TickInfo tick = node.Value;

                    if (!tick.DoTick(0))
                    {
                        mTickInfos.Remove(tick);

                        // 如果 mTickList.count = 0, 不应该让 TickManager 移除此TickInfos
                        //IsEffective = false;
                    }

                    node = node.Next;
                }
                // 会不会迭代器失效?
                //foreach (var tickInfo in mTickInfos)
                //{
                //    try
                //    {
                //        // 我只负责调你的 func 有没有到时间间隔, 你自己把控
                //        // 我不想把控, 也把控不了啊
                //        // 不可能把你们所有的时间间隔都记下来吧, 不可能
                //        tickInfo.DoTick(0);

                //    }
                //    catch (Exception ex)
                //    {

                //        Console.WriteLine($"TickManager.Execute tickInfos.DoTicks()\r\n" +
                //            $"{ex}");
                //    }
                //}

            }
        }
    }
}
