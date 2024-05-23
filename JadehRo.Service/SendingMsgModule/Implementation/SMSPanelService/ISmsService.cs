using JadehRo.Service.OptionServices.Dto;
using JadehRo.Service.SendingMsgModule.Messaging;

namespace JadehRo.Service.SendingMsgModule.Implementation.SMSPanelService
{
    public interface ISmsService
    {
        OptionDto Option { get; set; }
        Task<SendingSmsResponse> Send(SendingSmsRequest request);
    }
    public enum SmsPanelType
    {
        Auto = 0,
        Ippanel = 1,
        Kavenegar = 2
    }
}