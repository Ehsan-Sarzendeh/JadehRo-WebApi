namespace JadehRo.Service.SmsService.Panels.IppanelPanel.Models;

public class IppanelSendSmsResponse
{
    public string status { get; set; }
    public int code { get; set; }
    public string error_message { get; set; }
    public IppanelSendSmsResponseDate data { get; set; }
}

public class IppanelSendSmsResponseDate
{
    public int message_id { get; set; }
}