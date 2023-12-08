using System.ComponentModel;

namespace Montreal.Core.Crosscutting.Domain.Enum
{
    public enum ShipStatus
    {
        [Description("AGENDADO")]
        Scheduled = 1,

        [Description("AGUARDANDO")]
        Waiting = 2,

        [Description("ATRACADO")]
        Berthed = 3,

        [Description("CARREGADO")]
        Loaded = 4
    }
}
