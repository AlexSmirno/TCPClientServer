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
                try
                {
                    int amount = 2;

                    Client client1 = new Client(101);
                    Client client2 = new Client(102);

                    Task[] clients = new Task[2];
                    clients[0] = new Task(() => client1.ClientStart());
                    clients[1] = new Task(() => client2.ClientStart());

                    Task task1 = new Task(() => client1.addMessage("Привет, Мир!", 102));
                    task1.Start();

                    Task task2 = new Task(() => client1.addMessage("Пока, Мир!", 101));
                    task2.Start();

                    for (int i = 0; i < amount; i++)
                    {
                        clients[i].Start();
                    }

                    Console.ReadKey();

                    for (int i = 0; i < amount; i++)
                    {
                        clients[i].Dispose();
                    }

                    /*Task[] clients = new Task[amount];
                    for (int i = 0; i < amount; i++)
                    {
                        byte a = (byte)(i + 101);

                        Client client = new Client(a);
                        clients[i] = new Task(() => client.ClientStart());

                        new Task(() => client.addMessage("Привет мир!", a)).Start();
                    }

                    for (int i = 0; i < amount; i++)
                    {
                        clients[i].Start();
                    }

                    Console.ReadKey();

                    for (int i = 0; i < amount; i++)
                    {
                        clients[i].Dispose();
                    }*/
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
