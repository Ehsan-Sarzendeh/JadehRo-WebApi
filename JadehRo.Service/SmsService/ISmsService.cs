using JadehRo.Common.Utilities;
using JadehRo.Service.SmsService.Dtos;

namespace JadehRo.Service.SmsService;

public interface ISmsService : IScopedDependency
{
    Task<SendingSmsResponse> SendSms(SendingSmsRequest request);
    void SendSmsInBackground(SendingSmsRequest request);
}