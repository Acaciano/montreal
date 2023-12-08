using System.ComponentModel;

namespace Montreal.Core.Crosscutting.Domain.Enum
{
    public enum Status
    {
        [Description("Ativo")]
        Active = 1,
        [Description("Inativo")]
        Inactive = 2,
        [Description("Bloqueado")]
        Blocked = 3
    }
}
