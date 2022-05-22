using System;
using System.Collections.Generic;
using System.Text;

namespace Singleton.Manager
{
    public class TCPPacket
    {
        public int mCmd;
        public byte[] mBuff;
        public int mTotalLen;
        public TCPPacket(int cmd, byte[] buf, int len)
        {
            mCmd = cmd;
            mBuff = buf;
            mTotalLen = len;
        }
    }
}
