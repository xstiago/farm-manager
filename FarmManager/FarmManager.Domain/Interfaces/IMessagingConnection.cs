namespace FarmManager.Domain.Interfaces
{
    public interface IMessagingConnection
    {
        string HostName { get; }
        string UserName { get; }
        string Password { get; }
        int Port { get; }
    }
}
