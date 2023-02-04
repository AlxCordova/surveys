namespace Survey.API.Models;

public class Question
{
    public int? id { get; set; }
    public string? name { get; set; }
    public string? title { get; set; }
    public string? required { get; set; }
    public int? order { get; set; }
    public int? input_type_id { get; set; }
    public string? input_type { get; set; }
}
