namespace ElsaMina.Core.Client;

public interface IClient
{
    void Connect();
    void Send(string message);
}