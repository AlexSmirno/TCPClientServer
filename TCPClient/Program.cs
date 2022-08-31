using System;
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

                byte id;

                while (!byte.TryParse(Console.ReadLine(), out id))
                {
                    Console.WriteLine("Попробуйте еще раз, блять!");
                }

                StartClient(id);
            }
        }

        private static async void StartClient(byte id)
        {
            Client client = new Client();

            try
            {
                client.SetServerConfiguration("192.168.1.65", 8080);

                client.Id = id;

                await client.ConnectToServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Sender sender = client.GetSender();

            while (true)
            {
                try
                {
                    string responce = await sender.SendText(client.Id, "Привет");

                    
                    Console.WriteLine(responce);

                    Task.Delay(1000).Wait();

                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                    Console.ReadKey();
                }
            }
        }


    }
}
