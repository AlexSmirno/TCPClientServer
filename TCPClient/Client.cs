using PackUnpackMessages;
using PackUnpackMessages.Enums;

using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net;
using System;

namespace TCPClient
{
    public class Client
    {
        private const int iteractionDelay = 500;

        private byte Id;
        private Encoding encoding;
        private string serverIP;
        private int serverHost;
        private IPEndPoint endPoint;
        private Socket socket;
        public bool status { get; set; }
        private Queue<Message> messagesQueue = new Queue<Message>();
        public Client(byte Id)
        {
            this.Id = Id;
            status = false;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        #region configuration
        //В будущем будет хранится в файле конфигураций + заполняются пользовательским классом
        //TODO validation
        public void setServerIP(string IP)
        {
            serverIP = IP;
        }
        public void setServerHost(int host)
        {
            serverHost = host;
        }
        #endregion


        public async Task ClientStart()
        {
            try
            {
                endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverHost);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(endPoint);
                status = true;
                Console.WriteLine("Client " + Id + " connected");
            }
            catch (Exception)
            {
                socket.Dispose();
                status = false;
                Console.WriteLine("Client " + Id + " doesn't connected");
            }
        }


        public async Task SendMessage(Message message)
        {
            while (true)
            {
                if (messagesQueue.Count > 0)
                {
                    await IteractionWithServer(messagesQueue.Dequeue().GetMessage());
                }
                else
                {
                    await IteractionWithServer(await PackInfoMessage());
                }

                await Task.Delay(iteractionDelay);
            }
        }

        private async Task IteractionWithServer(byte[] message)
        {
            try
            {
                while (true)
                {
                    socket.Send(message);

                    byte[] responceBytes = new byte[ByteConst.sizeBytes];
                    int bytesRec = socket.Receive(responceBytes);

                    long offset = ByteConst.sizeBytes;

                    long size = BitConverter.ToInt64(responceBytes);

                    byte[] data;

                    byte[] buff;
                    data = new byte[size];
                    long bytes = 0;
                    long received = 0;
                    while (bytes < size)
                    {
                        if (size >= ByteConst.bufferSize)
                            buff = new byte[ByteConst.bufferSize];
                        else
                            buff = new byte[size % ByteConst.bufferSize];

                        received = socket.Receive(buff);
                        bytes += received;
                        Array.Copy(buff,
                                    0,
                                    data,
                                    0,
                                    buff.Length); //Копирование длины сообщения в буффер

                        offset += received;
                    }
                    Message recivedMessage = new Message(data);
                    Console.WriteLine(Id + ": " + recivedMessage.MessageType + encoding.GetString(recivedMessage.Data));
                }
            }
            catch (Exception ex)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                status = false;
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<byte[]> PackInfoMessage()
        {
            return new byte[0];
        }

        public async Task addMessage(string message, int toID)
        {

            while (true)
            {
                Message newMessage = new Message
                {
                    From = Id,
                    To = (byte)toID,
                    MessageType = (int)MessageTypes.SendText,
                    Data = encoding.GetBytes(message)
                };

                messagesQueue.Enqueue(newMessage);

                Thread.Sleep(5000);
            }
        }
    }
}