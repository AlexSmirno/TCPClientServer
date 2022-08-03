using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TCPClient
{
    internal class Messageoasfj
    {
        Encoding encoding;
        private const int sizeBytes = 8, typeBytes = 2, routeBytes = 5, hashBytes = 16;
        public MessageTypes MessageType { get; set; }
        private byte[]data;
        public byte[] Data  //Финальный массив для отправки. Возможно сделать массив массивов для разбиения больших файлов на пакеты
        { 
            get //Возможно переполнения для больших файлов
            {
                data = new byte[sizeBytes + routeBytes + hashBytes + Size];

                if (!(Route != null && Route.Length > 0
                    && dataOfMessage.Length > 0 && Size > 0))
                {
                    throw new System.Exception();
                }

                System.Array.Copy(encoding.GetBytes(Route), 
                                    0, 
                                    data, 
                                    0, 
                                    routeBytes); //Копирование маршрута в пакет
                System.Array.Copy(System.BitConverter.GetBytes((short)MessageType), 
                                    0, 
                                    data, 
                                    routeBytes, 
                                    sizeBytes); //Копирование типа сообщения в пакет
                System.Array.Copy(System.BitConverter.GetBytes(Size), 
                                    0, 
                                    data, 
                                    routeBytes, 
                                    sizeBytes); //Копирование длины сообщения в пакет
                System.Array.Copy(dataOfMessage, 
                                    0, 
                                    data, 
                                    routeBytes + sizeBytes, 
                                    Size); //Копирование сообщения в пакет
                System.Array.Copy(encoding.GetBytes(Hash.ToString()), 
                                    0, 
                                    data, 
                                    routeBytes + sizeBytes + dataOfMessage.Length,
                                    hashBytes); //Копирование хеша сообщения в пакет
                return data;
            }
        }

        public byte[] dataOfMessage { get; set; }
        public long Size { get; set; } //говорят, что максимум 2 гигабайта :)
        public string Route { get; set; }
        public StringBuilder Hash { get; private set; }

        public Messageoasfj()
        {
            //Кодировка для кириллицы (обычная не подходит)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        public void ConvertToBytes(string message)
        {
            if (MessageType != 0)
            {
                dataOfMessage = new byte[0];
                switch (MessageType)
                {
                    case MessageTypes.SendText:
                        dataOfMessage = encoding.GetBytes(message);
                        break;

                    case MessageTypes.SendFiles:
                        dataOfMessage = File.ReadAllBytes(message);
                        GetHashMD5(dataOfMessage);
                        break;

                    default:
                        throw new System.Exception("Wrong type of message");
                }
                Size = dataOfMessage.LongLength;
            }
        }

        private void GetHashMD5(byte[] source) //128 bit
        {
            Hash = new StringBuilder();
            using (MD5 md5Hasher = MD5.Create())
            {
                byte[] data = md5Hasher.ComputeHash(source);
                for (int index = 0; index < data.Length; index++)
                {
                    Hash.Append(data[index].ToString("x2"));
                }
            }
        }
    }
}
