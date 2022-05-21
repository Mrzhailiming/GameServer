using Base.Logger;
using System;
using System.Collections.Generic;

namespace Entity
{
    public class ConnectionEntity : IEntity, IEntityOwner
    {
        public Dictionary<Type, IComponent> mComponents { get; set; } = new Dictionary<Type, IComponent>();

        public string mOwner { get; set; }

        public void AddComponent(Type type, IComponent component)
        {
            bool res = mComponents.TryAdd(type, component);
            LoggerHelper.Instance().Log(LogType.Console, $"ConnectionEntity AddComponent mOwner:{mOwner} type:{type} result:{res}");

        }
        public IComponent GetComponent<T>()
        {
            Type type = typeof(T);
            mComponents.TryGetValue(type, out IComponent component);

            return component;
        }
    }
}
