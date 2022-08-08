using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PackUnpackMessages.Enums;
using TCPServer.ServerModels;

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
        private readonly Controller MyController;

        private static Dictionary<int, Queue<PackUnpackMessages.Message>> quequesMessages = new Dictionary<int, Queue<PackUnpackMessages.Message>>();
        private static List<User> activeUsers = new List<User>();

        public Server()
        {
            MyController = new Controller();
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
                    if (countListener < amountListener)
                    {
                        Socket handler = serverSocket.Accept();
                        int a = countListener;
                        listeners[countListener] = new Task(() => StartNewSocket(a, handler));

                        quequesMessages.Add(a, new Queue<PackUnpackMessages.Message>());

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
        private async Task StartNewSocket(int number, Socket handler)
        {
            try
            {
                while (true)
                {
                    byte[] smallBuff = new byte[50]; //50 байт

                    //Get route + type
                    byte[] buffer = new byte[ByteConst.routeBytes + ByteConst.messageTypeBytes + ByteConst.sizeBytes];
                    int received = handler.Receive(buffer);

                    int offset = 0;
                    Array.Copy(buffer,
                                offset,
                                smallBuff,
                                0,
                                ByteConst.routeBytes); //Копирование маршрута в буффер
                    byte[] route = smallBuff;
                    offset += ByteConst.routeBytes;

                    smallBuff = new byte[ByteConst.messageTypeBytes];
                    Array.Copy(buffer,
                               offset,
                               smallBuff,
                               0,
                               ByteConst.messageTypeBytes); //Копирование типа в буффер
                    int mesType = BitConverter.ToInt32(smallBuff);
                    offset += ByteConst.messageTypeBytes;

                    smallBuff = new byte[ByteConst.sizeBytes];
                    Array.Copy(buffer,
                               offset,
                               smallBuff,
                               0,
                               ByteConst.sizeBytes); //Копирование длины сообщения в буффер
                    offset += ByteConst.sizeBytes;

                    long size = BitConverter.ToInt64(smallBuff);

                    int bytes = 0;

                    byte[] data;
                    if (size > 0)
                    {
                        byte[] buff;
                        data = new byte[size];
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
                                        0,
                                        bytes); //Копирование длины сообщения в буффер
                        }
                    }
                    else
                    {
                        data = new byte[size];
                    }
                    Console.Write("Сокет #" + number + " получает сообщение от клиента по маршруту "
                                    + route + " типа " + mesType + " длины " + size
                                    + " байт. ");
                    /*if (mesType == (int)MessageTypes.SendText)
                    {
                        Console.Write("Сообщение: " + encoding.GetString(data, ByteConst.hashBytes, data.Length - ByteConst.hashBytes));
                    }*/

                    Console.WriteLine();

                    buffer = await MyController.ProcessMessage(mesType, route, data);

                    handler.Send(buffer);
                }
            }
            catch (Exception error)
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                handler.Dispose();
                countListener--;
                quequesMessages.Remove(number);
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