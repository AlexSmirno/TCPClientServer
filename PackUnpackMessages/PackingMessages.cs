using System;
using System.Text;
using System.Threading.Tasks;
using PackUnpackMessages.Enums;

namespace PackUnpackMessages
{
    public class PackingMessages
    {
        private const int serverID = 100;
        Encoding encoding;

        public PackingMessages()
        {
            //Кодировка для текста
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        public async Task<byte[]> packErrorMessage(int reciver, Errors error)
        {
            byte[] data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes + ByteConst.sizeBytes];
            int offset = 0;
            Array.Copy(encoding.GetBytes(100 + "  " + reciver),
                0,
                data,
                offset,
                ByteConst.routeBytes); //Копирование route файла в пакет
            offset += ByteConst.routeBytes;

            Array.Copy(BitConverter.GetBytes((int)MessageTypes.KeepConnection),
                                0,
                                data,
                                offset,
                                ByteConst.messageTypeBytes); //Копирование type файла в пакет
            offset += ByteConst.messageTypeBytes;

            Array.Copy(BitConverter.GetBytes((long)ByteConst.errorBytes),
                                0,
                                data,
                                offset,
                                ByteConst.sizeBytes); //Копирование size файла в пакет
            offset += ByteConst.sizeBytes;

            Array.Copy(BitConverter.GetBytes((int)error),
                                0,
                                data,
                                offset,
                                ByteConst.errorBytes); //Копирование error файла в пакет
            offset += ByteConst.errorBytes;

            return data;
        }

        public async Task<byte[]> packUsualMessage(Message message)
        {
            byte[] byteMessage = message.GetMessage();
            byte[] data = new byte[ByteConst.sizeBytes + byteMessage.Length];


            Array.Copy(BitConverter.GetBytes(byteMessage.LongLength),
                        0,
                        data,
                        0,
                        ByteConst.sizeBytes);

            Array.Copy(byteMessage,
                        0,
                        data,
                        ByteConst.sizeBytes,
                        byteMessage.Length);


            return data;
        }

        public async Task<byte[]> packKeepConnectionMessage(byte reciver)
        {
            Message message = new Message()
            {
                From = serverID,
                To = reciver,
                MessageType = (int)MessageTypes.KeepConnection,
                Data = new byte[0]
            };

            byte[] byteMessage = message.GetMessage();
            byte[] data = new byte[ByteConst.sizeBytes + byteMessage.Length];

            Array.Copy(BitConverter.GetBytes(byteMessage.LongLength),
                        0,
                        data,
                        0,
                        ByteConst.sizeBytes);

            Array.Copy(byteMessage,
                        0,
                        data,
                        ByteConst.sizeBytes,
                        byteMessage.Length);


            return data;
        }
    }
}
