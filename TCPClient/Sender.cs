using PackUnpackMessages;
using System.Text;

namespace TCPClient
{
    public class Sender
    {
        private IteractionProvider iteractionProvider;
        private MyTextEncoder encoder;

        private byte clientId;

        public Sender(IteractionProvider iteractionProvider, byte clientId)
        {
            this.iteractionProvider = iteractionProvider;
            this.clientId = clientId;
            encoder = new MyTextEncoder();
        }

        public async System.Threading.Tasks.Task SendText(byte route, string text)
        {
            Message message = new Message()
            {
                From = clientId,
                To = route,
                MessageType = (int)PackUnpackMessages.Enums.MessageTypes.SendText,
                Data = encoder.TextToBytes(text)
            };

            iteractionProvider.AddMessageToQueue(message);
        }

        public async System.Threading.Tasks.Task SendFile(string path)
        {

        }

    }
}
