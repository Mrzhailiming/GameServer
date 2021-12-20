using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Base.StateMachine
{
    /// <summary>
    /// 状态机
    /// tick对应的状态
    /// </summary>
    public class StateMachine
    {
        Dictionary<State, Action> mStateTable = new Dictionary<State, Action>();
        State mCurState = State.None;
        Action mCurAction = null;

        bool IsStateChange = false;

        public StateMachine()
        {
            Thread thread = new Thread(Update);
            thread.Name = "statemachine";
            thread.Start();
            // 要不让 tickManager tick? 不然消耗太大了把
        }

        public void AddState(State state, Action action)
        {
            mStateTable[state] = action;
        }

        public void ChangeState(State newState)
        {
            mCurState = newState;
            IsStateChange = true;
        }

        private void Update()
        {
            while (true)
            {
                if (!IsStateChange)
                {
                    mCurAction?.Invoke();
                }
                else
                {
                    mCurAction = mStateTable[mCurState];
                    IsStateChange = false;
                }

                Thread.Sleep(10);
            }
            
        }
    }


    public enum State
    {
        None,
        LogInServer,    // 登录中心服
        LogInRoomServer,    // 登录房间服
        JoinRoom,   // 加入房间
        Gaming, // 游戏中
    }
}
