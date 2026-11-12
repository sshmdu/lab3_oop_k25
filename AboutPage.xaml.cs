namespace lab3;

public partial class AboutPage : ContentPage
{
    public AboutPage() => InitializeComponent();
    private async void OnClose(object? s, EventArgs e) => await Navigation.PopModalAsync();
}