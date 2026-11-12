using System.Collections.ObjectModel;
namespace lab3;

public partial class MainPage : ContentPage
{
    private readonly JsonService _json = new();
    private readonly ObservableCollection<Book> _books = new();

    public MainPage()
    {
        InitializeComponent();
        ShowAll();
    }

    private void ShowAll() => BooksView.ItemsSource = _books.ToList();
    private void Show(IEnumerable<Book> src) => BooksView.ItemsSource = src.ToList();

    private async void OnOpen(object? s, EventArgs e)
    {
        var jsonType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".json" } },
            { DevicePlatform.Android, new[] { "application/json" } },
            { DevicePlatform.iOS, new[] { "public.json" } },
            { DevicePlatform.MacCatalyst, new[] { "public.json" } }
        });
        var file = await FilePicker.Default.PickAsync(
            new PickOptions { PickerTitle = "Choose JSON-file", FileTypes = jsonType });
        if (file == null) return;
        try
        {
            using var stream = await file.OpenReadAsync();
            var loaded = await _json.LoadBooksAsync(stream);
            _books.Clear();
            foreach (var b in loaded) _books.Add(b);
            ShowAll();
            StatusLabel.Text = $"{file.FileName}: {_books.Count} records";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Couldn't read file: " + ex.Message, "OK");
        }
    }

    private async void OnSave(object? s, EventArgs e)
    {
        string? path = null;

#if WINDOWS
        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedFileName = "books"
        };
        picker.FileTypeChoices.Add("JSON", new List<string> { ".json" });
        var hwnd = ((MauiWinUIWindow)Application.Current!.Windows[0].Handler!.PlatformView!).WindowHandle;
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        var f = await picker.PickSaveFileAsync();
        path = f?.Path;
#else
        path = Path.Combine(FileSystem.AppDataDirectory, "books.json");
#endif

        if (path == null)
        {
            StatusLabel.Text = "Cancelled save";
            return;
        }
        try
        {
            using var fs = File.Create(path);
            await _json.SaveBooksAsync(fs, _books);
            StatusLabel.Text = "Saved: " + path;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Couldn't save: " + ex.Message, "OK");
        }
    }

    private async void OnAdd(object? s, EventArgs e)
    {
        int id = _books.Count == 0 ? 1 : _books.Max(b => b.Id) + 1;
        await Navigation.PushModalAsync(new BookEditPage(
            new Book { Id = id, Year = DateTime.Now.Year },
            result => { _books.Add(result); ShowAll(); }));
    }

    private async void OnEdit(object? s, EventArgs e)
    {
        if ((s as Button)?.BindingContext is not Book book) return;
        int index = _books.IndexOf(book);
        await Navigation.PushModalAsync(new BookEditPage(
            book.Clone(),
            result => { _books[index] = result; ShowAll(); }));
    }

    private async void OnDelete(object? s, EventArgs e)
    {
        if ((s as Button)?.BindingContext is not Book book) return;
        if (await DisplayAlert("Deleting", $"Delete «{book.Title}»?", "Yes", "No"))
        {
            _books.Remove(book);
            ShowAll();
        }
    }

    private async void OnAbout(object? s, EventArgs e) =>
        await Navigation.PushModalAsync(new AboutPage());
// LINQ search
    private IEnumerable<Book> SearchByTitleOrAuthor(string query) =>
        _books.Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                          b.Author.Contains(query, StringComparison.OrdinalIgnoreCase));
    private IEnumerable<Book> FilterByGenre(string genre) =>
        _books.Where(b => b.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase));
    private IEnumerable<Book> FilterByYear(int year) =>
        _books.Where(b => b.Year == year);

    private void OnSearchText(object? s, EventArgs e) =>
        Show(SearchByTitleOrAuthor(SearchEntry.Text ?? ""));
    private void OnSearchGenre(object? s, EventArgs e) =>
        Show(FilterByGenre(GenreEntry.Text ?? ""));
    private async void OnSearchYear(object? s, EventArgs e)
    {
        if (!int.TryParse(YearEntry.Text, out int year))
        {
            await DisplayAlert("Error", "Enter a valid year, e.g. 2023", "OK");
            return;
        }
        Show(FilterByYear(year));
    }

    private void OnReset(object? s, EventArgs e)
    {
        SearchEntry.Text = GenreEntry.Text = YearEntry.Text = "";
        ShowAll();
    }
}