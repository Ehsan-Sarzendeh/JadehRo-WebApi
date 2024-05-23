using JadehRo.Service.Infrastructure;
using JadehRo.Service.SendingMsgModule.Implementation.SMSPanelService;

namespace JadehRo.Service.OptionServices.Dto;

public class OptionDto : BaseDto<OptionDto , Option>
{
    #region Messaging Option

    public bool EnableSms { set; get; }
    public bool EnableNotif { set; get; }

    #region SMS Data

    public IppanelSmsOption IppanelSmsOption { get; set; }
    public KavenegarSmsOption KavenegarSmsOption { get; set; }
    public SmsPanelType SmsPanelType { get; set; }
    public string BlokeDefaultSmsPanelDateTime { get; set; }
    public int BlokeDefaultSmsPanelHours { get; set; }

    #endregion

    #endregion
}
public class IppanelSmsOption
{
    public string Apikey { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ServicePath { get; set; }
    public string ServicePatternPath { get; set; }
    public string DataPatternJson { get; set; }
    public string SenderNo { get; set; }
}
public class KavenegarSmsOption
{
    public string Apikey { get; set; }
    public string SenderNo { get; set; }
}