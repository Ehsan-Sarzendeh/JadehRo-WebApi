using JadehRo.Common.Exceptions;
using JadehRo.Service.OptionServices;
using JadehRo.Service.OptionServices.Dto;
using JadehRo.Service.SmsService.Dtos;
using JadehRo.Service.SmsService.Panels;
using Microsoft.Extensions.Logging;

namespace JadehRo.Service.SmsService;

public class SmsService : ISmsService
{
    private readonly IOptionService _optionService;
    private readonly Func<SmsPanelType, ISmsPanelService> _smsSender;
    private readonly ILogger<SmsService> _logger;

    public SmsService(IOptionService optionService, Func<SmsPanelType, ISmsPanelService> smsSender, ILogger<SmsService> logger)
    {
        _optionService = optionService;
        _smsSender = smsSender;
        _logger = logger;
    }

    public void SendSmsInBackground(SendingSmsRequest request)
    {
        var option = _optionService.GetOption();
        if (!option.EnableSms) return;
        Task.Run(() => SendSmsInBackgroundTask(request, option));
    }
    public async Task SendSmsInBackgroundTask(SendingSmsRequest request, OptionDto option)
    {
        try
        {
            var result = await SendSms(request, option);
            if (!result.IsSuccess) throw new AppException();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $" خطا ارسال پیامک - پترن {request.PatternId}");
        }
    }
    public async Task<SendingSmsResponse> SendSms(SendingSmsRequest request)
    {
        var option = _optionService.GetOption();
        return await SendSms(request, option);
    }
    public async Task<SendingSmsResponse> SendSms(SendingSmsRequest request, OptionDto option)
    {
        var result = new SendingSmsResponse();

        if (!option.EnableSms)
        {
            result.IsSuccess = true;
            result.SmsSendId = "0";
            return result;
        }

        var sender = _smsSender(option.SmsPanelType);
        sender.Option = option;
        result = await sender.Send(request);

        return result;
    }
}