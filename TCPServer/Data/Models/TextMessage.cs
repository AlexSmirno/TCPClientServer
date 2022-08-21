namespace TCPServer.Data.Models
{
    public class TextMessage
    {
        public int Id { get; set; }
        public byte Sender { get; set; }
        public byte Reciver { get; set; }
        public byte[] Text { get; set; }
    }
}
