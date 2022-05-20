using Base;
using Base.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    /// <summary>
    /// 存储所有实体
    /// </summary>
    public class EntityManager : Singletion<EntityManager>
    {
        Dictionary<long, IEntity> mEtities = new Dictionary<long, IEntity>();
        public void AddEntity(IEntity entity, long id)
        {
            bool res = mEtities.TryAdd(id, entity);
            LoggerHelper.Instance().Log(LogType.Console, $"AddEntity id:{id} result:{res}");
        }
        public IEnumerator GetEnumerator()
        {
            foreach(IEntity entity in mEtities.Values)
            {
                yield return entity;
            }
        }
    }
}
