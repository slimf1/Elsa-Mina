using System.Globalization;

namespace ElsaMina.Core.Templates;

public class LocalizableViewModel
{
    public required CultureInfo Culture { get; init; }
}