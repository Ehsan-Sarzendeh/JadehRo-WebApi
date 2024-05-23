using JadehRo.Service.Infrastructure;

namespace JadehRo.Service.SendingMsgModule.Messaging
{
    public class SendingSmsRequest : BaseDto<SendingSmsRequest, SmsModel>
    {
        public string PatternId { get; set; }
        public string Body { get; set; }
        public List<string> To { get; set; }
        public List<string> PatternParams { get; set; }
        public List<string> PatternParamsKaveh { get; set; }
        public List<string> PatternValues { get; set; }

        public override void CustomMappings(IMappingExpression<SmsModel, SendingSmsRequest> mapping)
        {
            mapping.ForMember(x => x.To,
                opt =>
                    opt.MapFrom(o => JsonConvert.DeserializeObject<List<string>>(o.To))).ReverseMap();

            mapping.ForMember(x => x.PatternParams,
                opt =>
                    opt.MapFrom(o => JsonConvert.DeserializeObject<List<string>>(o.PatternParams))).ReverseMap();

            mapping.ForMember(x => x.PatternValues,
                opt =>
                    opt.MapFrom(o => JsonConvert.DeserializeObject<List<string>>(o.PatternValues))).ReverseMap();

            base.CustomMappings(mapping);
        }
    }
}
