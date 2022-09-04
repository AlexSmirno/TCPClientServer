using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PackUnpackMessages;
using PackUnpackMessages.Enums;

namespace TCPClient
{
    public class IteractionProvider
    {
        private const int iteractionDelay = 300;
        private const int serverId = 100;

        private IPEndPoint endPoint;
        private Socket socket;

        private string serverIP;
        private int serverHost;
        private byte clientId;
        private Queue<byte[]> messagesQueue = new Queue<byte[]>();
        private Queue<Message> serverMessages = new Queue<Message>();

        public IteractionProvider(string serverIP, int serverHost, byte clientId)
        {
            this.serverIP = serverIP;
            this.serverHost = serverHost;
            this.clientId = clientId;
        }

        public async Task ConnectToServer()
        {
            Task mainloop = new Task(MainLoop);

            try
            {
                endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverHost);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(endPoint);

                //messagesQueue.Enqueue((new Message() { From = clientId, MessageType = (int)MessageTypes.SendText, To = serverId, Data = new byte[]}).GetMessage());

                mainloop.Start();
            }
            catch (Exception ex)
            {
                mainloop.Dispose();
                Console.WriteLine(ex.Message);
            }
        }

        public async void AddMessageToQueue(Message message)
        {
            PackingMessages packingMessages = new PackingMessages();
            messagesQueue.Enqueue(await packingMessages.packUsualMessage(message));
        }

        public async Task<Message> GetResponce()
        {
            while (serverMessages.Count == 0)
            {
                Task.Delay(iteractionDelay / 5).Wait();
            }

            Message resp = serverMessages.Dequeue();
            return resp;
        }


        private async void MainLoop()
        {
            try
            {
                while (true)
                {
                    Message newMessage;

                    if (messagesQueue.Count > 0)
                    {
                        newMessage = new Message(await IteractionWithServer(messagesQueue.Dequeue()));
                    }
                    else
                    {
                        newMessage = new Message(await IteractionWithServer(await GetPollingMessage()));
                    }

                    serverMessages.Enqueue(newMessage);

                    Task.Delay(iteractionDelay).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something goes wrong =|" + ex.Message);
            }
        }

        private async Task<byte[]> IteractionWithServer(byte[] message)
        {
            try
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
                                buff.Length);

                    offset += received;
                }

                return await Task.FromResult(data);
            }
            catch (Exception ex)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<byte[]> GetPollingMessage()
        {
            Message message = new Message()
            {
                From = clientId,
                To = serverId,
                MessageType = (int)MessageTypes.KeepConnection,
                Data = new byte[0]
            };

            PackingMessages packingMessages = new PackingMessages();
            return await Task.FromResult(await packingMessages.packUsualMessage(message));
        }
    }
}
