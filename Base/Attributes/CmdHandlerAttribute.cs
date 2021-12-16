using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Attributes
{
    public class CmdHandlerAttribute : Attribute
    {
        public CMDS CmdID { get; set; }

        public CMDType CMDType { get; set; }
    }
}
