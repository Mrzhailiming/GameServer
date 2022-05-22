using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public enum ConnectType
    {
        Send,
        Recv,
    }

    public class Proto
    {
        /// <summary>
        /// 0-4 存放cmd指令
        /// </summary>
        public const int cmdIDOffset = 0; // 4
        /// <summary>
        /// 4-8 存放包长度
        /// </summary>
        public const int PacketLenOffset = 4; // 4
        public const int BodyOffset = 4; // 4
        public const int fileNameLengthOffset = 8; // 4
        public const int fileTotalLengthOffset = 12; // 4
        public const int fileKeyOffset = 16; // 8
        public const int fileNameOffset = 24; // 

        public const int protoHeadLen = 8;
        /// <summary>
        /// 文件名的偏移
        /// </summary>
        public const int sendOffset = fileNameOffset;
    }

    public enum TCPCMDS
    {
        LOGIN,
        UPLOAD,
        DOWNLOAD,
        TEST,
    }
    
}
