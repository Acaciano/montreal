using System.ComponentModel;

namespace Montreal.Core.Crosscutting.Domain.Enum
{
    public enum ExchangeType
    {
        [Description("SIMPLES")]
        SIMPLE = 1,

        [Description("PERFORMANCE")]
        PERFORMANCE = 2,
    }
}
