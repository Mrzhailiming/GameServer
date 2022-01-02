using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Base.Tick
{
    public class TickTimer
    {
        private Stopwatch timer = null;
        private object mutex = new object();

        private int mMnterval;
        public double prevTickMS = 0;

        public TickTimer(int interval)
        {
            mMnterval = interval;
        }

        public bool CanTick
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
                else if (timer.ElapsedMilliseconds - prevTickMS > mMnterval)
                {
                    //LoggerHelper.Instance().Log(LogType.Console, $"TickTimer CanTick 间隔{timer.ElapsedMilliseconds - prevTickMS}");
                    prevTickMS = timer.ElapsedMilliseconds;
                    return true;
                }

                return false;
            }
            set
            {

            }
        }
    }

}
