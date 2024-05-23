using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using JadehRo.Service.OptionServices.Dto;
using JadehRo.Service.SendingMsgModule.Messaging;
using JadehRo.Service.SendingMsgModule.ViewModels;

namespace JadehRo.Service.SendingMsgModule.Implementation.SMSPanelService
{
    public class IppanelService : ISmsService
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

            using var httpClientHandler = new HttpClientHandler();

            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using var client = new HttpClient(httpClientHandler);

            var smsSenderNos = Option.IppanelSmsOption.SenderNo.Split(",");

            var smsData = new SendSmsViewModel
            {
                op = "send",
                from = smsSenderNos[0],
                message = message,
                uname = Option.IppanelSmsOption.UserName,
                pass = Option.IppanelSmsOption.Password,
                to = phoneNumbers
            };
            client.BaseAddress = new Uri(Option.IppanelSmsOption.ServicePath);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsJsonAsync("/api/select", smsData);

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var sendingResult = JsonConvert.DeserializeObject<List<string>>(data);

            if (sendingResult is { Count: 2 } && sendingResult[0] == "0")
            {
                result.SmsSendId = sendingResult[1];
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
                if (sendingResult != null)
                    result.StatusCode = int.Parse(sendingResult[0]);
                result.Message.Add("خطا در ارسال پیامک");
                if (sendingResult?.Count == 2)
                    result.Message.Add(sendingResult[1]);
            }

            return result;
        }

        private async Task<SendingSmsResponse> SendLookupSms(string phoneNumber, string templateName, List<string> patternParams, List<string> patternValues)
        {
            var result = new SendingSmsResponse();

            using var httpClientHandler = new HttpClientHandler();

            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using var client = new HttpClient(httpClientHandler);

            var smsSenderNos = Option.IppanelSmsOption.SenderNo.Split(",");

            var sbPatternValue = new StringBuilder();

            for (var index = 0; index < patternParams.Count; index++)
                sbPatternValue.AppendFormat("&p{0}={1}", index + 1, patternParams[index]);
            for (var index = 0; index < patternParams.Count; index++)
                sbPatternValue.AppendFormat("&v{0}={1}", index + 1, patternValues[index]);

            var data = string.Format(Option.IppanelSmsOption.DataPatternJson, Option.IppanelSmsOption.Apikey, templateName, smsSenderNos[0], phoneNumber, sbPatternValue);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(Option.IppanelSmsOption.ServicePatternPath + "?" + data);

            response.EnsureSuccessStatusCode();

            var resultData = await response.Content.ReadAsStringAsync();

            var sendingPatternResult = long.TryParse(resultData, out var smsmId);

            if (sendingPatternResult)
            {
                result.IsSuccess = true;
                result.SmsSendId = smsmId.ToString();
            }
            else
            {
                result.IsSuccess = false;
                result.Message.Add(resultData);
            }

            return result;
        }
    }
}