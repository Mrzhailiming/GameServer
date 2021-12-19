using System;
using System.Collections.Generic;
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
                try
                {
                    var node = mTickInfos.First;

                    while (null != node)
                    {
                        TickInfo tick = node.Value;
                        bool ret = true;
                        try
                        {
                            ret = tick.DoTick(0);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"TickManager Execute DoTick 出现异常\r\n" +
                                $"{ex}");
                            ret = false;
                        }
                        finally
                        {
                            if (!ret)
                            {
                                Console.WriteLine($"移除tick owner:{tick.mOwner}");
                                mTickInfos.Remove(tick);
                            }
                        }

                        node = node.Next;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TickManager Execute 出现异常\r\n" +
                        $"{ex}");
                }
            }
        }
    }
}
