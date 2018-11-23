namespace Simple_IoC_Container.Services.Interfaces
{
    public interface IMessageService
    {
        void SendMessage(string message);
        int GetSentCount();
    }
}