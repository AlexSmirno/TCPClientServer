using System.Threading.Tasks;
using System.Text;
using System;

namespace TCPClient
{
    public class Client
    {
        private const int serverId = 100;

        private Encoding encoding;
        private IteractionProvider iteractionProvider;

        public bool status { get; private set; } //It is true, when authorization correct
        public byte Id { get; private set; }

        public Client(byte id)
        {
            Id = id;
            status = false;
        }

        //В будущем будет хранится в файле конфигураций + заполняются пользовательским классом
        //TODO validation
        public void SetServerConfiguration(string ip, int host)
        {
            iteractionProvider = new IteractionProvider(ip, host, this.Id);
        }

        //TODO validation
        public async Task ConnectToServer() // Подключение к серверу с авторизацией
        {
            try
            {
                await iteractionProvider.ConnectToServer();
                status = true;
                Console.WriteLine("Client connected");
            }
            catch (Exception)
            {
                status = false;
                Console.WriteLine("Client doesn't connected");
            }
        }

        public Sender GetSender()
        {
            return new Sender(iteractionProvider, this.Id);
        }

    }
}