namespace JadehRo.Service.SendingMsgModule.Messaging
{
    public class SendingEmailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> To { get; set; }
    }
}
