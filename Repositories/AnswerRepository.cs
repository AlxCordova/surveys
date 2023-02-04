using Survey.API.Interfaces;
using Survey.API.Models;
using Survey.API.SQL;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Survey.API.Repositories
{
    public class AnswerRepository : IAnswer
    {
        private readonly IConfiguration _config;
        private readonly Settings _settings;

        public AnswerRepository(IConfiguration config)
        {
            _config = config;
            _settings = _config.GetSection("Settings").Get<Settings>();
        }
        public Response GetResponsesBySurveyId(int survey_id)
        {
            Response response = new Response(null, "Ocurrio un error en la consulta, intente más tarde.", 500);
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    var result = Queries.ExecuteQuery<SurveyResponse>(conn, _settings.Procedures.Surveys, CommandType.StoredProcedure, new { action = "R", id = survey_id });
                    if (result.Count.Equals(0))
                        return new Response(null, "Encuesta no encontrada con el id especificado", 404);
                    
                    SurveyResponse survey = result.FirstOrDefault();
                    survey.answers = GetAnswers(survey_id);

                    response.data = survey.answers.Count > 0 ? survey : null;
                    response.status = response.data is null ? 400 : 200;
                    response.message = response.data is null ? "No hay respuestas con el id de encuesta especificado" : "Respuestas obtenida con éxito";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        public Response SaveResponse(List<SurveyAnswers> answers)
        {
            Response response = new Response(null, "No se puede procesar la petición actual, intente más tarde.", 500);
            try
            {
                if(answers == null || answers.Count.Equals(0))
                    return response;

                answers.ForEach(answer => SaveAnswer(answer));

                response.status = 200;
                response.message = "Respuestas almacenadas con éxito";
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return response;
        }

        private List<Answer> GetAnswers(int survey_id)
        {
            List<Answer> answers = new();
            try
            {
                using (var conn = new SqlConnection(_settings.DefaultConnection))
                {
                    answers = Queries.ExecuteQuery<Answer>(conn, _settings.Procedures.Responses, CommandType.StoredProcedure, new { action = "R", survey_id });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            }
            return answers;
        }

        private void SaveAnswer(SurveyAnswers answer)
        {
            using (var conn = new SqlConnection(_settings.DefaultConnection))
            {
                conn.Open();
                var cmd = new SqlCommand(_settings.Procedures.Responses, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "C");
                cmd.Parameters.AddWithValue("@survey_id", answer.survey_id);
                cmd.Parameters.AddWithValue("@survey_question_id", answer.survey_question_id);
                cmd.Parameters.AddWithValue("@response", answer.response);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
