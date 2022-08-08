using PackUnpackMessages;
using PackUnpackMessages.Enums;
using System.Security.Cryptography;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace TCPServer
{
    public class Controller
    {
        private PackingMessages packMessage;

        public Controller()
        {
            packMessage = new PackingMessages();
        }

        public async Task<byte[]> ProcessMessage(int type, byte[] route, byte[] recivedMessage)
        {
            Errors error = new Errors();
            byte[] mess;

            if (!CheckHash(recivedMessage))
            {
                error = Errors.WrongHash;
                return await packMessage.packErrorMessage(route[1], error);
            }

            Message message = new Message()
            {
                MessageType = type,
                From = route[0],
                To = route[1],
                Data = recivedMessage
            };

            if (error > 0)
            {
                mess = await packMessage.packErrorMessage(message.To, error);
            }

            mess = await packMessage.packUsualMessage(message);
            return mess;
        }
        
        public async void RedirectionFile()
        {

        }

        public async void SaveFile()
        {

        }

        public async void RedirectionText(Message mes)
        {

        }

        private bool CheckHash(byte[] message)
        {
            byte[] hash = new byte[ByteConst.hashBytes];
            Array.Copy(message,
                       0,
                       hash,
                       0,
                       ByteConst.hashBytes);
            return Enumerable.SequenceEqual(hash, GetHashMD5(message));
        }

        private byte[] GetHashMD5(byte[] source) //128 bits = 16bytes
        {
            byte[] data;
            using (MD5 md5Hasher = MD5.Create())
            {
                data = md5Hasher.ComputeHash(source);
            }
            return data;
        }
    }
}
