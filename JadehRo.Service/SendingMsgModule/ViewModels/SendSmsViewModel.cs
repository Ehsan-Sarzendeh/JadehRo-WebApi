namespace JadehRo.Service.SendingMsgModule.ViewModels
{
    public class SendSmsViewModel
    {
        public string op { get; set; }
        public string uname { get; set; }
        public string pass { get; set; }
        public string message { get; set; }
        public string from { get; set; }
        public List<string> to { get; set; }
    }
    public class SendSmsPatternViewModel
    {
        public string pattern_code { get; set; }
    
        public string originator { get; set; }
        public string recipient { get; set; }

        public Dictionary<string,string> values { get; set; }
    }
}
