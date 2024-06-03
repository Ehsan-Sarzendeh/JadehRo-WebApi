namespace JadehRo.Service.SmsService.Dtos;

public class SendingSmsRequest
{
    public string PatternId { get; set; }
    public string Body { get; set; }
    public List<string> To { get; set; }
    public List<string> PatternParams { get; set; }
    public List<string> PatternValues { get; set; }
}