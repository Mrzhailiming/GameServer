using Base;
using Base.Interface;
using Base.Logger;
using Base.StateMachine;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string ips = "::ffff:127.0.0.1&100|::ffff:127.0.0.1&200";

            string[] ret = ips.Split('|');

            int beginIndex = ret[0].LastIndexOf(':');

            string ipPor = ret[0].Substring(beginIndex + 1, ret[0].Length - 7);
            string[] ipt = ipPor.Split('&');

            LoggerHelper.Instance().Log(LogType.Console, ips);
            LoggerHelper.Instance().Log(LogType.Console, "22222222222");

            LoggerHelper.Instance().Log(LogType.Console, "Hello World!");

            StartInitManager.Instance().StartInit(InitType.Server);

            TestStateMachine testStateMachine = new TestStateMachine();
        }
    }

    public class TestStateMachine
    {
        StateMachine stateMachine = new StateMachine();

        public TestStateMachine()
        {
            stateMachine.AddState(State.LogInServer, DoSomeThing);

            stateMachine.ChangeState(State.LogInServer);
        }

        private void DoSomeThing()
        {
            LoggerHelper.Instance().Log(LogType.Console, $"state");
        }
    }


    //public class testInitInterface1 : Singletion<testInitInterface1>, StartInitInterface
    //{
    //    object StartInitInterface.Instance { get => testInitInterface1.Instance(); }

    //    public void Init(/*params string[] param*/)
    //    {
    //        LoggerHelper.Instance().Log(LogType.Console, "testInitInterface1 success");
    //    }
    //}

    //public class testInitInterface2 : Singletion<testInitInterface2>, StartInitInterface
    //{
    //    object StartInitInterface.Instance { get => testInitInterface2.Instance(); }

    //    public void Init(/*params string[] param*/)
    //    {
    //        LoggerHelper.Instance().Log(LogType.Console, "testInitInterface2 success");
    //    }
    //}

    //public class testInitInterface3 : Singletion<testInitInterface3>, StartInitInterface
    //{
    //    object StartInitInterface.Instance { get => testInitInterface3.Instance(); }

    //    public void Init(/*params string[] param*/)
    //    {
    //        LoggerHelper.Instance().Log(LogType.Console, "testInitInterface3 success");
    //    }
    //}
}
