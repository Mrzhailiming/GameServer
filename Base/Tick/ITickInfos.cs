using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Tick
{
    public interface ITickInfos
    {
        bool DoTicks(long interval);
    }
}
