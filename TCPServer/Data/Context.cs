using System.Collections.Generic;
using TCPServer.Data.Models;

namespace TCPServer.Data
{
    public static class Context
    {
        public static List<TextMessage> TextMessages = new List<TextMessage>();
        public static List<FileMessage> FileMessages = new List<FileMessage>();
        public static List<KeyValuePair<int, int>> UsersInchannel = new List<KeyValuePair<int, int>>(); //Pair (user, channel)
    }
}
