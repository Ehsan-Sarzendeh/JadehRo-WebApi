using JadehRo.Database.Entities.Common;
using JadehRo.Service.Infrastructure;
using JadehRo.Service.SmsService.Panels;

namespace JadehRo.Service.OptionServices.Dto;

public class OptionDto : BaseDto<OptionDto , Option>
{
    #region Messaging Option

    public bool EnableSms { set; get; }

    public IppanelSmsOption IppanelSmsOption { get; set; }
    public KavenegarSmsOption KavenegarSmsOption { get; set; }
    public SmsPanelType SmsPanelType { get; set; }

    #endregion
}

public class IppanelSmsOption
{
    public string Apikey { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ApiPath { get; set; }
    public string SenderNo { get; set; }
}

public class KavenegarSmsOption
{
    public string Apikey { get; set; }
    public string SenderNo { get; set; }
}