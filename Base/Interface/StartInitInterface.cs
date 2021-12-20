using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Interface
{
    /// <summary>
    /// 需要在启动的时候执行初始化的类继承此接口
    /// 目前不支持 init 函数传参数
    /// </summary>
    public interface StartInitInterface // 用特性的方式可以不? 不太行
    {
        /// <summary>
        /// 服务器和客户端需要执行 init 的类可能不一样
        /// </summary>
        InitType InitType { get; }
        /// <summary>
        /// 要执行 init 的实例
        /// </summary>
        object Instance { get;}

        /// <summary>
        /// 自定义参数 (参数咋传啊)
        /// </summary>
        /// <param name="param"></param>
        void Init(/*params string[] param*/);
    }


    public enum InitType
    {
        Client,
        Server,
        Both,
    }
}
