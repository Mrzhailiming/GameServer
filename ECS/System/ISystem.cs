using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySystem
{
    public interface ISystem
    {
        void Tick(long tick, IEntity entity);
    }
}
