using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackUnpackMessages
{
    public class MyTextEncoder
    {
        private Encoding encoding;

        public MyTextEncoder()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(1251);
        }

        public byte[] TextToBytes(string text)
        {
            return encoding.GetBytes(text);
        }

        public string BytesToText(byte[] bytes)
        {
            return encoding.GetString(bytes);
        }
    }
}
