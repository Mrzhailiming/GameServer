using System;

namespace Base
{
    public class Singletion<T> where T : class, new()
    {
        private static T mInstance = default(T);

        private static readonly object mMutex = new object();

        protected Singletion()
        {

        }

        public static T Instance()
        {
            return InstanceS;
        }

        public static T InstanceS
        {
            get
            {
                if (null == mInstance)
                {
                    lock (mMutex)
                    {
                        if (null == mInstance)
                        {
                            mInstance = new T();
                        }
                    }
                }

                return mInstance;
            }
            private set
            {

            }
        }
    }
}
