using System;

namespace Singleton
{
    public class Singleton<T> where T : class, new()
    {
        private static T mInstance = default(T);

        private static readonly object mMutex = new object();

        public static T Instance()
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


    }
}
