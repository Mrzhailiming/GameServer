namespace Entity
{
    /// <summary>
    /// 每个实体(Entity)有多个组件(Component)
    /// </summary>
    public interface IEntity
    {
        IComponent GetComponent<T>();
    }
}
