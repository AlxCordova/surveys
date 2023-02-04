using Survey.API.Models;
using Survey.API.SQL;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Survey.API.Repositories;

public class QuestionRepository
{
    private readonly IConfiguration _config;
    private readonly Settings _settings;

    public QuestionRepository(IConfiguration config)
    {
        _config = config;
        _settings = _config.GetSection("Settings").Get<Settings>();
    }

    public void Create(int? survey_id, Question model)
    {
        using (var conn = new SqlConnection(_settings.DefaultConnection))
        {
            conn.Open();
            var cmd = new SqlCommand(_settings.Procedures.Questions, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "C");
            cmd.Parameters.AddWithValue("@survey_id", survey_id);
            cmd.Parameters.AddWithValue("@name", model.name);
            cmd.Parameters.AddWithValue("@title", model.title);
            cmd.Parameters.AddWithValue("@required", model.required);
            cmd.Parameters.AddWithValue("@order", model.order);
            cmd.Parameters.AddWithValue("@input_type_id", model.input_type_id);
            cmd.ExecuteNonQuery();
        }
    }

    public int Delete(int survey_id, int id)
    {
        try
        {
            using (var conn = new SqlConnection(_settings.DefaultConnection))
            {
                return Queries.ExecuteNonQuery(conn, _settings.Procedures.Questions, CommandType.StoredProcedure, new { action = "D", survey_id, id });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
            return 0;
        }
    }

    public List<Question> FindAllBySurveyId(int? survey_id)
    {
        List<Question> questions = new();
        try
        {
            using (var conn = new SqlConnection(_settings.DefaultConnection))
            {
                questions = Queries.ExecuteQuery<Question>(conn, _settings.Procedures.Questions, CommandType.StoredProcedure, new { action = "R", survey_id });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
        }
        return questions;
    }

    public List<Question> FindById(int survey_id, int id)
    {
        List<Question> questions = new();
        try
        {
            using (var conn = new SqlConnection(_settings.DefaultConnection))
            {
                questions = Queries.ExecuteQuery<Question>(conn, _settings.Procedures.Questions, CommandType.StoredProcedure, new { action = "R", survey_id, id });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()?.Name} . Exception: {e.Message}");
        }
        return questions;
    }

    public void Update(int? survey_id, Question model)
    {
        using (var conn = new SqlConnection(_settings.DefaultConnection))
        {
            conn.Open();
            var cmd = new SqlCommand(_settings.Procedures.Questions, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "U");
            cmd.Parameters.AddWithValue("@id", model.id);
            cmd.Parameters.AddWithValue("@survey_id", survey_id);
            cmd.Parameters.AddWithValue("@name", model.name);
            cmd.Parameters.AddWithValue("@title", model.title);
            cmd.Parameters.AddWithValue("@required", model.required);
            cmd.Parameters.AddWithValue("@order", model.order);
            cmd.Parameters.AddWithValue("@input_type_id", model.input_type_id);
            cmd.ExecuteNonQuery();
        }
    }
}
