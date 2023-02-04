using Survey.API.Models;

namespace Survey.API.Interfaces;

public interface IAuth
{
    Response ValidateUser(Login model);
    Response CreateUser(User model);
}
