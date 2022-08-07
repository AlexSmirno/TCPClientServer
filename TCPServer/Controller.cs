using PackUnpackMessages;
using PackUnpackMessages.Enums;

namespace TCPServer
{
    public class Controller
    {
        private PackingMessages packMessage;

        public Controller()
        {
            packMessage = new PackingMessages();
        }

        public byte[] ProcessMessage(int type, byte[] route, byte[] recivedMessage)
        {
            byte[] mess = new byte[1];
            Message message = new Message()
            {
                MessageType = type,
                From = route[0],
                To = route[1],
                Data = recivedMessage
            };

            if (error > 0)
            {
                packMessage.packErrorMessage(message.To, error);
            }
            return mess;
        }
         
        public void RedirectionFile()
        {

        }

        public void SaveFile()
        {

        }

        public void RedirectionText(Message mes)
        {

        }
    }
}
