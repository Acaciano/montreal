using System.ComponentModel;

namespace Montreal.Core.Crosscutting.Common.Enum
{
    public enum EntityStatus
    {
        [Description("Ativo")]
        Active = 1,
        [Description("Inativo")]
        Inactive = 2,
        [Description("Bloqueado")]
        Blocked = 3
    }
}
