namespace Survey.API.Models;

public class SurveyAnswers
{
    public int survey_id { get; set; }
    public int survey_question_id { get; set; }
    public string response { get; set; }
}
