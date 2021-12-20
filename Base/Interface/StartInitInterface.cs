using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Interface
{
    /// <summary>
    /// 需要在启动的时候执行初始化的类继承此接口
    /// </summary>
    public interface StartInitInterface
    {
        /// <summary>
        /// 要执行 init 的实例
        /// </summary>
        public object Instance { get;}

        /// <summary>
        /// 自定义参数 (参数咋传啊)
        /// </summary>
        /// <param name="param"></param>
        void Init(/*params string[] param*/);
    }
}
