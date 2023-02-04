using Survey.API.Models;

namespace Survey.API.Interfaces;

public interface ISurvey
{
    Response FindById(int id);
    Response FindByLink(string link);
    Response FindAll();
    Response Create(Models.Survey model);
    Response Update(Models.Survey model);
    Response Delete(int id);
}
