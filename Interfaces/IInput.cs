using Survey.API.Models;

namespace Survey.API.Interfaces;

public interface IInput
{
    Response FindById(int id);
    Response FindAll();
    Response Insert(Input model);
}