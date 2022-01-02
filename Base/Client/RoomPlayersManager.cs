using Base.BaseData;
using Base.DataHelper;
using Base.Logger;
using ConnmonMessage;
using System;
using System.Collections.Concurrent;
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
        ConcurrentDictionary<long, CommonClient> mTeamers = new ConcurrentDictionary<long, CommonClient>();
        /// <summary>
        /// 敌人
        /// </summary>
        ConcurrentDictionary<long, CommonClient> mEnemys = new ConcurrentDictionary<long, CommonClient>();

        public void AddTeamer(CommonClient team)
        {
            mTeamers.TryAdd(team.RoleID, team);
            LoggerHelper.Instance().Log(LogType.Console, $"roomserver Add team RoleID:{team.RoleID}");
        }

        public void AddEnemy(CommonClient enemy)
        {
            mEnemys.TryAdd(enemy.RoleID, enemy);
            LoggerHelper.Instance().Log(LogType.Console, $"roomserver Add enemy RoleID:{enemy.RoleID}");
        }



        /// <summary>
        /// 广播消息(房间服把自己客户端的操作同步给其他客户端)
        /// </summary>
        /// <param name="message"></param>
        public void BroadCast(CommonMessage message)
        {
            SynchronousInfo synchronousInfo = message.GetObject<SynchronousInfo>();
            RSRCSynchronousInfo RSRCSynchronousInfo = new RSRCSynchronousInfo()
            {
                Name = synchronousInfo.Name,
                OperationInfo = synchronousInfo.OperationInfo,
                Camp = ClientInfo.MyCamp
            };

            //RSRCSynchronousInfo.Camp = "Team";

            //CommonMessage TeamMsg = new CommonMessage()
            //{
            //    mCMD = CMDS.RSRCFrameSynchronization,
            //    mMessageBuffer = MessageBufHelper.GetBytes(RSRCSynchronousInfo)
            //};

            //RSRCSynchronousInfo.Camp = "Enemy";

            //CommonMessage enemyMsg = new CommonMessage()
            //{
            //    mCMD = CMDS.RSRCFrameSynchronization,
            //    mMessageBuffer = MessageBufHelper.GetBytes(RSRCSynchronousInfo)
            //};

            CommonMessage TeamMsg = new CommonMessage()
            {
                mCMD = CMDS.RSRCFrameSynchronization,
                mMessageBuffer = MessageBufHelper.GetBytes(RSRCSynchronousInfo)
            };

            foreach (CommonClient team in mTeamers.Values)
            {
                team.Send(TeamMsg);
            }

            foreach (CommonClient enemy in mEnemys.Values)
            {
                enemy.Send(TeamMsg);
            }
        }
    }
}
