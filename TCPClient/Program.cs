using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.ReadKey();
                try
                {
                    int amount = 3;

                    Client client = new Client(101);
                    Task task = new Task(() => client.addMessage("Привет мир!", 101));
                    task.Start();

                    client.ClientStart();
                    /*Task[] clients = new Task[amount];
                    for (int i = 0; i < amount; i++)
                    {
                        int a = i + 101;

                        Client client = new Client(a);
                        clients[i] = new Task(() => client.ClientStart());

                        new Task(() => client.addMessage("Привет мир!", a)).Start();
                    }

                    for (int i = 0; i < amount; i++)
                    {
                        clients[i].Start();
                    }

                    Console.ReadKey();

                    for (int i = 0; i < amount; i++)
                    {
                        clients[i].Dispose();
                    }*/
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                    Console.ReadKey();
                }
            }
        }


    }

    public class Client
    {
        private Queue<Message> quequesMessages = new Queue<Message>();

        private const int routeBytes = 8;
        private const int messageTypeBytes = 4;
        private const int sizeBytes = 8;
        private const int hashBytes = 16;
        private const int errorBytes = 4;
        private const int serverID = 100;

        private int id;
        private Encoding encoding;

        public Client(int id)
        {
            this.id = id;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        public async Task ClientStart()
        {
            Console.WriteLine("Client " + id + " was created");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.1.64"), 8080);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);
            Console.WriteLine("Client " + id + " connected");


            try
            {
                while (true)
                {
                    socket.Send(await packMessage(Errors.NoError));
                    
                    byte[] responceBytes = new byte[256];
                    int bytesRec = socket.Receive(responceBytes);

                    Console.WriteLine(encoding.GetString(responceBytes, 0, bytesRec));

                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Console.WriteLine(ex.Message);
            }
        }

        public async Task addMessage(string message, int toID)
        {

            while (true)
            {
                Message newMessage = new Message
                {
                    From = id,
                    To = toID,
                    MessageType = (int)MessageTypes.SendText,
                    Data = encoding.GetBytes(message)
                };

                quequesMessages.Enqueue(newMessage);

                Thread.Sleep(5000);
            }
        }

        private async Task<byte[]> packMessage(Errors error)
        {
            byte[] data;
            int offset = 0;

            if (error > 0)
            {
                data = new byte[routeBytes + messageTypeBytes + sizeBytes + errorBytes];
                Array.Copy(encoding.GetBytes(id + "  " + serverID),
                    0,
                    data,
                    offset,
                    routeBytes); //Копирование route файла в пакет
                offset += routeBytes;

                Array.Copy(BitConverter.GetBytes((int)MessageTypes.Error),
                                    0,
                                    data,
                                    offset,
                                    messageTypeBytes); //Копирование type файла в пакет
                offset += messageTypeBytes;

                Array.Copy(BitConverter.GetBytes((long)4),
                                    0,
                                    data,
                                    offset,
                                    sizeBytes); //Копирование size файла в пакет
                offset += sizeBytes;

                Array.Copy(BitConverter.GetBytes((int)error),
                                    0,
                                    data,
                                    offset,
                                    errorBytes); //Копирование error файла в пакет
                offset += errorBytes;

                return await Task.FromResult(data);
            }

            if (quequesMessages.Count > 0)
            {
                Message mes = quequesMessages.Dequeue();
                data = new byte[routeBytes + messageTypeBytes + sizeBytes + hashBytes + mes.Data.LongLength];
                Array.Copy(encoding.GetBytes(mes.From + "  " + mes.To),
                                    0,
                                    data,
                                    offset,
                                    routeBytes); //Копирование route файла в пакет
                offset += routeBytes;

                Array.Copy(BitConverter.GetBytes(mes.MessageType),
                                    0,
                                    data,
                                    offset,
                                    messageTypeBytes); //Копирование type файла в пакет
                offset += messageTypeBytes;

                Array.Copy(BitConverter.GetBytes(mes.Data.LongLength + hashBytes),
                                    0,
                                    data,
                                    offset,
                                    sizeBytes); //Копирование size файла в пакет
                offset += sizeBytes;

                Array.Copy(await GetHashMD5(mes.Data),
                                    0,
                                    data,
                                    offset,
                                    hashBytes); //Копирование hash файла в пакет
                offset += hashBytes;

                Array.Copy(mes.Data,
                                    0,
                                    data,
                                    offset,
                                    mes.Data.Length); //Копирование сообщения в пакет
                return await Task.FromResult(data);
            }

            data = new byte[routeBytes + messageTypeBytes + sizeBytes];

            Array.Copy(encoding.GetBytes(id + "  " + serverID),
                                0,
                                data,
                                offset,
                                routeBytes); //Копирование route файла в пакет
            offset += routeBytes;

            Array.Copy(BitConverter.GetBytes((int)MessageTypes.KeepConnection),
                                0,
                                data,
                                offset,
                                messageTypeBytes); //Копирование type файла в пакет
            offset += messageTypeBytes;

            Array.Copy(BitConverter.GetBytes((long)0),
                                0,
                                data,
                                offset,
                                sizeBytes); //Копирование size файла в пакет
            offset += sizeBytes;

            return await Task.FromResult(data);
        }

        private async Task<byte[]> GetHashMD5(byte[] source) //128 bits = 16bytes
        {
            byte[] data;
            using (MD5 md5Hasher = MD5.Create())
            {
                data = md5Hasher.ComputeHash(source);
            }
            return await Task.FromResult(data);
        }
    }
}
