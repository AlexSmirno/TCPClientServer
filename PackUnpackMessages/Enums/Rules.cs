namespace PackUnpackMessages.Enums
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
