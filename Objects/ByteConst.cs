using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects
{
    public class ByteConst
    {        
        //Константы показывающие кол-во байт выделенных под дополнительную информацию в сообщении
        public const int routeBytes = 2;
        public const int messageTypeBytes = 4;
        public const int errorBytes = 4;
        public const int emptyBytes = 0;
        public const int sizeBytes = 8;
        public const int hashBytes = 16;
        public const int bufferSize = 104857600; //100 Мб
        public const int maxMessageLength = 32768; //2^15
    }
}
