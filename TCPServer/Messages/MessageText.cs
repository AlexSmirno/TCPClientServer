using System;
using System.Linq;
using System.Text;
using TCPServer.Objects;

namespace TCPServer.Messages
{
    public class MessageText : MessageBase
    {

        private byte[] Data;
        private byte[] Hash;
        private string Message;
        
        private Encoding encoding;
        public MessageText()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        public override void ConvertToBytes(string message)
        {
            byte[] bytes = encoding.GetBytes(message);
            long size = bytes.Length;

            Array.Copy(GetHashMD5(bytes),
                       0,
                       Data,
                       0,
                       ByteConst.hashBytes); //Копирование сообщения в буффер

            Array.Copy(encoding.GetBytes(message),
                       0,
                       Data,
                       ByteConst.hashBytes,
                       bytes.Length - ByteConst.hashBytes); //Копирование сообщения в пакет
        }

        public override bool ConvertToMessage(byte[] data)
        {
            byte[] messageBytes = new byte[ByteConst.hashBytes];
            Array.Copy(data,
                       0,
                       messageBytes,
                       0,
                       ByteConst.hashBytes); //Копирование сообщения в буффер
            byte[] hash = messageBytes;
            messageBytes = new byte[data.LongLength - ByteConst.hashBytes];
            Array.Copy(data,
                       ByteConst.hashBytes,
                       messageBytes,
                       0,
                       data.LongLength - ByteConst.hashBytes); //Копирование сообщения в буффер
            byte[] mes = messageBytes.Where(elem => elem != 0).ToArray();
            Message = encoding.GetString(mes);
            Hash = GetHashMD5(mes);
            return equalHash(hash, Hash);
        }

        public bool EqualHash(byte[] data)
        {
            byte[] messageBytes = new byte[ByteConst.hashBytes];
            Array.Copy(data,
                       0,
                       messageBytes,
                       0,
                       ByteConst.hashBytes); //Копирование сообщения в буффер
            byte[] hash = messageBytes;
            messageBytes = new byte[data.LongLength - ByteConst.hashBytes];
            Array.Copy(data,
                       ByteConst.hashBytes,
                       messageBytes,
                       0,
                       data.LongLength - ByteConst.hashBytes); //Копирование сообщения в буффер
            byte[] mes = messageBytes.Where(elem => elem != 0).ToArray();
            return equalHash(hash, GetHashMD5(mes));
        }

        public string GetMessage()
        {
            return Message;
        }

        public byte[] GetHash()
        {
            return Hash;
        }
    }
}
