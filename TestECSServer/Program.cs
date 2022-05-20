using Base.Logger;
using Entity;
using Entity.Component;
using MySystem;
using Server;
using System;
using System.Threading;

namespace TestECSServer
{
    class Program
    {
        /// <summary>
        /// ECSServer
        /// 实体没有函数
        /// 系统没有字段
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            MyServer.Instance().AddSystem(new ConnectSysytem());
            Add("111EntityOwner", 1, "compoName111");
            Add("222EntityOwner", 2, "compoName222");
            Add("333EntityOwner", 3, "compoName333");
            Add("444EntityOwner", 4, "compoName444");
            while (true)
            {
                MyServer.Instance().Tick(DateTime.Now.Ticks);

                LoggerHelper.Instance().Log(LogType.Console, $"main tick");

                Thread.Sleep(1000 * 2);
            }
        }

        public static void Add(string owner, long id, string name)
        {
            ConnectionEntity entity = new ConnectionEntity();
            entity.mOwner = owner;
            ConnectionComponent cp = new ConnectionComponent();
            cp.mOwner = entity;
            cp.mName = name;
            entity.AddComponent(typeof(ConnectionComponent), cp);



            EntityManager.Instance().AddEntity(entity, id);
        }
    }
}
