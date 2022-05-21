using System;
using System.Collections.Generic;

namespace Entity
{
    /// <summary>
    /// 每个实体(Entity)有多个组件(Component)
    /// </summary>
    public interface IEntity
    {
        Dictionary<Type, IComponent> mComponents { get; }
        void AddComponent(Type type, IComponent component);
        IComponent GetComponent<T>();
    }
}
