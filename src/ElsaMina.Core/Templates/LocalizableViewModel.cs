using System.Globalization;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Core.Templates;

public class LocalizableViewModel
{
    public CultureInfo Culture { get; init; }
}