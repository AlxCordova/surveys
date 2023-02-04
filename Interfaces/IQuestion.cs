using Survey.API.Models;

namespace Survey.API.Interfaces;

public interface IQuestion
{
    List<Question> FindById(int survey_id, int id);
    List<Question> FindAllBySurveyId(int survey_id);
    void Create(int survey_id, Question model);
    Response Update(int survey_id, Question model);
    Response Delete(int survey_id, int id);
}
