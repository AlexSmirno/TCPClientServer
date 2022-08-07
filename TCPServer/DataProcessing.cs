using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Messages;
using TCPServer.Objects;
using TCPServer.ServerModels;

namespace TCPServer
{
    public class DataProcessing
    {
        private Encoding encoding;
        private string Route;
        private int Thread;

        public DataProcessing(int thread)
        {
            Thread = thread;
            
            //Кодировка для текста
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        public async Task<Errors> DataProcessingChoise(byte[] route, int mesType, byte[] data)
        {
            ParceRoute(route);
            AddUser();

            Errors response = Errors.NoError;
            switch (mesType)
            {
                case (int)MessageTypes.GetFiles:
                    Console.WriteLine("Type is GetFiles");
                    break;

                case (int)MessageTypes.SendText:
                    response = GetText(mesType, data);
                    break;

                case (int)MessageTypes.GetFilesList:
                    Console.WriteLine("Type is GetFilesList");
                    break;

                case (int)MessageTypes.Error:
                    Console.WriteLine("Type is Error");
                    break;

                case (int)MessageTypes.Undefinded:
                    Console.WriteLine("Type is Undefinded");
                    break;

                case (int)MessageTypes.KeepConnection:
                    break;

                default:
                    Console.WriteLine("Switch got default");
                    break;
            }

            return response;
        }

        //Если сообщение на сервер
        private Errors GetText(int mesType, byte[] data)
        {
            int bytes = 0;

            MessageText newMessage = new MessageText();

            if (!newMessage.EqualHash(data))
            {
                Console.WriteLine("Полученный хэш не совпадает с хэшом полученного сообщением");
                return Errors.WrongHash;
            }

            //Сохранить

            //Add message to Queue
            int senderID = int.Parse(Route.Split("  ")[0]);
            int reciverID = int.Parse(Route.Split("  ")[1]);

            if (Server.activeUsers.Where(user => user.Id == reciverID).Any())
            {
                Server.quequesMessages[Server.activeUsers.FirstOrDefault(user => user.Id == reciverID).Thread]
                    .Enqueue(new Message() { From = senderID, To = reciverID, MessageType = mesType, Data = data });

                return Errors.NoError;
            }

            return Errors.NotFoundClient;
        }


        private void ParceRoute(byte[] route)
        {
            Route = encoding.GetString(route);
        }

        private void AddUser()
        {
            //Add new User
            int userID = int.Parse(Route.Split("  ")[0]);

            if (!Server.activeUsers.Where(user => user.Id == userID).Any())
            {
                Server.activeUsers.Add(new User() { Id = userID, Thread = this.Thread, Rule = Rules.ReadGetSendChat });
            }
        }
    }
}
