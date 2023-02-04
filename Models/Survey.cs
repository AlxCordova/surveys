namespace Survey.API.Models;

public class Survey
{
    public int? id { get; set; }
    public string? name { get; set; }
    public string? description { get; set; }
    public string? link { get; set; }
    public List<Question> questions { get; set; }
}
