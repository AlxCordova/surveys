using Dapper;
using Newtonsoft.Json;
using Survey.API.Interfaces;
using Survey.API.Models;
using Survey.API.SQL;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Survey.API.Repositories
{
    public class InputRepository : IInput
    {
        private readonly IConfiguration _config;
        private readonly Settings _settings;

        public InputRepository(IConfiguration config)
        {
            _config = config;
            _settings = _config.GetSection("Settings").Get<Settings>();
        }

        public Response FindAll()
        {
            Response response = new Response(null, "Ocurrio un error en la consulta, intente más tarde.", 500);
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    var result = Queries.ExecuteQuery(conn, _settings.Procedures.Inputs, CommandType.StoredProcedure, new { action = "R" });
                    response.data = result;
                    response.status = response.data is null ? 400 : 200;
                    response.message = response.data is null ? response.message : "Inputs obtenidos con éxito";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        public Response FindById(int id)
        {
            Response response = new Response(null, "Ocurrio un error en la consulta, intente más tarde.", 500);
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    var result = Queries.ExecuteQuery(conn, _settings.Procedures.Inputs, CommandType.StoredProcedure, new { action = "R", id });
                    response.data = result.Count > 0 ? result : null;
                    response.status = response.data is null ? 400 : 200;
                    response.message = response.data is null ? "No se encontro el registro especificado" : "Finalizado con éxito";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        public Response Insert(Input model)
        {
            Response response = new Response(null, "Ocurrio un error en la consulta, intente más tarde.", 500);
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    var success = Queries.ExecuteQuery<int>(conn, _settings.Procedures.Inputs, CommandType.StoredProcedure, new { action = "C", name = model.name });
                    if (success.Count > 0)
                        model.id = success[0];

                    response.data = model.id > 0 ? model : null;
                    response.status = response.data is null ? 400 : 200;
                    response.message = response.data is null ? "No creo el registro especificado" : "Input creado con éxito";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }
    }
}
