using ElsaMina.Core.Services.Config;

namespace ElsaMina.Console;

public class VersionProvider : IVersionProvider
{
    public string Version => GitVersionInformation.SemVer;
}