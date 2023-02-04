namespace Survey.API.Models;

public class Answer
{
    public int? question_id { get; set; }
    public string? name { get; set; }
    public string? title { get; set; }
    public string? required { get; set; }
    public string? response { get; set; }
    public int? order { get; set; }
    public string? created_at { get; set; }
}
