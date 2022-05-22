namespace Entity.Component
{
    public class BufferComponent : IComponent, IComponentOwner
    {
        public IEntity mOwner { get; set; }

        public byte[] mTotalBuffer { get; set; }

        public int mHadUseCount { get; set; } = 0;
    }
}
