using System.Globalization;

namespace ElsaMina.Core.Templates;

public class LocalizableViewModel
{
    public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
}