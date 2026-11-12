namespace lab3;
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Genre { get; set; } = "";
    public int Year { get; set; }
    public decimal Price { get; set; }
    public int Pages { get; set; }
    public Book Clone() => (Book)MemberwiseClone();
}