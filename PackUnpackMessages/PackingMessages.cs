using System;
using System.Text;
using System.Threading.Tasks;
using PackUnpackMessages.Enums;

namespace PackUnpackMessages
{
    public class PackingMessages
    {
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
            byte[] data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes + ByteConst.sizeBytes];
            int offset = 0;

            data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes + ByteConst.hashBytes + message.Data.Length];

            Array.Copy(encoding.GetBytes(message.From + "  " + message.To),
                                0,
                                data,
                                offset,
                                ByteConst.routeBytes); //Копирование route файла в пакет
            offset += ByteConst.routeBytes;

            Array.Copy(BitConverter.GetBytes(message.MessageType),
                                0,
                                data,
                                offset,
                                ByteConst.messageTypeBytes); //Копирование type файла в пакет
            offset += ByteConst.messageTypeBytes;

            Array.Copy(BitConverter.GetBytes((long)(message.Data.Length + ByteConst.hashBytes)),
                                0,
                                data,
                                offset,
                                ByteConst.sizeBytes); //Копирование size файла в пакет
            offset += ByteConst.sizeBytes;

            Array.Copy(message.Data,
                                0,
                                data,
                                offset,
                                ByteConst.hashBytes); //Копирование hash файла в пакет
            offset += ByteConst.hashBytes;

            Array.Copy(message.Data,
                                ByteConst.hashBytes,
                                data,
                                offset,
                                message.Data.Length - ByteConst.hashBytes); //Копирование сообщения в пакет
            return data;
        }

        public async Task<byte[]> packKeepConnectionMessage(int reciver)
        {
            byte[] data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes + ByteConst.sizeBytes];
            int offset = 0;

            data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes];

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

            Array.Copy(BitConverter.GetBytes((long)ByteConst.emptyBytes),
                                0,
                                data,
                                offset,
                                ByteConst.sizeBytes); //Копирование size файла в пакет
            offset += ByteConst.sizeBytes;

            return data;
        }
    }
}
