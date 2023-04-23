using ElsaMina.Core.Models;

namespace ElsaMina.Core.Services.Config;

public interface IConfigurationService
{
    IConfiguration? Configuration { get; }
    Task LoadConfiguration(TextReader textReader);
}