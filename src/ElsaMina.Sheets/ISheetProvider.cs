namespace ElsaMina.Sheets;

public interface ISheetProvider : IDisposable
{
    Task<ISheet> GetSheetAsync(string spreadsheetName, string sheetName, CancellationToken cancellationToken = default);
}