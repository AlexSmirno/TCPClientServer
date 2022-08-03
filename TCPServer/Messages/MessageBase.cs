using System.Security.Cryptography;

namespace TCPServer.Messages
{
    public abstract class MessageBase
    {
        abstract public void ConvertToBytes(string message);
        abstract public bool ConvertToMessage(byte[] data);

        private protected byte[] GetHashMD5(byte[] source) //128 bits = 16bytes
        {
            byte[] data;
            //StringBuilder hash = new StringBuilder();
            using (MD5 md5Hasher = MD5.Create())
            {
                data = md5Hasher.ComputeHash(source);
                
                /*for (int index = 0; index < data.Length; index++)
                {
                    hash.Append(data[index].ToString("x2"));
                }*/
            }
            return data;
        }

        private protected bool equalHash(byte[] first, byte[] second)
        {
            return System.Linq.Enumerable.SequenceEqual(first, second);
        }
    }
}
