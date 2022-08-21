
using PackUnpackMessages;
using PackUnpackMessages.Enums;
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

    public class Client
    {
        private Queue<Message> quequesMessages = new Queue<Message>();
        private byte id;
        private Encoding encoding;

        public Client(byte id)
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
                    Console.WriteLine(id + ": " + recivedMessage.MessageType + encoding.GetString(recivedMessage.Data));

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
                    To = (byte)toID,
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

            if (quequesMessages.Count > 0)
            {
                Message mesage = quequesMessages.Dequeue();
                byte[] byteMessage = mesage.GetMessage();
                data = new byte[ByteConst.sizeBytes + byteMessage.Length];

                Array.Copy(BitConverter.GetBytes(byteMessage.LongLength),
                            0,
                            data,
                            0,
                            ByteConst.sizeBytes);

                Array.Copy(byteMessage,
                            0,
                            data,
                            ByteConst.sizeBytes,
                            byteMessage.Length);

                return await Task.FromResult(data);
            }

            Message mes = new Message()
            {
                MessageType = (int)PackUnpackMessages.Enums.MessageTypes.KeepConnection,
                From = id,
                To = 100,
                Data = new byte[0]
            };
            byte[] message = mes.GetMessage();
            data = new byte[PackUnpackMessages.Enums.ByteConst.sizeBytes + message.Length];

            Array.Copy(BitConverter.GetBytes(message.LongLength),
                        0,
                        data,
                        0,
                        PackUnpackMessages.Enums.ByteConst.sizeBytes);

            Array.Copy(message,
                        0,
                        data,
                        PackUnpackMessages.Enums.ByteConst.sizeBytes,
                        message.Length);

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
