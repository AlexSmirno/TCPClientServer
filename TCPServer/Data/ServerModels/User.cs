using PackUnpackMessages.Enums;

namespace TCPServer.Data.ServerModels
{
    public class User
    {
        public int Id { get; set; }
        public int Thread { get; set; } //Thread, в котором сервер общается с клиентом
        public Rules Rule { get; set; }
    }
}
