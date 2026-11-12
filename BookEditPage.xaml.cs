namespace lab3;

public partial class BookEditPage : ContentPage
{
    private readonly Book _book;
    private readonly Action<Book> _onSave;

    public BookEditPage(Book book, Action<Book> onSave)
    {
        InitializeComponent();
        _book = book;
        _onSave = onSave;
        TitleEntry.Text = book.Title;
        AuthorEntry.Text = book.Author;
        GenreEntry.Text = book.Genre;
        YearEntry.Text = book.Year.ToString();
        PagesEntry.Text = book.Pages.ToString();
        PriceEntry.Text = book.Price.ToString();
    }

    private async void OnOk(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text) ||
            !int.TryParse(YearEntry.Text, out int year) || year < 1 || year > DateTime.Now.Year ||
            !int.TryParse(PagesEntry.Text, out int pages) || pages < 1 ||
            !decimal.TryParse(PriceEntry.Text, out decimal price) || price < 0)
        {
            await DisplayAlert("Error", "Check name, year, pages and price", "OK");
            return;
        }

        _book.Title = TitleEntry.Text.Trim();
        _book.Author = AuthorEntry.Text?.Trim() ?? "";
        _book.Genre = GenreEntry.Text?.Trim() ?? "";
        _book.Year = year;
        _book.Pages = pages;
        _book.Price = price;

        _onSave(_book);
        await Navigation.PopModalAsync();
    }

    private async void OnCancel(object? sender, EventArgs e) => await Navigation.PopModalAsync();
}