using PackUnpackMessages;
using PackUnpackMessages.Enums;
using System.Threading.Tasks;

namespace TCPServer.Data
{
    public class SaveService
    {
        public async Task<Errors> SaveFile()
        {
            return Errors.NoError;
        }

        public async Task<Errors> SaveText(Message message)
        {
            try
            {
                Context.TextMessages.Add(new Models.TextMessage()
                {
                    Id = Context.TextMessages.Count + 1,
                    Sender = message.From,
                    Reciver = message.To,
                    Text = message.Data
                });
                return Errors.NoError;
            }
            catch (System.Exception exeption)
            {
                System.Console.WriteLine(exeption.Message);
                return Errors.SaveError;
            }
        }

        public async Task<Errors> AddNewUser(Message message)
        {
            try
            {
                return Errors.NoError;
            }
            catch (System.Exception exeption)
            {
                System.Console.WriteLine(exeption.Message);
                return Errors.SaveError;
            }
        }
    }
}
