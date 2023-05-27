namespace ElsaMina.DataAccess.Models;

public interface IKeyed<out T>
{
    T Key { get; }
}