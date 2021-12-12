using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Tick
{
    public interface ITick
    {
        /// <summary>
        /// 标记自己是否生效, false的话, 要从队列里移除
        /// </summary>
        bool IsEffective { get; }
        bool DoTick(long interval);

        /// <summary>
        /// 设置移除标记
        /// </summary>
        void SetRemoveFlag();
    }
}
