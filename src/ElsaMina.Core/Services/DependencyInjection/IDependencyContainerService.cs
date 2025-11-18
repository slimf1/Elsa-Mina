using Autofac;

namespace ElsaMina.Core.Services.DependencyInjection;

public interface IDependencyContainerService
{
    void SetContainer(IContainer container);
    T Resolve<T>() where T : notnull;
    T ResolveNamed<T>(string name);
    bool IsRegisteredWithName<T>(string name);
    IEnumerable<T> GetAllNamedRegistrations<T>() where T : class;
}
