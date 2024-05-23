using JadehRo.Database.Entities.Base;

namespace JadehRo.Database.Entities.Common;

public class Option : BaseOptionEntity 
{
    public OptionType OptionType { get; set; }
}

public enum OptionType
{
    Server = 0,
    Client = 1,
    Both = 2
}