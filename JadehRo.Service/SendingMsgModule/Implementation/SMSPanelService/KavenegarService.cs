using JadehRo.Service.OptionServices.Dto;
using JadehRo.Service.SendingMsgModule.Messaging;

namespace JadehRo.Service.SendingMsgModule.Implementation.SMSPanelService
{
    public class KavenegarService : ISmsService
    {
        public OptionDto Option { get; set; }

        public async Task<SendingSmsResponse> Send(SendingSmsRequest request)
        {
            SendingSmsResponse result;

            if (string.IsNullOrEmpty(request.PatternId))
                result = await SendPublicSms(request.To, request.Body);
            else
                result = await SendLookupSms(request.To.First(), request.PatternId, request.PatternParamsKaveh, request.PatternValues);

            return result;
        }

        private async Task<SendingSmsResponse> SendPublicSms(List<string> phoneNumber, string message)
        {
            var result = new SendingSmsResponse();

            try
            {
                var api = new Kavenegar.KavenegarApi(Option.KavenegarSmsOption.Apikey);

                var sendingResult = await api.Send(Option.KavenegarSmsOption.SenderNo, phoneNumber, message);

                if (sendingResult.First().Status != 6 && sendingResult.First().Status != 100)
                {
                    result.SmsSendId = sendingResult.First().Messageid.ToString();
                    result.IsSuccess = true;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message.Add("ارسال ناموفق");
                    result.Message.Add($"{sendingResult.First().Status} -- {sendingResult.First().StatusText}");
                }
            }
            catch (Kavenegar.Core.Exceptions.ApiException ex)
            {
                result.IsSuccess = false;
                result.Message.Add($"خطا در ارسال پیامک - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (Kavenegar.Core.Exceptions.HttpException ex)
            {
                result.IsSuccess = false;
                result.Message.Add($"خطا در ارسال پیامک - {ex.Message}");
                throw new Exception(ex.Message);
            }

            return result;
            throw new Exception( );

        }

        private async Task<SendingSmsResponse> SendLookupSms(string phoneNumber, string templateName, IList<string> patternParams, IReadOnlyList<string> patternValues)
        {
            //var result = new SendingSmsResponse();

            //try
            //{
            //    var api = new Kavenegar.KavenegarApi(Option.KavenegarSmsOption.Apikey);

            //    var token = (patternParams.Any(x => x == "token")) ? patternValues[patternParams.IndexOf("token")] : "";
            //    var token2 = (patternParams.Any(x => x == "token2")) ? patternValues[patternParams.IndexOf("token2")] : "";
            //    var token3 = (patternParams.Any(x => x == "token3")) ? patternValues[patternParams.IndexOf("token3")] : "";
            //    var token10 = (patternParams.Any(x => x == "token10")) ? patternValues[patternParams.IndexOf("token10")] : "";
            //    var token20 = (patternParams.Any(x => x == "token20")) ? patternValues[patternParams.IndexOf("token20")] : "";

            //    var sendingResult = await api.VerifyLookup(phoneNumber, token, token2, token3, token10, token20, templateName, VerifyLookupType.Sms);

            //    if (sendingResult.Status != 6 && sendingResult.Status != 100)
            //    {
            //        result.SmsSendId = sendingResult.Messageid.ToString();
            //        result.IsSuccess = true;
            //    }
            //    else
            //    {
            //        result.IsSuccess = false;
            //        result.Message.Add("ارسال ناموفق");
            //        result.Message.Add($"{sendingResult.Status} -- {sendingResult.StatusText}");
            //    }
            //}
            //catch (Kavenegar.Core.Exceptions.ApiException ex)
            //{
            //    result.IsSuccess = false;
            //    result.Message.Add($"خطا در ارسال پیامک - {ex.Message}");
            //    throw new Exception(ex.Message);
            //}
            //catch (Kavenegar.Core.Exceptions.HttpException ex)
            //{
            //    result.IsSuccess = false;
            //    result.Message.Add($"خطا در ارسال پیامک - {ex.Message}");
            //    throw new Exception(ex.Message);
            //}

            //return result;
            throw new Exception();

        }
    }
}