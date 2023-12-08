using System.ComponentModel;

namespace Montreal.Core.Crosscutting.Domain.Enum
{
    public enum AssetType
    {
        [Description("RAKE")]
        Rake = 0,

        [Description("BOX")]
        Box = 1,

        [Description("Empurrador")]
        Pusher = 2,

        [Description("Cabotagem")]
        Cabotage = 3
    }
}
