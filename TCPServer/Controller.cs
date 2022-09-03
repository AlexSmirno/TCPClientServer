using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using System;

using PackUnpackMessages.Enums;
using PackUnpackMessages;
using TCPServer.Data;

namespace TCPServer
{
    public class Controller
    {
        private PackingMessages packMessage;
        private const int serverId = 100;
        private int Thread;
        private Logger.Logger logger;
        public Controller(int thread)
        {
            logger = new Logger.Logger(this.GetType().FullName);
            packMessage = new PackingMessages();
            Thread = thread;
        }

        public async Task ProcessMessage(byte[] message)
        {
            Message newMessage = new Message(message);
            logger.InfoReport("Сообщение от " + newMessage.From);

            await AddUser(newMessage.From);

            await Checking(newMessage);
        }

        private async Task Checking(Message message)
        {
            Errors error = new Errors();

            if (message.MessageType != (int)MessageTypes.Error)
            {
                /*if (!CheckHash(message.Data))
                {
                    error = Errors.WrongHash;
                }*/

                if (message.To != serverId && ServerContext.ActiveThreads.Where(thread => 
                thread.User.Id == message.To).Count() == 0)
                {
                    error = Errors.NotFoundClient;
                    logger.ErrorReport("Wrong reciever from client " + message.From);
                }

                if (error > 0)
                {
                    await AddMessageToQueue(await packMessage.packErrorMessage(message.From, error), message.From);
                }
            }

            await Routing(message);
        }

        private async Task Routing(Message message)
        {
            MessageTypes messageType = (MessageTypes)message.MessageType;

            logger.InfoReport("Route message with type = " + messageType);

            switch (messageType)
            {
                case MessageTypes.Undefinded:
                    break;
                case MessageTypes.SendText:
                    await SaveMessage(message);
                    await AddMessageToQueue(await packMessage.packUsualMessage(message), message.To);
                    break;
                case MessageTypes.SendFiles:
                    await SaveMessage(message);
                    break;
                case MessageTypes.GetFilesList:
                    break;
                case MessageTypes.GetFiles:
                    break;
                case MessageTypes.GetChatHistory:
                    await AddMessageToQueue(await packMessage.packUsualMessage(await TakeData(message)), message.From);
                    break;
                case MessageTypes.Error:
                    break;
                case MessageTypes.KeepConnection:
                    break;
                default:
                    break;
            }
        }

        private async Task SaveMessage(Message message)
        {
            SaveService saveService = new SaveService();
            if (message.MessageType == (int)MessageTypes.SendText)
            {
                await saveService.SaveText(message);
            }
            else if (message.MessageType == (int)MessageTypes.SendFiles)
            {
                await saveService.SaveFile();
            }
        }
        
        private async Task<Message> TakeData(Message message)
        {
            TakeService takeService = new TakeService();

            byte[] data = default;
            switch ((MessageTypes)message.MessageType)
            {
                case MessageTypes.GetFilesList:
                    data = await takeService.TakeFileList();
                    break;
                case MessageTypes.GetFiles:
                    data = await takeService.TakeFile(message.Data);
                    break;
                case MessageTypes.GetChatHistory:
                    data = await takeService.TakeChatHistory(message.Data);
                    break;
                default:
                    break;
            }

            Message rewriteMessage = new Message()
            {
                From = message.To,
                To = message.From,
                MessageType = message.MessageType,
                Data = data
            };

            return rewriteMessage;
        }

        public async Task<Errors> SaveFile()
        {
            Errors error = Errors.NoError;
            return error;
        }

        //TODO: smth with big files
        public async void RedirectionMessage(Message mes)
        {
            await AddMessageToQueue(mes.GetMessage(), mes.To);
        }

        public async void PollingMessage(int to)
        {
            Message polling = new Message
            {
                From = serverId,
                To = (byte)to,
                MessageType = (int)MessageTypes.KeepConnection,
                Data = new byte[0]
            };
            await AddMessageToQueue(await packMessage.packUsualMessage(polling), polling.To);
        }

        private async Task AddUser(int id)
        {
            if (ServerContext.ActiveThreads.Where(thread => thread.User.Id == id).Any() == false)
            {
                ServerContext.ActiveThreads.Find(thread => thread.Id == Thread).User.Id = id;
                ServerContext.ActiveThreads.Find(thread => thread.Id == Thread).User.Username = id + "";
                logger.InfoReport("Сокет #" + Thread + "Новый пользователь с id = " + id);
            }
        }

        private async Task AddMessageToQueue(byte[] message, int Id)
        {
            ServerContext.QueuesMessages[ServerContext.ActiveThreads.Where(thread => thread.User.Id == Id).FirstOrDefault().Id].Enqueue(message);
        }

        private bool CheckHash(byte[] message)
        {
            byte[] hash = new byte[ByteConst.hashBytes];
            Array.Copy(message,
                       0,
                       hash,
                       0,
                       ByteConst.hashBytes);
            return Enumerable.SequenceEqual(hash, GetHashMD5(message));
        }

        private byte[] GetHashMD5(byte[] source) //128 bits = 16bytes
        {
            byte[] data;
            using (MD5 md5Hasher = MD5.Create())
            {
                data = md5Hasher.ComputeHash(source);
            }
            return data;
        }
    }
}
