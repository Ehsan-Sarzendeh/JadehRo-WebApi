namespace JadehRo.Service.SmsService.Panels.IppanelPanel.Models;

public class IppanelSendSmsRequest
{
    public string code { get; set; }
    public string sender { get; set; }
    public string recipient { get; set; }
    public Dictionary<string, string> variable { get; set; }
}