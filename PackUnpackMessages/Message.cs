using PackUnpackMessages.Enums;
using System;

namespace PackUnpackMessages
{
    public class Message
    {
        public byte From { get; set; }
        public byte To { get; set; }
        public int MessageType { get; set; }
        public byte[] Data { get; set; }

        public Message()
        {

        }

        public Message(byte[] message)
        {
            SetMessage(message);
        }

        public byte[] GetMessage()
        {
            byte[] message = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes + Data.Length];

            int offset = 0;
            byte[] route = new byte[] { From, To };
            Array.Copy(route,
                        0,
                        message,
                        offset,
                        ByteConst.routeBytes); //Копирование маршрута в буффер
            offset += ByteConst.routeBytes;
            
            Array.Copy(BitConverter.GetBytes(MessageType),
                        0,
                        message,
                        offset,
                        ByteConst.messageTypeBytes); //Копирование типа в буффер
            offset += ByteConst.messageTypeBytes;

            Array.Copy(BitConverter.GetBytes((long)Data.Length),
                        0,
                        message,
                        offset,
                        ByteConst.sizeBytes); //Копирование длину сообщения в буффер
            offset += ByteConst.sizeBytes;

            Array.Copy(Data,
                        0,
                        message,
                        offset,
                        Data.Length); //Копирование сообщение в буффер

            return message;
        }

        private void SetMessage(byte[] message)
        {
            int offset = ByteConst.routeBytes;
            From = message[0];
            To = message[1];
            byte[] buffer = new byte[ByteConst.messageTypeBytes];
            Array.Copy(message,
                        offset,
                        buffer,
                        0,
                        ByteConst.messageTypeBytes); //Копирование типа в буффер
            offset += ByteConst.messageTypeBytes;
            MessageType = BitConverter.ToInt32(buffer);

            Array.Copy(message,
                        offset,
                        Data,
                        0,
                        message.Length - ByteConst.routeBytes - ByteConst.messageTypeBytes); //Копирование сообщение в буффер
        }
    }
}
