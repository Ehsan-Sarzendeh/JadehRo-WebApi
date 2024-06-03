using System.Net.Http.Headers;
using System.Net.Http.Json;
using JadehRo.Service.OptionServices.Dto;
using JadehRo.Service.SmsService.Dtos;
using JadehRo.Service.SmsService.Panels.IppanelPanel.Models;
using Newtonsoft.Json;

namespace JadehRo.Service.SmsService.Panels.IppanelPanel;

public class IppanelPanelService : ISmsPanelService
{
    public OptionDto Option { get; set; }

    public async Task<SendingSmsResponse> Send(SendingSmsRequest request)
    {
        SendingSmsResponse result;

        if (string.IsNullOrEmpty(request.PatternId))
            result = await SendPublicSms(request.To, request.Body);
        else
            result = await SendLookupSms(request.To.First(), request.PatternId, request.PatternParams, request.PatternValues);

        return result;
    }

    private async Task<SendingSmsResponse> SendPublicSms(List<string> phoneNumbers, string message)
    {
        var result = new SendingSmsResponse();

        return result;
    }

    private async Task<SendingSmsResponse> SendLookupSms(string phoneNumber, string patternCode, List<string> patternParams, List<string> patternValues)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("apikey", Option.IppanelSmsOption.Apikey);
        client.Timeout = new TimeSpan(0, 0, 0, 5);

        var smsSenderNos = Option.IppanelSmsOption.SenderNo.Split(",");
        var content = new IppanelSendSmsRequest
        {
            code = patternCode,
            sender = smsSenderNos[0],
            recipient = phoneNumber,
            variable = patternParams.Zip(patternValues, (p, v) => new { p, v }).ToDictionary(x => x.p, x => x.v)
        };

        var res = await client.PostAsJsonAsync($"{Option.IppanelSmsOption.ApiPath}/sms/pattern/normal/send", content);
        var resultContent = await res.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<IppanelSendSmsResponse>(resultContent);

        var sendingSmsResponse = new SendingSmsResponse();
        if (result.code == 200)
        {
            sendingSmsResponse.IsSuccess = true;
            sendingSmsResponse.SmsSendId = result.data.message_id.ToString();
        }
        else
        {
            sendingSmsResponse.IsSuccess = false;
            sendingSmsResponse.Message.Add(result.error_message);
        }

        return sendingSmsResponse;
    }
}