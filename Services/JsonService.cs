using System.Collections.ObjectModel;
using System.Text.Json;
namespace lab3;
public class JsonService
{
    private static readonly JsonSerializerOptions Opts = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };
    public async Task<ObservableCollection<Book>> LoadBooksAsync(Stream stream) =>
        await JsonSerializer.DeserializeAsync<ObservableCollection<Book>>(stream, Opts) ?? new();

    public async Task SaveBooksAsync(Stream stream, IEnumerable<Book> books) =>
        await JsonSerializer.SerializeAsync(stream, books, Opts);
}