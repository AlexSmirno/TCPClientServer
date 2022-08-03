using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Messages;
using TCPServer.Objects;

namespace TCPServer
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

        public async Task<byte[]> packMessage(int number, Errors error)
        {
            byte[] data;

            if (error > 0)
            {
                data = await packErrorMessage(number, error);
                return await Task.FromResult(data);
            }

            if (Server.quequesMessages[number].Any())
            {
                data = await packUsualMessage(number);
                return await Task.FromResult(data);
            }

            data = await packKeepConnectionMessage(number);

            return await Task.FromResult(data);
        }

        private async Task<byte[]> packErrorMessage(int number, Errors error)
        {
            byte[] data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes + ByteConst.sizeBytes];
            int offset = 0;
            Array.Copy(encoding.GetBytes(100 + "  " + Server.activeUsers.FirstOrDefault(c => c.Thread == number).Id),
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

        private async Task<byte[]> packUsualMessage(int number)
        {
            byte[] data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes + ByteConst.sizeBytes];
            int offset = 0;

            Message mes = Server.quequesMessages[Server.activeUsers.FirstOrDefault(c => c.Thread == number).Thread].Dequeue();
            data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes + ByteConst.hashBytes + mes.Data.Length];

            Array.Copy(encoding.GetBytes(mes.From + "  " + mes.To),
                                0,
                                data,
                                offset,
                                ByteConst.routeBytes); //Копирование route файла в пакет
            offset += ByteConst.routeBytes;

            Array.Copy(BitConverter.GetBytes(mes.MessageType),
                                0,
                                data,
                                offset,
                                ByteConst.messageTypeBytes); //Копирование type файла в пакет
            offset += ByteConst.messageTypeBytes;

            Array.Copy(BitConverter.GetBytes((long)mes.Data.Length + ByteConst.hashBytes),
                                0,
                                data,
                                offset,
                                ByteConst.sizeBytes); //Копирование size файла в пакет
            offset += ByteConst.sizeBytes;

            Array.Copy(mes.Data,
                                0,
                                data,
                                offset,
                                ByteConst.hashBytes); //Копирование hash файла в пакет
            offset += ByteConst.sizeBytes;

            Array.Copy(mes.Data,
                                ByteConst.hashBytes,
                                data,
                                offset,
                                mes.Data.Length); //Копирование сообщения в пакет
            return data;
        }

        private async Task<byte[]> packKeepConnectionMessage(int number)
        {
            byte[] data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes + ByteConst.sizeBytes];
            int offset = 0;

            data = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes];

            Array.Copy(encoding.GetBytes(100 + "  " + Server.activeUsers.FirstOrDefault(c => c.Thread == number).Id),
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
