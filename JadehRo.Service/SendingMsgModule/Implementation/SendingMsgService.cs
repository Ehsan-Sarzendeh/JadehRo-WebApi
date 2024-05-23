using System.Text;
using JadehRo.Service.OptionServices;
using JadehRo.Service.OptionServices.Dto;
using JadehRo.Service.SendingMsgModule.Implementation.SMSPanelService;
using JadehRo.Service.SendingMsgModule.Messaging;

namespace JadehRo.Service.SendingMsgModule.Implementation
{
    public class SendingMsgService : ISendingMsgService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IOptionService _optionService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly Func<SmsPanelType, ISmsService> _smsSender;
        private readonly ILogger<SendingMsgService> _logger;
        private readonly IMapper _mapper;

        public SendingMsgService(IRepositoryWrapper repository, Func<SmsPanelType, ISmsService> smsSender, IMapper mapper, IOptionService optionService, ILogger<SendingMsgService> logger, IBackgroundJobClient backgroundJobClient)
        {
            _repository = repository;
            _smsSender = smsSender;
            _mapper = mapper;
            _optionService = optionService;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
        }

        #region SMS
        public void SendDirectSmsInBackground(SendingSmsRequest request)
        {
            var option = _optionService.GetOption();
            if (!option.EnableSms) return;
            _backgroundJobClient.Enqueue(() => SendDirectSms(request, option));
        }
        public async Task<SendingSmsResponse> SendDirectSms(SendingSmsRequest request, OptionDto option)
        {
            var result = new SendingSmsResponse();

            if (!option.EnableSms)
            {
                result.IsSuccess = true;
                result.SmsSendId = "0";
                return result;
            }

            var blokeDefaultSmsPanelDateTime = string.IsNullOrEmpty(option.BlokeDefaultSmsPanelDateTime)
                ? DateTime.Now
                : _mapper.Map<DateTime>(option.BlokeDefaultSmsPanelDateTime);

            if (option.SmsPanelType == SmsPanelType.Auto)
            {
                try
                {
                    result = await (DateTime.Now >= blokeDefaultSmsPanelDateTime ? SendSms(request, option, SmsPanelType.Ippanel) : SendSms(request, option, SmsPanelType.Kavenegar));
                    if (!result.IsSuccess)
                    {
                        UpdateBlokeDefaultSmsPanelDateTime(option);
                        result = await SendSms(request, option, SmsPanelType.Kavenegar);
                    }
                }
                catch (Exception)
                {
                    UpdateBlokeDefaultSmsPanelDateTime(option);
                    result = await SendSms(request, option, SmsPanelType.Kavenegar);
                    throw;
                }
            }
            else
            {
                result = await SendSms(request, option, option.SmsPanelType);
            }

            return result;
        }
        private void UpdateBlokeDefaultSmsPanelDateTime(OptionDto opt)
        {
            var blokeDefaultSmsPanelDateTime = DateTime.Now.AddHours(opt.BlokeDefaultSmsPanelHours);

            var optionRecord = _repository.Option.Table
                .Single(x => x.Key == "BlokeDefaultSmsPanelDateTime");

            optionRecord.Value = _mapper.Map<string>(blokeDefaultSmsPanelDateTime);
            _repository.Save();
        }
        public void AddSms(SendingSmsRequest request, bool saveNow = false)
        {
            var smsModel = new SmsModel()
            {
                PatternId = request.PatternId,
                Body = request.Body,
                PatternParams = JsonConvert.SerializeObject(request.PatternParams),
                PatternValues = JsonConvert.SerializeObject(request.PatternValues),
                To = JsonConvert.SerializeObject(request.To),
                CreatedUserId = 1,
                ModifiedUserId = 1,
                IsActive = true,
                CreatedDateTime = DateTime.Now,
                ModifiedDateTime = DateTime.Now
            };

            _repository.SmsModel.Add(smsModel, saveNow);
        }

        public async Task SendingAllSms()
        {
            var smss = _repository.SmsModel.Table
                .Where(x => x.IsSent != true && (x.ReTriesCount == 0 || x.ModifiedDateTime.Value.AddMinutes(5) < DateTime.Now))
                .ToList();

            var option = _optionService.GetOption();
            if (!option.EnableSms) return;

            foreach (var sms in smss)
            {
                var request = new SendingSmsRequest()
                {
                    PatternId = sms.PatternId,
                    Body = sms.Body,
                    PatternParams = JsonConvert.DeserializeObject<List<string>>(sms.PatternParams),
                    PatternValues = JsonConvert.DeserializeObject<List<string>>(sms.PatternValues),
                    To = JsonConvert.DeserializeObject<List<string>>(sms.To)
                };

                try
                {
                    var result = await SendSms(request, option);
                    sms.ReTriesCount++;
                    sms.ModifiedDateTime = DateTime.Now;

                    if (result.IsSuccess)
                    {
                        sms.IsSent = true;
                        sms.SmsSendId = result.SmsSendId;
                        sms.SmsResult = null;
                    }
                    else
                    {
                        sms.SmsResult = result.MessageString;
                    }

                }
                catch (Exception e)
                {
                    sms.SmsResult = e.Message;

                    _logger.LogError(e, "خطا ارسال پیامک");
                    throw;
                }

                _repository.SmsModel.Save();
            }
        }

        public async Task<SendingSmsResponse> SendSms(SendingSmsRequest request, OptionDto option, SmsPanelType smsPanelType = SmsPanelType.Ippanel)
        {
            var result = new SendingSmsResponse();

            var sender = _smsSender(smsPanelType);

            if (!option.EnableSms)
            {
                result.IsSuccess = true;
                result.SmsSendId = "0";
                return result;
            }

            sender.Option = option;

            result = await sender.Send(request);

            return result;
        }
 
        #endregion

        #region Notification

        public async Task SendNotification(SendingNotifRequest request)
        {
            try
            {
                var client = new HttpClient();

                var notification = new
                {
                    app_ids = request.AppIdes,
                    filters = new
                    {
                        device_id = request.DeviceIds
                    },
                    data = new
                    {
                        title = request.Title,
                        content = request.Content,
                        big_title = request.Title,
                        big_content = request.Content,
                        icon = "https://files.pushe.co/notification-images/2022/06/20220626-ef01eb292c6240fd86b592fdc104c3cb.png",
                        sound_url = "https://static.pushe.co/mp3/5.mp3",
                        wake_screen = true,

                    },
                    time_to_live = 2419200,
                    priority = 3
                };

                client.DefaultRequestHeaders.Add("authorization", "Token b90b3e2ddf410bbff108f264ea97fe45bcaf1371");
                var content = new StringContent(JsonConvert.SerializeObject(notification), Encoding.UTF8, "application/json");
                var res = await client.PostAsync("https://api.pushe.co/v2/messaging/notifications/", content);
                res.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "خطا در ارسال نوتیفیکیشن");
            }
        }

        public void SendNotificationInBackground(SendingNotifRequest request)
        {
            var option = _optionService.GetOption();
            if (!option.EnableNotif) return;
            _backgroundJobClient.Enqueue(() => SendNotification(request));
        }

    
        #endregion
    }
}