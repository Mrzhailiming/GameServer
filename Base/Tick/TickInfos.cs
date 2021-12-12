using System;
using System.Collections.Generic;
using System.Threading;

namespace Base.Tick
{
    public class TickInfo : ITick
    {
        /// <summary>
        /// TickInfo 的拥有者
        /// </summary>
        private TickInfos mOwner;

        /// <summary>
        /// tick时执行的函数
        /// </summary>
        private Func<long, bool> mFunc { get; set; } = null;

        /// <summary>
        /// 计时器
        /// </summary>
        TickTimer mTimer = null;

        /// <summary>
        /// tick的时间间隔
        /// </summary>
        private int mTickInterval { get; set; } = 0;
        /// <summary>
        /// 如果为FALSE ,上层应该从维护队列里删除此 Tick
        /// </summary>
        public bool IsEffective { get => mIsEffective; }

        private bool mIsEffective = true;


        TickInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">tick执行的函数</param>
        /// <param name="tickInterval">tick 间隔</param>
        /// <param name="owner">whitch is TickInfo Owner</param>
        public TickInfo(Func<long, bool> func, int tickInterval, TickInfos owner)
        {
            mFunc = func; // tick执行的函数
            mTickInterval = tickInterval; // tick 间隔
            mTimer = new TickTimer(mTickInterval); // 计时器
            mOwner = owner;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">时间间隔已经不需要传了, 留它何用</param>
        /// <returns></returns>
        public bool DoTick(long interval)
        {
            if (null == mFunc)
            {
                return true;
            }

            if (!mTimer.CanTick)
            {
                return true;
            }

            return mFunc.Invoke(interval);
        }

        public void SetRemoveFlag()
        {
            mIsEffective = false;
        }
    }

    public class TickInfos : ITick
    {
        /// <summary>
        /// TickInfos 的拥有者, 目前只有 CommonClient
        /// </summary>
        private object mOwner;

        /// <summary>
        /// 如果为FALSE ,上层应该从维护队列里删除此 TickInfos
        /// </summary>
        public bool IsEffective { get => mIsEffective; }

        private bool mIsEffective = true;

        /// <summary>
        /// 维护的 TickInfo 列表
        /// </summary>
        private LinkedList<TickInfo> mTickList = new LinkedList<TickInfo>();


        TickInfos() { }

        public TickInfos(object owner)
        {
            mOwner = owner;
        }

        public void AddTick(TickInfo tick)
        {
            mTickList.AddLast(tick);
        }

        public void SetRemoveFlag()
        {
            mIsEffective = false;
        }

        public bool DoTick(long interval)
        {
            var node = mTickList.First;

            while (null != node)
            {
                TickInfo tick = node.Value;

                if (!tick.DoTick(interval))
                {
                    mTickList.Remove(tick);

                    // 如果 mTickList.count = 0, 不应该让 TickManager 移除此TickInfos
                    //IsEffective = false;
                }

                node = node.Next;
            }

            return IsEffective;
        }
    }
}
