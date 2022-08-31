using PackUnpackMessages;
using System.Text;

namespace TCPClient
{
    public class Sender
    {
        private IteractionProvider iteractionProvider;
        private byte clientId;
        private Encoding encoding;

        public Sender(IteractionProvider iteractionProvider, byte clientId)
        {
            this.iteractionProvider = iteractionProvider;
            this.clientId = clientId;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        public async System.Threading.Tasks.Task<string> SendText(byte route, string text)
        {
            Message message = new Message()
            {
                From = clientId,
                To = route,
                MessageType = (int)PackUnpackMessages.Enums.MessageTypes.SendText,
                Data = encoding.GetBytes(text)
            };

            iteractionProvider.AddMessageToQueue(message);

            Message responce = await iteractionProvider.GetResponce();
            if (responce != null)
            {
                return encoding.GetString(responce.Data);
            }
            return null;
        }

        public void SendFile(string path)
        {

        }

        public void GetFile(string filename)
        {

        }
    }
}
