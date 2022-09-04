using System;
using System.Threading.Tasks;


namespace TCPClient
{
    class Program
    {

        static byte SecondClient;
        static Sender sender;

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    byte id;

                    while (!byte.TryParse(Console.ReadLine(), out id))
                    {
                        Console.WriteLine("Попробуй еще раз, блять!");
                    }

                    SecondClient = byte.Parse(Console.ReadLine());

                    StartClient(id);
                }
                catch (Exception)
                {
                    Console.WriteLine("End of session");
                }
            }
        }

        private static async void StartClient(byte id)
        {
            Client client = new Client(id);

            try
            {
                client.SetServerConfiguration("192.168.1.64", 8080);


                await client.ConnectToServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            sender = client.GetSender();

            new Task(MessageLoop).Start();

            while (true)
            {
                try
                {
                    await sender.SendText(SecondClient, Console.ReadLine());

                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                    Console.ReadKey();
                }
            }
        }

        private static async void MessageLoop()
        {
            while (true)
            {
                string responce = await sender.GetMessageFromServer();

                if (responce != null)
                {
                    Console.WriteLine("Server responce: " + responce);
                }

                Task.Delay(400).Wait();
            }
        }
    }
}
