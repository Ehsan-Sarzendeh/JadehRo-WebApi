using JadehRo.Service.OptionServices.Dto;
using JadehRo.Service.SendingMsgModule.Implementation.SMSPanelService;
using JadehRo.Service.SendingMsgModule.Messaging;

namespace JadehRo.Service.SendingMsgModule.Implementation
{
    public interface ISendingMsgService : IScopedDependency
    {
        void AddSms(SendingSmsRequest request, bool saveNow = false);
        Task SendingAllSms();
        Task<SendingSmsResponse> SendSms(SendingSmsRequest request,OptionDto option, SmsPanelType smsPanelType = SmsPanelType.Ippanel);

        Task SendNotification(SendingNotifRequest request);
        void SendNotificationInBackground(SendingNotifRequest request);
        Task<SendingSmsResponse> SendDirectSms(SendingSmsRequest request, OptionDto option);
        void SendDirectSmsInBackground(SendingSmsRequest request);
    }
}