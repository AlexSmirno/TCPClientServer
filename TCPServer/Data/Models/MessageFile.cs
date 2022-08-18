using System;
using System.IO;
using System.Text;
using System.Linq;

namespace TCPServer.Data.Models
{
    internal class MessageFile
    {
        /*private Encoding encoding;

        //Константы показывающие кол-во байт выделенных под дополнительную информацию в сообщении
        private const int filenameBytes = 50;
        private const int hashBytes = 16;


        public MessageTypes MessageType { get; }

        private string filename;
        public string Route { get; set; }


        private byte[] dataOfMessage { get; set; }
        private long Size; //говорят, что максимум 2 гигабайта :)
        private byte[] Hash { get; set; }

        public byte[] Data { get; set; } //Финальный массив для отправки. Возможно сделать массив массивов для разбиения больших файлов на пакеты

        //Для отправки
        public MessageFile(MessageTypes messageType, string route)
        {
            MessageType = messageType;
            Route = route;

            //Кодировка для кириллицы (обычная не подходит)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        //Для получения
        public MessageFile()
        {
            //Кодировка для кириллицы (обычная не подходит)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        public override void ConvertToBytes(string fullFilename)
        {
            dataOfMessage = File.ReadAllBytes(fullFilename);
            Size = dataOfMessage.Length;
            Hash = GetHashMD5(dataOfMessage);

            filename = Path.GetFileName(fullFilename);

            Data = new byte[filenameBytes + Size];

            if (!(Route != null && Route.Length > 0
                && dataOfMessage.Length > 0 && Size > 0))
            {
                throw new Exception("Message is not prepared");
            }

            int offset = 0;
            Array.Copy(encoding.GetBytes(filename),
                                0,
                                Data,
                                offset,
                                filenameBytes); //Копирование имя файла в пакет
            offset += filenameBytes;
            Array.Copy(dataOfMessage,
                                0,
                                Data,
                                offset,
                                Size); //Копирование сообщения в пакет
        }



        public override bool ConvertToMessage(byte[] data)
        {
            byte[] messageBytes = new byte[hashBytes];
            int offset = 0;
            Array.Copy(data,
                       0,
                       messageBytes,
                       offset,
                       hashBytes); //Копирование сообщения в буффер
            byte[] hash = messageBytes;
            messageBytes = new byte[50]; //50 байт
            offset += hashBytes;
            Array.Copy(data,
                       offset,
                       messageBytes,
                       0,
                       filenameBytes); //Копирование имя файла в буффер
            filename = encoding.GetString(messageBytes);
            offset += filenameBytes;

            messageBytes = new byte[data.Length - filenameBytes - hashBytes];
            Array.Copy(data,
                       offset,
                       messageBytes,
                       0,
                       data.Length - filenameBytes - hashBytes); //Копирование имя файла в буффер

            File.WriteAllBytes("C:\\Users\\79035\\OneDrive\\Рабочий стол\\" + filename, messageBytes);
            return equalHash(hash, GetHashMD5(messageBytes));
        }*/
    }
}
