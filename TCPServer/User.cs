using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Objects;

namespace TCPServer
{
    public class User
    {
        public int Id { get; set; }
        public int Thread { get; set; } //Thread, в котором сервер общается с клиентом
        public Rules Rule { get; set; }
    }
}
