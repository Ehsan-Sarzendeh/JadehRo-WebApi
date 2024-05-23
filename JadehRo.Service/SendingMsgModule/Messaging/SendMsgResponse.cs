
namespace JadehRo.Service.SendingMsgModule.Messaging
{
    public class SendMsgResponse:BaseResponse
    {
        public string SmsSendId { get; set; }
    }
    public class SendSmsPatternResponse
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public SendSmsPatternResData Data { get; set; }
    }
    public class SendSmsPatternResData
    {
        public long Bulk_id { get; set; }
    
    }
}
