namespace BookTracker.Api.Application.Booklist;

public class BookInfo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
}
