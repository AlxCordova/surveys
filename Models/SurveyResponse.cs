namespace Survey.API.Models
{
    public class SurveyResponse
    {
        public int? survey_id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public List<Answer>? answers { get; set; }
    }
}
