using System.Globalization;

namespace ElsaMina.Core.Templates;

public abstract class LocalizableViewModel
{
    public CultureInfo Culture { get; set; }
}