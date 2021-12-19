using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Attributes
{

    public class CMDTypeAttribute : Attribute
    {
        public CMDType CMDType { get; set; }
    }

    public class CmdHandlerAttribute : Attribute
    {
        public CMDS CmdID { get; set; }
    }
}
