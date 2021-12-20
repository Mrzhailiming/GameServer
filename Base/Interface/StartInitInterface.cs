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
        /// 自定义参数
        /// </summary>
        /// <param name="param"></param>
        void Init(params string[] param);
    }
}
