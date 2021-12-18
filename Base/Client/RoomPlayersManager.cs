using Base.BaseData;
using Base.DataHelper;
using ConnmonMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Client
{
    /// <summary>
    /// 管理房间的玩家
    /// </summary>
    public class RoomPlayersManager : Singletion<RoomPlayersManager>
    {
        /// <summary>
        /// 队友
        /// </summary>
        Dictionary<long, CommonClient> mTeamers = new Dictionary<long, CommonClient>();
        /// <summary>
        /// 敌人
        /// </summary>
        Dictionary<long, CommonClient> mEnemys = new Dictionary<long, CommonClient>();

        public void AddTeamer(CommonClient team)
        {
            mTeamers.Add(team.RoleID, team);
            Console.WriteLine($"Add team RoleID:{team.RoleID}");
        }

        public void AddEnemy(CommonClient enemy)
        {
            mEnemys.Add(enemy.RoleID, enemy);
            Console.WriteLine($"Add enemy RoleID:{enemy.RoleID}");
        }



        /// <summary>
        /// 广播消息(房间服把自己客户端的操作同步给其他客户端)
        /// </summary>
        /// <param name="message"></param>
        public void BroadCast(CommonMessage message)
        {
            SynchronousInfo synchronousInfo = message.GetObject<SynchronousInfo>();
            RoomServerSynchronousInfo roomServerSynchronousInfo = new RoomServerSynchronousInfo()
            {
                Name = "这是房间服务器同步我的的玩家操作",
                OperationInfo = synchronousInfo.OperationInfo
            };

            CommonMessage newMsg = new CommonMessage()
            {
                mCMD = CMDS.RoomServerFrameSynchronization,
                mMessageBuffer = MessageBufHelper.GetBytes(roomServerSynchronousInfo)
            };

            foreach (CommonClient team in mTeamers.Values)
            {
                team.Send(newMsg);
            }

            foreach (CommonClient enemy in mEnemys.Values)
            {
                enemy.Send(newMsg);
            }
        }
    }
}
