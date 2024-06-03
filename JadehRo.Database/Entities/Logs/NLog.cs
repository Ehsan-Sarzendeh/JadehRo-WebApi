using JadehRo.Database.Entities.Base;

namespace JadehRo.Database.Entities.Logs;

public class NLog : BaseEntity
{
    public string MachineName { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserAgent { get; set; }
    public DateTime Logged { get; set; }
    public string Method { get; set; }
    public string RequestUrl { get; set; }
    public string RequestQuerystring { get; set; }
    public string RequestBody { get; set; }
    public string Level { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string Logger { get; set; }
    public string Properties { get; set; }
    public string Exception { get; set; }
    public string StackTrace { get; set; }
}