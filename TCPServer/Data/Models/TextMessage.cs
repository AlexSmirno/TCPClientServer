namespace TCPServer.Data.Models
{
    public class TextMessage
    {
        public int Id { get; set; }
        public byte UserId { get; set; }
        public int Channel { get; set; }
        public byte[] Text { get; set; }
    }
}
