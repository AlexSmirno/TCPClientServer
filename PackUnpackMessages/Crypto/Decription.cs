using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackUnpackMessages.Crypto
{
    public class Decription
    {
        public async Task<byte[]> DecriptionMessage(IEnumerable<byte> message)
        {
            return message.ToArray();
        }
    }
}
