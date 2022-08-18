namespace TCPServer.Data.Models
{
    public class FileMessage
    {
        private string basePath = @"D:\Projects\Files";

        public int Id { get; set; }
        public byte From { get; set; }

        private string filename;
        public string FileName
        {
            get { return basePath + "/" + filename; }
            set { filename = value; }
        }
    }
}
