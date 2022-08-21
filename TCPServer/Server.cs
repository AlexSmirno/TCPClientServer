using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PackUnpackMessages.Enums;
using TCPServer.Data;
using System.Diagnostics;

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

        private Logger.Logger logger = new Logger.Logger();

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

            Console.WriteLine("Server have been started");
            logger.InfoReport("Server have been started");
        }

        public void startListeners()
        {
            if (amountListener < 1)
            {
                throw new Exception("There is not any listener");
            }

            listeners = new Task[amountListener];

            int listenerID = 0;
            try
            {
                while (true)
                {
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
                logger.ErrorReport(ex.Message);
            }


            /*Socket handler = serverSocket.Accept();
            quequesMessages.Add(++countListener, new Queue<Message>());
            StartNewSocket(countListener, handler);*/

        }
        #endregion

        //Прослушивания входящих запросов

        private async Task StartNewSocket(int threadNumber, Socket handler)
        {
            ServerContext.ActiveThreads.Add(new Data.ServerModels.Thread() { Id = threadNumber });
            logger.InfoReport("Start new Socket with id = " + threadNumber);

            try
            {
                while (true)
                {
                    //Get route + type
                    byte[] buffer = new byte[ByteConst.sizeBytes];
                    int received = handler.Receive(buffer);

                    int offset = ByteConst.sizeBytes;

                    long size = BitConverter.ToInt64(buffer);

                    byte[] data;

                    byte[] buff;
                    data = new byte[size];
                    long bytes = 0;
                    while (bytes < size)
                    {
                        if (size >= ByteConst.bufferSize)
                            buff = new byte[ByteConst.bufferSize];
                        else
                            buff = new byte[size % ByteConst.bufferSize];

                        received = handler.Receive(buff);
                        bytes += received;
                        Array.Copy(buff,
                                    0,
                                    data,
                                    0,
                                    buff.Length); //Копирование длины сообщения в буффер

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
                        MyController.PollingMessage(ServerContext.ActiveThreads.Where(thread => thread.Id == threadNumber).FirstOrDefault().User.Id);
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
                ServerContext.ActiveThreads.Remove(ServerContext.ActiveThreads.Where(thread => thread.Id == threadNumber).FirstOrDefault());
                Console.WriteLine("Чел ушел");
                logger.ErrorReport(error.Message);
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