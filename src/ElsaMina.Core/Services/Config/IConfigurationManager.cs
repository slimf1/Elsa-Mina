using ElsaMina.Core.Models;

namespace ElsaMina.Core.Services.Config;

public interface IConfigurationManager
{
    IConfiguration Configuration { get; }

    Task LoadConfigurationAsync(TextReader textReader);
}