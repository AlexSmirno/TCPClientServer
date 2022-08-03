using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient
{
    internal enum MessageTypes
    {
        Undefinded = 0,
        SendText = 1,
        SendFiles = 2,
        GetFilesList = 3,
        GetFiles = 4,
        Error = 5,
        KeepConnection = 6
    }
}
