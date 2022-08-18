using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PackUnpackMessages.Enums;
using TCPServer.Data;

namespace TCPServer
{
    public class Server
    {
        private string IP;
        private int host;
        private int amountListener;
        private IPEndPoint endPoint;
        private Socket serverSocket;
        private Task[] listeners;
        private int countListener = 0;


        public Server()
        {

        }

        #region configuration
        //В будущем будет хранится в файле конфигураций + заполняются пользовательским классом
        //TODO validation
        public void setIP(string IP)
        {
            this.IP = IP;
        }
        public void setHost(int host)
        {
            this.host = host;
        }
        public void setAmountListener(int amountListener)
        {
            this.amountListener = amountListener;
        }
        #endregion

        #region StartServer
        //Настройка сервера
        public void SetServer()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(IP), host);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(amountListener);

            Console.WriteLine("Server is running...");
        }

        public void startListeners()
        {
            if (amountListener < 1)
            {
                throw new Exception("There is not any listener");
            }

            listeners = new Task[amountListener];

            try
            {
                while (true)
                {
                    int listenerID = 0;
                    if (countListener < amountListener)
                    {
                        Socket handler = serverSocket.Accept();
                        int id = ++listenerID;
                        listeners[countListener] = new Task(() => StartNewSocket(id, handler));

                        ServerContext.QueuesMessages.Add(id, new Queue<byte[]>());

                        listeners[countListener].Start();

                        countListener++;
                    }
                }

                Task.WaitAll(listeners);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server : startListeners : str112");
                Console.WriteLine(ex.ToString());
            }


            /*Socket handler = serverSocket.Accept();
            quequesMessages.Add(++countListener, new Queue<Message>());
            StartNewSocket(countListener, handler);*/

        }
        #endregion

        //Прослушивания входящих запросов
        private async Task StartNewSocket(int threadNumber, Socket handler)
        {
            try
            {
                while (true)
                {
                    //Get route + type
                    byte[] buffer = new byte[ByteConst.sizeBytes];
                    int received = handler.Receive(buffer);

                    byte[] smallBuff = new byte[ByteConst.sizeBytes];
                    Array.Copy(buffer,
                               0,
                               smallBuff,
                               0,
                               ByteConst.sizeBytes); //Копирование длины сообщения в буффер
                    int offset = ByteConst.sizeBytes;

                    long size = BitConverter.ToInt64(smallBuff);

                    byte[] data;

                    byte[] buff;
                    data = new byte[size];
                    long bytes = 0;
                    while (bytes < size)
                    {
                        if (bytes >= ByteConst.bufferSize)
                            buff = new byte[ByteConst.bufferSize];
                        else
                            buff = new byte[size % ByteConst.bufferSize];

                        received = handler.Receive(buff);
                        bytes += received;
                        Array.Copy(buff,
                                    0,
                                    data,
                                    offset,
                                    bytes); //Копирование длины сообщения в буффер

                        offset += received;
                    }
                    Console.Write("Сокет #" + threadNumber + " получает сообщение от клиента");
                    /*if (mesType == (int)MessageTypes.SendText)
                    {
                        Console.Write("Сообщение: " + encoding.GetString(data, ByteConst.hashBytes, data.Length - ByteConst.hashBytes));
                    }*/

                    Console.WriteLine();

                    Controller MyController = new Controller(threadNumber);
                    await MyController.ProcessMessage(data);

                    if (ServerContext.QueuesMessages[threadNumber].Count == 0)
                    {
                        MyController.PollingMessage(ServerContext.ActiveUsers.Where(user => user.Thread == threadNumber).FirstOrDefault().Id);
                    }

                    handler.Send(ServerContext.QueuesMessages[threadNumber].Dequeue());
                }
            }
            catch (Exception error)
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                handler.Dispose();
                countListener--;
                ServerContext.QueuesMessages.Remove(threadNumber);
                Console.WriteLine("Чел ушел");
                Console.WriteLine(error.ToString());
            }
        }

        public void ServerClose()
        {
            if (listeners != null)
            {
                for (int i = 0; i < amountListener; i++)
                {
                    listeners[i].Dispose();
                }
            }
            serverSocket.Dispose();
        }
    }
}