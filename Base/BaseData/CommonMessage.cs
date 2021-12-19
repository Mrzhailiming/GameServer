using ConnmonMessage;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Base.BaseData
{
    public class CommonMessageBase
    {
        public int mCMDSOffset => 8;
    }

    public class CommonMessage : CommonMessageBase
    {
        public CMDS mCMD;

        public byte[] mMessageBuffer;
        public int mTotalLength => mCMDSOffset + mMessageBuffer.Length;

        public T GetObject<T>() where T : IMessage<T>, new()
        {
            return GetObj<T>(typeof(T));
        }

        private T GetObj<T>(Type type) where T : IMessage<T> ,new()
        {
            T obj;

            // 从 Parser 对象缓存
            MessageParser parser = CMDHelperManager.Instance().GetMessageParser(new T());

            // 新建
            if(null == parser)
            {
                // 运行时通过反射创建 Parser 对象消耗太大
                // 建一次即可
                Func<T> func = new Func<T>(() => new T());
                MessageParser<T> Parser = Activator.CreateInstance(typeof(MessageParser<T>), new object[] { func }) as MessageParser<T>;

                obj = Parser.ParseFrom(mMessageBuffer);

                CMDHelperManager.Instance().AddMessageParser(type.FullName, Parser);
            }
            // 从缓存中取到了
            else
            {
                obj = (T)parser.ParseFrom(mMessageBuffer);
            }


            return obj;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] ret = new byte[mTotalLength];
            // 
            //SetBytes(BitConverter.GetBytes(mTotalLength), 0, ret, 0, 4);
            byte[] cmdArr = BitConverter.GetBytes((long)mCMD);
            Array.Copy(cmdArr, 0, ret, 0, 8);
            Array.Copy(mMessageBuffer, 0, ret, 8, mMessageBuffer.Length);

            return ret;
        }

        public static void SetBytes(byte[] src, int srcIndex, byte[] tar, int tarIndex, int Count)
        {
            for(int i = 0; i < Count; ++i)
            {
                tar[tarIndex + Count - i - 1] = src[srcIndex + i];
            }
        }
    }
}
