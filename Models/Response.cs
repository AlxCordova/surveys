namespace Survey.API.Models
{
    public class Response
    {
        public Response(object data, string message, int status)
        {
            this.data = data;
            this.message = message;
            this.status = status;
        }

        public object data { get; set; }
        public string message { get; set; }
        public int status { get; set; }
    }
}
