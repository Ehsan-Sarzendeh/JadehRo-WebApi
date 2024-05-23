namespace JadehRo.Service.SendingMsgModule.Messaging
{
    public class SendMsgRequest
    {
        public List<string> To { get; set; }
        public string PatternId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public  List<string>  PatternParams { get; set; }
        public  List<string>  PatternValues { get; set; }
    }
}
