using Base;
using Base.Logger;
using Entity;
using MySystem;
using System;
using System.Collections.Generic;

namespace Server
{
    public class MyServer : Singletion<MyServer>
    {
        public List<ISystem> mSystems = new List<ISystem>();

        public void AddSystem(ISystem system)
        {
            mSystems.Add(system);
            LoggerHelper.Instance().Log(LogType.Console, $"AddSystem");
        }

        public void Tick(long tick)
        {
            foreach(ISystem system in mSystems)
            {
                foreach(Entity.IEntity entity in EntityManager.Instance())
                {
                    system.Tick(tick, entity);
                }
            }
        }
    }
}
