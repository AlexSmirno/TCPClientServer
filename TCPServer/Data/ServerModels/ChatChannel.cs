using System;
using System.Collections.Generic;
using System.Linq;
using TCPServer.Data.Models;

namespace TCPServer.Data.ServerModels
{
    public class ChatChannel
    {
        public int Id { get; set; }
        public List<User> Users { get; set; }
        public List<TextMessage> MessageList { get; set; }
    }
}
