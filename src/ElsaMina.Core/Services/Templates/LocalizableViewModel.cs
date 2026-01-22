using System.Globalization;

namespace ElsaMina.Core.Services.Templates;

public class LocalizableViewModel
{
    public required CultureInfo Culture { get; init; }
}