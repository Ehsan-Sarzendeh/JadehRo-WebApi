using JadehRo.Service.OptionServices.Dto;
using JadehRo.Service.SmsService.Dtos;

namespace JadehRo.Service.SmsService.Panels;

public interface ISmsPanelService
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