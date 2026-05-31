namespace 12lab;

public class Note
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}