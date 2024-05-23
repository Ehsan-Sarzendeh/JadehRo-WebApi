namespace JadehRo.Service.SendingMsgModule.Messaging
{
    public class SendingNotifRequest
    {
        public string AppIdes { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> DeviceIds { get; set; }

    }
}
