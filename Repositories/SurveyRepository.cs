using Survey.API.Interfaces;
using Survey.API.Models;
using Survey.API.SQL;
using Survey.API.Utilities;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Survey.API.Repositories
{
    public class SurveyRepository : ISurvey
    {
        private readonly IConfiguration _config;
        private readonly Settings _settings;

        public SurveyRepository(IConfiguration config)
        {
            _config = config;
            _settings = _config.GetSection("Settings").Get<Settings>();
        }

        public Response Create(Models.Survey model)
        {
            Response response = new Response(null, "Ocurrio un error en la consulta, intente más tarde.", 500);
            try
            {
                if (string.IsNullOrEmpty(model.name))
                    return response;

                var survey = InsertSurvey(model);
                response.data = survey;
                response.status = survey.id > 0 ? 200 : 400;
                response.message = survey.id > 0 ? "Encuesta creada con éxito" : "No se pudo crear la encuesta";
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        public Response Delete(int id)
        {
            Response response = new Response(null, "Ocurrio un error en la petición, intente más tarde.", 500);
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    var result = Queries.ExecuteNonQuery(conn, _settings.Procedures.Surveys, CommandType.StoredProcedure, new { action = "D", id });
                    response.status = result > 0 ? 200 :404;
                    response.message = result > 0 ? "Encuesta eliminada con éxito" : "No se encontro la encuesta especificada";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        public Response FindAll()
        {
            Response response = new Response(null, "Ocurrio un error en la consulta, intente más tarde.", 500);
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    var result = Queries.ExecuteQuery<Models.Survey>(conn, _settings.Procedures.Surveys, CommandType.StoredProcedure, new { action = "R" });
                    response.data = GetQuestionsBySurveyId(result);
                    response.status = response.data is null ? 400 : 200;
                    response.message = response.data is null ? "No se encontró el registro especificado" : "Encuestas obtenidas con éxito";
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
                    var result = Queries.ExecuteQuery<Models.Survey>(conn, _settings.Procedures.Surveys, CommandType.StoredProcedure, new { action = "R", id });
                    var questions = new QuestionRepository(_config).FindAllBySurveyId(id);

                    Models.Survey survey = result.FirstOrDefault();
                    survey.questions = questions;

                    response.data = survey;
                    response.status = response.data is null ? 400 : 200;
                    response.message = response.data is null ? "No se encontró el registro especificado" : "Encuesta obtenida con éxito";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        public Response Update(Models.Survey model)
        {
            Response response = new Response(null, "Ocurrio un error en la consulta, intente más tarde.", 500);
            try
            {
                if (model.id == null)
                    return response;

                var success = UpdateSurvey(model);
                response.status = success ? 200 : 400;
                response.message = success ? "Encuesta modificada con éxito" : "No se pudo modificar la encuesta";
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        public Response FindByLink(string link)
        {
            Response response = new Response(null, "Ocurrio un error en la consulta, intente más tarde.", 500);
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    var result = Queries.ExecuteQuery<Models.Survey>(conn, _settings.Procedures.Surveys, CommandType.StoredProcedure, new { action = "L", link });
                    var questions = new QuestionRepository(_config).FindAllBySurveyId(result[0].id);

                    Models.Survey survey = result.FirstOrDefault();
                    survey.questions = questions;

                    response.data = survey;
                    response.status = response.data is null ? 400 : 200;
                    response.message = response.data is null ? "No se encontró el registro especificado" : "Encuesta obtenida con éxito";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        private Models.Survey InsertSurvey(Models.Survey model)
        {
            try
            {
                model.link = Utils.CreateRandomSurveyUrl();
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    conn.Open();
                    var cmd = new SqlCommand(_settings.Procedures.Surveys, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "C");
                    cmd.Parameters.AddWithValue("@name", model.name);
                    cmd.Parameters.AddWithValue("@description", model.description);
                    cmd.Parameters.AddWithValue("@link", model.link);
                    model.id = (int?)(decimal)cmd.ExecuteScalar();
                }
                if (model.id > 0)
                    model.questions.ForEach(question => new QuestionRepository(_config).Create(model.id, question));
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return model;
        }

        private bool UpdateSurvey(Models.Survey model)
        {
            try
            {
                int success;
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    conn.Open();
                    var cmd = new SqlCommand(_settings.Procedures.Surveys, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "U");
                    cmd.Parameters.AddWithValue("@id", model.id);
                    cmd.Parameters.AddWithValue("@name", model.name);
                    cmd.Parameters.AddWithValue("@description", model.description);
                    cmd.Parameters.AddWithValue("@link", model.link);
                    success = cmd.ExecuteNonQuery();
                }
                if (success.Equals(0))
                    return false;

                model.questions.ForEach(question => new QuestionRepository(_config).Update(model.id, question));
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return true;
        }

        private List<Models.Survey>? GetQuestionsBySurveyId(List<Models.Survey> result)
        {
            try
            {
                result.ForEach(survey =>
                {
                    survey.questions = new QuestionRepository(_config).FindAllBySurveyId(survey.id);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return result;
        }
    }
}
