using System;
namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();

            while (true)
            {
                Server server = new Server();
                try
                {
                    server.setIP("192.168.1.64");
                    server.setHost(8080);
                    server.setAmountListener(3);
                    server.SetServer();

                    server.startListeners();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Program. str 24");
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    server.ServerClose();
                }
            }
        }
    }
}