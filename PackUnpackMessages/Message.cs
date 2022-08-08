namespace PackUnpackMessages
{
    public class Message
    {
        public byte From { get; set; }
        public byte To { get; set; }
        public int MessageType { get; set; }
        public byte[] Data { get; set; }
    }
}
