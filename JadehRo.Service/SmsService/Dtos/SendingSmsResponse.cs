using Newtonsoft.Json;

namespace JadehRo.Service.SmsService.Dtos;

public class SendingSmsResponse : BaseResponse
{
    public string SmsSendId { get; set; }
}

public class BaseResponse
{
    public IList<string> Message { get; set; }
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public int LogRefNumber { get; set; }
    public bool IsFound { get; set; }
    public ResponseResult Result { get; set; }
    public string MessageString
    {
        get
        {
            return Message.Aggregate("", (current, msg) => current + msg + "\n\r");
        }
    }
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    public BaseResponse()
    {
        Message = new List<string>();
        Result = ResponseResult.Success;
        IsSuccess = true;
    }
}

public enum ResponseResult
{
    Error,
    Information,
    Warning,
    Success,
    ValidationFeiled,
    Question
}