using PackUnpackMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackUnpackMessages.Enums;
namespace TCPClient
{
    public class Reciever
    {
        IteractionProvider iteractionProvider;
        MyTextEncoder encoder;

        public Reciever(IteractionProvider iteractionProvider)
        {
            this.iteractionProvider = iteractionProvider;
            encoder = new MyTextEncoder();
        }

        public async Task GetMessageFromServer()
        {
            try
            {
                while (true)
                {
                    Message message = await iteractionProvider.GetResponce();

                    Route(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void Route(Message message)
        {
            MessageTypes messageTypes = (MessageTypes)message.MessageType;

            switch (messageTypes)
            {
                case MessageTypes.GetChatHistory:

                    break;
                case MessageTypes.GetFilesList:
                    break;
                case MessageTypes.GetFiles:
                    break;
                case MessageTypes.Error:
                    break;
                case MessageTypes.KeepConnection:
                    break;
                case MessageTypes.Undefinded:
                    break;
                default:
                    break;
            }
        }
    }
}
