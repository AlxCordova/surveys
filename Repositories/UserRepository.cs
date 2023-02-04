using Survey.API.Interfaces;
using Survey.API.Models;
using Survey.API.SQL;
using Survey.API.Utilities;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Survey.API.Repositories
{
    public class UserRepository : IAuth
    {
        private readonly IConfiguration _config;
        private readonly JwtRepository _jwt;
        private readonly Settings _settings;

        public UserRepository(JwtRepository jwt, IConfiguration config)
        {
            _jwt = jwt;
            _config = config;
            _settings = _config.GetSection("Settings").Get<Settings>();
        }
        public Response CreateUser(User model)
        {
            Response response = new Response(null, "Parámetros para crear usuario no válidos.", 500);
            try
            {
                if (string.IsNullOrEmpty(model.email))
                    return response;

                if(!InsertUser(model))
                    return new Response(null, "No se puede crear el usuario en este momento, intente más tarde", 400);

                model.api_token = _jwt.SetAuthorizationToken(model.first_name);
                
                response.data = model;
                response.status = 200;
                response.message = "Usuario creado con éxito";
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        public Response ValidateUser(Login model)
        {
            Response response = new Response(null, "Usuario o contraseña incorrectos", 400);
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    var users = Queries.ExecuteQuery<User>(conn, _settings.Procedures.Users, CommandType.StoredProcedure, new { action = "R", model.email});
                    if (users.Count.Equals(0))
                        return response;

                    var user = CheckUserCredentials(users[0], model.password);
                    if(string.IsNullOrEmpty(user.api_token))
                        return response;

                    response.data = user;
                    response.status = 200;
                    response.message = "Login correcto";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
                return new Response(null, "No se puede procesar la petición en este momento, intente más tarde", 400);
            }
            return response;
        }

        private User CheckUserCredentials(User user, string password)
        {
            if (Utils.CompareHash(user.password, password))
                user.api_token = _jwt.SetAuthorizationToken(user.email);

            user.password = null;
            return user;
        }
        private bool InsertUser(User user)
        {
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    conn.Open();
                    var cmd = new SqlCommand(_settings.Procedures.Users, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "C");
                    cmd.Parameters.AddWithValue("@first_name", user.first_name);
                    cmd.Parameters.AddWithValue("@last_name", user.last_name);
                    cmd.Parameters.AddWithValue("@email", user.email);
                    cmd.Parameters.AddWithValue("@password", Utils.ComputeHash(user.password, Utils.GenerateSalt()));
                    var success = cmd.ExecuteNonQuery();
                    return success > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
                return false;
            }
        }
    }
}
