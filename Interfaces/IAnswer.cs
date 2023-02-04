using Survey.API.Models;

namespace Survey.API.Interfaces;

public interface IAnswer
{
    Response SaveResponse(List<SurveyAnswers> answers);
    Response GetResponsesBySurveyId(int survey_id);
}