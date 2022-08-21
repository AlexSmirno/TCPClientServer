using System.Collections.Generic;
using TCPServer.Data.ServerModels;

namespace TCPServer.Data
{
    public class ServerContext
    {
        public static Dictionary<int, Queue<byte[]>> QueuesMessages = new Dictionary<int, Queue<byte[]>>();
        public static List<Thread> ActiveThreads = new List<Thread>();
    }
}
