using System;

namespace TCPClient
{
    public class Message
    {
        public int From { get; set; }
        public int To { get; set; }
        public int MessageType { get; set; }
        public byte[] Data { get; set; }
    }
}
