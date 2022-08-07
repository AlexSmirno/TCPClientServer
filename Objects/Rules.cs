using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects
{
    public enum Rules
    {
        //Обращения к серверу (включающая общение между клиентами)
        ReadOnly = 0,
        ReadGetSendChat = 1,
        ReadGetChat = 2,
        //Только пересылка между клиентами
        ChatOnly = 3
    }
}
