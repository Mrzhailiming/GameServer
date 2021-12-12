using System;
using System.Collections.Generic;

namespace Base.Tick
{

    public class TickInfo
    {
        private Func<long, bool> mFunc { get; set; } = null;

        MyTimer mTimer = null;

        private int mTickInterval { get; set; } = 0;

        TickInfo() { }

        public TickInfo(Func<long, bool> func, int tickInterval)
        {
            mFunc = func;
            mTickInterval = tickInterval;

            mTimer = new MyTimer(mTickInterval);
        }

        public bool DoFunc(long interval)
        {
            if (null == mFunc)
            {
                return true;
            }

            //if ((mTmpInterval += interval) < mTickInterval)
            //{
            //    return true;
            //}

            if (!mTimer.Tick)
            {
                return true;
            }


            return mFunc.Invoke(interval);
        }
    }

    public class TickInfos : ITickInfos
    {
        /// <summary>
        /// 谁的tickinfos
        /// </summary>
        private object mOwner;

        public bool IsEffective { get; set; } = true;

        private LinkedList<TickInfo> mTickList = new LinkedList<TickInfo>();

        TickInfos() { }

        public TickInfos(object owner)
        {
            mOwner = owner;
            TickManager.Instance().AddTickInfos(this);
        }

        public void AddTick(TickInfo tick)
        {
            mTickList.AddLast(tick);
        }


        public bool DoTicks(long interval)
        {
            var node = mTickList.First;

            while (null != node)
            {
                TickInfo tick = node.Value;

                if (!tick.DoFunc(interval))
                {
                    mTickList.Remove(tick);
                }

                node = node.Next;
            }

            return IsEffective;
        }
    }
}
